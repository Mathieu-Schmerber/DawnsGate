using Game.Managers;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class TraitDescriptorUi : MonoBehaviour
	{
		[Title("References")]
		[SerializeField] private TextMeshProUGUI _stats;
		[SerializeField] private TextMeshProUGUI _price;

		private Button _btn;

		private void Awake()
		{
			_btn = GetComponent<Button>();
		}

		public void SetClickAction(Action action) => _btn.onClick.AddListener(() => action.Invoke());

		public void SetStatsText(string richText) => _stats.text = richText;

		public void SetPrice(int cost)
		{
			bool affordable = GameManager.CanLobbyMoneyAfford(cost);

			_price.text = cost.ToString();
			_price.color = affordable ? Color.white : Color.red;
			_btn.interactable = affordable;
		}
	}
}
