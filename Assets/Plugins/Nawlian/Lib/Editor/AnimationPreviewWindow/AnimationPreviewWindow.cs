using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.Utilities.Editor;
using UnityEditor;
using System;
using Nawlian.Lib.Extensions;

namespace Nawlian.Lib.EditorTools.AnimationPreviewWindow
{
    public class AnimationPreviewWindow<TEnum> : PreviewEditorWindow<AnimationPreview, AnimationClip>
		where TEnum : Enum
    {
        internal class IndexRangePair
		{
			public int Index { get; set; }
			public float Range { get; set; }

            public IndexRangePair(int index, float range)
			{
                Index = index;
                Range = range;
			}
		}

		protected string AnimationCallback { get; private set; }

		#region Editor fields

		private List<AnimationEvent> _events = new List<AnimationEvent>();
		private Vector2 _scoll;

        #endregion

        public static void OpenWindow(AnimationClip clip, string animationEventCallback, GameObject animatedObject = null)
        {
            AnimationPreviewWindow<TEnum> window = CreateWindow<AnimationPreviewWindow<TEnum>>();

			window.AnimationCallback = animationEventCallback;
			window.DataPreview = clip;
			window.PreviewGameObject = animatedObject;
			window.SaveEvents();
            window.Show();
			window.Focus();
		}

		private void SaveEvents()
		{
			_events = DataPreview.events.ToList();
		}

		private bool HasOrderChanged()
		{
			if (_events.Count != DataPreview.events.Length)
				return true;

			string stored = string.Concat(_events.Select(evt => $"{evt.stringParameter}:{evt.time};"));
			string current = string.Concat(DataPreview.events.Select(evt => $"{evt.stringParameter}:{evt.time};"));

			return (stored != current);
		}

		private void RenderEvent(AnimationEvent animationEvent, ref int index)
		{
			List<string> events = Enum.GetNames(typeof(TEnum)).ToList();
			Color color = GUI.backgroundColor;
			bool remove;
			int select = events.IndexOf(animationEvent.stringParameter);

			GUI.backgroundColor = ColorExtensions.RandomColor(events[select]);
			SirenixEditorGUI.BeginInlineBox();
				GUILayout.BeginHorizontal();
					GUILayout.BeginVertical();
						GUI.backgroundColor = color;
						select = SirenixEditorFields.Dropdown("Event type", select, events.ToArray());
						animationEvent.stringParameter = events[select];
						animationEvent.time = SirenixEditorFields.RangeFloatField("Play At", animationEvent.time, 0, DataPreview.length);
					GUILayout.EndVertical();
					remove = SirenixEditorGUI.IconButton(EditorGUIUtility.IconContent("d_winbtn_mac_close_h@2x").image, width: 30, height: 30, tooltip: "delete");
				GUILayout.EndHorizontal();
			SirenixEditorGUI.EndInlineBox();

			if (remove)
			{
				_events.Remove(animationEvent);
				index--;
			}
		}

		protected override void OnGUI()
        {
			RenderPreview(GUILayoutUtility.GetRect(256, 256));

			GUILayout.Space(10);
			SirenixEditorGUI.Title("Animation Events", "", TextAlignment.Left, true);
			GUILayout.Space(10);

			if (GUILayout.Button("Add Event Keyframe"))
			{
				_events.Add(new AnimationEvent() { 
					functionName = AnimationCallback, 
					stringParameter = Enum.GetNames(typeof(TEnum))[0],
					time = PreviewEditor.CurrentTime
				});
				_events = _events.OrderBy(x => x.time).ToList();
				AnimationUtility.SetAnimationEvents(DataPreview, _events.ToArray());
			}
			_scoll = GUILayout.BeginScrollView(_scoll);
			for (int i = 0; i < _events.Count; i++)
				RenderEvent(_events[i], ref i);
			GUILayout.EndScrollView();

			// Update animation clip data WITHOUT updating the GUI list order
			AnimationUtility.SetAnimationEvents(DataPreview, _events.ToArray());

			// Update the GUI list order, if mouse button is up after sliding the values
			// MouseMove = Not using UI
			if (HasOrderChanged() && Event.current.type == EventType.MouseMove)
				SaveEvents();
			Repaint();
		}
	}
}