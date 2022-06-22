using UnityEngine;
using Nawlian.Lib.Systems.Pooling;
using Game.Systems.Items;
using Game.VFX;
using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Game.Systems.Combat.Attacks;
using Pixelplacement;

namespace Game.Entities.Miscellaneous
{
	public class Bomb : APoolableObject
	{
		[SerializeField] private GameObject _explosionFx;
		[SerializeField] private float _knockbackForce;
		[SerializeField] private Transform _gfx;

		private ActiveItemData.ActiveStage _data;
		private EntityCircle _circle;

		public EntityIdentity Caster { get; set; }

		private void Awake()
		{
			_circle = GetComponentInChildren<EntityCircle>();
		}

		public override void Init(object data)
		{
			_data = (ActiveItemData.ActiveStage)data;
			_circle.transform.localScale = Vector3.one * _data.Range * 2;
			Tween.LocalScale(_gfx, Vector3.one * .3f, Vector3.one * .7f, _data.Duration, 0, Tween.EaseBounce);
			Invoke(nameof(Release), _data.Duration);
		}

		protected override void OnReleasing()
		{
			base.OnReleasing();

			var inRange = Physics.OverlapSphere(transform.position, _data.Range);

			foreach (var obj in inRange)
			{
				if (obj.transform.gameObject.layer != Caster.gameObject.layer)
				{
					Damageable dmg = obj.GetComponent<Damageable>();

					if (dmg == null)
						continue;
					AttackBase.ApplyDamageLogic(Caster, dmg, KnockbackDirection.FROM_CENTER, _data.Damage, _knockbackForce);
				}
			}
			if (_explosionFx)
				ObjectPooler.Get(_explosionFx, transform.position, Quaternion.identity, null, null);
		}
	}
}
