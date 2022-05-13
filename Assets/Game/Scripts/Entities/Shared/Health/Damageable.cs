using Game.Entities.Shared;
using Pixelplacement;
using Pixelplacement.TweenSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Entities.Shared.Health
{
	/// <summary>
	/// Defines a damageable entity
	/// </summary>
	[RequireComponent(typeof(EntityIdentity))]
	public class Damageable : MonoBehaviour, IDamageProcessor
	{
		private EntityIdentity _identity;
		private TweenBase _knockbackMotion = null;

		public bool IsDead => _identity.CurrentHealth <= 0;

		public event Action OnDeath;
		public event Action OnDamage;

		private void Awake()
		{
			_identity = GetComponent<EntityIdentity>();
		}

		public void ApplyDamage(GameObject attacker, int damage)
		{
			if (IsDead) return;

			_identity.CurrentHealth -= damage;
			OnDamage?.Invoke();
			if (IsDead)
				Kill(attacker);
		}

		public void ApplyKnockback(GameObject attacker, Vector3 force, float knockbackTime)
		{
			if (IsDead) return;

			// TODO: apply knockback resistance before pushing
			if (_knockbackMotion != null)
				_knockbackMotion.Stop();
			_knockbackMotion = Tween.Position(transform, transform.position + force, knockbackTime, 0, Tween.EaseOut);
		}

		public void Kill(GameObject attacker)
		{
			OnDeath?.Invoke();
			_identity.CurrentHealth = 0;
		}
	}
}