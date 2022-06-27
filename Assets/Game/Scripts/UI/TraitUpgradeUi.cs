using Game.Managers;
using Game.Tools;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class TraitUpgradeUi : ACloseableMenu
	{
		private TraitDescriptorUi[] _descriptors;

		protected override void Awake()
		{
			base.Awake();
			_descriptors = GetComponentsInChildren<TraitDescriptorUi>();
		}

		public override void Open()
		{
			base.Open();
			EventSystem.current.SetSelectedGameObject(_descriptors.FirstOrDefault()?.gameObject);
		}

		public override void Close()
		{
			base.Close();
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}
