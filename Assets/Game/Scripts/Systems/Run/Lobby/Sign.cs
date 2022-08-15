using Game.Managers;
using Game.UI;
using Nawlian.Lib.Systems.Interaction;
using UnityEngine;

namespace Game.Systems.Run.Lobby
{
	public class Sign : MonoBehaviour, IInteractable
	{
		#region Interaction

		public string InteractionTitle => "How to play";

		public void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
			GuiManager.OpenMenu<HowToPlayUi>();
		}

		private void OnTriggerEnter(Collider other)
		{
			other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);
		}

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);

		#endregion
	}
}
