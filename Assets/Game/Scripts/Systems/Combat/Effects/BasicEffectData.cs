using Game.Entities.Shared;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.Combat.Effects
{
	public enum EffectAction
	{
		APPLY_DAMAGE,
		APPLY_MODIFIER,
		SPAWN_OBJECT
	}

	[CreateAssetMenu(menuName = "Data/Effects/Basic")]
	public class BasicEffectData : AEffectBaseData
	{
		[System.Serializable]
		public struct ActionDescriptor
		{
			public EffectAction Action;

			[ShowIf("@Action == EffectAction.APPLY_DAMAGE")] public float DamageAmount;
			[ShowIf("@Action == EffectAction.APPLY_MODIFIER")] public StatDictionary Modifiers;

			[ShowIf("@Action == EffectAction.SPAWN_OBJECT")] public GameObject Prefab;
			[ShowIf("@Action == EffectAction.SPAWN_OBJECT")] public bool StickToEntity;
			[ShowIf("@Action == EffectAction.SPAWN_OBJECT")] public bool AllowDuplicates;

		}

		public bool LimitStack;
		[ShowIf("@LimitStack")] public int MaxStack;

		public List<ActionDescriptor> Actions;
	}
}
