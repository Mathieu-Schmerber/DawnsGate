using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Entities.Shared
{
	[Serializable, InlineProperty]
	public class StatLineModifier
	{
		/// <summary>
		/// Base modifier (in %)
		/// </summary>
		[SerializeField, HideLabel] private float _baseModifier;

		/// <summary>
		/// Additional stat active for an undefined time (in %).
		/// </summary>
		/// <remarks>For example: Equipments and items can provide stats, which will be removed whenever unequiped.</remarks>
		[HideInInspector] public float BonusModifier;

		/// <summary>
		/// Additional stat active for a defined time (in %).
		/// </summary>
		/// <remarks>For example: A passive or a spell effect will provide stats for a defined time.</remarks>
		[HideInInspector] public float TemporaryModifier;

		/// <summary>
		/// Total value, temporary buffs included
		/// </summary>
		public float Value => _baseModifier + BonusModifier + TemporaryModifier;

		/// <summary>
		/// Static value, temporary buffs excluded
		/// </summary>
		public float StaticValue => _baseModifier + BonusModifier;

		public StatLineModifier() { }

		public StatLineModifier(float baseValue) => _baseModifier = baseValue;
	}
}