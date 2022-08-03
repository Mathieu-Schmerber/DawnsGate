using Game.Entities.AI.Dealer;
using Nawlian.Lib.Components;
using Pixelplacement;
using UnityEngine;

namespace Game.VFX
{
    public class RoomTotemEffect : MonoBehaviour
    {
        private RoomTotem _totem;
		private bool _lastCheck = false;

		private ParticleSystem _ps;
		private Light _light;
		private TorchLightEffect _torch;

		private void Awake()
		{
			_totem = GetComponentInParent<RoomTotem>();
			_light = GetComponentInChildren<Light>(true);
			_ps = GetComponentInChildren<ParticleSystem>(true);
			_torch = _light.GetComponent<TorchLightEffect>();
		}

		private void OnEnable()
		{
			RoomTotem.OnStateChanged += RoomTotem_OnStateChanged;
		}

		private void OnDisable()
		{
			RoomTotem.OnStateChanged -= RoomTotem_OnStateChanged;
		}

		private void RoomTotem_OnStateChanged(bool state)
		{
			if (_totem.IsActive != _lastCheck)
			{
				if (state)
					Activate();
				else
					DeActivate();
				_lastCheck = state;
			}
		}

		private void Activate()
		{
			_ps.Play(true);
			_light.intensity = 0;
			Tween.LightIntensity(_light, _torch.MaxIntensity, 0.2f, 0, Tween.EaseOut, completeCallback: () => _torch.enabled = true);
		}

		private void DeActivate()
		{
			_torch.enabled = false;
			Tween.LightIntensity(_light, 0, 0.2f, 0, Tween.EaseOut);
			_ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
	}
}