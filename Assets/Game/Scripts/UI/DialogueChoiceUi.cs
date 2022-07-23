using Game.Tools;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class DialogueChoiceUi : MonoBehaviour, ISelectHandler
	{
		[SerializeField] private TextMeshProUGUI _text;
		private Button _button;
		private Action<string> _onClick;
		private RandomAudioClip _rdmClip;

		public string ChoiceId { get; set; }

		private void Awake()
		{
			_button = GetComponent<Button>();
			_button.onClick.AddListener(OnClick);
			_rdmClip = GetComponent<RandomAudioClip>();
		}

		private void OnClick() => _onClick?.Invoke(ChoiceId);

		public void SetText(string text) => _text.text = text;

		public void SetOnClick(Action<string> onClick) => _onClick = onClick;

		public void SetNavUp(DialogueChoiceUi up)
		{
			var nav = _button.navigation;
			nav.selectOnUp = up._button;
			_button.navigation = nav;
		}

		public void SetNavDown(DialogueChoiceUi down)
		{
			var nav = _button.navigation;
			nav.selectOnDown = down._button;
			_button.navigation = nav;
		}

		public void OnSelect(BaseEventData eventData)
		{
			_rdmClip.PlayRandom();
		}
	}
}