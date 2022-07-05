using Game.Entities.AI;
using Game.Systems.Combat.Attacks;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entities.Lunaris
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Lunaris")]
	public class LunarisStatData : EnemyStatData
	{
		#region Types

		[System.Serializable]
		public class PhaseSettings
		{
			[FoldoutGroup("Passive")] public float SpawnRate;
			[FoldoutGroup("Passive")] public float PrevisualisationDuration;
			[FoldoutGroup("Passive")] public float PassiveSpread;
		}

		[System.Serializable]
		public class PhaseDictionary : SerializedDictionary<LunarisPhase, PhaseSettings> { }

		#endregion

		public PhaseDictionary Phases;
		[InlineEditor] public ModularAttackData PassiveAttack;
	}
}