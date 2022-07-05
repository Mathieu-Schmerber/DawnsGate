using System;
using UnityEngine;

namespace Game.VFX.Previsualisations
{
	public struct PrevisuParameters
	{
		public Vector3 Position { get; set; }
		public float Duration { get; set; }
		public float Size { get; set; }
		public Vector3 Direction { get; set; }
		public Action<PrevisuParameters> OnRelease { get; set; }
	}
}
