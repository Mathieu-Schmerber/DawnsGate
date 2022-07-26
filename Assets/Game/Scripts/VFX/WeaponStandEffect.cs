using Game.Systems.Run.Lobby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.VFX
{
	public class WeaponStandEffect : MonoBehaviour
	{
		private WeaponStand _stand;
		private MeshFilter _meshFilter;
		private MeshRenderer _meshRenderer;

		private void Awake()
		{
			_stand = GetComponentInParent<WeaponStand>();
			_meshFilter = transform.GetChild(0).GetComponent<MeshFilter>();
			_meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
		}

		private void OnEnable()
		{
			_stand.OnInteracted += OnInteracted;
		}

		private void OnDisable()
		{
			_stand.OnInteracted -= OnInteracted;
		}

		private void OnInteracted()
		{
			if (_stand.Empty)
				_meshFilter.mesh = null;
			else
			{
				_meshRenderer.material = _stand.Data.Material;
				_meshFilter.mesh = _stand.Data.Mesh;
			}
		}
	}
}
