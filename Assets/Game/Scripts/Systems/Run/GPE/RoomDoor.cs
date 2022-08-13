using Game.Managers;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class RoomDoor : MonoBehaviour, IInteractable
	{
		[SerializeField] private Collider[] _deActivateOnRoomCleared;

		protected bool _active = false;
		public event Action OnActivated;
		public event Action OnInteracted;

		public Room LeadToRoom { get; set; }

		public string InteractionTitle => $"Open";

		public virtual void Activate()
		{
			// Cannot activate a dead-end
			if (LeadToRoom == null)
				return;
			OnActivate();
		}

		protected void OnActivate()
		{
			_active = true;
			if (_deActivateOnRoomCleared?.Length > 0)
				_deActivateOnRoomCleared.ForEach(x => x.enabled = false);
			OnActivated?.Invoke();
		}

		#region Interaction

		public virtual void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
			OnInteracted?.Invoke();
		}

		public void EnterNextRoom() => RunManager.SelectNextRoom(LeadToRoom);

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