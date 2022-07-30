using Nawlian.Lib.Extensions;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tools
{
	[RequireComponent(typeof(AudioSource))]
	public class LayeredRandomAudioClip : SerializedMonoBehaviour
	{
		[SerializeField] private Dictionary<string, AudioClip[]> _layeredClips = new();
		private AudioSource _audioSource;

		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		public void PlayRandom(string layer)
		{
			if (!_layeredClips.ContainsKey(layer))
				return;
			_audioSource.PlayOneShot(_layeredClips[layer].Random());
		}
	}
}
