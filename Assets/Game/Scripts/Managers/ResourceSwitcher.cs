using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Managers
{
	public class ResourceSwitcher : MonoBehaviour
	{
		[Serializable]
		internal struct ResourceSwitch
		{
			[HideLabel, HorizontalGroup] public bool Enable;
			[HideLabel, HorizontalGroup] public GameObject Resource;
		}

		[SerializeField] private ResourceSwitch[] _gameplayResourceState;

		public void SwitchGameplayResources(bool switchOn)
		{
			foreach (var item in _gameplayResourceState)
				item.Resource?.SetActive(switchOn ? item.Enable : !item.Enable);
		}
	}
}
