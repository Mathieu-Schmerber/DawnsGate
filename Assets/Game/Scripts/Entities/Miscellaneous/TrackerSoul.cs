using Game.Entities.AI;
using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Game.Systems.Combat.Attacks;
using Game.Systems.Items;
using Game.VFX;
using Nawlian.Lib.Systems.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.Miscellaneous
{
	public class TrackerSoul : APoolableObject
	{
		[SerializeField] private GameObject _explosionFx;
		[SerializeField] private float _speed;
		[SerializeField] private float _rotateSpeed;

		private SpecialItemData.Stage _data;
		private Rigidbody _rb;
		private Transform _target;

		public EntityIdentity Caster { get; set; }

		private bool _exploded = false;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody>();
		}

		public override void Init(object data)
		{
			_data = (SpecialItemData.Stage)data;
			_target = null;
			_exploded = false;
			Invoke(nameof(Release), _data.Duration);
		}

		private Collider GetClosestEnemyCollider(Collider[] colliders)
		{
			float bestDistance = 99999.0f;
			Collider bestCollider = null;
			Collider[] filtered = colliders.Where(x => x.GetComponent<EnemyAI>() != null).ToArray();

			foreach (Collider enemy in filtered)
			{
				float distance = Vector3.Distance(transform.position, enemy.transform.position);

				if (distance < bestDistance)
				{
					bestDistance = distance;
					bestCollider = enemy;
				}
			}
			return bestCollider;
		}

		private void FixedUpdate()
		{
			var heading = _target.transform.position - transform.position;
			var rot = Quaternion.LookRotation(heading);

			Debug.DrawLine(transform.position, _target.position, Color.red);
			transform.rotation = Quaternion.Lerp(transform.rotation, rot, _rotateSpeed * Time.deltaTime);
			_rb.velocity = heading.normalized * _speed;
		}

		private void Update()
		{
			if (_target == null || !_target.gameObject.activeSelf)
				_target = GetClosestEnemyCollider(Physics.OverlapSphere(transform.position, 1000f))?.transform;

			if (!_exploded && Vector3.Distance(transform.position, _target.position) < 0.5f)
			{
				_exploded = true;
				CancelInvoke(nameof(Release));
				Release();
				AttackBase.ApplyDamageLogic(Caster, _target.GetComponent<Damageable>(), KnockbackDirection.FROM_CENTER, _data.Damage, 0, null, false);
			}
		}

		protected override void OnReleasing()
		{
			base.OnReleasing();

			if (_explosionFx)
				ObjectPooler.Get(_explosionFx, transform.position, Quaternion.identity, null, null);
		}
	}
}