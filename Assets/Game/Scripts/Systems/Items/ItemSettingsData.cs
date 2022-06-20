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

		#endregion

		[Title("General")]
		public int NumberOfUpgrades;

		[Title("Scripts")]
		[ValidateInput(nameof(ValidateStatEditor), "Script needs to inherit AEquippedItem.")]
		public MonoScript DefaultStatScript;
		[ValidateInput(nameof(ValidateActiveEditor), "Script needs to inherit AEquippedItem.")]
		public MonoScript DefaultActiveScript;
		[ValidateInput(nameof(ValidatePassiveEditor), "Script needs to inherit AEquippedItem.")]
		public MonoScript DefaultPassiveScript;

		[Title("Graphics")]
		public GfxStats StatGraphics;

#if UNITY_EDITOR
		private bool ValidateStatEditor() => DefaultStatScript != null && !DefaultStatScript.GetClass().IsAbstract && DefaultStatScript.GetClass().IsSubclassOf(typeof(AEquippedItem));
		private bool ValidateActiveEditor() => DefaultActiveScript != null && !DefaultActiveScript.GetClass().IsAbstract && DefaultActiveScript.GetClass().IsSubclassOf(typeof(AEquippedItem));
		private bool ValidatePassiveEditor() => DefaultPassiveScript != null && !DefaultPassiveScript.GetClass().IsAbstract && DefaultPassiveScript.GetClass().IsSubclassOf(typeof(AEquippedItem));

#endif
	}
}
