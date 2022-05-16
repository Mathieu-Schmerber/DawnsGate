using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Scriptables
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats")]
	public class BaseStatData : ScriptableObject, ICloneable
	{
		#region Types

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
		}

		#endregion

		/// <summary>
		/// Maximum health of an entity
		/// </summary>
		[SuffixLabel("In unit", true)] public StatLine MaxHealth;

		/// <summary>
		/// Movement speed of an entity
		/// </summary>
		[SuffixLabel("In meter/second", true)] public StatLine MovementSpeed;

		/// <summary>
		/// Attack range of an entity
		/// </summary>
		[SuffixLabel("In percent", true)] public StatLine AttackRange;

		/// <summary>
		/// Armor of an entity in % of maxHealth <br></br>
		/// Armor is another form of health point that gets affected prior to health
		/// </summary>
		[SuffixLabel("In percent", true)] public StatLine Armor;

		/// <summary>
		/// Attack damage of an entity in % of the performed attack
		/// </summary>
		[SuffixLabel("In percent", true)] public StatLine AttackDamage;

		/// <summary>
		/// Damage modifier when attacking armor, in % of the attack damage
		/// </summary>
		[SuffixLabel("In percent", true)] public StatLine ArmorDamage;

		/// <summary>
		/// Attack speed, in % of the weapon attack speed
		/// </summary>
		[SuffixLabel("In percent", true)] public StatLine AttackSpeed;

		/// <summary>
		/// The knockback force applied to an entity, in % of the performed attack
		/// </summary>
		[SuffixLabel("In percent", true)] public StatLine KnockbackForce;

		/// <summary>
		/// The resistance to the knockback force, in % of the force received
		/// </summary>
		[SuffixLabel("In percent", true)] public StatLine KnockbackResistance;

		/// <summary>
		/// The range of dash (in unit)
		/// </summary>
		[SuffixLabel("In meter", true)] public StatLine DashRange;

		/// <summary>
		/// The cooldown between two dashes (in seconds)
		/// </summary>
		[SuffixLabel("In second", true)] public StatLine DashCooldown;

		public object Clone() => Instantiate(this);
	}
}