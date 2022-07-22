using Game.Managers;
using Game.Systems.Run.Lobby;
using Game.Tools;
using Nawlian.Lib.Extensions;
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
		[SerializeField] private AudioClip _purchaseAudio;
		[SerializeField] private AudioClip _errorAudio;

		protected override void Awake()
		{
			base.Awake();
			_descriptors = GetComponentsInChildren<TraitDescriptorUi>();
		}

		private void Refresh(TraitDescriptorUi x)
		{
			TraitUpgradeData trait = x.Trait;
			int cost = GameManager.GetTraitUpgradeCost(trait);

			x.UpdatePrice(cost);
			x.SetStatsText($"{trait.StatName}: {GameManager.PlayerIdentity.Stats.Modifiers[trait.StatModified].Value}%<color=green>(+{trait.IncrementPerUpgrade}%)</color>");
			x.Interactable = GameManager.CanLobbyMoneyAfford(cost) && GameManager.IsUpgradable(trait);
		}

		private void OnSubmitted(TraitDescriptorUi descriptorUi)
		{
			if (GameManager.UpgradeTrait(descriptorUi.Trait))
			{
				_descriptors.ForEach(x => Refresh(x));
				descriptorUi.Interact();
				_source.PlayOneShot(_purchaseAudio);
				if (!descriptorUi.Interactable)
					EventSystem.current.SetSelectedGameObject(_descriptors.FirstOrDefault(x => x.Interactable)?.gameObject);
			}
			else
				_source.PlayOneShot(_errorAudio);
		}

		public override void Open()
		{
			base.Open();
			_descriptors.ForEach(x => Refresh(x));
			EventSystem.current.SetSelectedGameObject(_descriptors.FirstOrDefault(x => x.Interactable)?.gameObject);
			TraitDescriptorUi.OnTraitClicked += OnSubmitted;
		}

		public override void Close()
		{
			base.Close();
			EventSystem.current.SetSelectedGameObject(null);
			TraitDescriptorUi.OnTraitClicked -= OnSubmitted;
		}
	}
}
