using Game.Managers;
using Game.Systems.Dialogue;
using Game.Systems.Dialogue.Data;
using Game.UI;
using Nawlian.Lib.Systems.Interaction;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class Anvil : ADialogueInterpreter, IInteractable
	{
		[SerializeField] private DialogueData _choiceMenu;

		public string InteractionTitle => "Use the anvil";

		public void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
			OpenAndProcessDialogue(_choiceMenu);
		}

		private void OnTriggerEnter(Collider other) => other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);

		private void Upgrade()
		{
			CloseDialogue();
			GuiManager.OpenMenu<ItemUpgradeMenuUi>();
		}

		private void Merge()
		{
			CloseDialogue();
			GuiManager.OpenMenu<ItemMergeMenuUi>();
		}

		protected override string GetFormattedChoice(string choiceText) => choiceText;
	}
}
