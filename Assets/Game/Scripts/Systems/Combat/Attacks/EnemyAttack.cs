using Game.Entities.Player;
using Game.Entities.Shared;
using Game.Managers;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Combat.Attacks
{
	public class EnemyAttack : MonoBehaviour
	{
		[SerializeField] private int _baseDamage;
		[SerializeField] private float _baseKnockbackForce;
		[SerializeField] private float _activeTime;
		[SerializeField] private GameObject _onHitFx;
		[SerializeField] private KnockbackDirection _knockbackDirection;

		private EntityIdentity _identity;
		private AController _controller;
		private float _startTime;
		private Vector3 _positionOffset;
		private Vector3 _angleOffset;

		private void Awake()
		{
			_controller = GetComponentInParent<AController>();
			_identity = GetComponentInParent<EntityIdentity>();

			_positionOffset = transform.localPosition;
			_angleOffset = transform.localEulerAngles;
		}

		private void OnEnable()
		{
			_startTime = Time.time;

			Vector3 baseAngle = _controller.transform.forward;
			Vector3 aimedAngle = _controller.GetAimNormal();
			float angle = Vector3.SignedAngle(baseAngle, aimedAngle, Vector3.up);
			Vector3 dirToOffset = Quaternion.AngleAxis(angle, _controller.transform.up) * _positionOffset;

			transform.localPosition = dirToOffset;
			transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.WithY(_angleOffset.y + angle));
		}

		private void Update()
		{
			if (Time.time - _startTime >= _activeTime)
				gameObject.SetActive(false);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject == transform.parent.gameObject || other.gameObject.GetComponent<PlayerController>() == null) return;

			IDamageProcessor damageProcessor = other.GetComponent<IDamageProcessor>();

			if (damageProcessor != null)
			{
				Vector3 direction = _knockbackDirection == KnockbackDirection.FORWARD ? transform.forward : (other.transform.position - transform.position).normalized.WithY(0);
				float knockbackForce = _identity.Scale(_baseKnockbackForce, StatModifier.KnockbackForce);
				float totalDamage = _identity.Scale(_baseDamage, StatModifier.AttackDamage);

				damageProcessor.ApplyDamage(_identity, totalDamage);
				damageProcessor.ApplyKnockback(_identity, direction * knockbackForce);
				if (_onHitFx)
					ObjectPooler.Get(_onHitFx, other.transform.position.WithY(transform.position.y), Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), null);
			}
		}
	}
}
