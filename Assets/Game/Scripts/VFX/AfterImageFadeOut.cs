using Nawlian.Lib.Systems.Pooling;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
	public class AfterImageFadeOut : APoolableObject
	{
		[SerializeField] private float _lifetime;
		[SerializeField, Range(0, 1)] private float _baseAlpha;

		private Renderer _renderer;

		private void Awake()
		{
			_renderer = GetComponentInChildren<Renderer>();
		}

		public override void Init(object data)
		{
			Material mat = _renderer.material;
			Color color = mat.color;
			Color target = new Color(color.r, color.g, color.b, 0);

			Tween.ShaderColor(mat, "_BaseColor", new Color(color.r, color.g, color.b, _baseAlpha), target, _lifetime, 0, completeCallback: () => Release()); ;
		}
	}
}