using Game.Systems.Run.Rooms;
using Nawlian.Lib.Systems.Animations;
using Nawlian.Lib.Systems.Interaction;
using Pixelplacement;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class Hourglass : ATriggerInteractable, IAnimationEventListener
	{
		[SerializeField] private ARoom _room;

		private bool _used = false;
		private Animator _gfx;

		public override string InteractionTitle => "Activate the clock";

		private void Awake()
		{
			_gfx = GetComponentInChildren<Animator>();
		}

		public override void Interact(IInteractionActor actor)
		{
			if (_used)
				return;
			_used = true;
			actor.UnSuggestInteraction(this);
			_gfx.SetBool("Interacted", true);
		}

		protected override void OnSuggesting(IInteractionActor actor)
		{
			if (_used)
				return;
			base.OnSuggesting(actor);
		}

		public void OnAnimationEvent(string animationArg)
		{
			if (animationArg == "Activate")
				_room.Activate();
		}

		public void OnAnimationEnter(AnimatorStateInfo stateInfo){}

		public void OnAnimationExit(AnimatorStateInfo stateInfo){}
	}
}
