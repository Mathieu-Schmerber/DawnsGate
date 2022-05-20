using Game.Entities.Shared;
using Pixelplacement;
using Pixelplacement.TweenSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

		/// <summary>
		/// Applies damage to this entity. <br/>
		/// The amount of damage taken will get decreased or increased based on this entity's stats.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="damage"></param>
		public void ApplyDamage(EntityIdentity attacker, float damage)
		{
			if (IsDead) return;

			float totalDamage = damage;

			// Critical hit check
			if (Random.Range(0, 100) <= attacker.Stats.CriticalRate.Value)
				totalDamage += totalDamage * (attacker.Stats.AdditionalCriticalDamage.Value / 100);

			// Apply damage
			if (_identity.CurrentArmor > 0)
				_identity.CurrentArmor -= totalDamage * (attacker.Stats.ArmorDamage.Value / 100);
			else
				_identity.CurrentHealth -= totalDamage;

			// Lifesteal
			attacker.CurrentHealth += totalDamage * (attacker.Stats.LifeSteal.Value / 100);

			// Triggering event
			OnDamage?.Invoke();

			// Death check
			if (IsDead)
				Kill(attacker);
		}

		/// <summary>
		/// Knocks back to this entity. <br/>
		/// The amount of force applied will get decreased or increased based on this entity's stats.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="force"></param>
		/// <param name="damage"></param>
		public void ApplyKnockback(EntityIdentity attacker, Vector3 force, float knockbackTime)
		{
			if (IsDead) return;

			float resistanceRatio = Mathf.Clamp(_identity.Stats.KnockbackResistance.Value, 0, 100);
			Vector3 totalForce = force - (force * (resistanceRatio / 100));

			if (_knockbackMotion != null)
				_knockbackMotion.Stop();
			_knockbackMotion = Tween.Position(transform, transform.position + totalForce, knockbackTime, 0, Tween.EaseOut);
		}

		public void Kill(EntityIdentity attacker)
		{
			OnDeath?.Invoke();
			_identity.CurrentHealth = 0;
			_identity.CurrentArmor = 0;
		}
	}
}