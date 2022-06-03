using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// Implements an abstraction for the preview camera, as it's quite complex 
    /// and consists not only of a single camera, but three of them:
    /// 1) main camera
    /// 2) gizmo camera 
    /// 3) editor handles camera 
    /// ***
    /// Main camera is used to draw all the objects on the main layer, like 
    /// characters, models, objects etc.
    /// ***
    /// Gizmo camera draws an overlay and you can draw objects that should be on 
    /// top of all the other objects. It might be custom controls, icons, gizmos etc. 
    /// ***
    /// And the editor handles camera is used to draw UnityEditor.Handles objects, 
    /// it's still experimental, as it's a bit tricky to work with them. 
    /// ***
    /// The interface of the class itself is implemented as a copy of the original 
    /// UnityEngine.Camera class with some extensions, to provide similar functionality and easier learning curve. 
    /// </summary>
    public partial class PreviewCamera : System.IDisposable
    {
		public Camera MainCamera { protected set; get; }
		public Camera GizmoCamera { protected set; get; }

		/// <summary>
		/// Render texture of the main camera, use this to draw the main layer.
		/// </summary>
		public RenderTexture MainSurface => mainCameraSurface;

		/// <summary>
		/// Render texture of the gizmo camera, use this to draw the gizmo layer. 
		/// </summary>
		public RenderTexture GizmoSurface => gizmoCameraSurface;
        
        /// <summary>
        /// Create a preview camera for some specific preview instance. 
        /// </summary>
        public PreviewCamera( Preview preview, PreviewScene scene )
        {
            var transformContainerName = preview.GetPreviewGameObjectUniqueName( "TransformCameraContainer" );
            mTransformCameraContainer = scene.FindPreviewGameObject(transformContainerName);

            var animationContainerName = preview.GetPreviewGameObjectUniqueName( "AnimationCameraContainer" );
            mAnimationCameraContainer = scene.FindPreviewGameObject(animationContainerName);
            mAnimationCameraContainer.transform.parent = mTransformCameraContainer.transform;

			MainCamera = CreateCamera(preview, scene, "MainCamera");

			GizmoCamera = CreateCamera(preview, scene, "GizmoCamera");
			GizmoCamera.depth = MainCamera.depth + 1;
			GizmoCamera.clearFlags = CameraClearFlags.Color;
			GizmoCamera.backgroundColor = Color.clear;

			mHandlesCamera = CreateCamera(preview, scene, "HandlesCamera");

			mHandlesCamera.depth = GizmoCamera.depth + 1;
			mHandlesCamera.clearFlags = CameraClearFlags.Color;
			mHandlesCamera.backgroundColor = Color.clear;

			mStoredAnimationClip = null;
        }

		/// <summary>
		/// Disposes all the resources allocated during the class lifetime. 
		/// </summary>
		public void Dispose()
        {
            if (MainCamera != null)
            {
                MainCamera.targetTexture = null;
            }
            if (mainCameraSurface != null)
            {
                mainCameraSurface.Release();
            }

            if (GizmoCamera != null)
            {
                GizmoCamera.targetTexture = null;
            }
            if (gizmoCameraSurface != null)
            {
                gizmoCameraSurface.Release();
            }

            if (mTransformCameraContainer != null)
            {
				UnityEngine.Object.DestroyImmediate(mTransformCameraContainer);
            }
            if (mAnimationCameraContainer != null)
            {
				UnityEngine.Object.DestroyImmediate(mAnimationCameraContainer);
            }
        }

        /// <summary>
        /// Sets new surface rect for the camera render targets. 
        /// </summary>
        /// <param name="rect">New surface rect.</param>
        public void SetSurfaceRect( Rect rect )
        {
            surfaceRect = rect;

            MainCamera.targetTexture = null;
            CreateSurface(ref mainCameraSurface);
            MainCamera.aspect = surfaceRect.width / surfaceRect.height;
            MainCamera.targetTexture = mainCameraSurface;

			GizmoCamera.targetTexture = null;
            CreateSurface(ref gizmoCameraSurface);
            GizmoCamera.aspect = MainCamera.aspect;
            GizmoCamera.targetTexture = gizmoCameraSurface;

            mHandlesCamera.aspect = GizmoCamera.aspect;
            mDefaultHandleRect = mHandlesCamera.pixelRect;
        }

        /// <summary>
        /// Plays the animation clip on the camera at the specifics time.
        /// </summary>
        /// <param name="clip">Animation clip to play.</param>
        /// <param name="time">Animation clip time to play.</param>
        public void SampleAnimation(AnimationClip clip, float time)
        {
            if( mStoredAnimationClip != clip )
            {
                mAnimationCameraContainer.transform.localPosition = Vector3.zero;
                mAnimationCameraContainer.transform.localEulerAngles = Vector3.zero;
                mStoredAnimationClip = clip; 
            }
            
            clip?.SampleAnimation(mAnimationCameraContainer, time);
        }

        /// <summary>
        /// Renders the camera into the render textures assigned to them. 
        /// </summary>        
        public void Render()
        {
			MainCamera.enabled = GizmoCamera.enabled = true;

			GizmoCamera.Render();
			MainCamera.Render();

			MainCamera.enabled = GizmoCamera.enabled = false;
		}
        /// <summary>
        /// Renders the list of handles.
        /// </summary>
        /// <param name="handles">Preview handles to draw.</param>
        public void RenderHandles( List<IPreviewHandle> handles )
        {
			mHandlesCamera.enabled = true;

			//It's a bit hacky solution to draw handles 
			//Basically we want to set pixel rect of the camera 
			//manually, because otherwise it is drawn over the inspector 
			//view and not fit inside the preview.
			var adjustedRect = surfaceRect;
            adjustedRect.y = surfaceRect.y < 20 ? 0 : 45;

            mHandlesCamera.pixelRect = mDefaultHandleRect;
            if (Event.current.type != EventType.Repaint)
            {
                Handles.SetCamera(surfaceRect, mHandlesCamera);
            }
            else
            {
                if (Event.current.type == EventType.Repaint)
                {
                    mHandlesCamera.pixelRect = adjustedRect;
                    Handles.SetCamera(mHandlesCamera);
                }
            }

            //This conflicts the with line above, it distorts the matrices a bit 
            //For some reason Pop call doesn't work? Or we need to separate them into two different cameras? Or 
            //events? 
            foreach (var previewHandler in handles)
            {
                HandleUtility.PushCamera(mHandlesCamera);
                if (previewHandler.Visible)
                {
                    previewHandler.Draw(mHandlesCamera, adjustedRect);
                }
                HandleUtility.PopCamera(mHandlesCamera);
            }

			mHandlesCamera.enabled = false;
		}

        /// <summary>
        /// A method to create a typical camera object with some specific name 
        /// and assigned to some preview instance. 
        /// </summary>
        /// <param name="preview">Parent instance of the preview.</param>
        /// <param name="name">Name of the camera object.</param>
        /// <returns></returns>
        private Camera CreateCamera( Preview preview, PreviewScene scene, string name )
        {
            var cameraObjectName = preview.GetPreviewGameObjectUniqueName(name);
            var cameraObject = scene.FindPreviewGameObject( cameraObjectName );
            cameraObject.transform.parent = mAnimationCameraContainer.transform;

			var camera = cameraObject.GetComponent<Camera>();
			if( camera == null ) camera = cameraObject.AddComponent<Camera>();

			var grey = 49.0f / 255.0f;
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = new Color(grey, grey, grey);
            camera.nearClipPlane = 0.001f;
            camera.orthographic = false;
            camera.depth = -1;
            camera.orthographicSize = 1.0f;
            camera.fieldOfView = 60.0f;
			camera.cullingMask = 1 << 1;

            return camera;
        }
        /// <summary>
        /// Creates a surface and writes it into the render texture. 
        /// </summary>
        /// <param name="surface">Render texture to write into.</param>
        private void CreateSurface(ref RenderTexture surface)
        {
            if (surface != null)
            {
                surface.Release();
				UnityEngine.Object.DestroyImmediate(surface);
            }

            surface = new RenderTexture(Mathf.Max(Mathf.RoundToInt(surfaceRect.width), 1),
                Mathf.Max(Mathf.RoundToInt(surfaceRect.height), 1), 24, RenderTextureFormat.ARGB32);
            surface.antiAliasing = 4;
            surface.Create();
        }

        private Camera mHandlesCamera;

		private GameObject mTransformCameraContainer;
        private GameObject mAnimationCameraContainer;

        protected RenderTexture mainCameraSurface;
        protected RenderTexture gizmoCameraSurface;
        protected Rect surfaceRect;

        private Rect mDefaultHandleRect;

        private AnimationClip mStoredAnimationClip; 
    }
}