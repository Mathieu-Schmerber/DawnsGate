using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.Combat.Attacks
{
	public abstract class AttackBase : APoolableObject
	{
		public struct InitData
		{
			public AttackBaseData Data;
			public EntityIdentity Caster;
		}

		protected AttackBaseData _data;

		/// <summary>
		/// Determines if the attack follows the caster (takes it as an anchor point)
		/// </summary>
		public abstract bool FollowCaster { get; }

		/// <summary>
		/// The range of the attack (only informative for the aim assist to act, will not affect the attack behaviour).
		/// </summary>
		public abstract float Range { get; }

		protected EntityIdentity Caster { get; private set; }

		public Action<AttackBaseData, Collider> OnAttackHitEvent { get; set; }

		public override void Init(object data)
		{
			InitData init = (InitData)data;

			_data = init.Data;
			Caster = init.Caster;
		}

		public abstract void OnStart(Vector3 offset, Vector3 travelDistance);

#if UNITY_EDITOR
		protected abstract void OnAttackHit(Collider collider);
#endif

		public abstract (bool isValid, string message) IsAttackEditorValid();

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject == Caster.gameObject)
				return;
			OnAttackHit(other);
		}

		public static void ApplyDamageLogic(EntityIdentity caster, Damageable target, KnockbackDirection knockbackDirection, float damage, float kbForce, GameObject hitFx = null, bool isDamageDirect = true)
		{
			if (target == null || caster.gameObject.layer == target.gameObject.layer)
				return;
			Vector3 direction = knockbackDirection == KnockbackDirection.FORWARD ? caster.transform.forward : (target.transform.position - caster.transform.position).normalized.WithY(0);
			float knockbackForce = caster.Scale(kbForce, StatModifier.KnockbackForce);
			float totalDamage = caster.Scale(damage, StatModifier.AttackDamage);

			if (isDamageDirect)
				target.ApplyDamage(caster, totalDamage);
			else
				target.ApplyPassiveDamage(totalDamage);
			target.ApplyKnockback(caster, direction * knockbackForce, .2f);
			if (hitFx)
				ObjectPooler.Get(hitFx, target.transform.position.WithY(caster.transform.position.y), Quaternion.Euler(0, caster.transform.rotation.eulerAngles.y, 0), null);
		}
	}
}