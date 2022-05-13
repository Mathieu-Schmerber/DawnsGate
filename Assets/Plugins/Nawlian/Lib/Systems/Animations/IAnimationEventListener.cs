using UnityEngine;

namespace Nawlian.Lib.Systems.Animations
{
	/// <summary>
	/// Provides the ability to receive animation states and events.
	/// </summary>
	public interface IAnimationEventListener
	{
		void OnAnimationEvent(string animationArg);
		void OnAnimationEnter(AnimatorStateInfo stateInfo);
		void OnAnimationExit(AnimatorStateInfo stateInfo);
	}
}