using Game.Managers;
using Game.Systems.Run.GPE;
using Nawlian.Lib.Systems.Pooling;
using System.Collections;
using UnityEngine;

namespace Game.VFX
{
	public class HealingStationEffect : MonoBehaviour
	{
		[SerializeField] private float _animationStarterTime;
		[SerializeField] private GameObject _healingFx;
		private HealingStation _station;
		private AudioSource _source;

		private void Awake()
		{
			_station = GetComponentInParent<HealingStation>();
			_source = GetComponent<AudioSource>();
		}

		private void OnEnable()
		{
			_station.OnInteracted += AnimateHealingProcess;
		}

		private void OnDisable()
		{
			_station.OnInteracted -= AnimateHealingProcess;
		}

		private void AnimateHealingProcess()
		{
			void RestrictPlayer()
			{
				GameManager.Player.LockTarget(transform);
				GameManager.Camera.LockTemporaryTarget(transform, 0.8f);
				GameManager.Player.Restrict();
				GameManager.Player.LockAim = false;
			}

			void UnRestrictPlayer()
			{
				GameManager.Player.UnlockTarget();
				GameManager.Camera.UnlockTarget();
				GameManager.Player.UnRestrict();
			}

			IEnumerator Animate()
			{
				GameManager.Camera.LockTemporaryTarget(transform, 0.8f);
				RestrictPlayer();
				yield return new WaitForSeconds(1f);
				GameManager.Player.SetAnimatorState("IsPraising", true);
				yield return new WaitForSeconds(_animationStarterTime / 2);
				_source.Play();
				yield return new WaitForSeconds(_animationStarterTime / 2);
				yield return new WaitForSeconds(1f);
				_station.Heal();
				ObjectPooler.Get(_healingFx, GameManager.Player.transform.position, Quaternion.identity, null);
				yield return new WaitForSeconds(1f);
				GameManager.Player.SetAnimatorState("IsPraising", false);
				yield return new WaitForSeconds(_animationStarterTime);
				GameManager.Camera.UnlockTarget();
				UnRestrictPlayer();
			}
			StartCoroutine(Animate());
		}
	}
}
