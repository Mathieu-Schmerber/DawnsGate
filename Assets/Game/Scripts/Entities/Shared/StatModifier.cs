using Sirenix.OdinInspector;

namespace Game.Entities.Shared
{
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
		[LabelText("Stun Res. (%)")] StunResistance,

		/// <summary>
		/// Amount of gold gained per room cleared
		/// </summary>
		[LabelText("Gold gain. (%)")] GoldGain,

		/// <summary>
		/// Health when revived
		/// </summary>
		[LabelText("Revive Health (%)")] ReviveHealth
	}
}