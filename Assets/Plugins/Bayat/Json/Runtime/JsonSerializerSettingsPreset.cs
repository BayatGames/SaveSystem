using System.Globalization;
using System.Runtime.Serialization.Formatters;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Bayat.Json.Serialization;

using System.Diagnostics;

namespace Bayat.Json
{

    /// <summary>
    /// The json serializer settings preset.
    /// </summary>
    [CreateAssetMenu(menuName = "Bayat/Json/Json Serializer Settings Preset")]
    public class JsonSerializerSettingsPreset : ScriptableObject
    {

        private static JsonSerializerSettingsPreset defaultPreset;

        /// <summary>
        /// The default preset.
        /// </summary>
        public static JsonSerializerSettingsPreset DefaultPreset
        {
            get
            {
                if (defaultPreset == null)
                {
                    defaultPreset = Resources.Load<JsonSerializerSettingsPreset>("Bayat/Json/Settings/Default");
#if UNITY_EDITOR
                    if (defaultPreset == null)
                    {
                        defaultPreset = ScriptableObject.CreateInstance<JsonSerializerSettingsPreset>();
                        System.IO.Directory.CreateDirectory("Assets/Resources/Bayat/Json/Settings");
                        AssetDatabase.CreateAsset(defaultPreset, "Assets/Resources/Bayat/Json/Settings/Default.asset");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
#endif
                }
                return defaultPreset;
            }
        }

        [SerializeField]
        protected bool checkAdditionalContent = JsonSerializerSettings.DefaultCheckAdditionalContent;
        [SerializeField]
        protected bool serializeScriptableObjects = JsonSerializerSettings.DefaultSerializeScriptableObjects;
        [SerializeField]
        protected ConstructorHandling constructorHandling = JsonSerializerSettings.DefaultConstructorHandling;
        [SerializeField]
        protected DateFormatHandling dateFormatHandling = JsonSerializerSettings.DefaultDateFormatHandling;
        [SerializeField]
        protected string dateFormatString = JsonSerializerSettings.DefaultDateFormatString;
        [SerializeField]
        protected DateParseHandling dateParseHandling = JsonSerializerSettings.DefaultDateParseHandling;
        [SerializeField]
        protected DateTimeZoneHandling dateTimeZoneHandling = JsonSerializerSettings.DefaultDateTimeZoneHandling;
        [SerializeField]
        protected DefaultValueHandling defaultValueHandling = JsonSerializerSettings.DefaultDefaultValueHandling;
        [SerializeField]
        protected FloatFormatHandling floatFormatHandling = JsonSerializerSettings.DefaultFloatFormatHandling;
        [SerializeField]
        protected FloatParseHandling floatParseHandling = JsonSerializerSettings.DefaultFloatParseHandling;
        [SerializeField]
        protected FormatterAssemblyStyle formatterAssemblyStyle = JsonSerializerSettings.DefaultFormatterAssemblyStyle;
        [SerializeField]
        protected Formatting formatting = JsonSerializerSettings.DefaultFormatting;
        [SerializeField]
        protected MetadataPropertyHandling metadataPropertyHandling = JsonSerializerSettings.DefaultMetadataPropertyHandling;
        [SerializeField]
        protected MissingMemberHandling missingMemberHandling = JsonSerializerSettings.DefaultMissingMemberHandling;
        [SerializeField]
        protected NullValueHandling nullValueHandling = JsonSerializerSettings.DefaultNullValueHandling;
        [SerializeField]
        protected ObjectCreationHandling objectCreationHandling = JsonSerializerSettings.DefaultObjectCreationHandling;
        [SerializeField]
        protected PreserveReferencesHandling preserveReferencesHandling = JsonSerializerSettings.DefaultPreserveReferencesHandling;
        [SerializeField]
        protected ReferenceLoopHandling referenceLoopHandling = JsonSerializerSettings.DefaultReferenceLoopHandling;
        [SerializeField]
        protected StringEscapeHandling stringEscapeHandling = JsonSerializerSettings.DefaultStringEscapeHandling;
        [SerializeField]
        protected FormatterAssemblyStyle typeNameAssemblyFormat = JsonSerializerSettings.DefaultTypeNameAssemblyFormat;
        [SerializeField]
        protected TypeNameHandling typeNameHandling = JsonSerializerSettings.DefaultTypeNameHandling;
        [SerializeField]
        protected string cultureName = string.Empty;
        [SerializeField]
        protected int maxDepth = 0;
        [SerializeField]
        protected bool enableUnityTraceWriter = false;
        [Tooltip("The verbose option causes errors during serialization, prevent using it")]
        [SerializeField]
        protected TraceLevel traceLevel = TraceLevel.Info;

        protected JsonSerializerSettings customSettings;

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializerSettings"/> and applies the settings to it.
        /// </summary>
        public virtual JsonSerializerSettings NewSettings
        {
            get
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                ApplyTo(settings);
                return settings;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializerSettings"/> and applies the settings to it if the custom settings is null otherwise returns the existing instance.
        /// </summary>
        public virtual JsonSerializerSettings CustomSettings
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

        protected virtual void OnValidate()
        {
            if (this.traceLevel == TraceLevel.Verbose)
            {
                UnityEngine.Debug.LogWarning("The Trace Level of Verbose is not supported");
                this.traceLevel = TraceLevel.Info;
            }
        }

        /// <summary>
        /// Applies the settings to the <see cref="JsonSerializerSettings"/> instance.
        /// </summary>
        /// <param name="settings"></param>
        public virtual void ApplyTo(JsonSerializerSettings settings)
        {
            settings.CheckAdditionalContent = this.checkAdditionalContent;
            settings.SerializeScriptableObjects = this.serializeScriptableObjects;
            settings.ConstructorHandling = this.constructorHandling;
            settings.DateFormatHandling = this.dateFormatHandling;
            settings.DateFormatString = this.dateFormatString;
            settings.DateParseHandling = this.dateParseHandling;
            settings.DateTimeZoneHandling = this.dateTimeZoneHandling;
            settings.DefaultValueHandling = this.defaultValueHandling;
            settings.FloatFormatHandling = this.floatFormatHandling;
            settings.FloatParseHandling = this.floatParseHandling;
            settings.Formatting = this.formatting;
            settings.MetadataPropertyHandling = this.metadataPropertyHandling;
            settings.MissingMemberHandling = this.missingMemberHandling;
            settings.NullValueHandling = this.nullValueHandling;
            settings.ObjectCreationHandling = this.objectCreationHandling;
            settings.PreserveReferencesHandling = this.preserveReferencesHandling;
            settings.ReferenceLoopHandling = this.referenceLoopHandling;
            settings.StringEscapeHandling = this.stringEscapeHandling;
            settings.TypeNameAssemblyFormat = this.typeNameAssemblyFormat;
            settings.TypeNameHandling = this.typeNameHandling;
            settings.Converters = JsonSerializer.AvailableConverters;

            if (this.enableUnityTraceWriter)
            {
                var traceWriter = new UnityTraceWriter();
                traceWriter.LevelFilter = this.traceLevel;
                settings.TraceWriter = traceWriter;
            }

            if (this.maxDepth > 0)
            {
                settings.MaxDepth = this.maxDepth;
            }

            CultureInfo culture = CultureInfo.GetCultureInfo(this.cultureName);
            if (culture == null)
            {
                culture = CultureInfo.InvariantCulture;
            }
            settings.Culture = culture;
        }

    }

}