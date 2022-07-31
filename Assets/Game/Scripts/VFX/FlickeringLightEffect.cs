using Pixelplacement;
using UnityEngine;

namespace Game.VFX
{
	[RequireComponent(typeof(Light)), DisallowMultipleComponent]
	public class FlickeringLightEffect : MonoBehaviour
	{
		[SerializeField] private Vector2 _onTimeMinMax = new Vector2(.5f, 3);
		[SerializeField] private Vector2 _offTimeMinMax = new Vector2(.1f, .2f);
		[SerializeField] private float _turnOnDuration = 0.01f;

		private Light _light;
		private float _baseIntensity;


		private void Awake()
		{
			_light = GetComponent<Light>();
			_baseIntensity = _light.intensity;
		}

		private void Start()
		{
			TurnOn();
		}

		private void TurnOn()
		{
			Tween.Value(0, _baseIntensity, (value) => _light.intensity = value, _turnOnDuration, 0, Tween.EaseInOut);
			//_light.intensity = _baseIntensity;
			Invoke(nameof(TurnOff), Random.Range(_onTimeMinMax.x, _onTimeMinMax.y));
		}

		private void TurnOff()
		{
			_light.intensity = 0;
			Invoke(nameof(TurnOn), Random.Range(_offTimeMinMax.x, _offTimeMinMax.y));
		}
	}
}
