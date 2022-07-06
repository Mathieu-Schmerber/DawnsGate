using Game.Entities.Shared;
using Game.Systems.Combat.Attacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entities.Lunaris
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Lunaris")]
	public class LunarisStatData : BaseStatData
	{
		#region Types

		[System.Serializable, InlineProperty, HideLabel]
		public class PhaseSettings
		{
			[TabGroup("Passive")] public float SpawnRate;
			[TabGroup("Passive")] public float PrevisualisationDuration;
			[TabGroup("Passive")] public float PassiveSpread;

			[TabGroup("Attack"), InlineEditor] public ModularAttackData LightAttack;
			[TabGroup("Attack"), InlineEditor] public ModularAttackData HeavyAttack;
		}

		#endregion

		[Title("Phases")]
		[InlineEditor] public ModularAttackData PassiveAttack;
		[FoldoutGroup("Phase 1: Scythe")] public PhaseSettings ScythePhase = new();
		[FoldoutGroup("Phase 2: Katana")] public PhaseSettings KatanaPhase = new();
		[FoldoutGroup("Phase 3: Staff")] public PhaseSettings StaffPhase = new();
	}
}