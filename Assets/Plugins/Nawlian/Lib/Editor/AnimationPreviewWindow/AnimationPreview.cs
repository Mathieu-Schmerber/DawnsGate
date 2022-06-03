using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using StartAssets.PowerfulPreview;
using StartAssets.PowerfulPreview.Controls;
using UnityEditor;

namespace Nawlian.Lib.EditorTools.AnimationPreviewWindow 
{
	public class AnimationPreview : EmbeddedPreview<AnimationClip>
	{
		#region Properties

		private PreviewAnimator _previewAnimator;
		private Timeline _timeline;
		private AnimationEventDrawer _eventDrawer;

		public float CurrentTime { get => _timeline.CurTime; }

		#endregion

		public override void OnCreate(AnimationClip data, GameObject previewGO = null)
		{
			base.OnCreate(data, previewGO);
			_previewAnimator = new PreviewAnimator(Preview);
			_previewAnimator.Animation = data;

			_eventDrawer = new AnimationEventDrawer();
			_eventDrawer.Clip = data;
			_timeline = new Timeline();
			_timeline.Visible = data != null;
			_timeline.AddDrawer(_eventDrawer, 0);
			Preview.AddControl(_timeline);
			if (data != null)
			{
				_timeline.EndTime = data.length;
				_timeline.Framerate = data.frameRate;
			}
			SetupScene();
		}

		protected override void OnGUIUpdate()
		{
			_eventDrawer.Clip = Data;
			serializedObject.Update();
			//serializedObject.ApplyModifiedProperties();
			_timeline.EndTime = Data.length;
			_timeline.Framerate = Data.frameRate;
			if (_timeline != null) _timeline.Visible = Data != null;
			serializedObject.ApplyModifiedProperties();
		}

		protected override void OnPreviewUpdate()
		{
			if (_previewAnimator == null || _timeline == null)
			{
				return;
			}
			_previewAnimator.SampleAnimation(_timeline.CurTime);
		}

		public override void OnPreviewSettings()
		{
			if (_timeline != null)
			{
				var style = new GUIStyle(EditorStyles.boldLabel);
				style.normal.textColor = Color.white;
				style.fontSize = 11;
				EditorGUILayout.LabelField("Skeleton visible: ", style, GUILayout.Width(100));
				float playSpeed = _timeline.Speed;
				EditorGUILayout.LabelField("Speed: ", style, GUILayout.Width(50));
				playSpeed = GUILayout.HorizontalSlider(playSpeed, 0.0f, 2.0f, GUILayout.Width(100));
				EditorGUILayout.LabelField(playSpeed.ToString("F2"), style, GUILayout.Width(30));
				_timeline.Speed = playSpeed;
			}
		}

		protected override void SetupScene()
		{
			if (!Preview.CanInstantiate || PreviewGameObject == null || _previewAnimator == null)
				return;
			_previewAnimator.Character = PreviewGameObject;
			_previewAnimator.Character.transform.position = Vector3.zero;
			_previewAnimator.Character.transform.eulerAngles = Vector3.zero;
			Preview.Camera.nearClipPlane = 0.01f;
			Preview.Camera.transform.position = new Vector3(5, 3.5f, 5);
			Preview.Camera.transform.rotation = Quaternion.Euler(new Vector3(20, -135, 0));
			Preview.Camera.orthographic = true;
			Preview.Camera.orthographicSize = 2;
			_previewAnimator.Animation = Data;
		}
	}
}