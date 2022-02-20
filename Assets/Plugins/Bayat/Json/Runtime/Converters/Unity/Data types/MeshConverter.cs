using System;

using Bayat.Json.Serialization;

using UnityEngine;

namespace Bayat.Json.Converters
{

    public class MeshConverter : ObjectJsonConverter
    {

        public struct SerializedIndices
        {
            public MeshTopology meshTopology;
            public int[] indices;
        }

        public override string[] GetObjectProperties()
        {
            return new string[] { "indexFormat", "bindposes", "subMeshCount", "bounds", "vertices", "normals", "tangents", "uv", "uv2", "uv3", "uv4", "colors", "colors32", "triangles", "boneWeights", "name", "hideFlags" };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Mesh);
        }

        public override void WriteProperties(JsonObjectContract contract, JsonWriter writer, object value, Type objectType, JsonSerializerWriter internalWriter)
        {
            var instance = (UnityEngine.Mesh)value;
            if (!instance.isReadable)
            {
                return;
            }
#if UNITY_2017_3
            internalWriter.SerializeProperty(writer, "indexFormat", instance.indexFormat);
#endif
            internalWriter.SerializeProperty(writer, "bindposes", instance.bindposes);
            writer.WriteProperty("subMeshCount", instance.subMeshCount);
            internalWriter.SerializeProperty(writer, "bounds", instance.bounds);
            internalWriter.SerializeProperty(writer, "vertices", instance.vertices);
            internalWriter.SerializeProperty(writer, "normals", instance.normals);
            internalWriter.SerializeProperty(writer, "tangents", instance.tangents);
            internalWriter.SerializeProperty(writer, "uv", instance.uv);
            internalWriter.SerializeProperty(writer, "uv2", instance.uv2);
            internalWriter.SerializeProperty(writer, "uv3", instance.uv3);
            internalWriter.SerializeProperty(writer, "uv4", instance.uv4);
            internalWriter.SerializeProperty(writer, "colors", instance.colors);
            internalWriter.SerializeProperty(writer, "colors32", instance.colors32);

            writer.WritePropertyName("triangles");
            writer.WriteStartArray();
            for (int i = 0; i < instance.subMeshCount; i++)
            {
                internalWriter.Serialize(writer, instance.GetTriangles(i));
            }
            writer.WriteEndArray();

            writer.WritePropertyName("indices");
            writer.WriteStartArray();
            for (int i = 0; i < instance.subMeshCount; i++)
            {
                SerializedIndices serializedIndices = new SerializedIndices();
                serializedIndices.meshTopology = instance.GetTopology(i);
                serializedIndices.indices = instance.GetIndices(i);
                internalWriter.Serialize(writer, serializedIndices);
            }
            writer.WriteEndArray();

            internalWriter.SerializeProperty(writer, "boneWeights", instance.boneWeights);
            writer.WriteProperty("name", instance.name);
            internalWriter.SerializeProperty(writer, "hideFlags", instance.hideFlags);
        }

        public override object PopulateMember(string memberName, JsonContract contract, JsonReader reader, Type objectType, object targetObject, JsonSerializerReader internalReader)
        {
            var instance = (UnityEngine.Mesh)targetObject;
            if (!instance.isReadable)
            {
                reader.Skip();
                return instance;
            }
            switch (memberName)
            {
#if UNITY_2017_3
                case "indexFormat":
                    instance.indexFormat = internalReader.DeserializeProperty<UnityEngine.Rendering.IndexFormat>(reader);
                    break;
#endif
                case "bindposes":
                    instance.bindposes = internalReader.DeserializeProperty<UnityEngine.Matrix4x4[]>(reader);
                    break;
                case "subMeshCount":
                    instance.subMeshCount = reader.ReadProperty<System.Int32>();
                    break;
                case "bounds":
                    instance.bounds = internalReader.DeserializeProperty<UnityEngine.Bounds>(reader);
                    break;
                case "vertices":
                    instance.vertices = internalReader.DeserializeProperty<UnityEngine.Vector3[]>(reader);
                    break;
                case "normals":
                    instance.normals = internalReader.DeserializeProperty<UnityEngine.Vector3[]>(reader);
                    break;
                case "tangents":
                    instance.tangents = internalReader.DeserializeProperty<UnityEngine.Vector4[]>(reader);
                    break;
                case "uv":
                    instance.uv = internalReader.DeserializeProperty<UnityEngine.Vector2[]>(reader);
                    break;
                case "uv2":
                    instance.uv2 = internalReader.DeserializeProperty<UnityEngine.Vector2[]>(reader);
                    break;
                case "uv3":
                    instance.uv3 = internalReader.DeserializeProperty<UnityEngine.Vector2[]>(reader);
                    break;
                case "uv4":
                    instance.uv4 = internalReader.DeserializeProperty<UnityEngine.Vector2[]>(reader);
                    break;
                case "colors":
                    instance.colors = internalReader.DeserializeProperty<UnityEngine.Color[]>(reader);
                    break;
                case "colors32":
                    instance.colors32 = internalReader.DeserializeProperty<UnityEngine.Color32[]>(reader);
                    break;
                case "triangles":
                    var triangles = internalReader.DeserializeProperty<System.Int32[][]>(reader);
                    for (int i = 0; i < triangles.GetLength(0); i++)
                    {
                        instance.SetTriangles(triangles[i], i);
                    }
                    break;
                case "indices":
                    var indices = internalReader.DeserializeProperty<SerializedIndices[]>(reader);
                    for (int i = 0; i < indices.GetLength(0); i++)
                    {
                        instance.SetIndices(indices[i].indices, indices[i].meshTopology, i);
                    }
                    break;
                case "boneWeights":
                    instance.boneWeights = internalReader.DeserializeProperty<UnityEngine.BoneWeight[]>(reader);
                    break;
                case "name":
                    instance.name = reader.ReadProperty<System.String>();
                    break;
                case "hideFlags":
                    instance.hideFlags = internalReader.DeserializeProperty<UnityEngine.HideFlags>(reader);
                    break;
                default:
                    reader.Skip();
                    break;
            }
            return instance;
        }

    }

}