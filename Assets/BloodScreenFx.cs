using Game.Entities.Player;
using Game.Entities.Shared.Health;
using Game.Managers;
using Game.Tools;
using Nawlian.Lib.Utils;
using Plugins.Nawlian.Lib.Systems.Menuing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class BloodScreenFx : AMenu
    {
		public override bool RequiresGameFocus => false;

		private RandomAudioClip _clips;

		protected override void Awake()
		{
			base.Awake();
			_clips = GetComponent<RandomAudioClip>();
		}

		private void OnEnable()
		{
			GameManager.Player.GetComponent<Damageable>().OnDamaged += ShowHurtEffect;
		}

		private void OnDisable()
		{
			GameManager.Player.GetComponent<Damageable>().OnDamaged -= ShowHurtEffect;
		}

		private void ShowHurtEffect(float obj)
		{
			Open();
			_clips.PlayRandom();
			Awaiter.WaitAndExecute(_duration, Close);
		}

		public override void Show()
		{
			// Instantly set alpha to 1
			_grp.alpha = 1;
		}
	}
}
