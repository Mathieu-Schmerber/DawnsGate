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
		[SerializeField] private BoxCollider _groundCollider;
		[SerializeField] private Collider[] _deActivateOnRoomCleared;

		protected bool _active = false;
		public event Action OnActivated;

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
			_deActivateOnRoomCleared.ForEach(x => x.enabled = false);
			OnActivated?.Invoke();
		}

		#region Interaction

		public virtual void Interact(IInteractionActor actor)
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

		private void OnDrawGizmos()
		{
			if (_groundCollider == null || Application.isPlaying)
				return;
			Vector3[] corners = new Vector3[4]
			{
				new Vector3(_groundCollider.bounds.max.x, _groundCollider.bounds.max.y, _groundCollider.bounds.max.z),
				new Vector3(_groundCollider.bounds.min.x, _groundCollider.bounds.max.y, _groundCollider.bounds.min.z),
				new Vector3(_groundCollider.bounds.max.x, _groundCollider.bounds.max.y, _groundCollider.bounds.min.z),
				new Vector3(_groundCollider.bounds.min.x, _groundCollider.bounds.max.y, _groundCollider.bounds.max.z),
			};
			Vector3 backLeft = corners.Closest(transform.position - transform.forward - transform.right);
			Vector3 backRight = corners.Closest(transform.position - transform.forward + transform.right);
			Vector3 frontLeft = backLeft - transform.forward * _groundCollider.size.x;
			Vector3 frontRight = backRight - transform.forward * _groundCollider.size.x;

			Gizmos.color = Color.red;
			Gizmos.DrawSphere(backLeft, .1f);
			Gizmos.DrawSphere(backRight, .1f);
			Gizmos.DrawSphere(frontLeft, .1f);
			Gizmos.DrawSphere(frontRight, .1f);
		}
	}
}
