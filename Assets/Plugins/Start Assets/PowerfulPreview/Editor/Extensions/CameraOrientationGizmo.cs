using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// Experimental handle for the preview, which draws a simple box 
    /// which shows current orientation of the preview camera. 
    /// </summary>
    public class CameraOrientationGizmo : IPreviewHandle
    {
        public bool Visible
        {
            set;get;
        }

        public CameraOrientationGizmo()
        {
            Visible = true;
        }

        public void Draw(Camera handlesCamera, Rect surfaceRect)
        {
            handlesCamera.fieldOfView = 1;
            handlesCamera.aspect = 1;

            var viewPortSize = new Vector2(128, 128);
            var viewPortOffset = new Vector2( viewPortSize.x, 32 + viewPortSize.y / 2 - surfaceRect.y);

            handlesCamera.pixelRect = new Rect
            (
                x: surfaceRect.width - viewPortOffset.x,
                y: surfaceRect.height - viewPortOffset.y,
                width: viewPortSize.x,
                height: viewPortSize.y
            );

            Handles.SetCamera(handlesCamera);
            Handles.CubeHandleCap(0,
                handlesCamera.transform.position + 
                handlesCamera.transform.TransformDirection(Vector3.forward) * 20 +
                handlesCamera.transform.TransformDirection(Vector3.right) * 0.05f,
                Quaternion.identity, 0.05f, EventType.Repaint);
        }
    }
}
