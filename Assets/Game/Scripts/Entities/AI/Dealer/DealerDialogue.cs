using Game.Entities.Shared;
using Game.Systems.Dialogue;
using Nawlian.Lib.Systems.Interaction;
using UnityEngine;

namespace Game.Entities.AI.Dealer
{
	public class DealerDialogue : ADialogueInterpreter, IInteractable
	{
		private const string ON_CANNOT_DEAL = "OnCannotDeal";
		private const string ON_DEAL_DONE = "OnDealDone";

		private bool _dealDone = false;
		private DealerStatData _stats;
		private DealerAI _dealer;

		private void Awake()
		{
			_stats = GetComponentInParent<EntityIdentity>().Stats as DealerStatData;
			_dealer = GetComponentInParent<DealerAI>();
		}

		#region Interaction

		public string InteractionTitle => "Talk";

		public void Interact(IInteractionActor actor)
		{
			if (_stats.Dialogue == null || _dealDone)
				return;
			actor.UnSuggestInteraction(this);
			OpenAndProcessDialogue(_stats.Dialogue);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!_dealDone)
				other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);
		}

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);

		#endregion

		#region Dialogue events

		private void Deal()
		{
			if (CanDeal())
				OnDealDone();
			else
				OnCannotDeal();
		}

		private void OnRefused()
		{
			_dealDone = true;
			_dealer.TakeAction();
		}

		protected override string GetFormattedChoice(string choiceText)
		{
			return choiceText;
		}

		#endregion

		#region Deal

		private bool CanDeal() => true;

		private void OnDealDone()
		{
			_dealDone = true;
			ProcessCheckpoint(ON_DEAL_DONE);
			_dealer.Room.Clear();
		}

		private void OnCannotDeal() => ProcessCheckpoint(ON_CANNOT_DEAL);

		#endregion
	}
}
