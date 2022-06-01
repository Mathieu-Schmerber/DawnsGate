using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Scriptables
{
	[Serializable, InlineProperty]
	public class StatLine
	{
		/// <summary>
		/// In Unit
		/// </summary>
		[SerializeField, HideLabel] private float _naturalUnit;

		/// <summary>
		/// Additional stat active for an undefined time (in %).
		/// </summary>
		/// <remarks>For example: Equipments and items can provide stats, which will be removed whenever unequiped.</remarks>
		[HideInInspector] public int BonusModifier;

		/// <summary>
		/// Additional stat active for a defined time (in %).
		/// </summary>
		/// <remarks>For example: A passive or a spell effect will provide stats for a defined time.</remarks>
		[HideInInspector] public int TemporaryModifier;

		/// <summary>
		/// Total value, temporary buffs included
		/// </summary>
		public float Value => _naturalUnit + ((BonusModifier + TemporaryModifier) / 100) * _naturalUnit;

		/// <summary>
		/// Static value, temporary buffs excluded
		/// </summary>
		public float StaticValue => _naturalUnit + (BonusModifier / 100) * _naturalUnit;

		public StatLine() { }

		public StatLine(float baseValue) => _naturalUnit = baseValue;
	}

	[Serializable] public class StatDictionary : SerializedDictionary<StatModifier, StatLine> { }

	public enum StatModifier
	{
		///// <summary>
		///// Max health
		///// </summary>
		[LabelText("Max Health (%)")] MaxHealth,

		///// <summary>
		///// Movement speed
		///// </summary>
		[LabelText("Speed (%)")] MovementSpeed,

		///// <summary>
		///// Armor of an entity in % of maxHealth <br></br>
		///// Armor is another form of health point that gets affected prior to health
		///// </summary>
		[LabelText("Armor (%)")] ArmorRatio,

		///// <summary>
		///// Attack damage of an entity in % of the performed attack
		///// </summary>
		[LabelText("Dmg. (%)")] AttackDamage,

		///// <summary>
		///// Lifesteal, in % of the attack damage
		///// </summary>
		[LabelText("Life steal (%)")] LifeSteal,

		///// <summary>
		///// Crit chance
		///// </summary>
		[LabelText("Crit. Rate (%)")] CriticalRate,

		///// <summary>
		///// Damage of a critical attack, in % of attack damage
		///// </summary>
		[LabelText("Crit. Dmg. (%)")] CriticalDamage,

		///// <summary>
		///// Damage modifier when attacking armor, in % of the attack damage
		///// </summary>
		[LabelText("Armor Dmg. (%)")] ArmorDamage,

		///// <summary>
		///// Attack speed, in % of the weapon attack speed
		///// </summary>
		[LabelText("Attack speed (%)")] AttackSpeed,

		///// <summary>
		///// The knockback force applied to an entity, in % of the performed attack
		///// </summary>
		[LabelText("KB Force (%)")] KnockbackForce,

		///// <summary>
		///// The resistance to the knockback force, in % of the force received
		///// </summary>
		[LabelText("KB Res. (%)")] KnockbackResistance,

		///// <summary>
		///// The range of the dash 
		///// </summary>
		[LabelText("Dash Rng. (%)")] DashRange,

		///// <summary>
		///// The cooldown between two dashes
		///// </summary>
		[LabelText("Dash Cdr. (%)")] DashCooldown,

		/// <summary>
		/// Resistance to stun
		/// </summary>
		[LabelText("Stun Res. (%)")] StunResistance
	}

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

		protected virtual void Awake() => SetDefaultModifiers();

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