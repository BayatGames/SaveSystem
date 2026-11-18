using System.Linq;
using Bayat.Core;
using Bayat.Json;
using Bayat.Json.Converters;
using Bayat.Json.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_6000_0_OR_NEWER
using PhysicsMaterial = UnityEngine.PhysicsMaterial;
#else
using PhysicsMaterial = UnityEngine.PhysicMaterial;
#endif

namespace Bayat.SaveSystem.Tests
{
    public class SaveCompatibilityTests
    {
        [Test]
        public void LegacyAssetReferenceResolvesToTheOriginalAsset()
        {
            const string guid = "e66958bf3f9149f4aaaa07a028d723a6";
            const string oldSave = "{\"$unityRef\":\"" + guid + "\"}";
            var expected = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(
                UnityEditor.AssetDatabase.GUIDToAssetPath("8b909a2f0b703364b93a0bb1984ecf04"));

            Assert.That(expected, Is.Not.Null);
            Assert.That(AssetReferenceResolver.Current.Get(guid), Is.SameAs(expected));
            Assert.That(JsonConvert.DeserializeObject<Material>(oldSave), Is.SameAs(expected));
            Assert.That(AssetReferenceResolver.Current.Get(expected), Is.EqualTo(guid));
        }

        [TestCase("PhysicMaterial", "PhysicMaterialCombine")]
        [TestCase("PhysicsMaterial", "PhysicsMaterialCombine")]
        public void LoadsPhysicsMaterialsFromEitherEditorGeneration(string materialType, string combineType)
        {
            const string module = ", UnityEngine.PhysicsModule";
            string oldSave = "{\"$type\":\"UnityEngine." + materialType + module + "\","
                + "\"dynamicFriction\":0.375,\"bounciness\":0.25}";
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            var material = JsonConvert.DeserializeObject<PhysicsMaterial>(oldSave, settings);
            try
            {
                Assert.That(material.dynamicFriction, Is.EqualTo(0.375f));
                Assert.That(material.bounciness, Is.EqualTo(0.25f));
                var currentSave = JObject.Parse(JsonConvert.SerializeObject(material, settings));
                Assert.That((string)currentSave["$type"], Is.EqualTo(typeof(PhysicsMaterial).FullName + module));
                Assert.That(Bayat.Json.Serialization.DefaultSerializationBinder.Instance.BindToType(
                    "UnityEngine.PhysicsModule", "UnityEngine." + combineType), Is.EqualTo(material.frictionCombine.GetType()));
            }
            finally
            {
                Object.DestroyImmediate(material);
            }
        }

        [TestCase("instanceID", "-9042")]
        [TestCase("entityID", "9223372036854775808")]
        public void ObjectMetadataDoesNotReplacePersistentReferences(string idKey, string idValue)
        {
            var original = new GameObject("Before loading");
            var resolverObject = new GameObject("Compatibility test resolver");
            try
            {
                var resolver = resolverObject.AddComponent<SceneReferenceResolver>();
                const string reference = "3f9cb7c7e49e44f588a188b0c31fe225";
                resolver.Add(original, reference);
                string oldSave = "{\"$unityRef\":\"" + reference + "\",\"" + idKey + "\":" + idValue
                    + ",\"name\":\"Loaded object\"}";
                var settings = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    Converters = new JsonConverter[] { new GameObjectConverter(), new TransformConverter() }
                };
                var loaded = JsonConvert.DeserializeObject<GameObject>(oldSave, settings);

                Assert.That(loaded, Is.SameAs(original));
                Assert.That(loaded.name, Is.EqualTo("Loaded object"));
                var newSave = JObject.Parse(JsonConvert.SerializeObject(loaded, settings));
                Assert.That((string)newSave["$unityRef"], Is.EqualTo(reference));
                Assert.That(newSave["instanceID"], Is.Not.Null);
                Assert.That(newSave["instanceID"].Type, Is.EqualTo(JTokenType.Integer));
                Assert.That((long)newSave["instanceID"], Is.InRange((long)int.MinValue, (long)int.MaxValue));
                Assert.That(newSave["entityID"], Is.Null);
            }
            finally
            {
                Object.DestroyImmediate(original);
                Object.DestroyImmediate(resolverObject);
            }
        }

        [Test]
        public void ResolutionKeepsTheLegacyIntegerRefreshRate()
        {
            const string oldSave = "{\"height\":1080,\"width\":1920,\"refreshRate\":60}";
            var converter = new ResolutionConverter();
            var resolution = JsonConvert.DeserializeObject<Resolution>(oldSave, converter);
            Assert.That(resolution.width, Is.EqualTo(1920));
            Assert.That(resolution.height, Is.EqualTo(1080));
            Assert.That(JToken.DeepEquals(JObject.Parse(JsonConvert.SerializeObject(resolution, converter)), JObject.Parse(oldSave)), Is.True);

#if UNITY_2022_2_OR_NEWER
            resolution.refreshRateRatio = new RefreshRate { numerator = 60000, denominator = 1001 };
            Assert.That((int)JObject.Parse(JsonConvert.SerializeObject(resolution, converter))["refreshRate"], Is.EqualTo(60));
#endif
        }

        [Test]
        public void TextMeshProLoadsLegacyWrappingAndKerningFlags()
        {
            var textObject = new GameObject("Compatibility test text", typeof(RectTransform));
            var resolverObject = new GameObject("Compatibility test resolver");
            try
            {
                textObject.SetActive(false);
                var label = textObject.AddComponent<TextMeshProUGUI>();
                var resolver = resolverObject.AddComponent<SceneReferenceResolver>();
                const string reference = "3f9cb7c7e49e44f588a188b0c31fe226";
                resolver.Add(label, reference);
                JsonConverter converter = JsonSerializer.AvailableConverters.Single(
                    c => c.GetType().Name == "TextMeshProConverter");
                string oldSave = "{\"$unityRef\":\"" + reference + "\",\"enableWordWrapping\":false,\"enableKerning\":true}";

                Assert.That(JsonConvert.DeserializeObject<TextMeshProUGUI>(oldSave, converter), Is.SameAs(label));
#if UNITY_6000_0_OR_NEWER
                Assert.That(label.textWrappingMode, Is.EqualTo(TextWrappingModes.NoWrap));
                Assert.That(label.fontFeatures, Does.Contain(UnityEngine.TextCore.OTL_FeatureTag.kern));
#else
                Assert.That(label.enableWordWrapping, Is.False);
                Assert.That(label.enableKerning, Is.True);
#endif
            }
            finally
            {
                Object.DestroyImmediate(textObject);
                Object.DestroyImmediate(resolverObject);
            }
        }
    }
}
