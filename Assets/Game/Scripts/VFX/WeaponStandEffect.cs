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

		private void Awake()
		{
			_stand = GetComponentInParent<WeaponStand>();
			_meshFilter = transform.GetChild(0).GetComponent<MeshFilter>();
		}

		private void OnEnable()
		{
			_stand.OnIntracted += OnInteracted;
		}

		private void OnDisable()
		{
			_stand.OnIntracted -= OnInteracted;
		}

		private void OnInteracted()
		{
			Debug.Log($"Interacted: {_stand.Data}");
			if (_stand.Empty)
				_meshFilter.mesh = null;
			else
				_meshFilter.mesh = _stand.Data.Mesh;
		}
	}
}
