using Game.Entities.Shared;
using Game.Systems.Combat.Attacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entities.AI.Lunaris
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Lunaris")]
	public class LunarisStatData : BaseStatData
	{
		#region Types

		[System.Serializable]
		public class PhaseAttack
		{
			public AnimationClip Animation;
			public float AttackSpeed;
			[InlineEditor] public ModularAttackData AttackData;
			public Vector3 StartOffset;
			public float TravelDistance;
		}

		[System.Serializable, InlineProperty, HideLabel]
		public class PhaseSettings
		{
			[TabGroup("Passive")] public float SpawnRate;
			[TabGroup("Passive")] public float PrevisualisationDuration;
			[TabGroup("Passive")] public float PassiveSpread;

			[TabGroup("Attack")] public PhaseAttack LightAttack;
			[TabGroup("Attack")] public PhaseAttack LightAttack2;
			[TabGroup("Attack")] public PhaseAttack HeavyAttack;

			[TabGroup("Settings")] public Vector2Int LightBeforeHeavyNumber;
			[TabGroup("Settings")] public Mesh Weapon;
			[TabGroup("Settings")] public float AttackCooldown;
			[TabGroup("Settings")] public float RestingTime;
		}

		#endregion

		[Title("Phases")]
		public float PhaseSwitchTime;
		public AnimationClip RestAnimation;
		public AnimationClip DeathAnimation;
		[InlineEditor] public ModularAttackData PassiveAttack;
		[InlineEditor] public ModularAttackData PhaseSwitchAttack;
		[FoldoutGroup("Phase 1: Scythe")] public PhaseSettings ScythePhase = new();
		[FoldoutGroup("Phase 2: Katana")] public PhaseSettings KatanaPhase = new();
		[FoldoutGroup("Phase 3: Staff")] public PhaseSettings StaffPhase = new();

	}
}