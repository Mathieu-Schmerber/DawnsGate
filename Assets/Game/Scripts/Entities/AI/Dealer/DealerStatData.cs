using Game.Systems.Dialogue.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entities.AI.Dealer
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Dealer")]
	public class DealerStatData : EnemyStatData
	{
		[Title("Kind mode")]
		public DialogueData Dialogue;
	}
}
