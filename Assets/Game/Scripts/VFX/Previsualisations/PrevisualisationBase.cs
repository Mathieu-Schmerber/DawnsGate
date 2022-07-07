using Nawlian.Lib.Systems.Pooling;

namespace Game.VFX.Previsualisations
{
	public class PrevisualisationBase : APoolableObject
	{
		protected PrevisuParameters _params;

		public override void Init(object data)
		{
			_params = (PrevisuParameters)data;
			Invoke(nameof(Release), _params.Duration);
		}

		protected override void OnReleasing()
		{
			base.OnReleasing();
			_params.OnRelease?.Invoke(_params);
		}
	}
}
