using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tools
{
	public class RemoteJoint : MonoBehaviour
	{
		[SerializeField] private Transform _parentJoint;
		[SerializeField] private bool _syncPosition = true;
		[SerializeField] private bool _syncRotation = true;

		[Button]
		private void Stick()
		{
			if (_syncPosition)
				transform.position = _parentJoint.position;
			if (_syncRotation)
				transform.rotation = _parentJoint.rotation;
		}

		private void Update() => Stick();
	}
}