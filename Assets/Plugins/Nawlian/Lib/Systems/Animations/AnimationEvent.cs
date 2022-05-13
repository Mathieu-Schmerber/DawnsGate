using System.Collections.Generic;
using UnityEngine;
using Nawlian.Lib.Extensions;

namespace Nawlian.Lib.Systems.Animations
{
	/// <summary>
	/// Sends animation states to parent.
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class AnimationEvent : MonoBehaviour
	{
		private List<IAnimationEventListener> _receivers;

		private void Awake()
		{
			_receivers = transform.GetComponentsInParent<IAnimationEventListener>(true, true);
		}

		public void OnAnimationEvent(string animation) => _receivers.ForEach(x => x.OnAnimationEvent(animation));
		public void OnAnimationEnter(AnimatorStateInfo stateInfo) => _receivers.ForEach(x => x.OnAnimationEnter(stateInfo));
		public void OnAnimationExit(AnimatorStateInfo stateInfo) => _receivers.ForEach(x => x.OnAnimationExit(stateInfo));
	}
}