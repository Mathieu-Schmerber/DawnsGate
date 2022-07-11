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

using Random = UnityEngine.Random;

namespace Game.UI
{
	public class DialogueUi : ACloseableMenu
	{
		private readonly int DEFAULT_SPEED = 30;
		private readonly float WAVE_AMP = 3.5f;
		private readonly float WAVE_SPEED = 5;

		[Title("References")]
		[SerializeField] private TextMeshProUGUI _authorNameText;
		[SerializeField] private TextMeshProUGUI _promptText;
		[SerializeField] private DialogueChoiceUi _choicePrefab;
		[SerializeField] private Transform _choiceList;
		[SerializeField] private List<DialogueChoiceUi> _choices = new();

		private Mesh mesh;
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
			_promptText.ForceMeshUpdate();
			mesh = _promptText.mesh;
			var vertices = mesh.vertices;
			ParsedElement activeEffect;

			for (int i = 0; i < _promptText.maxVisibleCharacters; i++)
			{
				activeEffect = _textEffects.LastOrDefault(x => x.Contains(i) && x.Name == "effect");
				if (activeEffect == null || string.IsNullOrEmpty(activeEffect.Value))
					continue;

				TMP_CharacterInfo c = _promptText.textInfo.characterInfo[i];
				int index = c.vertexIndex;
				Vector3 offset = ExecuteEffect(activeEffect?.Value ?? "", Time.time + i);

				vertices[index] += offset;
				vertices[index + 1] += offset;
				vertices[index + 2] += offset;
				vertices[index + 3] += offset;
			}
			mesh.vertices = vertices;
			_promptText.canvasRenderer.SetMesh(mesh);
		}

		private Vector3 ExecuteEffect(string name, float timeArg)
		{
			if (name == "wave")
				return Wave(timeArg);
			else if (name == "shake")
				return Shake(timeArg);
			else if (name == "wobble")
				return Wobble(timeArg);
			return Vector3.zero;
		}

		private Vector3 Shake(float time) => new Vector2(Random.Range(0f, 2f), Random.Range(0f, 2f));

		private Vector3 Wave(float time) => new Vector2(0, Mathf.Sin(WAVE_SPEED * time) * WAVE_AMP);

		private Vector3 Wobble(float time) => new Vector2(Mathf.Sin(time * 3.3f), Mathf.Cos(time * 2.5f));

		private void OnApprearTick()
		{
			ParsedElement speed = _textEffects.LastOrDefault(x => x.Contains(_promptText.maxVisibleCharacters) && x.Name == "speed");

			if (speed != null)
			{
				int speedValue;
				_appreanceTimer.Interval = 1.0f / (int.TryParse(speed.Value, out speedValue) ? speedValue * DEFAULT_SPEED : DEFAULT_SPEED);
			}
			_promptText.maxVisibleCharacters++;
			if (_promptText.maxVisibleCharacters == _promptText.textInfo.characterCount)
				_appreanceTimer.Stop();
		}

		#endregion

		#region Interactions logic

		public void DisplayPrompt(DialoguePromptNode node)
		{
			string richText = node.Text.Replace("\n", "\n ").Insert(0, " ");
			string lightText = richText;

			ClearDialogues();

			_promptText.gameObject.SetActive(true);
			_textEffects = TextElementParser.Parse(richText.RemoveTextMeshProTags());
			_promptText.text = TextElementParser.RemoveElements(lightText, _textEffects);
			_displayedNode = node;
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

		public void SetAuthor(string author)
		{
			_authorNameText.text = author;
		}

		private void ClearDialogues()
		{
			_appreanceTimer.Stop();
			EventSystem.current.SetSelectedGameObject(null);
			_displayedNode = null;
			_choiceList.gameObject.SetActive(false);
			_promptText.gameObject.SetActive(false);
			_promptText.text = string.Empty;
			_promptText.maxVisibleCharacters = 0;
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