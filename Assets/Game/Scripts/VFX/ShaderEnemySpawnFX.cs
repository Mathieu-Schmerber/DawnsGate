using Nawlian.Lib.Utils;
using Pixelplacement;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.VFX
{
	[RequireComponent(typeof(AudioSource))]
	public class ShaderEnemySpawnFX : AEnemySpawnFX
	{
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

		protected override void ExecuteFx(Action onCompleted)
		{
			foreach (Renderer renderer in _renderers)
			{
				renderer.material.SetFloat(_exposedShaderSliderValue, _startToEndValues.x);
				Tween.ShaderFloat(renderer.material, _exposedShaderSliderValue, _startToEndValues.y, _duration, 0);
			}
			_source.PlayOneShot(_audio);
			Awaiter.WaitAndExecute(_duration, onCompleted);
		}
	}
}
