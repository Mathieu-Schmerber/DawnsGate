using Game.Tools;
using Nawlian.Lib.Utils;
using Pixelplacement;
using System;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class Clock : MonoBehaviour
	{
		[SerializeField] private Transform _pivot;
		[SerializeField] private AudioClip _endAudio;

		private RandomAudioClip _rdm;

		private void Awake()
		{
			_rdm = GetComponent<RandomAudioClip>();
		}

		public void ClockIn(int time)
		{
			float distance = 360 / time;

			Awaiter.WaitAndExecute(time, () => _rdm.Play(_endAudio));
			for (int i = 0; i < time; i++)
				Tween.Rotate(_pivot, Vector3.up * distance, Space.World, 1, i, Tween.EaseBounce, startCallback: () => _rdm.PlayRandom());

		}
	}
}
