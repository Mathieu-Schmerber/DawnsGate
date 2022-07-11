using Nawlian.Lib.Systems.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.VFX.Preview
{
	public static class Preview
	{
		public static void Show(PreviewBase previsualisation, Vector3 position, float radius, float duration, Action<PreviewParameters> OnRelease = null)
		{
			if (previsualisation == null)
				return;

			ObjectPooler.Get(previsualisation.gameObject, position, Quaternion.identity, new PreviewParameters()
			{
				Position = position,
				Duration = duration,
				Size = radius * 2,
				OnRelease = OnRelease
			}, null);
		}

		public static void Show(PreviewBase previsualisation, Vector3 position, Quaternion rotation, float radius, float duration, Action<PreviewParameters> OnRelease = null, Action<PreviewParameters> OnUpdate = null)
		{
			if (previsualisation == null)
				return;

			ObjectPooler.Get(previsualisation.gameObject, position, rotation, new PreviewParameters()
			{
				Position = position,
				Rotation = rotation,
				Duration = duration,
				Size = radius * 2,
				OnRelease = OnRelease,
				OnUpdate = OnUpdate
			}, null);
		}
	}
}
