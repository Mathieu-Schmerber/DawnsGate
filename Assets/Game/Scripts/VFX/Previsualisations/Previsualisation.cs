using Nawlian.Lib.Systems.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.VFX.Previsualisations
{
	public static class Previsualisation
	{
		public static void Show(PrevisualisationBase previsualisation, Vector3 position, float radius, float duration, Action<PrevisuParameters> OnRelease = null)
		{
			if (previsualisation == null)
			{
				Debug.LogError("Cannot display a null previsualisation.");
				return;
			}

			ObjectPooler.Get(previsualisation.gameObject, position, Quaternion.identity, new PrevisuParameters()
			{
				Position = position,
				Duration = duration,
				Size = radius * 2,
				OnRelease = OnRelease
			}, null);
		}

		public static void Show(PrevisualisationBase previsualisation, Vector3 position, Quaternion rotation, float radius, float duration, Action<PrevisuParameters> OnRelease = null, Action<PrevisuParameters> OnUpdate = null)
		{
			if (previsualisation == null)
			{
				Debug.LogError("Cannot display a null previsualisation.");
				return;
			}

			ObjectPooler.Get(previsualisation.gameObject, position, rotation, new PrevisuParameters()
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
