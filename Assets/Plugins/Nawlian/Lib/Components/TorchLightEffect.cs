using UnityEngine;

namespace Nawlian.Lib.Components
{
	[RequireComponent(typeof(Light)), DisallowMultipleComponent]
	public class TorchLightEffect : MonoBehaviour
	{
        [Tooltip("maximum possible intensity the light can flicker to")]
        [SerializeField, Min(0)] private float _maxIntensity;

        [Tooltip("minimum possible intensity the light can flicker to")]
        [SerializeField, Min(0)] private float _minIntensity = 0;

        [Tooltip("maximum frequency of often the light will flicker in seconds")]
        [SerializeField, Min(0)] private float _maxFlickerFrequency = 1f;

        [Tooltip("minimum frequency of often the light will flicker in seconds")]
        [SerializeField, Min(0)] private float _minFlickerFrequency = 0.1f;

        [Tooltip("how fast the light will flicker to it's next intensity, a very high value will" +
            "look like a dying lightbulb while a lower value will look more like an organic fire")]
        [SerializeField, Min(0)] private float _strength = 5f;

        private float _baseIntensity;
        private float _nextIntensity;
        private float _flickerFrequency;
        private float _timeOfLastFlicker;
        private Light _light;

        public float MaxIntensity => _maxIntensity;

		private void OnValidate()
        {
            _light = GetComponent<Light>();
            _maxIntensity = _light.intensity;
            if (_maxIntensity < _minIntensity) _minIntensity = _maxIntensity;
            if (_maxFlickerFrequency < _minFlickerFrequency) _minFlickerFrequency = _maxFlickerFrequency;
        }

        private void Awake()
        {
            _light = GetComponent<Light>();
            _baseIntensity = _light.intensity;

            _timeOfLastFlicker = Time.time;
        }

        private void Update()
        {
            if (_timeOfLastFlicker + _flickerFrequency < Time.time)
            {
                _timeOfLastFlicker = Time.time;
                _nextIntensity = Random.Range(_minIntensity, _maxIntensity);
                _flickerFrequency = Random.Range(_minFlickerFrequency, _maxFlickerFrequency);
            }

            Flicker();
        }

        private void Flicker()
        {
            _light.intensity = Mathf.Lerp(
                _light.intensity,
                _nextIntensity,
                _strength * Time.deltaTime
            );
        }

        public void Reset()
        {
            _light.intensity = _baseIntensity;
        }
    }
}
