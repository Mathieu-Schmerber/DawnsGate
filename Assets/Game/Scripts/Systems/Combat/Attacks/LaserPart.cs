using Nawlian.Lib.Extensions;
using UnityEngine;

namespace Game.Systems.Combat.Attacks
{
	public class LaserPart : AttackSubPart
	{
		private CapsuleCollider _collider;
		private LineRenderer _lr;
		[SerializeField] private LayerMask _wallMask;

		protected override void Awake()
		{
			base.Awake();
			_collider = GetComponent<CapsuleCollider>();
			_lr = GetComponent<LineRenderer>();
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

				_lr.SetPosition(0, transform.position);
				_lr.SetPosition(1, transform.position + transform.forward * distance);
				_collider.center = _collider.center.WithZ(distance / 2);
				_collider.height = distance;
			}
		}
	}
}