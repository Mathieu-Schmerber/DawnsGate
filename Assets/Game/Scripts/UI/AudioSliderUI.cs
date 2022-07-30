using Game.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Game.UI
{
	[RequireComponent(typeof(RandomAudioClip))]
    public class AudioSliderUI : MonoBehaviour
    {
        [SerializeField] private Image _fill;
		[SerializeField] private float _minimumFillAmount;
		[SerializeField] private AudioMixerGroup _audioGroup; 
        private Slider _slider;
		private RandomAudioClip _clips;

		private void Awake()
		{
			_clips = GetComponent<RandomAudioClip>();
			_slider = GetComponent<Slider>();
			_slider.minValue = -45;
			_slider.maxValue = 0;
		}

		private void Start()
		{
			_slider.onValueChanged.AddListener(OnValueChanged);
		}

		public void Refresh()
		{
			float value;
			_audioGroup.audioMixer.GetFloat(_audioGroup.name, out value);
			_slider.value = value;
			_fill.fillAmount = 1 - (value / _slider.minValue) + _minimumFillAmount;
		}

		private void OnValueChanged(float value)
		{
			_fill.fillAmount = 1 - (value / _slider.minValue) + _minimumFillAmount;
			_audioGroup.audioMixer.SetFloat(_audioGroup.name, value);
			if (_slider.interactable)
				_clips.PlayRandom();
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
