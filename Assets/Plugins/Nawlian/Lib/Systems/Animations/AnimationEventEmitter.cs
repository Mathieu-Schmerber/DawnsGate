using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nawlian.Lib.Systems.Animations
{
	public class AnimationEventEmitter : StateMachineBehaviour
	{
		private AnimationEvent _animationEvent;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_animationEvent == null)
				_animationEvent = animator.GetComponent<AnimationEvent>();
			_animationEvent.OnAnimationEnter(stateInfo);
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_animationEvent == null)
				_animationEvent = animator.GetComponent<AnimationEvent>();
			_animationEvent.OnAnimationExit(stateInfo);
		}
	}
}