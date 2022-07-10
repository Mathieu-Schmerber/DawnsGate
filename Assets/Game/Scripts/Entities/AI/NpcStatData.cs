using Game.Entities.Shared;
using Game.Systems.Dialogue.Data;
using UnityEngine;

namespace Game.Entities.AI
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/NPC")]
	public class NpcStatData : BaseStatData
	{
		public DialogueData DialogueData;
	}
}
