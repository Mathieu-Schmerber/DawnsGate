using UnityEngine;
using StartAssets.PowerfulPreview.Drawers;

namespace StartAssets.PowerfulPreview.Samples
{
    /// <summary>
    /// An example of the custom preview drawer, which draws 
    /// the hierarchy of the skeleton bones with the green line.
    /// </summary>
    [AddComponentMenu("")]
    [ExecuteInEditMode()]
    public class BonesDrawer : PreviewDrawer
    {
        /// <summary>
        /// You should set the root as a root of the game object.
        /// It will find the bone child automatically. 
        /// </summary>
        public Transform Root { set;get; }

        protected override void Draw()
        {
            if( Root == null )
            {
                return;
            }
            GL.PushMatrix();
            GL.Begin( GL.QUADS );
            LineMaterial.SetPass( 0 );
            DrawChildren( Root );
            foreach( Transform child in Root )
            {
                DrawChildren( child );
            }
            GL.End();
            GL.PopMatrix();
        }
        protected void DrawChildren( Transform root )
        {
            foreach( Transform child in root )
            {
                DrawLine( root.position, child.position, Color.green, 0.01f );
                DrawChildren( child );
            }
        }

        private Transform mRoot;
    }
}