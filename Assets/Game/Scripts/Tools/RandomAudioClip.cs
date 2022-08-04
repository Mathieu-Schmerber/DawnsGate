using Nawlian.Lib.Extensions;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Tools
{
	[RequireComponent(typeof(AudioSource))]
    public class RandomAudioClip : MonoBehaviour, ISelectHandler
    {
        [SerializeField] private AudioClip[] _clips;
		[SerializeField] private bool _playOnEnable;
		[SerializeField] private bool _playOnSelect;

		private AudioSource _audioSource;
		private AudioClip _lastPlayed;

		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		private void OnEnable()
		{
			if (_playOnEnable)
				PlayRandom();
		}

		public void Play(AudioClip audio)
		{
			if (audio == null)
				return;
			_audioSource.PlayOneShot(audio);
		}

		public void PlayRandom()
		{
			AudioClip selected = _clips.Length > 1 ? _clips.Where(x => x != _lastPlayed).Random() : _clips[0];

			_lastPlayed = selected;
			_audioSource.PlayOneShot(selected);
		}

		public void PlayRandom(AudioClip[] audios)
		{
			if (audios == null)
				return;
			_clips = audios;
			PlayRandom();
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (_playOnSelect)
				PlayRandom();
		}
	}
}
