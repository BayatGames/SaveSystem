using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Bayat.Core;
using Bayat.Core.EditorWindows;
using Bayat.Core.Reflection;
using Bayat.Json.Serialization;

using UnityEditor;

using UnityEngine;

namespace Bayat.Json
{

    public class CreateObjectConverterWindow : EditorWindowWrapper
    {

        public const int TypesPerPage = 20;

        private static CreateObjectConverterWindow instance;

        public static CreateObjectConverterWindow Instance
        {
            get
            {
                return instance;
            }
        }

        public const string TemplateFilePath = "Bayat/Json/Editor/ObjectJsonConverterTemplate.cs";

        public const string WritePropertyFormat =
@"            writer.WriteProperty(""{0}"", instance.{0});";

        public const string SerializePropertyFormat =
@"            internalWriter.SerializeProperty(writer, ""{0}"", instance.{0});";

        public const string ReadPropertyFormat =
@"				case ""{0}"":
				    instance.{0} = reader.ReadProperty<{1}>();
                    break;";

        public const string DeserializePropertyFormat =
@"				case ""{0}"":
				    instance.{0} = internalReader.DeserializeProperty<{1}>(reader);
				    break;";

        protected string[] assemblyNames;
        protected Assembly[] availableAssemblies;
        protected string[] typeNames;
        protected Type[] availableTypes;
        protected List<Assembly> filteredAssemblies = new List<Assembly>();
        protected List<Type> filteredTypes = new List<Type>();

        protected readonly string[] tabs = new string[] { "Assemblies", "Types" };
        protected int selectedTabIndex;
        protected int selectedAssemblyIndex = -1;
        protected Assembly selectedAssembly;
        protected int selectedTypeIndex = -1;
        protected Type selectedType;

        protected string assembliesSearchText = string.Empty;
        protected string typesSearchText = string.Empty;
        protected string converterFolderPath = "Assets/Scripts/Generated/Converters";
        protected string converterFileName = string.Empty;

        protected Vector2 contentScrollPosition;
        protected Vector2 typeInfoScrollPosition;
        protected Vector2 assemblyInfoScrollPosition;

        protected JsonConverter[] availableConverters;
        protected bool importScript = true;
        protected bool refreshAssetDatabase = true;
        protected bool showEditorAssemblies = false;

        protected int typesCount = 0;
        protected int typesPageCount = 0;
        protected int typesPageIndex = 0;
        protected int typesOffsetIndex = 0;

        //[InitializeOnLoadMethod]
        //private static void InitializeOnLoad()
        //{
        //    EditorApplication.update += OnEditorUpdate;
        //}

        //private static void OnEditorUpdate()
        //{
        //    EditorApplication.update -= OnEditorUpdate;
        //    var window = EditorWindow.GetWindow<CreateObjectConverterWindow>();
        //    if (window != null)
        //    {
        //        window.Initialize();
        //    }
        //}

        private void Initialize()
        {
            GetAssemblies();
            GetTypes();
            this.availableConverters = JsonSerializer.GetAllAvailableConverters();
        }

        [MenuItem("Window/Bayat/Json/Create Object Converter")]
        public static void InitWindow()
        {
            var window = new CreateObjectConverterWindow();
            window.Show();
        }

        protected override void ConfigureWindow()
        {
            window.titleContent = new GUIContent("Create Object Converter");
            window.minSize = window.maxSize = new Vector2(800, 500);
        }

        public new void Show()
        {
            if (window == null)
            {
                ShowUtility();
                window.Center();
            }
            else
            {
                window.Focus();
            }
        }

        public override void OnShow()
        {
            instance = this;
            this.converterFolderPath = EditorPrefs.GetString("bayat.json.createobjectconverter.containerfolderpath", "Assets/Scripts/Generated/Converters");
            this.importScript = EditorPrefs.GetBool("bayat.json.createobjectconverter.importscript");
            this.refreshAssetDatabase = EditorPrefs.GetBool("bayat.json.createobjectconverter.refreshassetdatabase");
            this.showEditorAssemblies = EditorPrefs.GetBool("bayat.json.createobjectconverter.showeditorassemblies");
        }

