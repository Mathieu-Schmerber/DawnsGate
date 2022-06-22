using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nawlian.Lib.Extensions;

namespace Nawlian.Lib.Systems.Pooling
{
	public class PoolableParticleSystem : BasicPoolableBehaviour
	{
		protected List<ParticleSystem> _ps;

		private void Awake()
		{
			_ps = transform.GetComponentsInChildren<ParticleSystem>(includeThis: true).ToList();
		}

		public override void Release()
		{
			float time = _ps.Max(x => x.main.startLifetime.constantMax) + _ps.Max(x => x.main.startDelay.constantMax);

			_ps.FirstOrDefault()?.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			Invoke(nameof(TrueRelease), time);
		}

		private void TrueRelease() => base.Release();
	}
}
