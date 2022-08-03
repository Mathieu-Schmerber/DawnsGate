using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Game.VFX.Preview;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using System;
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

		public EntityIdentity Caster { get; private set; }

		public Action<AttackBaseData, Damageable, float> OnAttackHitEvent { get; set; }

		public override void Init(object data)
		{
			InitData init = (InitData)data;

			_data = init.Data;
			Caster = init.Caster;
		}

		public abstract void OnStart(Vector3 offset, float travelDistance);

		public abstract void OnAttackHit(Collider collider);

#if UNITY_EDITOR
		public abstract (bool isValid, string message) IsAttackEditorValid();
#endif

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
			target.ApplyKnockback(caster, direction * knockbackForce);
			if (hitFx)
				ObjectPooler.Get(hitFx, target.transform.position.WithY(caster.transform.position.y), Quaternion.Euler(0, caster.transform.rotation.eulerAngles.y, 0), null);
		}

		public static GameObject Spawn<T>(T attack, Vector3 position, Quaternion rotation, InitData init) where T : AttackBaseData
			=> ObjectPooler.Get(attack.Prefab.gameObject, position, rotation, init);

		public static void ShowAttackPrevisu<T>(T attack, Vector3 position, float duration, AController caster, Action<PreviewParameters> OnRelease = null, Action<PreviewParameters> OnUpdate = null, bool stickToCaster = false) where T : AttackBaseData
		{
			if (attack.StickToCaster || stickToCaster)
			{
				Preview.Show(attack.Previsualisation,
					position,
					Quaternion.identity,
					attack.AttackRange,
					duration, 
					OnRelease,
					OnUpdate: (PreviewParameters previsu) =>
					{
						previsu.Transform.position = caster.transform.position;
						previsu.Transform.rotation = Quaternion.LookRotation(caster.GetAimNormal());
						OnUpdate?.Invoke(previsu);
					});
			}
			else
			{
				Preview.Show(attack.Previsualisation, 
					position,
					Quaternion.LookRotation(caster.GetAimNormal()), 
					attack.AttackRange, 
					duration, 
					OnRelease,
					OnUpdate);
			}
		}
	}
}