using Game.Entities.Player;
using Game.Systems.Combat.Weapons;
using Nawlian.Lib.Systems.Interaction;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Systems.Run.Lobby
{
	public class WeaponStand : MonoBehaviour, IInteractable
	{
		[SerializeField] private WeaponData _weapon;

		public event Action OnIntracted;

		public WeaponData Data { get => _weapon; private set => _weapon = value; }
		public bool Empty => Data == null;

		private void Start()
		{
			// Update GFX
			OnIntracted?.Invoke();
		}

		#region Interaction

		public void Interact(IInteractionActor actor)
		{
			MonoBehaviour behaviour = actor as MonoBehaviour;
			PlayerWeapon playerWeapon = behaviour.GetComponent<PlayerWeapon>();
			WeaponData current = playerWeapon.CurrentWeapon;

			playerWeapon.EquipWeapon(_weapon);
			_weapon = playerWeapon.CurrentWeapon;
			OnIntracted?.Invoke();
		}

		private void OnTriggerEnter(Collider other) => other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);

		#endregion
	}
}
