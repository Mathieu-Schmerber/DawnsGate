using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using StartAssets.PowerfulPreview;
using UnityEditor;

namespace Nawlian.Lib.EditorTools.AnimationPreviewWindow
{
	/// <summary>
	/// Defines a generic embbed preview
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class EmbeddedPreview<T> : UnityEditor.Editor
		where T : Object
	{
		#region Properties

		public Preview Preview { get; private set; }
		public GameObject PreviewGameObject { get; set; }
		public T Data { get; set; }

		#endregion

		protected abstract void OnPreviewUpdate();
		protected abstract void OnGUIUpdate();
		protected abstract void SetupScene();


		public sealed override bool RequiresConstantRepaint() => true;
		public sealed override bool HasPreviewGUI() => true;

		public override void OnPreviewSettings() { }

		public virtual void OnCreate(T data, GameObject previewGO = null)
		{
			Data = data;
			Preview = Preview.Create(this);
			if (previewGO)
			{
				PreviewGameObject = previewGO;
				SetupScene();
				Preview.CameraController.Target = previewGO.transform.position;
			}
			Preview.SetButtonsEnabled(true);
			Preview.CameraController.SetStatesEnabled(true);
		}

		public sealed override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (Preview == null || r.width <= 2 || r.height <= 2) return;

			Preview.SetSurfaceRect(r);
			Preview.Update();
			OnPreviewUpdate();
		}

		public override sealed void OnInspectorGUI()
		{
			if (Preview == null)
				return;
			serializedObject.Update();
			Handles.BeginGUI();
			OnGUIUpdate();
			Handles.EndGUI();
			serializedObject.ApplyModifiedProperties();
			Repaint();
		}

		protected virtual void OnDisable()
		{
			Preview?.Dispose();
		}
	}
}