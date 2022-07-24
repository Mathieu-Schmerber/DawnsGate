using Nawlian.Lib.Extensions;
using System;
using UnityEngine;

namespace Game.Tools
{
	[RequireComponent(typeof(AudioSource))]
    public class RandomAudioClip : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private bool _playOnEnable;

        private AudioSource _audioSource;

		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		private void OnEnable()
		{
			if (_playOnEnable)
				PlayRandom();
		}

		public void PlayRandom()
		{
			_audioSource.PlayOneShot(_clips.Random());
		}

		public void PlayRandom(AudioClip[] audios)
		{
			if (audios == null)
				return;
			_clips = audios;
			PlayRandom();
		}
	}
}
