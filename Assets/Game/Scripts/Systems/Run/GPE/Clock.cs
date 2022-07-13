using Pixelplacement;
using System;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class Clock : MonoBehaviour
	{
		[SerializeField] private Transform _pivot;

		public void ClockIn(int time)
		{
			float distance = 360 / time;

			for (int i = 0; i < time; i++)
				Tween.Rotate(_pivot, Vector3.up * distance, Space.World, 1, i, Tween.EaseBounce);
		}
	}
}
