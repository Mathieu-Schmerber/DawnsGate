using Game.Entities.Shared;
using Game.Entities.Shared.Health;

namespace Game.Entities.AI.Lunaris
{
	public class LunarisDamageable : Damageable
	{
		private LunarisAI _ai;

		protected override void Awake()
		{
			base.Awake();
			_ai = GetComponent<LunarisAI>();
		}

		public override void Kill(EntityIdentity attacker)
		{
			if (_ai.IsLastPhase)
				base.Kill(attacker);
			else
				_ai.SetNextPhase();
		}
	}
}