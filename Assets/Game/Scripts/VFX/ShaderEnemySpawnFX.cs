using Nawlian.Lib.Utils;
using Pixelplacement;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.VFX
{
	public class ShaderEnemySpawnFX : AEnemySpawnFX
	{
		[SerializeField] private string _exposedShaderSliderValue = "_Progress";
		[SerializeField] private float _duration = 0.5f;
		[SerializeField] private Vector2 _startToEndValues;

		private List<Renderer> _renderers = new();

		private void Awake()
		{
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
			Awaiter.WaitAndExecute(_duration, onCompleted);
		}
	}
}
