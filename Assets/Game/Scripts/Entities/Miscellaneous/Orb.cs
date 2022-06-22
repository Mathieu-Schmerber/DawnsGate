using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Game.Systems.Combat.Attacks;
using Game.Systems.Items;
using Nawlian.Lib.Systems.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.Miscellaneous
{
	public class Orb : MonoBehaviour
	{
		[SerializeField] private Material _active;
		[SerializeField] private Material _inactive;
		[SerializeField] private float _inactiveTime;

		private MeshRenderer _renderer;
		private bool _isActive = true;
		private EntityIdentity _caster;
		
		public float Damage { get; set; }

		private void Awake()
		{
			_caster = GetComponentInParent<EntityIdentity>();
			_renderer = GetComponent<MeshRenderer>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer == _caster.gameObject.layer || !_isActive)
				return;

			Damageable damage = other.GetComponent<Damageable>();

			if (damage)
			{
				AttackBase.ApplyDamageLogic(_caster, damage, KnockbackDirection.FROM_CENTER, Damage, 0, isDamageDirect: false);
				SwitchOff();
				Invoke(nameof(SwitchOn), _inactiveTime);
			}
		}

		private void SwitchOn()
		{
			_isActive = true;
			_renderer.material = _active;
		}

		private void SwitchOff()
		{
			_isActive = false;
			_renderer.material = _inactive;
		}
	}
}
