using Game.Entities.Player;
using Game.Managers;
using Nawlian.Lib.Systems.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Run.Lobby
{
	public class Portal : MonoBehaviour, IInteractable
	{
		#region Interaction
		public string InteractionTitle => $"Enter portal";

		public void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
			RunManager.StartNewRun();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<PlayerWeapon>()?.CurrentWeapon != null)
				other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);
		}

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);

		#endregion
	}
}
