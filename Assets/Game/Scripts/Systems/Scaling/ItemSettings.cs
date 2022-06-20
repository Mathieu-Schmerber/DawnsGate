using Game.Entities.Shared;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Systems.Scaling
{
	[CreateAssetMenu(menuName = "Data/Items/Settings")]
	public class ItemSettings : ScriptableObject
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

		public int NumberOfUpgrades;

		public GfxStats StatGraphics;
	}
}
