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
		[SerializeField] private GameObject _praiseLight;
		private HealingStation _station;
		private AudioSource _source;

		private void Awake()
		{
			_station = GetComponentInParent<HealingStation>();
			_source = GetComponent<AudioSource>();
			_praiseLight.SetActive(false);
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
				yield return new WaitForSeconds(_animationStarterTime * .8f);
				_source.Play();
				_praiseLight.transform.position = new Vector3(GameManager.Player.transform.position.x, _praiseLight.transform.position.y, GameManager.Player.transform.position.z);
				_praiseLight.SetActive(true);
				yield return new WaitForSeconds(3f);
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
