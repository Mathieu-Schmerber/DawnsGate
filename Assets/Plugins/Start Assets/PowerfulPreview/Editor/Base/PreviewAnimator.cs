using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// Utility class that helps you to sample animations on the characters. 
    /// Also it allows you to use the default unity character for your previews. 
    /// </summary>
    public class PreviewAnimator
    {
        /// <summary>
        /// Returns a reference to the default unity character prefab.
        /// </summary>
        public static GameObject DefaultUnityCharacter
        {
            get
            {
				if(mUnityCharacter == null) mUnityCharacter = EditorGUIUtility.Load("avatar/defaultavatar.fbx") as GameObject;

				var renderers = mUnityCharacter.GetComponentsInChildren<Renderer>();
				foreach (var renderer in renderers)
				{
					var materials = renderer.sharedMaterials;
					for (int i = 0; i < materials.Length; i++)
					{
						var material = materials[i];
						if (material.shader != PreviewShaders.SimpleLitShader)
						{
							material.shader = PreviewShaders.SimpleLitShader;
							if (material.HasProperty(PreviewShaders.cLegacyBaseMapField))
							{
								material.SetTexture(PreviewShaders.BaseMapField,
									material.GetTexture(PreviewShaders.cLegacyBaseMapField));
							}
							if (material.HasProperty(PreviewShaders.cBaseColorField))
							{
								material.SetColor(PreviewShaders.BaseColorField,
									material.GetColor(PreviewShaders.cLegacyBaseColorField));
							}
						}
					}
				}
				return mUnityCharacter;
            }
        }
        private static GameObject mUnityCharacter;

        /// <summary>
        /// Current character assigned to the preview animator.
        /// IMPORTANT NOTE: It auto instantiates any prefab you set to it!
        /// </summary>
        public GameObject Character
        {
            set
            {
                if (mCharacter != null )
                {
                    mPreview.Scene.DestroyInstance(mCharacter);
                    mCharacter = null;
                }

                value = value ?? DefaultUnityCharacter;
                if( value != null )
                {
                    mCharacter = mPreview.Scene.Instantiate(value);
                    Animator = mCharacter.GetComponent<Animator>();
                    if (Animator == null)
                    {
                        Animator = mCharacter.AddComponent<Animator>();
                    }
                    if (Animator.runtimeAnimatorController == null)
                    {
                        var animatorController = new UnityEditor.Animations.AnimatorController();
                        animatorController.AddLayer("Default");
                        animatorController.layers[0].stateMachine.AddState("Default");
                        Animator.runtimeAnimatorController = animatorController;
                    }
                    Animator.applyRootMotion = false;
                    Animator.enabled = false;
                }
            }
            get
            {
                return mCharacter;
            }
        }
        /// <summary>
        /// Current animation clip assigned to the preview animator.
        /// </summary>
        public AnimationClip Animation { set; get; }
        /// <summary>
        /// Current animator component of the current character. 
        /// Mostly it may be used as a way to get bone transforms.
        /// </summary>
        public Animator Animator { private set; get; }

        /// <summary>
        /// Creates a preview animator instance. 
        /// </summary>
        /// <param name="preview">A reference to current editor preview instance.</param>
        public PreviewAnimator( Preview preview )
        {
            mPreview = preview;
            Character = DefaultUnityCharacter; 
        }

        /// <summary>
        /// Samples animation on the current character.
        /// </summary>
        /// <param name="time">Current animation time (non-normalized).</param>
        public void SampleAnimation( float time )
        {
            if( Animation != null && mCharacter != null )
            {
                Animation.SampleAnimation(mCharacter, time);
            }
        }

		private Preview mPreview;
        private GameObject mCharacter;
    }
}
