using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Scriptables
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats")]
	public class EntityStatData : ScriptableObject
	{
		#region Types

		[Serializable]
		public class StatLine
		{
			[SerializeField] private int _baseValue;

			/// <summary>
			/// Natural stat, immutable
			/// </summary>
			public int BaseValue => _baseValue;

			/// <summary>
			/// Additional stat active for an undefined time.
			/// </summary>
			/// <remarks>For example: Equipments and items can provide stats, which will be removed whenever unequiped.</remarks>
			[HideInInspector] public int BonusValue;

			/// <summary>
			/// Additional stat active for a defined time
			/// </summary>
			/// <remarks>For example: A passive or a spell effect will provide stats for a defined time.</remarks>
			[HideInInspector] public int TemporaryValue;

			/// <summary>
			/// Total value, temporary buffs included
			/// </summary>
			public int Value { get => BaseValue + BonusValue + TemporaryValue; }

			/// <summary>
			/// Static value, temporary buffs excluded
			/// </summary>
			public int StaticValue { get => BaseValue + BonusValue; }
		}

		#endregion

		public StatLine MaxHealth;
		public StatLine Speed;
	}
}