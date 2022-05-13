using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Camera
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] private Vector3 _offset;
		[SerializeField] private Transform _target;
		[SerializeField] private float _lerpAmount = 0.123f;

		private void LateUpdate()
		{
			transform.position = Vector3.Lerp(transform.position, _target.position + _offset, _lerpAmount);
		}

		public void Shake(Vector3 intensity, float duration)
		{
			if (duration == 0 || intensity.magnitude == 0)
				return;
			Tween.Shake(transform, transform.position, intensity, duration, 0);
		}
	}
}