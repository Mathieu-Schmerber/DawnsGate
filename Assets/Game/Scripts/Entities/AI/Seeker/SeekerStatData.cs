using Game.VFX.Preview;
using UnityEngine;

namespace Game.Entities.AI.Seeker
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Seeker")]
	public class SeekerStatData : EnemyStatData
	{
		public float BaseDamage;
		public float BaseKnockBackForce;
		public float AttackRadius;
		public PreviewBase DashPreview;
		public GameObject HitFx;
	}
}
