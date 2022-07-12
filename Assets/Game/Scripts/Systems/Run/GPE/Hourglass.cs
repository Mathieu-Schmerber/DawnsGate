using Game.Systems.Run.Rooms;
using Nawlian.Lib.Systems.Interaction;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class Hourglass : ATriggerInteractable
	{
		[SerializeField] private ARoom _room;

		public override string InteractionTitle => "Activate the clock";

		public override void Interact(IInteractionActor actor)
		{
			_room.Activate();
			actor.UnSuggestInteraction(this);
			Destroy(gameObject);
		}
	}
}
