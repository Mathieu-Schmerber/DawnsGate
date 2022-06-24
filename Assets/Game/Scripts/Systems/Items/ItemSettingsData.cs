using Game.Entities.Shared;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.Systems.Items
{
	[CreateAssetMenu(menuName = "Data/Items/Settings")]
	public class ItemSettingsData : ScriptableObject
	{
		#region Types

		[System.Serializable]
		public struct StatGraphic
		{
			public string Name;
		}

		[System.Serializable]
		public class GfxStats : SerializedDictionary<StatModifier, StatGraphic> { }

		[System.Serializable]
		public class ItemCost : SerializedDictionary<ItemType, Vector2Int> {
			public int RandomCost(ItemType type) => Random.Range(this[type].x, this[type].y + 1);
		}

		#endregion

		[Title("General")]
		public int NumberOfUpgrades;
		public int InventorySlots;
		public ItemCost ItemCosts;
		public int PriceInflationPerUpgrade;

		[Title("Scripts")]
		[ValidateInput(nameof(ValidateStatEditor), "Script needs to inherit AEquippedItem.")]
		public MonoScript DefaultStatScript;

		[Title("Graphics")]
		public GfxStats StatGraphics;


#if UNITY_EDITOR
		private bool ValidateStatEditor() => DefaultStatScript != null && !DefaultStatScript.GetClass().IsAbstract && DefaultStatScript.GetClass().IsSubclassOf(typeof(AEquippedItem));
#endif
	}
}
