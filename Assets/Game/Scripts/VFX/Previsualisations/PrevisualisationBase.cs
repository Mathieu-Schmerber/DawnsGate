using Nawlian.Lib.Systems.Pooling;
using UnityEngine;

namespace Game.VFX.Previsualisations
{
	public class PrevisualisationBase : APoolableObject
	{
		protected PrevisuParameters _params;

		public override void Init(object data)
		{
			_params = (PrevisuParameters)data;
			_params.Transform = transform;
			Invoke(nameof(Release), _params.Duration);
		}

		private void Update() => _params.OnUpdate?.Invoke(_params);

		protected override void OnReleasing()
		{
			base.OnReleasing();
			_params.OnRelease?.Invoke(_params);
		}
	}
}
