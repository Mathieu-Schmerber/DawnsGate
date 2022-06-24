using Game.Managers;
using Game.UI;
using Nawlian.Lib.Systems.Interaction;

namespace Game.Systems.Run.GPE
{
	public class ItemMerger : ATriggerInteractable
	{
		public override string InteractionTitle => "Merge item";

		public override void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
			GuiManager.OpenMenu<ItemMergeMenuUi>();
		}
	}
}
