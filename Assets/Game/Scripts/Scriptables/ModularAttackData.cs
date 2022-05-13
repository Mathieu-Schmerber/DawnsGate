using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Scriptables
{
	[CreateAssetMenu(menuName = "Data/Attacks/ModularAttack")]
	public class ModularAttackData : AttackBaseData
	{
		public float HitYRotation;
		public float ActiveTime;
		public bool FollowCaster;
		public float Range;
	}
}
