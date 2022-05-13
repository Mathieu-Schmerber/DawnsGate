using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Nawlian.Lib.Utils
{
	public class Awaiter
	{
		public static async void WaitAndExecute(float time, Action execute)
		{
			float start = Time.time;
			float end = start + time;

			while (Time.time < end)
				await Task.Yield();
			execute.Invoke();
		}

		public static async Task ExecuteAndWait(float time, Action execute)
		{
			execute.Invoke();

			float start = Time.time;
			float end = start + time;

			while (Time.time < end)
				await Task.Yield();
		}
	}
}
