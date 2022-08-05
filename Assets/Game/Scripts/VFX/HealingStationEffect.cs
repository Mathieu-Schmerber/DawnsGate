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

		private void Awake()
		{
			_station = GetComponentInParent<HealingStation>();
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
				GameManager.Player.State = Entities.Shared.EntityState.STUN;
				GameManager.Player.LockMovement = true;
				GameManager.Player.LockTarget(transform);
				GameManager.Camera.LockTemporaryTarget(transform, 0.8f);
			}

			void UnRestrictPlayer()
			{
				GameManager.Player.State = Entities.Shared.EntityState.IDLE;
				GameManager.Player.LockMovement = false;
				GameManager.Player.UnlockTarget();
				GameManager.Camera.UnlockTarget();
			}

			IEnumerator Animate()
			{
				GameManager.Camera.LockTemporaryTarget(transform, 0.8f);
				RestrictPlayer();
				yield return new WaitForSeconds(1f);
				GameManager.Player.SetAnimatorState("IsPraising", true);
				yield return new WaitForSeconds(_animationStarterTime);
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
