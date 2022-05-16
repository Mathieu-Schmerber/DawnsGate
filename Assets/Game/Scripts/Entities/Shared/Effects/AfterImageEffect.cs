using Nawlian.Lib.Systems.Pooling;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.Shared.Effects
{
	public class AfterImageEffect : MonoBehaviour
	{
		private SkinnedMeshRenderer _skin;
		private AController _controller;

		private void Awake()
		{
			_skin = GetComponentInChildren<SkinnedMeshRenderer>();
			_controller = GetComponentInParent<AController>();
		}

		private void OnEnable()
		{
			_controller.OnDashStarted += OnDash;
		}

		private void OnDisable()
		{
			_controller.OnDashStarted -= OnDash;
		}

		private IEnumerator AfterImage(float duration, int number)
		{
			float interval = duration / number;

			for (int i = 0; i < number; i++)
			{
				GameObject image = ObjectPooler.Get(PoolIdEnum.AFTER_IMAGE, transform.position, transform.rotation, null);
				var filter = image.GetComponentInChildren<MeshFilter>();

				_skin.BakeMesh(filter.mesh);
				yield return new WaitForSeconds(interval);
			}
		}

		private void OnDash(DashParameters obj) => StartCoroutine(AfterImage(obj.Time, Mathf.FloorToInt(obj.Distance)));
	}
}