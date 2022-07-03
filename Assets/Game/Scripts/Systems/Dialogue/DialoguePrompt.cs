using Sirenix.OdinInspector;
using System;
using UnityEditor.Experimental.GraphView;

namespace Game.Systems.Dialogue
{
	public class DialoguePrompt : IDialogueElement
	{
		private Guid _id;
		public string Author;
		public string RichText;
		public bool RequiresUserChoice;

		public Guid NextPromptId;
		public Guid YesAnwerId;
		public Guid NoAnwerId;

		public Guid Id { get => _id; set => _id = value; }
		public NodeType Type => NodeType.PROMPT;
	}
}
