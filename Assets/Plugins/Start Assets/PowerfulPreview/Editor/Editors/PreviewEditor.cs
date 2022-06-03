using UnityEditor;
using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// It's a ready to use class for any custom editor which is going 
    /// to utilize the preview. 
    /// </summary>
    public abstract class PreviewEditor<T> : Editor where T : UnityEngine.Object
	{
        /// <summary>
        /// Preview instance created for this editor. 
        /// </summary>
        public Preview preview { private set; get;  }
        /// <summary>
        /// Asset assigned for this editor.
        /// </summary>
        public T asset { private set;  get;  }

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			//TODO: is needed to draw unity gizmos inside the preview, but it's still work in progress 
			if (preview == null) OnEnable();
			if (preview == null || r.width <= 2 || r.height <= 2) return;

			preview.SetSurfaceRect(r);
			preview.Update();
			OnPreviewUpdate();
		}
		public override bool HasPreviewGUI()
		{
			return true;
		}

		public override sealed void OnInspectorGUI()
		{
			if (preview == null)
			{
				return;
			}
			serializedObject.Update();
			Handles.BeginGUI();
			OnGUIUpdate();
			Handles.EndGUI();
			serializedObject.ApplyModifiedProperties();
			Repaint();
		}

		protected virtual void OnEnable()
        {
            preview = Preview.Create( this );
            if( preview == null )
            {
                return; 
            }
            asset = target as T;
            SetupPreviewCamera();
            OnCreate();
        }
        protected virtual void OnDisable()
        {
            preview?.Dispose();
            preview = null;
        }

        /// <summary> 
        ///Is called when editor has initialized all necessary data. 
        ///Put your asset initializing code here. 
        /// </summary>
        protected abstract void OnCreate();
        /// <summary>
        /// Is called when editor checks all necessary data, and prepares inspector to work.  
        /// Put your asset inspector GUI update code here. 
        /// </summary>
        protected abstract void OnGUIUpdate();
        /// <summary>
        /// Is called when the preview is updated OnInterfactivePreviewGUI method call. 
        /// </summary>
        protected virtual void OnPreviewUpdate() { }

        /// <summary>
        /// Sets up predefined properties to the preview's camera. 
        /// The camera is set up to look like the original Unity Engine preview camera. 
        /// Override this method to extend the settings, or to recreate them from scratch.  
        /// </summary>
        protected virtual void SetupPreviewCamera()
        {
            var previewCamera = preview.Camera;
            if (previewCamera.orthographic)
            {
                previewCamera.fieldOfView = 1.0f;
                previewCamera.transform.eulerAngles = Quaternion.LookRotation(Vector3.back).eulerAngles;
                previewCamera.transform.position = new Vector3(0, 0.9f, 1);
            }
            else
            {
                previewCamera.fieldOfView = 29.0f;
                previewCamera.transform.position = new Vector3(4.9f, 2.8f, 2.76f);
                previewCamera.transform.eulerAngles = new Vector3(18.6f, -119.0f, 0.0f);
            }

            preview.CameraController.OrbitRadius = previewCamera.transform.position.magnitude;
            preview.CameraController.Target = new Vector3(0.0f, 0.9f, 0.0f);

            preview.SetButtonsEnabled(true);
            preview.CameraController.SetStatesEnabled(true);
        }
    }
}