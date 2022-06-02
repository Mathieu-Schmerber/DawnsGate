using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Entities.Shared
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Base")]
	public class BaseStatData : ScriptableObject, ICloneable
	{
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

			Modifiers.Add(StatModifier.MaxHealth, new StatLine(100));
			Modifiers.Add(StatModifier.MovementSpeed, new StatLine(100));
			Modifiers.Add(StatModifier.ArmorRatio, new StatLine(0));
			Modifiers.Add(StatModifier.AttackDamage, new StatLine(100));
			Modifiers.Add(StatModifier.LifeSteal, new StatLine(0));
			Modifiers.Add(StatModifier.CriticalRate, new StatLine(0));
			Modifiers.Add(StatModifier.CriticalDamage, new StatLine(150));
			Modifiers.Add(StatModifier.ArmorDamage, new StatLine(100));
			Modifiers.Add(StatModifier.AttackSpeed, new StatLine(100));
			Modifiers.Add(StatModifier.KnockbackForce, new StatLine(100));
			Modifiers.Add(StatModifier.KnockbackResistance, new StatLine(0));
			Modifiers.Add(StatModifier.DashRange, new StatLine(100));
			Modifiers.Add(StatModifier.DashCooldown, new StatLine(100));
			Modifiers.Add(StatModifier.StunResistance, new StatLine(0));
		}
	}
}