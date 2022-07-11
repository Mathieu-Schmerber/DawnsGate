using Game.Systems.Dialogue;
using System;
using UnityEngine;

namespace Game.Systems.Dialogue.Data
{
	[Serializable]
	public class DialoguePromptNode : ADialogueNode
	{
		public override NodeType Type => NodeType.PROMPT;

		[TextArea] public string Text;
		public string NextNodeId;
		public AudioClip Audio;
	}
}
