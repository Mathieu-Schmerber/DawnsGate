using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// It's a class that allows you to store different settings (pre-sets) for the 
    /// preview camera and apply it on the current preview camera. 
    /// </summary>
    public class PreviewCameraSetup
    {
        /// <summary>
        /// Background color of the preview camera 
        /// </summary>
        public Color BackgroundColor { set; get; }
        /// <summary>
        /// Field of view of the preview camera
        /// </summary>
        public float FieldOfView { set; get; }
        /// <summary>
        /// Is the preview camera orthographic 
        /// </summary>
        public bool Orthographic { set; get; }
        /// <summary>
        /// Size of the orthographic camera frustum 
        /// </summary>
        public float OrthographicSize { set; get; }
        /// <summary>
        /// Position of the preview camera 
        /// </summary>
        public Vector3 Position { set; get; }
        /// <summary>
        /// Euler angles of the preview camera 
        /// </summary>
        public Vector3 EulerAngles
        {
            set
            {
                Rotation = Quaternion.Euler( value );
            }
            get
            {
                return Rotation.eulerAngles; 
            }
        }
        /// <summary>
        /// Rotation of the preview camera 
        /// </summary>
        public Quaternion Rotation { set; get; }
        /// <summary>
        /// Target the preview camera is pointing at 
        /// </summary>
        public Vector3 Target { set; get; }

        /// <summary>
        /// Creates an instance of the camera setup based on some preview camera settings. 
        /// </summary>
        /// <param name="preview">Preview instance, from which the camera settings will be copied.</param>
        public PreviewCameraSetup( Preview preview )
        {
            if( preview != null && preview.Camera == null )
            {
                return;
            }

            mPreview = preview;

            var camera = mPreview.Camera;
            BackgroundColor = camera.backgroundColor;
            FieldOfView = camera.fieldOfView;
            Orthographic = camera.orthographic;
            OrthographicSize = camera.orthographicSize;
            Position = camera.transform.position;
            EulerAngles = camera.transform.eulerAngles;
        }

        /// <summary>
        /// Aplies current setup to the preview camera 
        /// </summary>
        public void Apply()
        {
            var camera = mPreview.Camera;

            camera.backgroundColor = BackgroundColor;
            camera.fieldOfView = FieldOfView;
            camera.orthographic = Orthographic;
            camera.orthographicSize = OrthographicSize;
            camera.transform.position = Position;
            camera.transform.eulerAngles = EulerAngles;
        }
        
        private Preview mPreview; 
    }
}
