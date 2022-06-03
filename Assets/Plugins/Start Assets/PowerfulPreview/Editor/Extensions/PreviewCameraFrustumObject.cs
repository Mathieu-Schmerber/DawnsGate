using StartAssets.PowerfulPreview.Drawers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// An extension for the preview, which makes it's possible to create a PreviewCameraFrustum
    /// instance for the preview camera and control it.
    /// It's not a camera, it's just a visual object which you can use to show how does the camera frustum
    /// look. 
    /// </summary>
    public class PreviewCameraFrustumObject : IDisposable
    {
        /// <summary>
        /// Creates an instance of the class for some specific preview. 
        /// </summary>
        public PreviewCameraFrustumObject(Preview preview)
        {
            mPreview = preview;

            var gizmoTransformContainerName = preview.GetPreviewGameObjectUniqueName( "TransformFrustumContainter" );
            mTransformFrustumContainer = preview.Scene.FindPreviewGameObject(gizmoTransformContainerName);

            var gizmoAnimationContainerName = preview.GetPreviewGameObjectUniqueName( "AnimationFrustumContainter" );
            mAnimationFrustumContainer = preview.Scene.FindPreviewGameObject(gizmoAnimationContainerName);
            mAnimationFrustumContainer.transform.parent = mTransformFrustumContainer.transform;

            var cameraGizmoName = preview.GetPreviewGameObjectUniqueName( "CameraFrustum" );
            mCameraFrustumObject = preview.Scene.FindPreviewGameObject(cameraGizmoName);
            mCameraFrustumObject.transform.parent = mAnimationFrustumContainer.transform;
            mCameraFrustumObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            mCameraFrustum = mCameraFrustumObject.AddComponent<PreviewCameraFrustum>();
            mCameraFrustum.Visible = false;

            preview.OnSurfaceRectUpdated += (rect) =>
            {
                AspectRatio = Mathf.Max( rect.width / rect.height, 0.1f );
            };
        }

        public void Dispose()
        {
            if (mCameraFrustumObject != null)
            {
                UnityEngine.Object.DestroyImmediate(mCameraFrustumObject);
            }
            if (mAnimationFrustumContainer != null)
            {
                UnityEngine.Object.DestroyImmediate(mAnimationFrustumContainer);
            }
            if (mTransformFrustumContainer != null)
            {
                UnityEngine.Object.DestroyImmediate(mTransformFrustumContainer);
            }
        }

        /// <summary>
        /// Sets the gizmo visible/invisible. 
        /// </summary>
        public bool Visible
        {
            set
            {
                CameraFrustum.Visible = value;
            }
            get
            {
                return CameraFrustum.Visible;
            }
        }
        /// <summary>
        /// Sets the camera frustum aspect ration. It's useful to makes the frustum to fit the preview  area.
        /// </summary>
        public float AspectRatio
        {
            set
            {
                CameraFrustum.aspectRatio = value;
            }
            get
            {
                return CameraFrustum.aspectRatio;
            }
        }

        /// <summary>
        /// Sets the camera frustum far clip plane.
        /// </summary>
        public float FarClipPlane
        {
            set
            {
                CameraFrustum.farClipPanel = value;
            }
            get
            {
                return CameraFrustum.farClipPanel;
            }
        }
        /// <summary>
        /// Sets the camera frustum near clip plane. 
        /// </summary>
        public float NearClipPlane
        {
            set
            {
                CameraFrustum.nearClipPanel = value;
            }
            get
            {
                return CameraFrustum.nearClipPanel;
            }
        }
        /// <summary>
        /// Sets the camera frustum field of view. 
        /// </summary>
        public float FieldOfView
        {
            set
            {
                CameraFrustum.fieldOfView = value;
            }
            get
            {
                return CameraFrustum.fieldOfView;
            }
        }
        /// <summary>
        /// Sets the camera frustum to be an orthographic mode. 
        /// </summary>
        public bool Orthographic
        {
            set
            {
                CameraFrustum.orthographic = value;
            }
            get
            {
                return CameraFrustum.orthographic;
            }
        }
        /// <summary>
        /// Sets the orthographic camera frustum size. 
        /// </summary>
        public float OrthographicSize
        {
            set
            {
                CameraFrustum.orthographicSize = value;
            }
            get
            {
                return CameraFrustum.orthographicSize;
            }
        }

        /// <summary>
        /// Returns the frustum object transform. 
        /// </summary>
        public Transform transform
        {
            get
            {
                return mTransformFrustumContainer.transform;
            }
        }

        /// <summary>
        /// Play the animation on the camera frustum object. 
        /// </summary>
        /// <param name="clip">Animation clip to play.</param>
        /// <param name="time">Animation time to play.</param>
        public void SampleAnimation(AnimationClip clip, float time)
        {
            clip.SampleAnimation(mAnimationFrustumContainer, time);
        }
        
        private PreviewCameraFrustum CameraFrustum
        {
            get
            {
                if (mCameraFrustum == null)
                {
                    if (mCameraFrustumObject != null)
                    {
                        mCameraFrustum = mCameraFrustumObject.GetComponent<PreviewCameraFrustum>();
                    }
                }
                return mCameraFrustum;
            }
        }
        private PreviewCameraFrustum mCameraFrustum;

        private Preview mPreview;

        private GameObject mTransformFrustumContainer;
        private GameObject mAnimationFrustumContainer;
        private GameObject mCameraFrustumObject;
    }
}