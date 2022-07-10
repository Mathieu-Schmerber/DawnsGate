using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class DialogueChoiceUi : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _text;
		private Button _button;
		private Action<string> _onClick;

		public string ChoiceId { get; set; }

		private void Awake()
		{
			_button = GetComponent<Button>();
			_button.onClick.AddListener(OnClick);
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
	}
}