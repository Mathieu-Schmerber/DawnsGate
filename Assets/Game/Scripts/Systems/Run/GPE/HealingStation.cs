using Game.Entities.Player;
using Game.Entities.Shared;
using Nawlian.Lib.Systems.Interaction;
using System;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class HealingStation : ATriggerInteractable
	{
		private bool _used = false;
		private GameObject _praiser;

		public event Action OnInteracted;
		public override string InteractionTitle => "Praise the sun";

		public override void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
			_praiser = (actor as PlayerInteraction)?.gameObject;
			_used = true;
			OnInteracted?.Invoke();
		}

		public void Heal()
		{
			_praiser.GetComponent<EntityIdentity>().CurrentHealth += 50; // TODO: define a heal amount or calculation
		}

		protected override void OnSuggesting(IInteractionActor actor)
		{
			if (!_used)
				base.OnSuggesting(actor);
		}
	}
}
