using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public class CombatRoom : ARewardRoom
	{
		private CombatRoomData _settings;
		private int _waveNumber;
		private int _currentWave = 0;
		private List<GameObject> _wave = new();

		public override bool RequiresNavBaking => true;

		protected override void Awake()
		{
			base.Awake();
			_settings = Databases.Database.Data.Run.Combats;
			_waveNumber = Random.Range(_settings.MinWaveNumber, _settings.MinWaveNumber + 1);
			_currentWave = 0;
		}

		protected override void Start()
		{
			base.Start();
			Activate();
		}

		protected override void OnActivate() => StartCoroutine(SpawnWave());

		private void SpawnEnemy()
		{
			GameObject enemy = _settings.Enemies.Random();
			Vector3 spawnPos = _roomInfo.Data.SpawnablePositions.Random();

			_wave.Add(ObjectPooler.Get(enemy, spawnPos, Quaternion.identity, this));
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

		public void OnEnemyKilled(GameObject gameObject)
		{
			_wave.Remove(gameObject);
			if (_wave.Count == 0 && _currentWave < _waveNumber)
				StartCoroutine(SpawnWave());
			else if (_wave.Count == 0)
				Clear();
		}
	}
}
