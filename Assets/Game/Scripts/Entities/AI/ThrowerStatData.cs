using Game.Systems.Combat.Attacks;
using Game.Systems.Combat.Effects;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.AI
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Thrower")]
	public class ThrowerStatData : EnemyStatData
	{
		[Title("Projectile")]

		[ValidateInput("@Projectile?.GetComponent<ThrowerProjectile>() != null", "The Projectile has no ThrowerProjectile component.")]
		public GameObject Projectile;
		public float TravelTime;
		public float MaxAltitude;
		[InlineEditor] public ModularAttackData AoeAttack;
	}
}
