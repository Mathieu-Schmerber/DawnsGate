using Nawlian.Lib.Extensions;
using Nawlian.Lib.Utils.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using GUIHelper = Nawlian.Lib.Utils.Editor.GUIHelper;

namespace Nawlian.Lib.Utils.Database.Editor
{
    public class DatabaseAssetPanel
    {
        private GUIContent m_Title;
        public static readonly GUIContent PlusIcon = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add Asset");

        public string Title
        {
            get => m_Title?.text;
            set => m_Title = new GUIContent(value);
        }

        public static class Styles
        {
            private static GUIStyle StyleWithBackground(string fileName)
            {
                return new GUIStyle("Label").WithNormalBackground(GUIHelper.LoadTexture($"{fileName}"));
            }

            public static readonly GUIStyle yellowRect = StyleWithBackground("yellow.png");
            public static readonly GUIStyle greenRect = StyleWithBackground("green.png");
            public static readonly GUIStyle blueRect = StyleWithBackground("blue.png");
            public static readonly GUIStyle pinkRect = StyleWithBackground("pink.png");

            public static readonly GUIStyle text = new GUIStyle("Label").WithAlignment(TextAnchor.MiddleLeft);
            public static readonly GUIStyle selectedText = new GUIStyle("Label").WithAlignment(TextAnchor.MiddleLeft).WithNormalTextColor(Color.white);
            public static readonly GUIStyle redText = new GUIStyle("Label").WithAlignment(TextAnchor.MiddleLeft).WithNormalTextColor(Color.red);
            public static readonly GUIStyle backgroundWithoutBorder = new GUIStyle("Label")
                .WithNormalBackground(GUIHelper.LoadTexture("actionTreeBackgroundWithoutBorder.png"));
            public static readonly GUIStyle border = new GUIStyle("Label")
                .WithNormalBackground(GUIHelper.LoadTexture("actionTreeBackground.png"))
                .WithBorder(new RectOffset(0, 0, 0, 1));
            public static readonly GUIStyle backgroundWithBorder = new GUIStyle("Label")
                .WithNormalBackground(GUIHelper.LoadTexture("actionTreeBackground.png"))
                .WithBorder(new RectOffset(3, 3, 3, 3))
                .WithMargin(new RectOffset(4, 4, 4, 4));
            public static readonly GUIStyle columnHeaderLabel = new GUIStyle(EditorStyles.toolbar)
                .WithAlignment(TextAnchor.MiddleLeft)
                .WithFontStyle(FontStyle.Bold)
                .WithPadding(new RectOffset(10, 6, 0, 0));
        }

        private ReorderableList _reorderableList;
        private DatabaseData.Section _currentSection;
        private DatabaseData.DatabaseAsset _selectedAsset = null;
        private string _loadFolderPath;
        private Vector2 _scrollPosition = Vector2.zero;

        private readonly int LIST_ELEMENT_HEIGHT = 23;
        private readonly int SCROLLBAR_W = 13;

        public DatabaseAssetPanel(string title)
        {
            Title = title;
        }

        private void UpdateSection(DatabaseData.Section section)
        {
            _currentSection = section;
            _reorderableList = new ReorderableList(section.Assets, typeof(DatabaseData.DatabaseAsset), true, false, false, false);
            _reorderableList.drawElementCallback = DrawListElement;
            _reorderableList.onSelectCallback = (ReorderableList list) => _selectedAsset = section.Assets[list.index];
        }

