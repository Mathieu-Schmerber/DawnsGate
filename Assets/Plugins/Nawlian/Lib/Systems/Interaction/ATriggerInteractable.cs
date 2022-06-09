using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nawlian.Lib.Systems.Interaction
{
	public abstract class ATriggerInteractable : MonoBehaviour, IInteractable
	{
		#region Interaction
		public abstract void Interact(IInteractionActor actor);

		private void OnTriggerEnter(Collider other) => other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);
		#endregion
	}
}
