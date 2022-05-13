using Nawlian.Lib.Utils.Editor;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
using GUIHelper = Nawlian.Lib.Utils.Editor.GUIHelper;

namespace Nawlian.Lib.Utils.Database.Editor
{
    public class DatabaseDataEditorWindow : EditorWindow
    {
        private DatabaseData _data;
        private SectionTreeView _sectionTree;
        private DatabaseAssetPanel _assetPanel;

        private static bool _isRefreshPending;

        #region Static access

        /// <summary>
        /// Open window if someone clicks on an asset or an action inside of it.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "line", Justification = "line parameter required by OnOpenAsset attribute")]
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceId);
            var asset = obj as DatabaseData;

            if (asset == null)
                return false;

            var window = OpenEditor(asset);

            return true;
        }

        public static DatabaseDataEditorWindow FindEditorForAsset(DatabaseData asset)
        {
            var windows = Resources.FindObjectsOfTypeAll<DatabaseDataEditorWindow>();
            return windows.FirstOrDefault(x => x._data == asset || x._data == null);
        }

        /// <summary>
        /// Open the specified <paramref name="asset"/> in an editor window. Used when someone hits the "Edit Asset" button in the
        /// importer inspector.
        /// </summary>
        /// <param name="asset">The asset to open.</param>
        /// <returns>The editor window.</returns>
        public static DatabaseDataEditorWindow OpenEditor(DatabaseData asset)
        {
            // See if we have an existing editor window that has the asset open.
            var window = FindEditorForAsset(asset);
            if (window == null)
                window = CreateWindow<DatabaseDataEditorWindow>();
            window.SetAsset(asset);
            window.Show();
            window.Focus();
            return window;
        }

        #endregion

        #region Initialization

        private void SetAsset(DatabaseData asset)
		{
            if (asset == null)
                return;

            _data = asset;
#pragma warning disable CS0618 // Type or member is obsolete
			title = asset.name;
#pragma warning restore CS0618 // Type or member is obsolete

			_sectionTree = new SectionTreeView("Sections", _data);
            _assetPanel = new DatabaseAssetPanel("Assets");
        }

        /// <summary>
        /// Load window from save
        /// </summary>
		private void OnEnable()
		{
            if (_data != null)
                return;
            string path = EditorPrefs.GetString("DatabaseDataEditorWindow", null);

            if (path != null)
                SetAsset(Resources.Load<DatabaseData>(path));
        }

        /// <summary>
        /// Save window for next engine opening
        /// </summary>
		protected void OnDisable()
        {
            if (_data != null)
                EditorPrefs.SetString("DatabaseDataEditorWindow", _data.AssetPathFromResources);
        }

        #endregion

        #region Rendering

        private void OnGUI() => Draw();

        public void DrawAssets(Rect rect)
		{
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(rect.width), GUILayout.MinHeight(rect.height), GUILayout.MaxHeight(rect.height));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            var columnRect = GUILayoutUtility.GetLastRect();

            _assetPanel.OnGUI(columnRect, _sectionTree.Selected?.Data);
        }

        public void DrawSections(Rect rect)
		{
            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(rect.width), GUILayout.MinHeight(rect.height), GUILayout.MaxHeight(rect.height));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            var columnRect = GUILayoutUtility.GetLastRect();

            _sectionTree.OnGUI(columnRect);
        }

        public void Draw()
        {
            if (_data == null)
                return;
            else if (_sectionTree == null || _assetPanel == null)
                SetAsset(_data);

            const int PADDING = 10;
            const int TOOLBAR_H = 22;

            SirenixEditorGUI.BeginHorizontalToolbar(TOOLBAR_H);
            if (SirenixEditorGUI.ToolbarButton("Save Asset"))
                DatabaseDataEditor.SaveAsset(_data);
            if (SirenixEditorGUI.ToolbarButton("Locate Asset"))
            {
                Selection.objects = new Object[] { _data };
                EditorGUIUtility.PingObject(_data);
            }
            GUILayout.Label($"Class file: {_data.DatabaseClassPath}");
            SirenixEditorGUI.EndHorizontalToolbar();

            GUILayout.Space(PADDING);
            
            GUILayout.BeginHorizontal();
			{
                GUILayout.Space(PADDING);
                DrawSections(new Rect(PADDING, PADDING + TOOLBAR_H, position.width * 0.4f - PADDING, position.height - PADDING * 2 - TOOLBAR_H));
                GUILayout.Space(PADDING);
                DrawAssets(new Rect(PADDING + position.width * 0.4f, PADDING + TOOLBAR_H, position.width * 0.6f - PADDING * 2, position.height - PADDING * 2 - TOOLBAR_H));
            }
            GUILayout.EndHorizontal();
        }

        #endregion
    }
}