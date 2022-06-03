using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// This part of the PreviewCamera class contains only copy of the UnityEngine.Camera interface. 
    /// The documentation for those methods and properties you can find in the unity scrip reference
    /// Url: https://docs.unity3d.com/ScriptReference/Camera.html
    /// 
    /// This specific file might be extended later to fit all the new UnityEngine.Camera features, 
    /// but for now it's sufficient to fit all the preview needs. 
    /// </summary>
    public partial class PreviewCamera
    {
        public Color backgroundColor
        {
            set
            {
                MainCamera.backgroundColor = value;
            }
            get
            {
                return MainCamera.backgroundColor;
            }
        }
        public Matrix4x4 cameraToWorldMatrix
        {
            get
            {
                return MainCamera.cameraToWorldMatrix;
            }
        }
        public CameraType cameraType
        {
            get
            {
                return CameraType.Preview;
            }
        }
        public float farClipPlane
        {
            set
            {
                MainCamera.farClipPlane = value;
                GizmoCamera.farClipPlane = value;
                mHandlesCamera.farClipPlane = value;
            }
            get
            {
                return MainCamera.farClipPlane;
            }
        }
        public float nearClipPlane
        {
            set
            {
                MainCamera.nearClipPlane = value;
                GizmoCamera.nearClipPlane = value;
                mHandlesCamera.nearClipPlane = value;
            }
            get
            {
                return MainCamera.nearClipPlane;
            }
        }

        public int pixelHeight
        {
            get
            {
                return MainCamera.pixelHeight;
            }
        }
        public int pixelWidth
        {
            get
            {
                return MainCamera.pixelWidth;
            }
        }
        public Rect pixelRect
        {
            get
            {
                return MainCamera.pixelRect;
            }
        }
        public Rect rect
        {
            get
            {
                return MainCamera.rect;
            }
        }
        public Vector3 velocity
        {
            get
            {
                return MainCamera.velocity;
            }
        }

        public float fieldOfView
        {
            set
            {
                MainCamera.fieldOfView = value;
                GizmoCamera.fieldOfView = value;
                mHandlesCamera.fieldOfView = value;
            }
            get
            {
                return MainCamera.fieldOfView;
            }
        }
        public bool orthographic
        {
            set
            {
                MainCamera.orthographic = value;
                GizmoCamera.orthographic = value;
                mHandlesCamera.orthographic = value;
            }
            get
            {
                return MainCamera.orthographic;
            }
        }
        public float orthographicSize
        {
            set
            {
                MainCamera.orthographicSize = value;
                GizmoCamera.orthographicSize = value;
                mHandlesCamera.orthographicSize = value;
            }
            get
            {
                return MainCamera.orthographicSize;
            }
        }
        public float maxOrthographicSize
        {
            set
            {
                maxOrthographicSize = value;
            }
            get
            {
                return maxOrthographicSize;
            }
        }
        public float minOrthographicSize
        {
            set
            {
                minOrthographicSize = value;
            }
            get
            {
                return minOrthographicSize;
            }
        }
        public Transform transform
        {
            get
            {
                return mTransformCameraContainer.transform;
            }
        }

        public Vector3 WorldToViewportPoint(Vector3 position)
        {
            return MainCamera.WorldToViewportPoint(position);
        }
        public Vector3 WorldToScreenPoint(Vector3 position)
        {
            return MainCamera.WorldToScreenPoint(position);
        }
        public Vector3 ViewportToWorldPoint(Vector3 position)
        {
            return MainCamera.ViewportToWorldPoint(position);
        }
        public Vector3 ViewportToScreenPoint(Vector3 position)
        {
            return MainCamera.ViewportToScreenPoint(position);
        }
        public Ray ViewportPointToRay(Vector3 position)
        {
            return MainCamera.ViewportPointToRay(position);
        }
        public Vector3 ScreenToWorldPoint(Vector3 position)
        {
            return MainCamera.ScreenToWorldPoint(position);
        }
        public Vector3 ScreenToViewportPoint(Vector3 position)
        {
            return MainCamera.ScreenToViewportPoint(position);
        }
        public Ray ScreenPointToRay(Vector3 position)
        {
            return MainCamera.ScreenPointToRay(position);
        }

        /// <summary>
        /// Casts PreviewCamera to UnityEngine.Camera. 
        /// </summary>
        /// <returns>The main camera.</returns>
        public static implicit operator UnityEngine.Camera(PreviewCamera previewCamera)
        {
            return previewCamera.MainCamera;
        }
    }
}