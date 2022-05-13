using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nawlian.Lib.Systems.Health
{
	/// <summary>
	/// Defines a way of procressing damage
	/// </summary>
	public class BasicDamageProcessor : MonoBehaviour, IDamageProcessor
	{
		protected IDamageableEntity _entity;

		public bool IsDead => _entity.CurrentHealth <= 0;

		public static event Action<GameObject> OnDeath;
		public event Action OnDamage;

		protected virtual void Awake()
		{
			_entity = GetComponent<IDamageableEntity>();
		}

		public void Damage(GameObject attacker, float amount)
		{
			if (IsDead) return;

			_entity.CurrentHealth -= amount;
			OnDamage?.Invoke();
			if (IsDead)
				Kill(attacker);
		}

		public virtual void Kill(GameObject attacker)
		{
			_entity.CurrentHealth = 0;
			OnDeath?.Invoke(gameObject);
		}
	}
}