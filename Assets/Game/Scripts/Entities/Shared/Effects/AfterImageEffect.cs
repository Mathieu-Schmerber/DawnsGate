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

		private void Awake()
		{
			_skin = GetComponentInChildren<SkinnedMeshRenderer>();
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

		public void Play(float duration, int number)
		{
			StartCoroutine(AfterImage(duration, number));
		}
	}
}