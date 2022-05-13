using Game.Inputs;
using Nawlian.Lib.Systems.Interaction;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Player
{
	public class PlayerInteraction : MonoBehaviour, IInteractionActor
	{
		private HashSet<MonoBehaviour> _nearbyInteractions = new();
		private MonoBehaviour _lastNearest = null;
		private InputHandler _inputs;

		/// <summary>
		/// Triggers when the interaction suggestion changed, the interactable object may be null.
		/// </summary>
		public static event Action<IInteractable> OnInteractionChanged;

		#region Unity builtins

		private void OnEnable()
		{
			_inputs.OnInteractPressed += InteractWithClosest;
		}

		private void OnDisable()
		{
			_inputs.OnInteractPressed -= InteractWithClosest;
		}

		private void Awake()
		{
			_inputs = GetComponent<InputHandler>();
		}

		private void Update()
		{
			MonoBehaviour changed = _lastNearest;

			_lastNearest = GetClosestInteraction();
			if (changed != _lastNearest)
				OnInteractionChanged?.Invoke(_lastNearest?.GetComponent<IInteractable>());
		}

		#endregion

		/// <summary>
		/// Returns the closest interaction
		/// </summary>
		/// <returns></returns>
		private MonoBehaviour GetClosestInteraction()
		{
			MonoBehaviour bestTarget = null;
			float closestDistanceSqr = Mathf.Infinity;
			Vector3 currentPosition = transform.position;

			if (_nearbyInteractions.Count == 0)
				return null;
			foreach (MonoBehaviour interaction in _nearbyInteractions)
			{
				Vector3 directionToTarget = interaction.transform.position - currentPosition;
				float dSqrToTarget = directionToTarget.sqrMagnitude;
				if (dSqrToTarget < closestDistanceSqr)
				{
					closestDistanceSqr = dSqrToTarget;
					bestTarget = interaction;
				}
			}
			return bestTarget;
		}

		private void InteractWithClosest() => _lastNearest?.GetComponent<IInteractable>().Interact(this);

		#region Interaction interface

		public void SuggestInteraction(IInteractable interactable)
		{
			// We are casting to monobehaviour here, to then get the position of the interaction every frame on Update()
			// Better to cast here once, rather than everyframe or by adding a Position property within the interface.
			_nearbyInteractions.Add(interactable as MonoBehaviour);
		}

		public void UnSuggestInteraction(IInteractable interactable) => _nearbyInteractions.Remove(interactable as MonoBehaviour);

		#endregion
	}
}
