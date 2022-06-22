using Game.Systems.Items;
using Nawlian.Lib.Systems.Pooling;
using System;
using UnityEngine;

namespace Game.Entities.Miscellaneous
{
	public class OrbPivot : MonoBehaviour
	{
		[SerializeField] private float _rotationSpeed;
		private SpecialItemData.Stage _data;
		private Orb[] _orbs;

		private void Awake()
		{
			_orbs = GetComponentsInChildren<Orb>(true);
		}

		public void SetOrbCount(int count, float damage)
		{
			if (count > 3)
				return;
			for (int i = 0; i < count; i++)
			{
				_orbs[i].gameObject.SetActive(true);
				_orbs[i].Damage = damage;
			}
		}

		private void Update()
		{
			transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
		}
	}
}
