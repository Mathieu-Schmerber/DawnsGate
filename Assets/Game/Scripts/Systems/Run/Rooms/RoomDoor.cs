using Game.Managers;
using Nawlian.Lib.Systems.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public class RoomDoor : MonoBehaviour, IInteractable
	{
		private bool _active = false;
		public event Action OnActivated;

		public Room LeadToRoom { get; set; }

		public void Activate()
		{
			// Cannot activate a dead-end
			if (LeadToRoom == null)
				return;
			_active = true;
			OnActivated?.Invoke();
		}

		#region Interaction

		public void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
			RunManager.SelectNextRoom(LeadToRoom);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (_active)
				other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);
		}

		private void OnTriggerExit(Collider other)
		{
			if (_active)
				other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);
		}

		#endregion
	}
}
