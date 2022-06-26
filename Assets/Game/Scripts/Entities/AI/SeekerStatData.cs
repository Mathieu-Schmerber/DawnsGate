using UnityEngine;

namespace Game.Entities.AI
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Seeker")]
	public class SeekerStatData : EnemyStatData
	{
		public float BaseDamage;
		public float BaseKnockBackForce;
		public float AttackRadius;
		public GameObject HitFx;
	}
}
