
using UnityEngine;
using System;
using System.Collections.Generic;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// This class implements a preview object. Preview object is drawn inside the preview, 
    /// and it's not visible for other cameras. 
    /// </summary>
	[ExecuteInEditMode()]
    [AddComponentMenu( "" )]
    public class PreviewObject : MonoBehaviour
	{
		public Renderer Renderer { private set; get; } 

        /// <summary>
        /// Is the object visible. 
        /// </summary>
        public bool Visible { set; get; }

        /// <summary>
        ///Set's up the renderer material. It's useful, when you want create object 
        ///from scratch with some custom material. 
        /// </summary>
        public Material RendererMaterial
        {
            set
            {
                if( mOriginalMaterials == null )
                {
                    mOriginalMaterials = new Material[ 1 ];
                }
                mOriginalMaterials[ 0 ] = value;
            }
        }
        
        /// <summary>
        /// Initializes all the need data, prepares 
        /// materials and then makes all children 
        /// game object to be preview objects too.
        /// </summary>
        /// <param name="gizmo">Is the object should be drawn on the gizmo layer?</param>
        /// <param name="root">Is this object is a root object in the hierarchy?</param>
        public void SetUp( Camera camera)
        {
            Visible = true;

            Renderer = gameObject.GetComponent<Renderer>();
			mInvisibleMaterial = PreviewShaders.InvisibleMaterialCopy;

			if ( Renderer != null )
            {
				mTargetCamera = camera;
				mOriginalMaterials = new Material[ Renderer.sharedMaterials.Length ];
                for( int i = 0; i < Renderer.sharedMaterials.Length; i++ )
                {
					var copy = mInvisibleMaterial; 
					var sharedMaterial = Renderer.sharedMaterials[i];
					if(sharedMaterial != null )
					{
						copy = new Material(sharedMaterial);
						copy.shader = sharedMaterial.shader;
					}
					mOriginalMaterials[i] = copy;					
				}
			}

            foreach( Transform child in transform )
            {
                var previewObject = child.gameObject.GetComponent<PreviewObject>();
                if( previewObject == null )
                {
                    previewObject = child.gameObject.AddComponent<PreviewObject>();
                }
                previewObject.SetUp(camera);
            }
        }

		private void OnDestroy()
		{
			mTargetCamera = null;

			if (mOriginalMaterials != null)
			{
				foreach (var mat in mOriginalMaterials)
				{
					if (mat != null) DestroyImmediate(mat);
				}
			}
			mOriginalMaterials = null;

		}
        private void Update()
        {
            gameObject.layer = 1;
        }

		private void MakeVisible(bool value)
		{
			if (Renderer == null) return;
			var materials = Renderer.sharedMaterials;
			for (int i = 0; i < mOriginalMaterials.Length; i++)
			{
				materials[i] = value ? mOriginalMaterials[i] : mInvisibleMaterial;
			}
			Renderer.sharedMaterials = materials;
		}

		private void OnWillRenderObject()
		{
			if (Renderer != null) MakeVisible(Camera.current == mTargetCamera);
		}

		private Material[] mOriginalMaterials;
		private Material mInvisibleMaterial;

		private Camera mTargetCamera;
    }
}