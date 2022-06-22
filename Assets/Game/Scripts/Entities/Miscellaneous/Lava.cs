using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Game.Systems.Combat.Attacks;
using Game.Systems.Items;
using Nawlian.Lib.Systems.Pooling;
using Nawlian.Lib.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Entities.Miscellaneous
{
	public class Lava : APoolableObject
	{
		private SpecialItemData.Stage _data;
		private List<ParticleSystem> _ps;
		private float _lifetime;

		public EntityIdentity Caster { get; set; }

		private void Awake()
		{
			_ps = transform.GetComponentsInChildren<ParticleSystem>(includeThis: true).ToList();
			_lifetime = _ps.Max(x => x.main.startLifetime.constantMax) + _ps.Max(x => x.main.startDelay.constantMax);
		}

		public override void Init(object data)
		{
			_data = (SpecialItemData.Stage)data;
			_ps.FirstOrDefault()?.Play(true);
			Invoke(nameof(Release), _lifetime);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (Caster.gameObject.layer == other.gameObject.layer)
				return;
			AttackBase.ApplyDamageLogic(Caster, other.GetComponent<Damageable>(), KnockbackDirection.FORWARD, _data.Damage, 0, isDamageDirect: false);
		}
	}
}
