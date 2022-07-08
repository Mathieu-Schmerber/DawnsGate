using Pixelplacement;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using Game.Entities.Shared;
using static Game.Systems.Combat.Attacks.AttackBaseData;
using System.Linq;
using Game.Entities.Shared.Health;

namespace Game.Systems.Combat.Attacks
{
	public class ModularAttack : AttackBase
	{
		[SerializeField] private ModularAttackData _attackData;

		public override bool FollowCaster => _attackData.FollowCaster;

		/// TODO: calculate range from particle system and sh*t rather than affecting it manually
		public override float Range => _attackData?.Range ?? 0;

		private List<Collider> _hitColliders = new List<Collider>();
		private float _startTime;
		private Vector3 _baseOffset;
		private Vector3 _velocity;

		public override void Init(object data)
		{
			base.Init(data);
			_startTime = Time.time;
			_hitColliders.Clear();
			_attackData = _data as ModularAttackData;
		}

		public override void OnStart(Vector3 offset, float travelDistance)
		{
			_baseOffset = offset;

			Vector3 localOffsetDir = transform.InverseTransformDirection(_baseOffset);
			localOffsetDir.x *= -1;
			transform.position = Caster.transform.position + localOffsetDir;

			if (travelDistance != 0)
			{
				Vector3 travelDest = transform.position + transform.forward * travelDistance;

				Debug.DrawRay(transform.position, transform.forward * travelDistance, Color.magenta, _attackData.ActiveTime);
				Tween.LocalPosition(transform, travelDest, _attackData.ActiveTime, 0, Tween.EaseLinear);
			}
		}

		private void Update()
		{
			if (Time.time - _startTime >= _attackData.ActiveTime)
				Release();
			else if (FollowCaster)
			{
				Vector3 localOffsetDir = transform.InverseTransformDirection(_baseOffset);

				localOffsetDir.x *= -1;
				transform.position = Caster.transform.position + localOffsetDir;
			}
		}

		protected override void OnAttackHit(Collider collider)
		{
			if (_hitColliders.Contains(collider)) return;

			Damageable damageProcessor = collider.GetComponent<Damageable>();

			_hitColliders.Add(collider);
			if (damageProcessor != null)
			{
				Vector3 direction = _attackData.KnockbackDir == KnockbackDirection.FORWARD ? transform.forward : (collider.transform.position - transform.position).normalized.WithY(0);
				float knockbackForce = Caster.Scale(_attackData.BaseKnockbackForce, StatModifier.KnockbackForce);
				float totalDamage = Caster.Scale(_attackData.BaseDamage, StatModifier.AttackDamage);
				float applied = damageProcessor.ApplyDamage(Caster, totalDamage);

				damageProcessor.ApplyKnockback(Caster, direction * knockbackForce);
				ObjectPooler.Get(_attackData.HitFx, collider.transform.position.WithY(transform.position.y), Quaternion.Euler(0, transform.rotation.eulerAngles.y + _attackData.HitYRotation, 0), null);
				OnAttackHitEvent?.Invoke(_data, damageProcessor, applied);
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, new Vector3(0.4f, 2, .4f));

			Gizmos.color = Color.white;
			Gizmos.DrawRay(transform.position, transform.forward * Range);
		}

#if UNITY_EDITOR

		public override (bool isValid, string message) IsAttackEditorValid()
		{
			var colliderCheck = GetComponents<Collider>();

			if (colliderCheck.Length == 0)
				return (false, "No collider found, this attack won't hit");
			else if (colliderCheck.Any(x => x.isTrigger == false))
				return (false, "Some colliders are not triggers, those won't hit");

			var psCheck = GetComponents<ParticleSystem>();

			if (psCheck.Length == 0)
				return (false, "No particle system found");

			return (true, "");
		}
#endif
	}
}