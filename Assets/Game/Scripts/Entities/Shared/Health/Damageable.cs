using Game.VFX;
using Nawlian.Lib.Utils;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using StatModifier = Game.Entities.Shared.StatModifier;

namespace Game.Entities.Shared.Health
{
	/// <summary>
	/// Defines a damageable entity
	/// </summary>
	public class Damageable : MonoBehaviour, IDamageProcessor
	{
		protected EntityIdentity _identity;
		protected AController _controller;
		protected Rigidbody _rb;

		public virtual bool IsDead => _identity.CurrentHealth <= 0;

		public static event Action<Damageable> OnDeath;
		public event Action OnBeforeDamaged;
		public event Action<float> OnDamaged;

		protected virtual void Awake()
		{
			_identity = GetComponent<EntityIdentity>();
			_controller = GetComponent<AController>();
			_rb = GetComponent<Rigidbody>();
		}

		/// <summary>
		/// Applies damage to this entity. <br/>
		/// The amount of damage taken will get decreased or increased based on this entity's stats.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="damage"></param>
		public virtual float ApplyDamage(EntityIdentity attacker, float damage)
		{
			if (IsDead || _identity.IsInvulnerable) return 0;

			OnBeforeDamaged?.Invoke();

			float totalDamage = damage;

			// Critical hit check
			if (Random.Range(0, 100) <= attacker.Stats.Modifiers[StatModifier.CriticalRate]?.Value)
			{
				totalDamage = attacker.Scale(totalDamage, StatModifier.CriticalDamage);
				QuickText.ShowDamageText(transform.position, totalDamage - damage, new Color(1, 0.25f, 0, 1));
			}

			// Apply damage
			if (_identity.CurrentArmor > 0)
				_identity.CurrentArmor -= attacker.Scale(totalDamage, StatModifier.ArmorDamage);
			else
				_identity.CurrentHealth -= totalDamage;

			// Stun
			if (_identity.CurrentArmor == 0) {
				float baseStunTime = 0.5f;
				float resistanceRatio = Mathf.Clamp(_identity.Stats.Modifiers[StatModifier.StunResistance]?.Value ?? 0, 0, 100);
				float reduced = baseStunTime - (baseStunTime * (resistanceRatio / 100));

				_controller?.Stun(reduced);
			}

			// Lifesteal
			attacker.CurrentHealth += attacker.Scale(totalDamage, StatModifier.LifeSteal);

			// Triggering event
			OnDamaged?.Invoke(totalDamage);

			// Death check
			if (IsDead)
				Kill(attacker);
			return totalDamage;
		}

		public virtual void ApplyPassiveDamage(float damage)
		{
			if (IsDead || _identity.IsInvulnerable) return;

			// Apply damage
			if (_identity.CurrentArmor > 0)
				_identity.CurrentArmor -= damage;
			else
				_identity.CurrentHealth -= damage;

			QuickText.ShowDamageText(transform.position, damage, Color.magenta);

			// Triggering event
			OnDamaged?.Invoke(damage);

			// Death check
			if (IsDead)
				Kill(null);
		}

		/// <summary>
		/// Knocks back this entity. <br/>
		/// The amount of force applied will get decreased or increased based on this entity's stats.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="force"></param>
		public virtual void ApplyKnockback(EntityIdentity attacker, Vector3 force)
		{
			if (IsDead || _identity.IsInvulnerable) return;

			float resistanceRatio = Mathf.Clamp(_identity.Stats.Modifiers[StatModifier.KnockbackResistance]?.Value ?? 0, 0, 100);
			Vector3 totalForce = force - (force * (resistanceRatio / 100));

			if (totalForce == Vector3.zero)
				return;

			float speed = IDamageProcessor.KNOCKBACK_SPEED;
			Vector3 velocity = speed * totalForce.normalized;
			float time = velocity.magnitude / speed;

			_rb.velocity = velocity;
			Awaiter.WaitAndExecute(time, () => _rb.velocity = Vector3.zero);
		}

		public virtual void Kill(EntityIdentity attacker)
		{
			OnDeath?.Invoke(this);
			_identity.CurrentHealth = 0;
			_identity.CurrentArmor = 0;
		}
	}
}