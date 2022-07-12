using Game.Managers;
using Game.Systems.Dialogue.Data;
using Game.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.Dialogue
{
	public abstract class ADialogueInterpreter : MonoBehaviour
	{
		protected DialogueData _dialogue;
		private DialogueUi _uiMenu;

		public void OpenAndProcessDialogue(DialogueData dialogue)
		{
			OpenDialogue(dialogue);
			ProcessNode(_dialogue.GetFirstNode());
		}

		private void OnSubmission(ADialogueNode current, string selection)
		{
			if (current.IsEndNode)
			{
				CloseDialogue();
				return;
			}	
			switch (current.Type)
			{
				case NodeType.PROMPT:
					ProcessNode((current as DialoguePromptNode).NextNodeId);
					break;
				case NodeType.CHOICE:
					ProcessNode(selection);
					break;
			}
		}

		private void ProcessNode(string id) => ProcessNode(_dialogue.Nodes[id]);

		private void ProcessNode(ADialogueNode node)
		{
			if ((_uiMenu == null || !_uiMenu.IsOpen) && _dialogue != null)
				OpenDialogue(_dialogue);

			if (!string.IsNullOrEmpty(node.Author))
				_uiMenu.SetAuthor(node.Author);
			switch (node.Type)
			{
				case NodeType.PROMPT:
					_uiMenu.DisplayPrompt(node as DialoguePromptNode);
					break;
				case NodeType.CHOICE:
					_uiMenu.DisplayChoices(node as DialogueChoiceNode, GetFormattedChoice);
					break;
				case NodeType.EVENT:
					OnEvent((node as DialogueEventNode).FunctionName);
					if (node.IsEndNode)
					{
						CloseDialogue();
						return;
					}
					break;
			}
		}

		protected void ProcessCheckpoint(string checkpointName)
		{
			DialogueCheckpointNode checkpoint = _dialogue.GetCheckpoint(checkpointName);

			if (checkpoint == null)
				throw new ArgumentException($"No dialogue checkpoint named {checkpointName} exist !");
			ProcessNode(checkpoint.NextNodeId);
		}

		protected virtual void OpenDialogue(DialogueData dialogue)
		{
			_dialogue = dialogue;
			_uiMenu = GuiManager.OpenMenu<DialogueUi>();
			_uiMenu.OnSubmitted += OnSubmission;
			_uiMenu.SetAuthor("???");
			GuiManager.CloseMenu<PlayerUi>();
		}

		protected virtual void CloseDialogue()
		{
			GuiManager.OpenMenu<PlayerUi>();
			GuiManager.CloseMenu<DialogueUi>();
			_uiMenu.OnSubmitted -= OnSubmission;
		}

		public void OnEvent(string functionName) => SendMessage(functionName);

		protected abstract string GetFormattedChoice(string choiceText);
	}
}