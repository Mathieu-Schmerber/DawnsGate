using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Systems.Items
{
	[System.Serializable]
	public class ItemSummary
	{
		public ItemBaseData Data;
		[HideInInspector] public ItemSummary Merge;
		public int Quality;
		public bool isMerged => Merge != null;
	}
}
