using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nawlian.Lib.Systems.Interaction
{
	public abstract class ATriggerInteractable : MonoBehaviour, IInteractable
	{
		#region Interaction
		public abstract string InteractionTitle { get; }

		public abstract void Interact(IInteractionActor actor);

		protected virtual void OnSuggesting(IInteractionActor actor)
		{
			actor.SuggestInteraction(this);
		}

		protected virtual void OnUnSuggesting(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
		}

		private void OnTriggerEnter(Collider other)
		{
			var actor = other.GetComponent<IInteractionActor>();

			if (actor != null)
				OnSuggesting(actor);
		}

		private void OnTriggerExit(Collider other)
		{
			var actor = other.GetComponent<IInteractionActor>();

			if (actor != null)
				OnUnSuggesting(actor);
		}
		#endregion
	}
}
