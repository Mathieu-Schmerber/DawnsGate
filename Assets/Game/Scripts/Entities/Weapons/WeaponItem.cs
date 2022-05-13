using Game.Entities.Player;
using Nawlian.Lib.Systems.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Weapons
{
	public class WeaponItem : MonoBehaviour, IInteractable
	{
		private WeaponState _state;
		private MeshFilter _meshFilter;

		private void Awake()
		{
			_state = GetComponent<WeaponState>();
			_meshFilter = GetComponent<MeshFilter>();
		}

		private void Start()
		{
			_meshFilter.mesh = _state.EquippedState.Data.Mesh;
		}

		private void OnTriggerEnter(Collider other) => other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);

		public void Interact(IInteractionActor actor)
		{
			MonoBehaviour behaviour = actor as MonoBehaviour;
			PlayerWeapon weapon = behaviour.GetComponent<PlayerWeapon>();

			if (weapon == null)
				return;
			actor.UnSuggestInteraction(this);
			weapon.EquipWeapon(_state);
		}
	}
}