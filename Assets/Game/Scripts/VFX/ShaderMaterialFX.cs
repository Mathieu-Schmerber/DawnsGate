using Nawlian.Lib.Utils;
using Pixelplacement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.VFX
{
	[RequireComponent(typeof(AudioSource))]
	public class ShaderMaterialFX : AMaterialFx
	{
		[SerializeField] private Shader _shader;
		[SerializeField] private string _exposedShaderSliderValue = "_Progress";
		[SerializeField] private float _duration = 0.5f;
		[SerializeField] private Vector2 _startToEndValues;
		[SerializeField] private AudioClip _audio;

		private AudioSource _source;
		private List<Renderer> _renderers = new();

		private void Awake()
		{
			_source = GetComponent<AudioSource>();
			_renderers = GetComponentsInChildren<Renderer>(true).ToList();

			var mine = GetComponent<Renderer>();
			if (mine)
				_renderers.Add(mine);
		}

		protected override void ExecuteFx(Action onCompleted, bool isSpawn)
		{
			float start = isSpawn ? _startToEndValues.x : _startToEndValues.y;
			float end = isSpawn ? _startToEndValues.y : _startToEndValues.x;

			foreach (Renderer renderer in _renderers)
			{
				if (renderer is ParticleSystemRenderer || renderer is ICanvasElement)
					continue;
				Shader defaultSh = renderer.material.shader;
				renderer.material.shader = _shader;
				renderer.material.SetFloat(_exposedShaderSliderValue, start);

				Tween.ShaderFloat(renderer.material, _exposedShaderSliderValue, end, _duration, 0, 
					completeCallback: () => renderer.material.shader = defaultSh);
			}
			_source.PlayOneShot(_audio);
			Awaiter.WaitAndExecute(_duration, onCompleted);
		}
	}
}
