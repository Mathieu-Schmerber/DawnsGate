using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nawlian.Lib.Extensions
{
	public static class ColorExtensions
	{
		public static Color Alpha(this Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);

		public static Color32 RandomColor(int alpha = 255)
		{
			System.Random random = new System.Random();

			return new Color32(
				(byte)random.Next(0, 255),
				(byte)random.Next(0, 255),
				(byte)random.Next(0, 255),
				(byte)alpha
			);
		}

		public static Color32 RandomColor(string seed, int alpha = 255)
		{
			System.Random random = new System.Random(seed.GetHashCode());

			return new Color32(
				(byte)random.Next(0, 255),
				(byte)random.Next(0, 255),
				(byte)random.Next(0, 255),
				(byte)alpha
			);
		}
	}
}
