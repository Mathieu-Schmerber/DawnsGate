using Pixelplacement;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using Game.Entities.Shared;
using Game.Scriptables;
using static Game.Scriptables.AttackBaseData;
using System.Linq;

namespace Game.Entities.Shared.Attacks
{
	public class ModularAttack : AttackBase
	{
		private ModularAttackData _attackData;

		public override bool FollowCaster => _attackData.FollowCaster;

		/// TODO: calculate range from particle system and sh*t rather than affecting it manually
		public override float Range => _attackData.Range;

		private List<Collider> _hitColliders = new List<Collider>();
		private float _startTime;
		private Vector3 _baseOffset;
		private Vector3 _velocity;

		public override void Init(AttackBaseData data, EntityIdentity caster)
		{
			base.Init(data, caster);
			_startTime = Time.time;
			_hitColliders.Clear();
			_attackData = data as ModularAttackData;
		}

		public override void OnStart(Vector3 offset, Vector3 travelDistance)
		{
			_baseOffset = offset;

			Vector3 localOffsetDir = transform.InverseTransformDirection(_baseOffset);
			localOffsetDir.x *= -1;
			transform.position = Caster.transform.position + localOffsetDir;

			Vector3 localTravelDir = transform.InverseTransformDirection(travelDistance);
			localTravelDir.x *= -1;
			_velocity = localTravelDir / _attackData.ActiveTime;
		}

		private void Update()
		{
			if (Time.time - _startTime >= _attackData.ActiveTime)
				gameObject.SetActive(false);
			else if (FollowCaster)
			{
				Vector3 localOffsetDir = transform.InverseTransformDirection(_baseOffset);

				localOffsetDir.x *= -1;
				transform.position = Caster.transform.position + localOffsetDir;
			}
			transform.position += _velocity * Time.deltaTime;
		}

		protected override void OnAttackHit(Collider collider)
		{
			if (_hitColliders.Contains(collider)) return;

			IDamageProcessor damageProcessor = collider.GetComponent<IDamageProcessor>();

			_hitColliders.Add(collider);
			if (damageProcessor != null)
			{
				Vector3 direction = _attackData.KnockbackDir == KnockbackDirection.FORWARD ? transform.forward : (collider.transform.position - transform.position).normalized.WithY(0);
				float knockbackForce = _attackData.BaseKnockbackForce;

				// TODO: apply Caster stats
				damageProcessor.ApplyKnockback(gameObject, direction * knockbackForce);
				damageProcessor.ApplyDamage(gameObject, _attackData.BaseDamage);
				ObjectPooler.Get(PoolIdEnum.SLASH_HIT_FX, collider.transform.position.WithY(transform.position.y), Quaternion.Euler(0, transform.rotation.eulerAngles.y + _attackData.HitYRotation, 0), null);
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f - transform.forward * 1.75f, new Vector3(0.4f, 2, .4f));

			Gizmos.color = Color.white;
			Gizmos.DrawRay(transform.position - transform.forward * 1.75f, transform.forward * Range);
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