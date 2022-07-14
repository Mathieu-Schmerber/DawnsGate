using Game.Entities.Shared;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

		[Title("Graphics")]
		public GfxStats StatGraphics;


#if UNITY_EDITOR
		[Title("Scripts")]
		[ValidateInput("ValidateStatEditor", "Script needs to inherit AEquippedItem.")]
		public MonoScript DefaultStatScript;
		private bool ValidateStatEditor() => DefaultStatScript != null && !DefaultStatScript.GetClass().IsAbstract && DefaultStatScript.GetClass().IsSubclassOf(typeof(AEquippedItem));

		[Button]
		private void UpdateAllItemsScripts()
		{
			var items = Databases.Database.Data.Item.All<ItemBaseData>();

			foreach (var item in items)
				item.OnScriptChanged();
			Debug.Log("Scripts updated successfully within the data model");
		}
#endif
	}
}
