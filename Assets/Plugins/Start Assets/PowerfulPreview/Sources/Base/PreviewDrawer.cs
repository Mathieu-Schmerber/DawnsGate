using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// It's the base class for all custom preview drawers. 
    /// Also it contains base drawing functions. 
    /// </summary>
    [AddComponentMenu("")]
    [ExecuteInEditMode()]
    public abstract class PreviewDrawer : MonoBehaviour
    {
        /// <summary>
        /// Draws one pixel line from one 3D point to another. 
        /// </summary>
        /// <param name="start">Start point of the line</param>
        /// <param name="end">End point of the line</param>
        public static void DrawLine(Vector3 start, Vector3 end)
        {
            GL.Vertex3(start.x, start.y, start.z);
            GL.Vertex3(end.x, end.y, end.z);
        }
        /// <summary>
        /// Draws line with custom width from one 3D point to another. 
        /// </summary>
        /// <param name="start">Start point of the line</param>
        /// <param name="end">End point of the line.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="width">Width of the line.</param>
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float width)
        {
            GL.Color(color);
            var normal = Vector3.Cross(start, end).normalized;
            var side = Vector3.Cross(normal, end - start);
            side.Normalize();

            for (int angle = 0; angle <= 90; angle += 90)
            {
                var newSide = Quaternion.AngleAxis(angle, (end - start).normalized) * side;
                var a = start + newSide * (width / 2);
                var b = start + newSide * (width / -2);
                var c = end + newSide * (width / 2);
                var d = end + newSide * (width / -2);
                DrawPlane(a, c, d, b);
            }
        }
        /// <summary>
        /// Draws wireframe plane with four points. 
        /// </summary>
        public static void DrawPlane(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            DrawLine(v1, v2);
            DrawLine(v2, v3);
            DrawLine(v3, v4);
            DrawLine(v4, v1);
        }
        /// <summary>
        /// Draws wireframe plane with four points and specific line width. 
        /// </summary>
        public static void DrawPlane(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float width)
        {
            DrawLine(v1, v2, Color.white, width);
            DrawLine(v2, v3, Color.white, width);
            DrawLine(v3, v4, Color.white, width);
            DrawLine(v4, v1, Color.white, width);
        }

        ///@brief   Is the game object visible. 
        public bool Visible
        {
            set;
            get;
        }
        
        protected void Awake()
        {
            Visible = true;
		}
	
		protected void OnRenderObject()
		{
			if (string.IsNullOrEmpty(mUUID))
			{
				mUUID = gameObject.name.Substring(gameObject.name.LastIndexOf('_'));
				mCameraToDraw = "PREVIEW__GizmoCamera" + mUUID;
			}
			if (Camera.current == null ||
				!Camera.current.name.Equals(mCameraToDraw) ||
				!Visible)
			{
				return;
			}
			Draw();
        }

		protected abstract void Draw();

        /// <summary>
        /// Creates a material to draw the line. 
        /// </summary>
        protected Material LineMaterial
        {
            get
            {
				if (mLineMaterial == null)
				{
                    var shader = Shader.Find( "Hidden/Internal-Colored" );
 
                    mLineMaterial = new Material( shader );
                    mLineMaterial.hideFlags = HideFlags.HideAndDontSave;
					mLineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					mLineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					mLineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
					mLineMaterial.SetInt("_ZWrite", 0);
				}
                return mLineMaterial;
            }
        }

        private string mCameraToDraw = "";
        private string mUUID = "";
        private Material mLineMaterial;
    }
}
  