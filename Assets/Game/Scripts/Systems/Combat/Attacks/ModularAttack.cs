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

		public override float Range => _attackData?.Range ?? 0;

		private bool _isOff = false;

		private List<Collider> _hitColliders = new List<Collider>();
		private float _startTime;
		private Vector3 _baseOffset;
		private Vector3 _baseScale = Vector3.zero;
		private ParticleSystem[] _pss;

		private void Awake()
		{
			_pss = GetComponentsInChildren<ParticleSystem>(true);
		}

		public override void Init(object data)
		{
			base.Init(data);
			_startTime = Time.time;
			_hitColliders.Clear();
			_attackData = _data as ModularAttackData;
			_isOff = false;

			if (_baseScale != Vector3.zero)
				transform.localScale = _baseScale;
		}

		public override void OnStart(Vector3 offset, float travelDistance)
		{
			_baseOffset = offset;
			_baseScale = transform.localScale;

			Vector3 localOffsetDir = transform.InverseTransformDirection(_baseOffset);
			localOffsetDir.x *= -1;
			transform.position = Caster.transform.position + localOffsetDir;

			if (travelDistance != 0)
			{
				Vector3 travelDest = transform.position + transform.forward * travelDistance;
				Tween.LocalPosition(transform, travelDest, _attackData.ActiveTime, 0, Tween.EaseLinear);
			}

			if (_attackData.ScaleOverLifetime)
				Tween.LocalScale(transform, _attackData.EndScale, _attackData.ActiveTime, 0, Tween.EaseLinear);
			if (_attackData.RotateOverTime)
				Tween.Rotate(transform, _attackData.EndRotation, Space.Self, _attackData.ActiveTime, 0, Tween.EaseLinear);
		}

		private void Update()
		{
			if (_isOff)
				return;
			else if (Time.time - _startTime >= _attackData.ActiveTime)
				SmoothRelease();
			else if (FollowCaster)
			{
				Vector3 localOffsetDir = transform.InverseTransformDirection(_baseOffset);

				localOffsetDir.x *= -1;
				transform.position = Caster.transform.position + localOffsetDir;
			}
		}

		public override void OnAttackHit(Collider collider)
		{
			if ((!_attackData.ContinuouslyHit && _hitColliders.Contains(collider)) || _isOff) return;

			Damageable damageProcessor = collider.GetComponent<Damageable>();

			if (damageProcessor != null)
			{
				Vector3 direction = _attackData.KnockbackDir == KnockbackDirection.FORWARD ? transform.forward : (collider.transform.position - transform.position).normalized.WithY(0);
				float knockbackForce = Caster.Scale(_attackData.BaseKnockbackForce, StatModifier.KnockbackForce);
				float totalDamage = Caster.Scale(_attackData.BaseDamage, StatModifier.AttackDamage);
				float applied = damageProcessor.ApplyDamage(Caster, totalDamage);

				damageProcessor.ApplyKnockback(Caster, direction * knockbackForce);
				ObjectPooler.Get(_attackData.HitFx, collider.transform.position.WithY(transform.position.y), Quaternion.Euler(0, transform.rotation.eulerAngles.y + _attackData.HitYRotation, 0), null);
				OnAttackHitEvent?.Invoke(_data, damageProcessor, applied);
				if (applied > 0)
					_hitColliders.Add(collider);
				return;
			}
			_hitColliders.Add(collider);
		}

		private void SmoothRelease()
		{
			_isOff = true;
			if (_pss?.Length > 0)
			{
				float time = _pss.Max(x => x.main.startLifetime.constantMax) + _pss.Max(x => x.main.startDelay.constantMax);
				Invoke(nameof(Release), time);
			}
			else
				Release();
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
			var childCheck = GetComponentsInChildren<Collider>();
			var colliderCheck = GetComponents<Collider>();

			if (colliderCheck.Length == 0 && childCheck.Length == 0)
				return (false, "No collider found, this attack won't hit");
			else if (colliderCheck.Any(x => x.isTrigger == false) || childCheck.Any(x => x.isTrigger == false))
				return (false, "Some colliders are not triggers, those won't hit");

			var psCheck = transform.GetComponentsInChildren<ParticleSystem>();

			if (psCheck.Length == 0)
				return (false, "No particle system found");

			return (true, "");
		}
#endif
	}
}