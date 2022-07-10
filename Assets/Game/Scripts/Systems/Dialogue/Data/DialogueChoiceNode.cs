using Game.Systems.Dialogue;
using System;
using System.Collections.Generic;

namespace Game.Systems.Dialogue.Data
{
	[Serializable]
	public class DialogueChoiceNode : ADialogueNode
	{
		[Serializable]
		public struct ChoiceOutput
		{
			public string Text;
			public string NextNodeId;
		}

		public override NodeType Type => NodeType.CHOICE;

		public List<ChoiceOutput> Outputs;

		public DialogueChoiceNode()
		{
			Outputs = new();
		}
	}
}