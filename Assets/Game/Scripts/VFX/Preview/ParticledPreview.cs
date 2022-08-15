using UnityEngine;

namespace Game.VFX.Preview
{
	public class ParticledPreview : PreviewBase
	{
		[SerializeField] private bool _controlDuration = false;
		[SerializeField] private bool _controlScale = false;

		protected ParticleSystem _ps;

		protected virtual void Awake()
		{
			_ps = GetComponentInChildren<ParticleSystem>();
		}

		public override void Init(object data)
		{
			base.Init(data);

			if (_controlScale)
				transform.localScale = Vector3.one * _params.Size;

			if (_controlDuration)
			{
				var main = _ps.main;
				main.duration = _params.Duration;
				main.startLifetime = _params.Duration;
			}
			_ps.Play(true);
		}

		protected override void OnReleasing()
		{
			base.OnReleasing();
			_ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
	}
}
