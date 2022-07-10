using Game.Systems.Dialogue.Data;
using Game.Tools;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class DialogueUi : ACloseableMenu
	{
		[Title("References")]
		[SerializeField] private TextMeshProUGUI _promptText;
		[SerializeField] private DialogueChoiceUi _choicePrefab;
		[SerializeField] private Transform _choiceList;
		[SerializeField] private List<DialogueChoiceUi> _choices = new();

		public event Action<ADialogueNode, string> OnSubmitted;

		private ADialogueNode _displayedNode = null;

		public void DisplayPrompt(DialoguePromptNode node)
		{
			ClearDialogues();
			_promptText.text = node.Text;
			_displayedNode = node;
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
			EventSystem.current.SetSelectedGameObject(null);
			_displayedNode = null;
			_choiceList.gameObject.SetActive(false);
			_promptText.text = string.Empty;
			_choices.ForEach(x => x.gameObject.SetActive(false));
		}

		private void SubmitChoice(string id) => OnSubmitted?.Invoke(_displayedNode, id);

		protected override void OnSubmit()
		{
			if (_displayedNode.Type == NodeType.PROMPT)
				OnSubmitted?.Invoke(_displayedNode, _displayedNode.Id);
		}

		protected override void OnCancel()
		{
			// Leave empty, prevents from closing the menu
		}
	}
}