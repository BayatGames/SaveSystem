using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Bayat.Core.EditorWindows
{

    public class PrefabReferenceManagerWindow : EditorWindowWrapper
    {

        public const int ItemsPerPage = 17;

        protected Vector2 scrollPosition;
        protected int removeIndex = -1;
        protected string removeKey = string.Empty;

        protected int itemsCount = 0;
        protected int pageCount = 0;
        protected int pageIndex = 0;
        protected int offsetIndex = 0;

        [MenuItem("Window/Bayat/Core/Prefab Reference Manager")]
        private static void Initialize()
        {
            var window = new PrefabReferenceManagerWindow();
            window.Show();
        }

        protected override void ConfigureWindow()
        {
            window.titleContent = new GUIContent("Prefab Reference Manager");
            window.minSize = window.maxSize = new Vector2(630, 400);
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

        public override void OnGUI()
        {
            GUILayout.Label("Prefab Reference <b>Manager</b>", Styles.CenteredLargeLabel);
            var referenceResolver = PrefabReferenceResolver.Current;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset", EditorStyles.miniButton))
            {
                if (EditorUtility.DisplayDialog("Reset Prefab Reference Database?", "This action will reset whole prefab reference database and used GUIDs which makes the saved GUIDs obsolete, so there will be problems when loading previously saved data using this database.\n\nProceed at your own risk.", "Reset", "Cancel"))
                {
                    referenceResolver.GuidToReference.Clear();
                    referenceResolver.ReferenceToGuid.Clear();
                    referenceResolver.Reset();
                }
            }
            if (GUILayout.Button("Refresh References", EditorStyles.miniButton))
            {
                referenceResolver.RefreshDependencies();
            }

            EditorGUILayout.EndHorizontal();

            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);

            if (!string.IsNullOrEmpty(this.removeKey))
            {
                referenceResolver.GuidToReference.Remove(this.removeKey);
                this.removeKey = string.Empty;
            }

            if (referenceResolver.GuidToReference.Count == 0)
            {
                GUILayout.Label("There are no references in the database", EditorStyles.centeredGreyMiniLabel);
            }

            this.itemsCount = referenceResolver.GuidToReference.Count;
            if (this.itemsCount > 0)
            {
                this.pageCount = (this.itemsCount + ItemsPerPage - 1) / ItemsPerPage;
                this.offsetIndex = this.pageIndex * ItemsPerPage;

                for (int i = this.offsetIndex; i - this.offsetIndex < ItemsPerPage; i++)
                {
                    if (i < referenceResolver.GuidToReference.Count)
                    {
                        EditorGUILayout.BeginHorizontal();
                        var item = referenceResolver.GuidToReference.ElementAt(i);
                        EditorGUILayout.SelectableLabel(item.Key, EditorStyles.textField, GUILayout.Height(16));
                        referenceResolver.GuidToReference[item.Key] = EditorGUILayout.ObjectField(item.Value, typeof(UnityEngine.Object), false);
                        if (GUILayout.Button("Remove", EditorStyles.miniButtonRight))
                        {
                            if (EditorUtility.DisplayDialog("Remove Reference?", "This action will remove the asset reference from the database and used GUID which makes the saved GUID obsolote, so there will be problems when loading previously saved data using this database.\n\nProceed at your own risk.", "Remove", "Cancel"))
                            {
                                this.removeKey = item.Key;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            // Pagination
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            bool hasPrevPage = this.pageIndex - 1 >= 0;
            EditorGUI.BeginDisabledGroup(!hasPrevPage);
            if (GUILayout.Button("< Prev", EditorStyles.miniButton, GUILayout.Width(150)))
            {
                this.pageIndex--;
            }
            EditorGUI.EndDisabledGroup();

            GUI.SetNextControlName("PageIndex");
            EditorGUI.BeginChangeCheck();
            this.pageIndex = EditorGUILayout.IntField(this.pageIndex + 1, EditorStyles.numberField, GUILayout.Width(32)) - 1;
            if (EditorGUI.EndChangeCheck())
            {
                EditorGUI.FocusTextInControl("PageIndex");
                GUI.FocusControl("PageIndex");
            }
            GUILayout.Label("/" + this.pageCount.ToString());

            bool hasNextPage = this.pageIndex + 1 < this.pageCount;
            EditorGUI.BeginDisabledGroup(!hasNextPage);
            if (GUILayout.Button("Next >", EditorStyles.miniButton, GUILayout.Width(150)))
            {
                this.pageIndex++;
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            if (this.pageCount <= 0)
            {
                this.pageIndex = 0;
            }
            else if (this.pageIndex < 0)
            {
                this.pageIndex = 0;
                EditorGUI.FocusTextInControl("PageIndex");
                GUI.FocusControl("PageIndex");
            }
            else if (this.pageIndex >= this.pageCount)
            {
                this.pageIndex = this.pageCount - 1;
                EditorGUI.FocusTextInControl("PageIndex");
                GUI.FocusControl("PageIndex");
            }

            GUILayout.Label("Made with ❤️ by Bayat", EditorStyles.centeredGreyMiniLabel);
        }

        public static class Styles
        {
            static Styles()
            {
                CenteredLargeLabel = new GUIStyle(EditorStyles.largeLabel);
                CenteredLargeLabel.alignment = TextAnchor.MiddleCenter;
                CenteredLargeLabel.richText = true;
            }

            public static readonly GUIStyle CenteredLargeLabel;

        }

    }

}