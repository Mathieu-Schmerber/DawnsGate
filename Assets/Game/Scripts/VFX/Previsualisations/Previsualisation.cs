﻿using Nawlian.Lib.Systems.Pooling;
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
		public static GameObject Circle => Databases.Database.Previsualisations.Circle;

		public static void ShowCircle(Vector3 position, float radius, float duration)
		{
			ObjectPooler.Get(Circle, position, Quaternion.identity, new PrevisuParameters()
			{
				Position = position,
				Duration = duration,
				Size = radius * 2,
			}, null);
		}

		public static void ShowCircle(Vector3 position, float radius, float duration, Action<PrevisuParameters> OnRelease)
		{
			ObjectPooler.Get(Circle, position, Quaternion.identity, new PrevisuParameters()
			{
				Position = position,
				Duration = duration,
				Size = radius * 2,
				OnRelease = OnRelease
			}, null);
		}
	}
}
