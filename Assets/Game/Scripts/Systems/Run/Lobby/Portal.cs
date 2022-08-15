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

		private string _interactionTitle;
		public string InteractionTitle => _interactionTitle;

		private bool _canEnter;

		public void Interact(IInteractionActor actor)
		{
			if (_canEnter)
			{
				actor.UnSuggestInteraction(this);
				RunManager.StartNewRun();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			_canEnter = GameManager.Player.GetComponent<PlayerWeapon>().CurrentWeapon != null;
			_interactionTitle = _canEnter ? $"Enter portal" : $"<color=red>Please equip a weapon before entering the portal</color>";
			other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);
		}

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);

		#endregion
	}
}
