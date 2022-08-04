using Nawlian.Lib.Extensions;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Systems.Combat.Attacks
{
	public class LaserPart : AttackSubPart
	{
		private CapsuleCollider _collider;
		private VisualEffect _vfx;
		[SerializeField] private LayerMask _wallMask;

		protected override void Awake()
		{
			base.Awake();
			_collider = GetComponent<CapsuleCollider>();
			_vfx = GetComponentInChildren<VisualEffect>();
		}

		private void Update()
		{
			Vector3 rightOffset = transform.right * _collider.radius / 2;
			bool right = Physics.Raycast(transform.position + rightOffset, transform.forward, out RaycastHit rightHit, Mathf.Infinity, _wallMask);
			bool left = Physics.Raycast(transform.position - rightOffset, transform.forward, out RaycastHit leftHit, Mathf.Infinity, _wallMask);
			bool center = Physics.Raycast(transform.position, transform.forward, out RaycastHit centerHit, Mathf.Infinity, _wallMask);

			if (right || left || center)
			{
				float distance = Mathf.Min(rightHit.distance, leftHit.distance, centerHit.distance);

				_vfx.transform.localScale = _vfx.transform.localScale.WithZ(distance / 2);
				_vfx.transform.localPosition = _vfx.transform.localPosition.WithZ(distance / 2);
				_collider.center = _collider.center.WithZ(distance / 2);
				_collider.height = distance;
			}
		}
	}
}