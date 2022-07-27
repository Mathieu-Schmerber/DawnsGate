using Game.Managers;
using Game.Systems.Run.Lobby;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Pixelplacement;
using Game.Tools;

namespace Game.UI
{
	public class TraitDescriptorUi : Selectable, ISubmitHandler
	{
		[Title("References")]
		[SerializeField] private TraitUpgradeData _traitData;
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _description;
		[SerializeField] private TextMeshProUGUI _stats;
		[SerializeField] private TextMeshProUGUI _price;
		[SerializeField] private Image _resultRenderer;

		[Title("Feedback")]
		[SerializeField] private float _duration;
		[SerializeField] private Color _selectedColor;
		[SerializeField] private Color _disabledColor;
		[SerializeField] private Color _flashColor;
		[SerializeField] private float _bumpIntensity;

		private RandomAudioClip _audioRdn;
		private Color _resultDefaultColor;
		private Color _outlineDefaultColor;
		private Outline _outline;

		public static event Action<TraitDescriptorUi> OnTraitClicked;
		public TraitUpgradeData Trait => _traitData;

		public bool Interactable
		{
			get => interactable;
			set {
				interactable = value;
				if (!interactable)
					_outline.effectColor = _disabledColor;
			}
		}


		protected override void Awake()
		{
			base.Awake();
			_resultDefaultColor = _resultRenderer.color;
			_outline = GetComponentInChildren<Outline>();
			_outlineDefaultColor = _outline.effectColor;
			_audioRdn = GetComponent<RandomAudioClip>();
		}

		protected override void Start()
		{
			base.Start();
			_title.text = _traitData.Title;
			_description.text = _traitData.Description;
		}

		public void SetStatsText(string richText) => _stats.text = richText;
		public void UpdatePrice(int cost)
		{
			bool affordable = GameManager.CanLobbyMoneyAfford(cost);

			_price.text = cost.ToString();
			_price.color = affordable ? Color.white : Color.red;
		}

		public void OnSubmit(BaseEventData eventData)
		{
			if (interactable)
				OnTraitClicked?.Invoke(this);
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			_outline.effectColor = Interactable ? _selectedColor : _disabledColor;
			_audioRdn.PlayRandom();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			_outline.effectColor = Interactable ? _outlineDefaultColor : _disabledColor;
		}

		public void Interact()
		{
			// Flash
			Tween.Value(_outlineDefaultColor, _flashColor, (c) => _outline.effectColor = c, _duration / 2, 0);
			Tween.Value(_flashColor, _outlineDefaultColor, (c) => _outline.effectColor = c, _duration / 2, _duration / 2);

			// Bump
			Tween.LocalScale(transform, Vector3.one * _bumpIntensity, _duration / 2, 0, Tween.EaseBounce);
			Tween.LocalScale(transform, Vector3.one, _duration / 2, _duration / 2, Tween.EaseBounce);
		}
	}
}
