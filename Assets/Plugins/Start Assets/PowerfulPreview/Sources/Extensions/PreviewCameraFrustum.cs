using UnityEngine;

namespace StartAssets.PowerfulPreview.Drawers
{
    /// <summary>
    /// Another example of the drawer, which draws camera frustum according to it's settings.
    /// </summary>
    [AddComponentMenu( "" )]
    public class PreviewCameraFrustum : PreviewDrawer
    {
        public bool orthographic = false;
        public float orthographicSize = 1.0f; 
        public float fieldOfView = 60.0f;
        public float nearClipPanel = 1.0f;
        public float farClipPanel = 100.0f; 
        public float aspectRatio = 1.0f; 

        protected override void Draw()
        {
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            LineMaterial.SetPass(0);
            DrawFrustum();
            GL.End();
            GL.PopMatrix();
        }
        protected void DrawFrustum()
        {
            Vector3 c = transform.position;
            var near = nearClipPanel;
            var far = farClipPanel;
            var vFov = fieldOfView * Mathf.Deg2Rad / 2;
            var hFov = Mathf.Atan( Mathf.Tan( vFov / 2 ) * aspectRatio );

            var tanHorFov = Mathf.Tan( hFov );
            var tanVerFov = Mathf.Tan( vFov );

            var farPlaneWidth  = orthographic ? orthographicSize : (far * tanHorFov);
            var nearPlaneWidth = orthographic ? orthographicSize : (near * tanHorFov);

            var farPlaneHeight  = orthographic ? orthographicSize : (far * tanVerFov);
            var nearPlaneHeight = orthographic ? orthographicSize : (near * tanVerFov);

            Vector3 localXAxis = transform.TransformDirection( Vector3.right );
            Vector3 localYAxis = transform.TransformDirection( Vector3.up );
            Vector3 localZAxis = transform.TransformDirection( Vector3.forward );

            var farMinXVector = -localXAxis * farPlaneWidth;
            var farMaxXVector = localXAxis * farPlaneWidth;
            var farMinYVector = -localYAxis * farPlaneHeight;
            var farMaxYVector = localYAxis * farPlaneHeight;

            var farVector = localZAxis * far;
            var nearVector = localZAxis * near;

            var farTopLeftVector     = c + farVector + farMinXVector + farMaxYVector;
            var farTopRightVector    = c + farVector + farMaxXVector + farMaxYVector;
            var farBottomLeftVector  = c + farVector + farMinXVector + farMinYVector;
            var farBottomRightVector = c + farVector + farMaxXVector + farMinYVector;

            var nearMinXVector = -localXAxis * nearPlaneWidth;
            var nearMaxXVector = localXAxis * nearPlaneWidth;
            var nearMinYVector = -localYAxis * nearPlaneHeight;
            var nearMaxYVector = localYAxis * nearPlaneHeight;

            var nearTopLeftVector     = c + nearVector + nearMinXVector + nearMaxYVector;
            var nearTopRightVector    = c + nearVector + nearMaxXVector + nearMaxYVector;
            var nearBottomLeftVector  = c + nearVector + nearMinXVector + nearMinYVector;
            var nearBottomRightVector = c + nearVector + nearMaxXVector + nearMinYVector;

            GL.Color( Color.white );
            DrawPlane( nearTopLeftVector, nearTopRightVector, nearBottomRightVector, nearBottomLeftVector );
            DrawPlane( farTopLeftVector, farTopRightVector, farBottomRightVector, farBottomLeftVector );

            DrawPlane( nearTopLeftVector, farTopLeftVector, farBottomLeftVector, nearBottomLeftVector );
            DrawPlane( nearTopRightVector, farTopRightVector, farBottomRightVector, nearBottomRightVector );

            GL.Color( new Color( 0.8f, 0.8f, 0.8f ) );
            DrawPlane( c, nearTopLeftVector, nearTopRightVector, c );
            DrawPlane( c, nearBottomLeftVector, nearBottomRightVector, c );
        }
    }
}