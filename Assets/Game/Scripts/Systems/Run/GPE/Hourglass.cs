using Game.Systems.Run.Rooms;
using Nawlian.Lib.Systems.Interaction;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class Hourglass : ATriggerInteractable
	{
		[SerializeField] private ARoom _room;

		private bool _used = false;
		private AudioSource _source;

		public override string InteractionTitle => "Activate the clock";

		private void Awake()
		{
			_source = GetComponent<AudioSource>();
		}

		public override void Interact(IInteractionActor actor)
		{
			if (_used)
				return;
			_source.Play();
			_room.Activate();
			actor.UnSuggestInteraction(this);
			Destroy(gameObject, _source.time);
			_used = true;
		}
	}
}
