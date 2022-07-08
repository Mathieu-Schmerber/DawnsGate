using System;
using UnityEngine;

namespace Game.VFX.Previsualisations
{
	public struct PrevisuParameters
	{
		public Transform Transform { get; set; }
		public Vector3 Position { get; set; }
		public float Duration { get; set; }
		public float Size { get; set; }
		public Vector3 Direction { get; set; }
		public Quaternion Rotation { get; internal set; }
		public Action<PrevisuParameters> OnRelease { get; set; }
		public Action<PrevisuParameters> OnUpdate { get; internal set; }
	}
}
