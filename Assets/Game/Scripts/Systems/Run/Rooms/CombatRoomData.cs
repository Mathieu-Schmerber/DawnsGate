using Game.Entities.AI;
using Nawlian.Lib.Systems.Pooling;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	[CreateAssetMenu(menuName = "Data/Run/Room/Combat room settings")]
	public class CombatRoomData : RewardRoomData
	{
		[Title("Waves")]
		[MinValue(0)] public float DelayBetweenWaves;
		[MinValue(0)] public int MinWaveNumber;
		[MinValue(nameof(MinWaveNumber))] public int MaxWaveNumber;
		[MinValue(0)] public int MinEntityPerWave;
		[MinValue(nameof(MinEntityPerWave))] public int MaxEntityPerWave;

		[Title("Spawnables")]
		[Required, AssetsOnly, ValidateInput("@GetError().Item1", "@GetError().Item2")]
		public GameObject[] Enemies;

#if UNITY_EDITOR

		private (bool noError, string message) GetError()
		{
			GameObject error;

			if (Enemies == null || Enemies.Length == 0 || !Enemies.Any(x => x != null))
				return (false, "No enemy specified");
			error = Enemies.FirstOrDefault(x => !x.GetComponent<EnemyAI>());
			if (error != null)
				return (false, $"{error.name} must have a component inheriting EnemyAI.");
			return (true, null);
		}

#endif
	}
}
