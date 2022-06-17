using Nawlian.Lib.Systems.Pooling;
using Pixelplacement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.VFX.Previsualisations
{
	public class Circle : PrevisualisationBase
	{
		private ParticleSystem _ps;

		private void Awake()
		{
			_ps = GetComponent<ParticleSystem>();
		}

		public override void Init(object data)
		{
			base.Init(data);

			transform.localScale = Vector3.one * _params.Size;

			var main = _ps.main;
			main.duration = _params.Duration;
			main.startLifetime = _params.Duration;
			_ps.Play(true);
		}

		protected override void OnReleasing()
		{
			base.OnReleasing();
			_ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
	}
}
