using Game.Managers;
using Game.UI;
using Nawlian.Lib.Systems.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Systems.Run.GPE
{
	public class ItemUpgrader : ATriggerInteractable
	{
		public override string InteractionTitle => "Upgrade item";

		public override void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
			GuiManager.OpenMenu<ItemUpgradeMenuUi>();
		}
	}
}
