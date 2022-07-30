using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Game.UI
{
    public class AudioSliderUI : MonoBehaviour
    {
        [SerializeField] private Image _fill;
		[SerializeField] private float _minimumFillAmount;
		[SerializeField] private AudioMixerGroup _audioGroup; 
        private Slider _slider;

		private void Awake()
		{
			_slider = GetComponent<Slider>();
			_slider.minValue = -45;
			_slider.maxValue = 0;

			float value;
			_audioGroup.audioMixer.GetFloat(_audioGroup.name, out value);

			_slider.value = value;
		}

		private void Start()
		{
			_slider.onValueChanged.AddListener(OnValueChanged);
		}

		private void OnValueChanged(float value)
		{
			_fill.fillAmount = value + _minimumFillAmount;
			_audioGroup.audioMixer.SetFloat(_audioGroup.name, value);
		}

		private void OnValidate()
		{
			if (_fill != null)
			{
				_slider = GetComponent<Slider>();
				_fill.fillAmount = _slider.value + _minimumFillAmount;
			}
		}
	}
}
