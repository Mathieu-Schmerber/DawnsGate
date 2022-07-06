using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Entities.Shared
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Base")]
	public class BaseStatData : ScriptableObject, ICloneable
	{
		[TitleGroup("Main")] public string DisplayName;

		/// <summary>
		/// Maximum health of an entity
		/// </summary>
		[SuffixLabel("In unit", true), TitleGroup("Main")] public float StartHealth;

		/// <summary>
		/// Movement speed of an entity
		/// </summary>
		[SuffixLabel("In meter/second", true), TitleGroup("Mobility")] public float MovementSpeed;

		/// <summary>
		/// The range of dash (in unit)
		/// </summary>
		[SuffixLabel("In meter", true), TitleGroup("Mobility")] public float DashRange;

		/// <summary>
		/// The cooldown between two dashes (in seconds)
		/// </summary>
		[SuffixLabel("In second", true), TitleGroup("Mobility")] public float DashCooldown;

		[TitleGroup("Modifiers")] public StatDictionary Modifiers;

		public object Clone() => Instantiate(this);

		[Button]
		protected virtual void SetDefaultModifiers()
		{
			Modifiers = new StatDictionary();

			Modifiers.Add(StatModifier.MaxHealth, new StatLineModifier(100));
			Modifiers.Add(StatModifier.MovementSpeed, new StatLineModifier(100));
			Modifiers.Add(StatModifier.ArmorRatio, new StatLineModifier(0));
			Modifiers.Add(StatModifier.AttackDamage, new StatLineModifier(100));
			Modifiers.Add(StatModifier.LifeSteal, new StatLineModifier(0));
			Modifiers.Add(StatModifier.CriticalRate, new StatLineModifier(0));
			Modifiers.Add(StatModifier.CriticalDamage, new StatLineModifier(150));
			Modifiers.Add(StatModifier.ArmorDamage, new StatLineModifier(100));
			Modifiers.Add(StatModifier.AttackSpeed, new StatLineModifier(100));
			Modifiers.Add(StatModifier.KnockbackForce, new StatLineModifier(100));
			Modifiers.Add(StatModifier.KnockbackResistance, new StatLineModifier(0));
			Modifiers.Add(StatModifier.DashRange, new StatLineModifier(100));
			Modifiers.Add(StatModifier.DashCooldown, new StatLineModifier(100));
			Modifiers.Add(StatModifier.StunResistance, new StatLineModifier(0));
			Modifiers.Add(StatModifier.GoldGain, new StatLineModifier(100));
			Modifiers.Add(StatModifier.ReviveHealth, new StatLineModifier(0));
		}

		[OnInspectorInit]
		protected virtual void Init()
		{
			if (string.IsNullOrEmpty(DisplayName))
				DisplayName = name;
		}
	}
}