        public override void OnClose()
        {
            EditorPrefs.SetString("bayat.json.createobjectconverter.containerfolderpath", this.converterFolderPath);
            EditorPrefs.SetBool("bayat.json.createobjectconverter.importscript", this.importScript);
            EditorPrefs.SetBool("bayat.json.createobjectconverter.refreshassetdatabase", this.refreshAssetDatabase);
            EditorPrefs.SetBool("bayat.json.createobjectconverter.showeditorassemblies", this.showEditorAssemblies);
        }

        public override void OnGUI()
        {
            if (availableAssemblies == null || availableAssemblies.Length == 0)
            {
                GetAssemblies();
            }

            Rect position = this.window.position;
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.MinWidth(position.width / 2), GUILayout.MaxWidth(position.width), GUILayout.ExpandWidth(true));
            EditorGUI.BeginChangeCheck();
            this.selectedTabIndex = GUILayout.Toolbar(this.selectedTabIndex, this.tabs, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
            {
                GUI.FocusControl(null);
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.BeginChangeCheck();
            string searchText = "";
            switch (this.selectedTabIndex)
            {
                case 1:
                    searchText = this.typesSearchText;
                    break;
                default:
                    searchText = this.assembliesSearchText;
                    break;
            }
            searchText = EditorGUILayout.TextField(searchText, BayatEditorStyles.ToolbarSearchField);
            switch (this.selectedTabIndex)
            {
                case 1:
                    this.typesSearchText = searchText;
                    break;
                default:
                    this.assembliesSearchText = searchText;
                    break;
            }
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                // Remove focus if cleared
                switch (this.selectedTabIndex)
                {
                    case 1:
                        this.typesSearchText = string.Empty;
                        break;
                    default:
                        this.assembliesSearchText = string.Empty;
                        break;
                }
                GUI.FocusControl(null);
            }
            if (EditorGUI.EndChangeCheck())
            {
                GetFilteredAssemblies();
                GetFilteredTypes();
            }
            EditorGUILayout.EndHorizontal();

            this.contentScrollPosition = EditorGUILayout.BeginScrollView(this.contentScrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUI.skin.scrollView);
            switch (this.selectedTabIndex)
            {
                case 1:
                    OnTypesGUI();
                    break;
                default:
                    OnAssembliesGUI();
                    break;
            }
            OnTypesAndAssembliesGUI();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.MinWidth(position.width / 2), GUILayout.MaxWidth(position.width), GUILayout.ExpandWidth(true));

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, GUILayout.ExpandHeight(true));
            OnAssemblyInfoGUI();
            EditorGUILayout.EndVertical();

