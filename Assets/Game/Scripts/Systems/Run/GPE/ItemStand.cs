using Nawlian.Lib.Systems.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class ItemStand : ATriggerInteractable
	{
		public event Action OnItemPaid;

		public override void Interact(IInteractionActor actor)
		{
			// TODO: check if player has enough money & pay for the item
		}
	}
}
