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
		public static void Show(PrevisualisationBase previsualisation, Vector3 position, float radius, float duration)
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
			}, null);
		}

		public static void Show(PrevisualisationBase previsualisation, Vector3 position, float radius, float duration, Action<PrevisuParameters> OnRelease)
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
	}
}
