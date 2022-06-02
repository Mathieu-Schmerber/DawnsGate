using Game.Entities.Shared;
using Nawlian.Lib.Systems.Pooling;
using Nawlian.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Combat.Effects
{
	public abstract class AEffect : MonoBehaviour
	{
		private Timer _intervalTimer = new();

		protected float _startTime = 0;
		protected float _totalDuration { get; private set; }

		public virtual void OnStart(float duration, AEffectBaseData data)
		{
			if (_startTime == 0)
			{
				_startTime = Time.time;
				_totalDuration = duration;
				Invoke(nameof(Delete), duration);
			}
			else // Effect was already on the entity
			{
				_totalDuration = (Time.time - _startTime) + duration;
				CancelInvoke(nameof(Delete));
				Invoke(nameof(Delete), duration);
			}

			if (data.Interval.Enabled)
				_intervalTimer.Start(data.Interval.Value, true, OnActivation);
			OnActivation();
		}

		public virtual void Delete()
		{
			OnEnd();
			_intervalTimer.Stop();
			Destroy(this);
		}

		protected abstract void OnEnd();
		protected abstract void OnActivation();
	}
}
