using Game.Managers;
using Game.UI;
using Nawlian.Lib.Systems.Interaction;

namespace Game.Systems.Run.Lobby
{
	public class TraitUpgrader : ATriggerInteractable
	{
		public override string InteractionTitle => $"Upgrade traits";

		public override void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
			GuiManager.OpenMenu<TraitUpgradeUi>();
		}
	}
}