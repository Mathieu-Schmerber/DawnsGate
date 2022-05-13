using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Nawlian.Lib.Utils
{
	/// <summary>
	/// Flexible timer
	/// </summary>
	public class Timer
	{
		private float _lastTickFrame;
		private float _startFrame;
		private bool _started = false;

		/// <summary>
		/// Does the Timer automatically Stop() after ticking once ?
		/// </summary>
		public bool AutoReset { get; set; }

		/// <summary>
		/// Interval between each Timer.OnTick()
		/// </summary>
		public float Interval { get; set; }

		/// <summary>
		/// Callback executed whenever Timer.OnTick() triggers
		/// </summary>
		public Action OnTickCallback { get; set; }

		/// <summary>
		/// Is the Timer running ?
		/// </summary>
		public bool IsRunning { get; private set; }

		/// <summary>
		/// The elapsed time (in second) since the Cooldown started. <br/>
		/// This will only work as expected if the Cooldown is started, and will still be working after the Cooldown Stop() method is called.
		/// </summary>
		public float ElapsedTime => Time.time - _startFrame;

		/// <summary>
		/// The elapsed time (in second) since the Cooldown last ticked.
		/// </summary>
		public float TimeSinceLastTick => Time.time - _lastTickFrame;
		
		/// <summary>
		/// Time frame of last tick.
		/// </summary>
		public float LastTickFrame => _lastTickFrame;

		/// <summary>
		/// The remaining time (in second) until the Cooldown ticks again.
		/// </summary>
		public float TimeUntilNextTick => (_lastTickFrame + Interval) - Time.time;

		/// <summary>
		/// Starts the Cooldown
		/// </summary>
		/// <param name="interval">The time interval in second</param>
		/// <param name="onTickCallback">The ontick callback that will be called once per interval until the Cooldown is manually stopped.</param>
		public void Start(float interval, bool autoReset, Action onTickCallback = null)
		{
			Interval = interval;
			AutoReset = autoReset;
			OnTickCallback = onTickCallback;

			_startFrame = Time.time;
			_lastTickFrame = Time.time;
			_started = true;

			if (!IsRunning)
			{
				IsRunning = true;
				Run();
			}
		}

		/// <summary>
		/// Stops the Cooldown
		/// </summary>
		public void Stop()
		{
			IsRunning = false;
		}

		/// <summary>
		/// Returns true if the Timer has ticked. <br/>
		/// This method is useless if AutoReset has been set to true.
		/// </summary>
		/// <returns></returns>
		public bool IsOver() => AutoReset == false && IsRunning == false && _started == true;

		/// <summary>
		/// Restarts the Cooldown. <br/>
		/// Calls Stop() then Start().
		/// </summary>
		public void Restart()
		{
			Stop();
			Start(Interval, AutoReset, OnTickCallback);
		}

		/// <summary>
		/// Timer's async management loop
		/// </summary>
		private async void Run()
		{
			while (IsRunning)
			{
				if (Time.time - _lastTickFrame >= Interval)
				{
					OnTick();
					if (!AutoReset)
					{
						Stop();
						break;
					}
				}
				await Task.Yield();
			}
			await Task.CompletedTask;
		}

		private void OnTick()
		{
			_lastTickFrame = Time.time;
			OnTickCallback?.Invoke();
		}
	}
}