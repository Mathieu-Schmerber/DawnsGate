using Nawlian.Lib.Systems.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.AI
{
	public class Tank : EnemyAI, IAnimationEventListener
	{
		protected override bool UsesPathfinding => true;

		#region Attack

		public void OnAnimationEnter(AnimatorStateInfo stateInfo)
		{
		}

		public void OnAnimationEvent(string animationArg)
		{
		}

		public void OnAnimationExit(AnimatorStateInfo stateInfo)
		{
		}

		protected override void Attack()
		{
			OnAttackEnd();
		}

		#endregion
	}
}
