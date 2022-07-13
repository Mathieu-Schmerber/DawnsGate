using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entities.AI.Croupier
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Croupier")]
	public class CroupierStatData : NpcStatData
	{
		[Title("Money bet")]

		[@Tooltip("The minimum amount of money that can be used on the first bet."), MinValue(0)]
		public int MinimumBet;
		[@Tooltip("The % of the player money to use on the first bet."), MinValue(0), MaxValue(100)]
		public int InitialBetRatio;
	}
}
