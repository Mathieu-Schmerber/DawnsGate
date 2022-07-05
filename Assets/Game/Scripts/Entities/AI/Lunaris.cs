using System;

namespace Game.Entities.AI
{
	public class Lunaris : EnemyAI
	{
		protected override bool UsesPathfinding => true;

		protected override void Attack()
		{
			OnAttackEnd();
		}
	}
}
