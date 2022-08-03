using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Entities.Camera
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] private Vector3 _offset;
		[SerializeField] private Transform _target;
		[SerializeField] private float _lerpAmount = 0.123f;
		[SerializeField] private UnityEngine.Camera _cam;

		private float _baseZoom;
		private Transform _tmpTarget;

		public UnityEngine.Camera Camera => _cam;

		private void Awake()
		{
			_baseZoom = _cam.orthographicSize;
		}

		private void LateUpdate()
		{
			Transform use = _tmpTarget ?? _target;

			transform.position = Vector3.Lerp(transform.position, use.position + _offset, _lerpAmount);
		}

		public void Shake(Vector3 intensity, float duration)
		{
			if (duration == 0 || intensity.magnitude == 0)
				return;
			Tween.Shake(transform, transform.position, intensity, duration, 0);
		}

		public void LockTemporaryTarget(Transform target, float zoomMultiplier)
		{
			Tween.Value(_cam.orthographicSize, _cam.orthographicSize * zoomMultiplier, (value) => _cam.orthographicSize = value, 1f, 0f, Tween.EaseOut);
			_tmpTarget = target;
		}

		public void UnlockTarget()
		{
			Tween.Value(_cam.orthographicSize, _baseZoom, (value) => _cam.orthographicSize = value, .5f, 0f, Tween.EaseOut);
			_tmpTarget = null;
		}
	}
}