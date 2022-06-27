using Sirenix.OdinInspector;
using UnityEngine;
using Game.Entities.Shared;

namespace Game.Systems.Run.Lobby
{
	[CreateAssetMenu(menuName = "Data/Run/Trait upgrade")]
	public class TraitUpgradeData : ScriptableObject
	{
		public string Title;
		[TextArea] public string Description;
		public int NumberOfUpgrades;
		public int BasePrice;
		public float PriceInflationPerUpgrade;
		public string StatName;
		[HorizontalGroup] public StatModifier StatModified;
		[HorizontalGroup] public float IncrementPerUpgrade;
	}
}
