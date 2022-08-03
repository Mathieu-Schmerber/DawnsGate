using Game.Entities.Shared;
using Game.Entities.Shared.Health;

namespace Game.Entities.AI.Dealer
{
	public class DealerDamageable : Damageable
	{
		private DealerAI _ai;
		private DealerDialogue _dialogue;

		protected override void Awake()
		{
			base.Awake();
			_ai = GetComponent<DealerAI>();
			_dialogue = GetComponentInChildren<DealerDialogue>();
		}

		public override void Kill(EntityIdentity attacker)
		{
			//base.Kill(attacker);
			_ai.Stop();
			_dialogue.Apologize();
		}
	}
}
