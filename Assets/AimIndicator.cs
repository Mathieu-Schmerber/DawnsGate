using Game.Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
    public class AimIndicator : MonoBehaviour
    {
        private PlayerController _controller;
		[SerializeField, Range(0, 1)] private float _rotationSpeed;

		private void Awake()
		{
			_controller = GetComponentInParent<PlayerController>();
		}

		private void Update()
		{
			Vector3 aim = _controller.GetAimNormal();

			if (aim != Vector3.zero)
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(aim), _rotationSpeed);
		}
	}
}
