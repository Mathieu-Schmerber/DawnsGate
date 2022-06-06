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

		public void Activate()
		{
			_active = true;
			OnActivated?.Invoke();
		}

		#region Interaction

		public void Interact(IInteractionActor actor)
		{
			Debug.Log("Next room");
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
