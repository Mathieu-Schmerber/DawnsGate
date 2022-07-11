using System;
using UnityEngine;

namespace Game.VFX.Preview
{
	public struct PreviewParameters
	{
		public Transform Transform { get; set; }
		public Vector3 Position { get; set; }
		public float Duration { get; set; }
		public float Size { get; set; }
		public Vector3 Direction { get; set; }
		public Quaternion Rotation { get; internal set; }
		public Action<PreviewParameters> OnRelease { get; set; }
		public Action<PreviewParameters> OnUpdate { get; internal set; }
	}
}
