using Game.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class PlayerSpawn : MonoBehaviour
	{
		private void Spawn()
		{
			if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 10))
				GameManager.Player.transform.position = hit.point;
			else
				GameManager.Player.transform.position = transform.position;
		}

		private void OnEnable()
		{
			RunManager.OnBeforeSceneSwitched += Spawn;
		}

		private void OnDisable()
		{
			RunManager.OnBeforeSceneSwitched -= Spawn;
		}
	}
}