            DrawUILine(Color.grey);

            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, GUILayout.ExpandHeight(true));
            OnTypeInfoGUI();
            EditorGUILayout.EndVertical();

            DrawUILine(Color.grey);

            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, GUILayout.ExpandHeight(false));
            JsonConverter converter = GetAvailableConverter();
            if (converter != null)
            {
                EditorGUILayout.HelpBox("This type already have a converter which means if you create another converter out of this type there might be conflict issues.", MessageType.Warning);
            }
            this.converterFolderPath = EditorGUILayout.TextField("Folder", this.converterFolderPath);
            this.converterFileName = EditorGUILayout.TextField("File Name", this.converterFileName);
            EditorGUI.BeginChangeCheck();
            this.showEditorAssemblies = EditorGUILayout.ToggleLeft("Show Editor Assemblies", this.showEditorAssemblies);
            if (EditorGUI.EndChangeCheck())
            {
                GetAssemblies();
            }
            this.importScript = EditorGUILayout.ToggleLeft("Import Script", this.importScript);
            EditorGUILayout.BeginHorizontal();
            this.refreshAssetDatabase = EditorGUILayout.ToggleLeft("Refresh Asset Database", this.refreshAssetDatabase);
            EditorGUI.BeginDisabledGroup(EditorApplication.isUpdating);
            if (GUILayout.Button("Refresh", EditorStyles.miniButton))
            {
                AssetDatabase.Refresh();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(this.selectedType == null);
            if (GUILayout.Button("Create Converter", GUI.skin.button))
            {
                string fileName = this.converterFileName;
                if (!fileName.EndsWith(".cs"))
                {
                    fileName += ".cs";
                }
                string filePath = Path.Combine(this.converterFolderPath, fileName);
                CreateNewConverter(filePath, this.converterFileName, this.selectedType);
                if (this.importScript)
                {
                    AssetDatabase.ImportAsset(filePath);
                }
                if (this.refreshAssetDatabase)
                {
                    AssetDatabase.Refresh();
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Made with ❤️ by Bayat", EditorStyles.centeredGreyMiniLabel);
        }

        private void OnAssembliesGUI()
        {
            EditorGUI.BeginChangeCheck();
            this.selectedAssemblyIndex = GUILayout.SelectionGrid(this.selectedAssemblyIndex, this.assemblyNames, 1);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateSelectedAssembly();
                UpdateSelectedType();
            }
        }

        private void OnTypesGUI()
        {
            this.typesCount = this.typeNames.Length;
            this.typesPageCount = (this.typesCount + TypesPerPage - 1) / TypesPerPage;
            this.typesOffsetIndex = this.typesPageIndex * TypesPerPage;

            for (int i = this.typesOffsetIndex; i - this.typesOffsetIndex < TypesPerPage; i++)
            {
                if (i < this.typeNames.Length)
                {
                    if (GUILayout.Toggle(i == this.selectedTypeIndex, this.typeNames[i], GUI.skin.button))
                    {
                        this.selectedTypeIndex = i;
                        UpdateSelectedType();
                    }
                }
            }

            // Pagination
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            bool hasPrevPage = this.typesPageIndex - 1 >= 0;
            EditorGUI.BeginDisabledGroup(!hasPrevPage);
            if (GUILayout.Button("< Prev", EditorStyles.miniButton, GUILayout.Width(150)))
            {
                this.typesPageIndex--;
            }
            EditorGUI.EndDisabledGroup();

            GUI.SetNextControlName("PageIndex");
            EditorGUI.BeginChangeCheck();
            this.typesPageIndex = EditorGUILayout.IntField(this.typesPageIndex + 1, EditorStyles.numberField, GUILayout.Width(32)) - 1;
            if (EditorGUI.EndChangeCheck())
            {
                EditorGUI.FocusTextInControl("PageIndex");
                GUI.FocusControl("PageIndex");
            }
            GUILayout.Label("/" + this.typesPageCount.ToString());

            bool hasNextPage = this.typesPageIndex + 1 < this.typesPageCount;
            EditorGUI.BeginDisabledGroup(!hasNextPage);
            if (GUILayout.Button("Next >", EditorStyles.miniButton, GUILayout.Width(150)))
            {
                this.typesPageIndex++;
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            if (this.typesPageIndex < 0)
            {
                this.typesPageIndex = 0;
            }
            else if (this.typesPageIndex >= this.typesPageCount)
            {
                this.typesPageIndex = this.typesPageCount - 1;
            }
        }

        private void OnTypesAndAssembliesGUI()
        {
            if (this.selectedAssemblyIndex > this.filteredAssemblies.Count || this.selectedAssemblyIndex == -1 || this.filteredAssemblies.Count == 0)
            {
                this.selectedAssembly = null;
            }
            if ((this.selectedAssemblyIndex == -1 || this.selectedAssemblyIndex > this.filteredAssemblies.Count) && this.filteredAssemblies.Count > 0)
            {
                this.selectedAssemblyIndex = 0;
                this.selectedAssembly = this.filteredAssemblies[this.selectedAssemblyIndex];
                GetTypes();
            }
            if (this.selectedTypeIndex > this.filteredTypes.Count || this.selectedTypeIndex == -1 || this.filteredTypes.Count == 0)
            {
                this.selectedType = null;
            }
            if ((this.selectedTypeIndex == -1 || this.selectedTabIndex > this.filteredTypes.Count) && this.filteredTypes.Count > 0)
            {
                this.selectedTypeIndex = 0;
                this.selectedType = this.filteredTypes[this.selectedTypeIndex];
            }
        }

        private void UpdateSelectedAssembly()
        {
            if (this.selectedAssemblyIndex > this.filteredAssemblies.Count || this.selectedAssemblyIndex == -1 || this.filteredAssemblies.Count == 0)
            {
                this.selectedAssembly = null;
            }
            else
            {
                this.selectedAssembly = this.filteredAssemblies[this.selectedAssemblyIndex];
                GetTypes();
            }
        }

        private void UpdateSelectedType()
        {
            if (this.selectedTypeIndex > this.filteredTypes.Count || this.selectedTypeIndex == -1 || this.filteredTypes.Count == 0)
            {
                this.selectedType = null;
                if (this.selectedTypeIndex > this.filteredTypes.Count && this.filteredTypes.Count > 0)
                {
                    this.selectedTypeIndex = 0;
                    this.selectedType = this.filteredTypes[this.selectedTypeIndex];
                    this.converterFileName = this.selectedType.FullName.Replace(".", "") + "_Generated_Converter";
                }
            }
            else
            {
                this.selectedType = this.filteredTypes[this.selectedTypeIndex];
                this.converterFileName = this.selectedType.FullName.Replace(".", "") + "_Generated_Converter";
            }
        }

        private void OnAssemblyInfoGUI()
        {
            GUILayout.Label("Assembly Information", EditorStyles.boldLabel);
            if (this.selectedAssembly == null)
            {
                EditorGUILayout.HelpBox("Select an assembly to view its information here.", MessageType.Info);
                return;
            }
            this.assemblyInfoScrollPosition = EditorGUILayout.BeginScrollView(this.assemblyInfoScrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
            DrawInfoProperty("Assembly", this.assemblyNames[this.selectedAssemblyIndex]);
            EditorGUILayout.EndScrollView();
        }

        private void OnTypeInfoGUI()
        {
            GUILayout.Label("Type Information", EditorStyles.boldLabel);
            if (this.selectedType == null)
            {
                EditorGUILayout.HelpBox("Select a type to view its information here.", MessageType.Info);
                return;
            }
            JsonConverter converter = GetAvailableConverter();
            EditorGUILayout.BeginVertical();
            this.typeInfoScrollPosition = EditorGUILayout.BeginScrollView(this.typeInfoScrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
            DrawInfoProperty("Type", this.typeNames[this.selectedTypeIndex]);
            DrawInfoProperty("Has Converter", (converter != null).ToString());
            if (converter != null)
            {
                DrawInfoProperty("Converter", converter.GetType().FullName);
            }
            DrawInfoProperty("Is Abstract", this.selectedType.IsAbstract.ToString());
            DrawInfoProperty("Is Class", this.selectedType.IsClass.ToString());
            DrawInfoProperty("Is Generic Type", this.selectedType.IsGenericType.ToString());
            DrawInfoProperty("Is Interface", this.selectedType.IsInterface.ToString());
            DrawInfoProperty("Is Sealed", this.selectedType.IsSealed.ToString());
            DrawInfoProperty("Is Serializable", this.selectedType.IsSerializable.ToString());
            DrawInfoProperty("Is Value Type", this.selectedType.IsValueType.ToString());
            DrawInfoProperty("Has Converter", this.selectedType.IsValueType.ToString());
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        public static void DrawInfoProperty(string title, string text)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(title);
            EditorGUILayout.SelectableLabel(text, EditorStyles.toolbarTextField);
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public JsonConverter GetAvailableConverter()
        {
            if (this.selectedType == null)
            {
                return null;
            }
            if (this.availableConverters == null)
            {
                this.availableConverters = JsonSerializer.GetAllAvailableConverters();
            }
            return JsonSerializer.GetMatchingConverter(this.availableConverters, this.selectedType);
        }

        public void GetFilteredTypes()
        {
            this.filteredTypes.Clear();
            string searchPattern = this.typesSearchText.ToLowerInvariant();
            for (int i = 0; i < this.availableTypes.Length; i++)
            {
                Type type = this.availableTypes[i];
                if (string.IsNullOrEmpty(searchPattern) || type.FullName.ToLowerInvariant().Contains(searchPattern))
                {
                    this.filteredTypes.Add(type);
                }
            }
            this.typeNames = new string[this.filteredTypes.Count];
            for (int i = 0; i < this.filteredTypes.Count; i++)
            {
                Type type = this.filteredTypes[i];
                this.typeNames[i] = GetTypeFullName(type);
            }
        }

        public void GetFilteredAssemblies()
        {
            this.filteredAssemblies.Clear();
            string searchPattern = this.assembliesSearchText.ToLowerInvariant();
            for (int i = 0; i < this.availableAssemblies.Length; i++)
            {
                Assembly assembly = this.availableAssemblies[i];
                AssemblyName assemblyName = assembly.GetName();
                if (string.IsNullOrEmpty(searchPattern) || assemblyName.Name.ToLowerInvariant().Contains(searchPattern))
                {
                    this.filteredAssemblies.Add(assembly);
                }
            }
            this.assemblyNames = new string[this.filteredAssemblies.Count];
            for (int i = 0; i < this.filteredAssemblies.Count; i++)
            {
                Assembly assembly = this.filteredAssemblies[i];
                this.assemblyNames[i] = assembly.GetName().Name;
            }
        }

        public void GetTypes()
        {
            if (this.selectedAssembly == null)
            {
                return;
            }
            this.availableTypes = GetAssemblyValidTypes(this.selectedAssembly);
            GetFilteredTypes();
        }

        public void GetAssemblies()
        {
            var assemblies = Codebase.RuntimeAssemblies;
            List<Assembly> validAssemblies = new List<Assembly>();
            for (int i = 0; i < assemblies.Count; i++)
            {
                Assembly assembly = assemblies[i];
                if (GetAssemblyValidTypes(assembly).Length > 0)
                {
                    validAssemblies.Add(assembly);
                }
            }
            if (this.showEditorAssemblies)
            {
                for (int i = 0; i < Codebase.EditorAssemblies.Count; i++)
                {
                    var assembly = Codebase.EditorAssemblies[i];
                    if (GetAssemblyValidTypes(assembly).Length > 0)
                    {
                        validAssemblies.Add(assembly);
                    }
                }
            }
            this.availableAssemblies = validAssemblies.ToArray();
            GetFilteredAssemblies();
        }

        public static Type[] GetAssemblyValidTypes(Assembly assembly)
        {
            Type[] allTypes = assembly.GetTypes();
            List<Type> validTypes = new List<Type>();
            for (int i = 0; i < allTypes.Length; i++)
            {
                Type type = allTypes[i];
                if (type.IsVisible && !type.IsEnum && !typeof(Attribute).IsAssignableFrom(type))
                {
                    validTypes.Add(type);
                }
            }
            return validTypes.ToArray();
        }

        public static void CreateNewConverter(string filePath, string className, Type objectType)
        {
            JsonSerializer serializer = JsonSerializer.CreateDefault();
            JsonObjectContract contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(objectType);
            if (contract == null)
            {
                throw new NotSupportedException(string.Format("Cannot create a converetr for type: {0}", objectType));
            }
            TextAsset templateAsset = Resources.Load<TextAsset>(TemplateFilePath);
            string templateText = templateAsset.text;
            string typeName = GetTypeFullName(objectType);
            StringBuilder writeProperties = new StringBuilder();
            StringBuilder readProperties = new StringBuilder();
            string propertiesArray = '"' + string.Join("\", \"", contract.Properties) + '"';
            bool isFirst = true;
            foreach (var property in contract.Properties)
            {
                JsonContract propertyContract = property.PropertyContract;
                if (propertyContract == null)
                {
                    propertyContract = serializer.ContractResolver.ResolveContract(property.PropertyType);
                }
                string writeFormat = SerializePropertyFormat;
                string readFormat = DeserializePropertyFormat;
                if (propertyContract.ContractType == JsonContractType.Primitive && !property.PropertyType.IsEnum)
                {
                    writeFormat = WritePropertyFormat;
                    readFormat = ReadPropertyFormat;
                }
                string writeProperty = string.Format(writeFormat, property.PropertyName);
                string readProperty = string.Format(readFormat, property.PropertyName, GetTypeFullName(property.PropertyType));
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writeProperties.AppendLine();
                    readProperties.AppendLine();
                }
                writeProperties.Append(writeProperty);
                readProperties.Append(readProperty);
            }
            string contents = string.Format(templateText, className, typeName, writeProperties.ToString(), readProperties.ToString(), propertiesArray);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, contents);
        }

        public static string GetTypeFullName(Type type)
        {
            return type.GetFriendlyName();
        }

        public static class Styles
        {
            static Styles()
            {
                SelectedTypeButton = new GUIStyle(GUI.skin.button);
                SelectedTypeButton.normal = SelectedTypeButton.active;
            }

            public static readonly GUIStyle SelectedTypeButton;

        }

    }

}