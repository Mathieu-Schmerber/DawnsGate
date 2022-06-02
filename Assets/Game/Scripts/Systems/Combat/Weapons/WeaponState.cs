using System;
using UnityEngine;

namespace Game.Systems.Combat.Weapons
{
	/// <summary>
	/// Manages weapon state between being an item on the ground or equipped in hand
	/// </summary>
	[RequireComponent(typeof(Weapon), typeof(WeaponItem))]
	public class WeaponState : MonoBehaviour
	{
		public bool IsOnGround => transform.parent == null;
		public WeaponItem ItemState { get; private set; }
		public Weapon EquippedState { get; private set; }

		public event Action OnEquipStateEnabled;
		public event Action OnItemStateEnabled;

		private void Awake()
		{
			ItemState = GetComponent<WeaponItem>();
			EquippedState = GetComponent<Weapon>();
		}

		private void Start()
		{
			if (IsOnGround)
				ToItemState();
			else
				ToEquippedState();
		}

		public void ToItemState()
		{
			transform.parent = null;

			ItemState.enabled = true;
			EquippedState.enabled = false;
			OnItemStateEnabled?.Invoke();
		}

		public void ToEquippedState()
		{
			EquippedState.enabled = true;
			ItemState.enabled = false;
			OnEquipStateEnabled?.Invoke();
		}
	}
}
