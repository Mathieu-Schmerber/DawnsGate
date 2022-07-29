using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Run.GPE;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Run.Rooms.Events
{
	public class TimeRoom : CombatRoom
	{
		[SerializeField] private Clock _roomClock;
		[SerializeField] private AudioClip _activationTheme;

		public override bool RequiresNavBaking => true;
		public override bool ActivateOnStart => false;

		protected override void Awake()
		{
			base.Awake();
			_settings = Databases.Database.Data.Run.Combats;
			_waveNumber = 1;
		}

		protected override void OnClear()
		{
			base.OnClear();
			foreach (var item in _wave.ToList())
			{
				item.GetComponent<EntityIdentity>().SetInvulnerable(false);
				item.GetComponent<IDamageProcessor>().ApplyPassiveDamage(Mathf.Infinity);
			}
		}

		protected override void OnActivate()
		{
			AudioManager.PlayTheme(_activationTheme);
			_roomClock.ClockIn(60);
			StartCoroutine(SpawnEnemies(20, 3, () => Clear()));
		}

		private IEnumerator SpawnEnemies(int number, float delay, Action onDone)
		{
			for (int i = 0; i < number; i++)
			{
				if (!Cleared)
					SpawnEnemy();
				yield return new WaitForSeconds(delay);
			}
			onDone?.Invoke();
		}

		public override void OnEnemyKilled(GameObject gameObject)
		{

		}
	}
}
