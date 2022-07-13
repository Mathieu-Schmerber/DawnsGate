using Game.Entities.Shared;
using Game.Systems.Combat.Attacks;
using Game.Systems.Dialogue.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entities.AI.Dealer
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Dealer")]
	public class DealerStatData : BaseStatData
	{
		[Title("Kind mode")]
		public DialogueData Dialogue;
		[@Tooltip("Discount that the dealer applies on the item price in %"), MinValue(0), MaxValue(100)]public int PriceDiscount;

		[Title("Aggressive mode")]
		[InlineEditor] public ModularAttackData DashAttack;
		public AnimationClip StartDashAnimation;
		public Vector2Int ConsecutiveDashes;
	}
}
