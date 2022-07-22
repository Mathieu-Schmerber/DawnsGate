using Game.Entities.Player;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class InventoryAudio : SerializedMonoBehaviour
    {
        [SerializeField] private Dictionary<Inventory.InventoryUpdateEventArgs.UpdateType, AudioClip> _clips = new();
		private AudioSource _source;

		private void Awake()
		{
			_source = GetComponent<AudioSource>();
		}

		private void OnEnable()
		{
			Inventory.OnUpdated += PlaySound;
		}

		private void OnDisable()
		{
			Inventory.OnUpdated -= PlaySound;
		}

		private void PlaySound(Inventory.InventoryUpdateEventArgs obj)
		{
			if (_clips.ContainsKey(obj.Event))
				_source.PlayOneShot(_clips[obj.Event]);
		}
	}
}
