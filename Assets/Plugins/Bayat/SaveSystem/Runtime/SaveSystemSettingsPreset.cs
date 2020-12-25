using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Bayat.SaveSystem.Security;
using Bayat.SaveSystem.Storage;
using Bayat.Json;

namespace Bayat.SaveSystem
{

    /// <summary>
    /// The Save System settings preset.
    /// </summary>
    [CreateAssetMenu(menuName = "Bayat/Save System/Settings Preset")]
    public class SaveSystemSettingsPreset : ScriptableObject
    {

        private static SaveSystemSettingsPreset defaultPreset;

        /// <summary>
        /// The default settings preset.
        /// </summary>
        public static SaveSystemSettingsPreset DefaultPreset
        {
            get
            {
                if (defaultPreset == null)
                {
                    defaultPreset = Resources.Load<SaveSystemSettingsPreset>("Bayat/SaveSystem/Settings/Default");
#if UNITY_EDITOR
                    if (defaultPreset == null)
                    {
                        defaultPreset = ScriptableObject.CreateInstance<SaveSystemSettingsPreset>();
                        System.IO.Directory.CreateDirectory("Assets/Resources/Bayat/SaveSystem/Settings");
                        AssetDatabase.CreateAsset(defaultPreset, "Assets/Resources/Bayat/SaveSystem/Settings/Default.asset");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
#endif
                }
                return defaultPreset;
            }
        }

        [SerializeField]
        protected string storageConnectionString = "disk://path={0}";
        [SerializeField]
        protected bool useMetaData = true;
        [SerializeField]
        protected bool useCatalog = true;
        [SerializeField]
        protected bool useEncryption = false;
        [SerializeField]
        protected string password;
        [SerializeField]
        protected string ecryptionAlgorithmName = SaveSystemSymmetricEncryption.DefaultAlgorithmName;
        [SerializeField]
        protected JsonSerializerSettingsPreset serializerSettingsPreset;

        protected SaveSystemSettings customSettings;

        /// <summary>
        /// Creates a new instance of <see cref="SaveSystemSettings"/> and applies the settings to it.
        /// </summary>
        public virtual SaveSystemSettings NewSettings
        {
            get
            {
                SaveSystemSettings settings = new SaveSystemSettings();
                ApplyTo(settings);
                return settings;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="SaveSystemSettings"/> and applies the settings to it if the custom settings is null otherwise returns the existing instance.
        /// </summary>
        public virtual SaveSystemSettings CustomSettings
        {
            get
            {
                if (this.customSettings == null)
                {
                    this.customSettings = this.NewSettings;
                }
                return this.customSettings;
            }
        }

        /// <summary>
        /// Applies the preset settings to the <see cref="SaveSystemSettings"/> instance.
        /// </summary>
        /// <param name="settings"></param>
        public virtual void ApplyTo(SaveSystemSettings settings)
        {
            JsonSerializerSettingsPreset serializerSettingsPreset = this.serializerSettingsPreset ?? JsonSerializerSettingsPreset.DefaultPreset;
            settings.Serializer = new SaveSystemJsonSerializer(serializerSettingsPreset.NewSettings);
            settings.Storage = StorageFactory.FromConnectionString(this.storageConnectionString);
            settings.Storage.UseMetaData = this.useMetaData;
            settings.Storage.UseCatalog = this.useCatalog;
            settings.UseEncryption = this.useEncryption;
            settings.EncryptionAlgorithm = new SaveSystemSymmetricEncryption(this.ecryptionAlgorithmName);
            settings.Password = this.password;
        }

    }

}