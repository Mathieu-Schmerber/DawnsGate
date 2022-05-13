using Nawlian.Lib.Extensions;
using Nawlian.Lib.Utils.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Nawlian.Lib.Utils.Database.Editor
{
	public class SectionTreeView : TreeView
	{
		private GUIContent m_Title;
        public DatabaseData Data { get; set; }

        const int kColorTagWidth = 6;
        const int kFoldoutWidth = 14;
        public static readonly GUIContent PlusIcon = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add Section");

		private int _lastId { get; set; }

		public string Title
		{
			get => m_Title?.text;
			set => m_Title = new GUIContent(value);
		}

        public SectionTreeViewItem Selected => GetSelectedItems().FirstOrDefault();

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

        public SectionTreeView(string title, DatabaseData data) : base(new TreeViewState())
		{
			Title = title;
            Data = data;
			Reload();
		}

        #region Rendering

        private void DrawHeader(ref Rect rect)
        {
            var headerRect = rect;
            headerRect.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            rect.y += headerRect.height;
            rect.height -= headerRect.height;

            // Draw label.
            EditorGUI.LabelField(headerRect, m_Title, Styles.columnHeaderLabel);

            // Draw minus button.
            var buttonRect = headerRect;
            buttonRect.width = EditorGUIUtility.singleLineHeight;
            buttonRect.x += rect.width - buttonRect.width - EditorGUIUtility.standardVerticalSpacing;

            // Draw plus button.
            using (new EditorGUI.DisabledScope(false))
            {
                if (GUI.Button(buttonRect, PlusIcon, GUIStyle.none))
                {
                    Data.AddSection("NewSection");
                    Reload();
                    var newItem = rootItem.children.Last();
                    SetSelection(new List<int> { newItem.id }, TreeViewSelectionOptions.RevealAndFrame);
                    BeginRename(newItem);
                }
                buttonRect.x -= buttonRect.width + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        public override void OnGUI(Rect rect)
        {
            // Draw border rect.
            EditorGUI.LabelField(rect, GUIContent.none, Styles.backgroundWithBorder);
            rect.x += 1;
            rect.y += 1;
            rect.height -= 1;
            rect.width -= 2;

            DrawHeader(ref rect);

            base.OnGUI(rect);

            HandleKeyboardEvents(Event.current);
        }

		protected override void RowGUI(RowGUIArgs args)
		{
            var defaultColor = GUI.color;
			var item = args.item as SectionTreeViewItem;
			var isRepaint = Event.current.type == EventType.Repaint;

			// Color tag at beginning of line.
			var colorTagRect = EditorGUI.IndentedRect(args.rowRect);
			colorTagRect.x += kFoldoutWidth * item.depth - 5;
			colorTagRect.width = kColorTagWidth;
            if (isRepaint)
                Styles.yellowRect.Draw(colorTagRect, GUIContent.none, false, false, false, false);

            GUI.color = item.IsValid ? defaultColor : Color.red;

            // NOTE: When renaming, the renaming overlay gets drawn outside of our control so don't draw the label in that case
            //       as otherwise it will peak out from underneath the overlay.
            if (!args.isRenaming && isRepaint)
			{
				var text = item.displayName;
				var textRect = GetTextRect(args.rowRect, item);

				var style = args.selected ? Styles.selectedText : Styles.text;
				style.Draw(textRect, text, false, false, args.selected,
					args.focused);
			}

            // Bottom line.
            var lineRect = EditorGUI.IndentedRect(args.rowRect);
			lineRect.y += lineRect.height - 1;
			lineRect.height = 1;
			if (isRepaint)
				Styles.border.Draw(lineRect, GUIContent.none, false, false, false, false);

            var buttonRect = args.rowRect;
			buttonRect.x = buttonRect.width - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
			buttonRect.width = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			if (GUI.Button(buttonRect, PlusIcon, GUIStyle.none))
			{
				int predictedId = item.children?.Count > 0 ? item.id + GetCumulativeNumber(item) : item.id + 1;
				item.AddSubSection("NewSection");
                Reload();
				var newItem = FindItem(predictedId, rootItem);
				SetSelection(new List<int> { newItem.id }, TreeViewSelectionOptions.RevealAndFrame);
				BeginRename(newItem);
			}

            GUI.color = defaultColor;
        }

        #endregion

        #region Build

        private void BuildRecursiveTreeItems(DatabaseData.Section section, List<TreeViewItem> allItems, int depth, bool valid = true)
        {
            var sectionItem = new SectionTreeViewItem(section) { id = ++_lastId, displayName = section.Name, depth = depth, IsValid = valid };

            if (depth == 0)
                sectionItem.DeleteAtRootCallback = (DatabaseData.Section item) => Data.Sections.Remove(item);
            allItems.Add(sectionItem);
            foreach (var item in section.Sections)
                BuildRecursiveTreeItems(item, allItems, depth + 1, section.IsNameLegal(item.Name));
        }

        protected override TreeViewItem BuildRoot()
        {
            _lastId = 0;
            var root = new TreeViewItem { id = _lastId, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>();

            Data.Sections.ForEach(x => BuildRecursiveTreeItems(x, allItems, 0, Data.IsNameLegal(x.Name)));
            SetupParentsAndChildrenFromDepths(root, allItems);
            return root;
        }

        #endregion

        private Rect GetTextRect(Rect rowRect, TreeViewItem item, bool applyIndent = true)
        {
            var indent = kFoldoutWidth * (item.depth + 1);
            var textRect = applyIndent ? EditorGUI.IndentedRect(rowRect) : rowRect;
            textRect.x += indent;
            return textRect;
        }

        private int GetCumulativeNumber(TreeViewItem root)
		{
            int res = 0;

            if (root.children == null)
                return 1;
            res += 1;
			foreach (var item in root.children)
			{
                res += GetCumulativeNumber(item);
			}
            return res;
		}

        #region Rename

        protected override bool CanRename(TreeViewItem item) => true;

        protected override void RenameEnded(RenameEndedArgs args)
		{
            SectionTreeViewItem item = FindItem(args.itemID, rootItem) as SectionTreeViewItem;

            item.Rename(args.newName);
            Reload();
        }

        #endregion

        public IEnumerable<SectionTreeViewItem> GetSelectedItems()
        {
            foreach (var id in GetSelection())
            {
                if (FindItem(id, rootItem) is SectionTreeViewItem item)
                    yield return item;
            }
        }

        public void DeleteSelection()
		{
            IEnumerable<SectionTreeViewItem> selection = GetSelectedItems();

			foreach (SectionTreeViewItem item in selection)
                item.Delete();
            Reload();
		}

        public void HandleKeyboardEvents(Event uiEvent)
		{
            if (uiEvent.type == EventType.ValidateCommand)
            {
                switch (uiEvent.commandName)
                {
                    case "Delete":
                    case "SoftDelete":
                        DeleteSelection();
                        break;
                    default:
                        return;
                }
                uiEvent.Use();
            }
        }
    }

    public class SectionTreeViewItem : TreeViewItem
    {
        private DatabaseData.Section _section;
        public Action<DatabaseData.Section> DeleteAtRootCallback;

        public bool IsValid;

        public DatabaseData.Section Data => _section;

		public SectionTreeViewItem(DatabaseData.Section section) : base()
		{
            IsValid = true;
            _section = section;
        }

        public void Rename(string name) => _section.Name = name.ToPascalCase();

        public void AddSubSection(string name)
		{
            _section.AddSection(name);
		}

        public void Delete()
		{
            SectionTreeViewItem root = parent as SectionTreeViewItem;

            if (root == null && DeleteAtRootCallback != null)
                DeleteAtRootCallback(_section);
            else
                root._section.Sections.Remove(_section);
        }
	}
}