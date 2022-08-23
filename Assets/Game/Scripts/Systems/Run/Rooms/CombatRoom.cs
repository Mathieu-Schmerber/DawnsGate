using Game.Entities.Shared;
using Game.Managers;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public class CombatRoom : ARewardRoom
	{
		protected CombatRoomData _settings;
		protected int _waveNumber;
		protected int _currentWave = 0;
		protected List<GameObject> _wave = new();

		public override bool RequiresNavBaking => true;
		public virtual bool ActivateOnStart => true;

		protected override void Awake()
		{
			base.Awake();
			_settings = Databases.Database.Data.Run.Combats;
			_waveNumber = Random.Range(_settings.MinWaveNumber, _settings.MinWaveNumber + 1);
			_currentWave = 0;
		}

		protected override void OnRoomReady()
		{
			base.OnRoomReady();
			if (ActivateOnStart)
				Activate();
		}

		protected override void OnActivate() => StartCoroutine(SpawnWave());

		protected GameObject SpawnEnemy()
		{
			GameObject enemy = _settings.Enemies.Random();
			Vector3 spawnPos = Info.Data.SpawnablePositions.Random();
			GameObject instance = ObjectPooler.Get(enemy, spawnPos, Quaternion.identity, this);

			ScaleEnemyStats(instance.GetComponent<EntityIdentity>());
			_wave.Add(instance);
			return instance;
		}

		public void ScaleEnemyStats(EntityIdentity enemy)
		{
			if (enemy == null)
				return;

			int factor = RunManager.Instance.ReachedRooms.Count - 1; // we don't want the enemies to scale on the first room

			foreach (var stat in _settings.EnemyStatScalePerRoom)
				enemy.Stats.Modifiers[stat.Key].BonusModifier += stat.Value.Value * factor;
		}

		private IEnumerator SpawnWave()
		{
			int entityNumber = Random.Range(_settings.MinEntityPerWave, _settings.MaxEntityPerWave + 1);

			// Delay spawn
			yield return new WaitForSeconds(_settings.DelayBetweenWaves);

			for (int i = 0; i < entityNumber; i++)
				SpawnEnemy();
			_currentWave++;
		}

		public virtual void OnEnemyKilled(GameObject gameObject)
		{
			_wave.Remove(gameObject);
			if (_wave.Count == 0 && _currentWave < _waveNumber)
				StartCoroutine(SpawnWave());
			else if (_wave.Count == 0)
				Clear();
		}
	}
}
