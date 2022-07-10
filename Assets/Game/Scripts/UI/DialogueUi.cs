using Game.Extensions;
using Game.Systems.Dialogue.Data;
using Game.Tools;
using Game.Tools.TextElementParser;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class DialogueUi : ACloseableMenu
	{
		private readonly int DEFAULT_SPEED = 20;

		[Title("References")]
		[SerializeField] private TextMeshProUGUI _promptText;
		[SerializeField] private DialogueChoiceUi _choicePrefab;
		[SerializeField] private Transform _choiceList;
		[SerializeField] private List<DialogueChoiceUi> _choices = new();

		private List<ParsedElement> _textEffects;
		private ADialogueNode _displayedNode = null;
		private Timer _appreanceTimer = new();

		public event Action<ADialogueNode, string> OnSubmitted;


		#region Text effects

		// Update is called once per frame
		void Update()
		{
			if (!_isOpen)
				return;
			AnimateText();
		}

		private void AnimateText()
		{
			
		}

		private void OnApprearTick()
		{
			ParsedElement speed = _textEffects.LastOrDefault(x => x.Contains(_promptText.maxVisibleCharacters) && x.Name == "speed");

			if (speed != null)
			{
				int speedValue;
				_appreanceTimer.Interval = 1.0f / (int.TryParse(speed.Value, out speedValue) ? speedValue : DEFAULT_SPEED);
			}
			_promptText.maxVisibleCharacters++;
			if (_promptText.maxVisibleCharacters == _promptText.textInfo.characterCount)
				_appreanceTimer.Stop();
		}

		#endregion

		#region Interactions logic

		public void DisplayPrompt(DialoguePromptNode node)
		{
			string richText = node.Text;
			string lightText = node.Text;

			ClearDialogues();

			_textEffects = TextElementParser.Parse(richText.RemoveTextMeshProTags());
			_promptText.text = TextElementParser.RemoveElements(lightText, _textEffects);
			_displayedNode = node;
			_promptText.maxVisibleCharacters = 0;
			_appreanceTimer.Start(0.1f, true, OnApprearTick);
		}

		public void DisplayChoices(DialogueChoiceNode node)
		{
			int requirements = node.Outputs.Count - _choices.Count;

			ClearDialogues();
			_choiceList.gameObject.SetActive(true);
			for (int i = 0; i < requirements; i++)
			{
				var choice = Instantiate(_choicePrefab, _choiceList);
				
				choice.SetOnClick(SubmitChoice);
				_choices.Add(choice);
			}
			for (int i = 0; i < node.Outputs.Count; i++)
			{
				DialogueChoiceUi choiceUi = _choices[i];
				var output = node.Outputs[i];

				choiceUi.gameObject.SetActive(true);
				choiceUi.SetText(output.Text);
				choiceUi.ChoiceId = output.NextNodeId;
				if (i > 0)
				{
					_choices[0].SetNavUp(choiceUi);
					_choices[i - 1].SetNavDown(choiceUi);
					choiceUi.SetNavUp(_choices[i - 1]);
					choiceUi.SetNavDown(_choices[0]);
				}
			}
			_displayedNode = node;
			Awaiter.WaitAndExecute(0.05f, () => EventSystem.current.SetSelectedGameObject(_choices[0].gameObject));
		}

		private void ClearDialogues()
		{
			_appreanceTimer.Stop();
			EventSystem.current.SetSelectedGameObject(null);
			_displayedNode = null;
			_choiceList.gameObject.SetActive(false);
			_promptText.text = string.Empty;
			_choices.ForEach(x => x.gameObject.SetActive(false));
		}

		private void SubmitChoice(string id) => OnSubmitted?.Invoke(_displayedNode, id);

		protected override void OnSubmit()
		{
			if (_promptText.maxVisibleCharacters != _promptText.textInfo.characterCount)
			{
				_appreanceTimer.Stop();
				_promptText.maxVisibleCharacters = _promptText.textInfo.characterCount;
			}
			else if (_displayedNode.Type == NodeType.PROMPT)
				OnSubmitted?.Invoke(_displayedNode, _displayedNode.Id);
		}

		protected override void OnCancel()
		{
			// Leave empty, prevents from closing the menu
		}

		#endregion
	}
}