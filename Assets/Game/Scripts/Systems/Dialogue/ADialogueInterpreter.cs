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
			_dialogue = dialogue;

			_uiMenu = GuiManager.OpenMenu<DialogueUi>();
			_uiMenu.OnSubmitted += OnSubmission;
			GuiManager.CloseMenu<PlayerUi>();

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
			switch (node.Type)
			{
				case NodeType.PROMPT:
					_uiMenu.DisplayPrompt(node as DialoguePromptNode);
					break;
				case NodeType.CHOICE:
					_uiMenu.DisplayChoices(node as DialogueChoiceNode);
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

		protected void CloseDialogue()
		{
			GuiManager.OpenMenu<PlayerUi>();
			_uiMenu?.Close();
			_uiMenu.OnSubmitted -= OnSubmission;
		}

		public void OnEvent(string functionName) => SendMessage(functionName);
	}
}