        private void DrawHeader(ref Rect rect)
        {
            var headerRect = rect;
            headerRect.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            rect.y += headerRect.height;
            rect.height -= headerRect.height;

            // Draw label.
            EditorGUI.LabelField(headerRect, m_Title, Styles.columnHeaderLabel);

            // Draw plus button.
            var buttonRect = headerRect;
            buttonRect.width = EditorGUIUtility.singleLineHeight;
            buttonRect.x += rect.width - buttonRect.width - EditorGUIUtility.standardVerticalSpacing;
            using (new EditorGUI.DisabledScope(_currentSection == null))
            {
                if (GUI.Button(buttonRect, PlusIcon, GUIStyle.none))
                    _currentSection.Assets.Add(new DatabaseData.DatabaseAsset());
                buttonRect.x -= buttonRect.width + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            DatabaseData.DatabaseAsset asset = _currentSection.Assets[index];
            const int SPACING = 10;
            var oldPrefab = asset.Prefab;
            Color currentColor = GUI.color;

            GUI.color = _currentSection.IsAssetLegal(asset) ? currentColor : Color.red;

            // Boxing
            rect.y += (rect.height - EditorGUIUtility.singleLineHeight) / 2; // Center y
            Rect nameRect = new Rect(rect.position, new Vector2(rect.width * 0.5f, EditorGUIUtility.singleLineHeight));
            rect.x += nameRect.width + SPACING;
            Rect objectRect = new Rect(rect.position, new Vector2(rect.width * 0.5f - SPACING, EditorGUIUtility.singleLineHeight));

            // Fields
            asset.Name = SirenixEditorFields.TextField(nameRect, GUIContent.none, asset.Name)?.ToPascalCase();
            asset.Prefab = SirenixEditorFields.UnityObjectField(objectRect, GUIContent.none, asset.Prefab, typeof(UnityEngine.Object), false);

            // Handling changes
            if (oldPrefab != asset.Prefab && asset.Prefab != null && string.IsNullOrEmpty(asset.Name))
                asset.Name = asset.Prefab.name.ToPascalCase();

            GUI.color = currentColor;
        }

        private void DrawSectionBody(DatabaseData.Section section, Rect rect)
        {
            int btnWidth = 150;
            int btnPadding = 10;
            Vector2 folderFieldSize = new Vector2(rect.width - btnWidth - btnPadding, EditorGUIUtility.singleLineHeight);
            Rect loadBtnRect = new Rect(rect.x + folderFieldSize.x + btnPadding, rect.y, btnWidth, EditorGUIUtility.singleLineHeight);

            _loadFolderPath = SirenixEditorFields.FolderPathField(new Rect(rect.position, folderFieldSize), _loadFolderPath, null, false, true);
            if (GUI.Button(loadBtnRect, "Load folder content"))
                LoadFolderContent(_loadFolderPath);

            rect.y += loadBtnRect.height;
            rect.height -= loadBtnRect.height;
            _scrollPosition = GUI.BeginScrollView(rect, _scrollPosition, new Rect(rect.position, new Vector2(rect.width - SCROLLBAR_W, LIST_ELEMENT_HEIGHT * _reorderableList.count + 10)), false, false);
            rect.width -= SCROLLBAR_W;
            _reorderableList.DoList(rect);
            GUI.EndScrollView(true);
        }

        public void OnGUI(Rect rect, DatabaseData.Section section)
        {
            // Draw border rect.
            EditorGUI.LabelField(rect, GUIContent.none, Styles.backgroundWithBorder);
            rect.x += 1;
            rect.y += 1;
            rect.height -= 1;
            rect.width -= 2;

            DrawHeader(ref rect);

            if (section != null)
            {
                if (section != _currentSection)
                    UpdateSection(section);
                DrawSectionBody(section, rect);
                HandleKeyboardEvents(Event.current);
            }
        }

        public void HandleKeyboardEvents(Event uiEvent)
        {
            if (uiEvent.type == EventType.KeyUp)
            {
                switch (uiEvent.keyCode)
                {
                    case KeyCode.Delete:
                    case KeyCode.Backspace:
                        DeleteSelection();
                        break;
                    default:
                        return;
                }
                uiEvent.Use();
            }
        }

        private void LoadFolderContent(string path)
		{
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(ScriptableObject).Name, new string[1] { path });

            for (int i = 0; i < guids.Length; i++)
            {
                string sub = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(sub);

                if (_currentSection.Assets.Any(x => x.Prefab == asset))
                    continue;
                _currentSection.Assets.Add(new DatabaseData.DatabaseAsset() { Name = asset.name, Prefab = asset});
            }
        }

        private void DeleteSelection()
        {
            if (_currentSection != null && _selectedAsset != null)
                _currentSection.Assets.Remove(_selectedAsset);
        }
    }
}