using Game.Systems.Dialogue;
using System;

namespace Game.Systems.Dialogue.Data
{
	public static class DialogueNodeFactory
	{
		public static ADialogueNode Create(NodeType type) => type switch
		{
			NodeType.PROMPT => new DialoguePromptNode(),
			NodeType.CHOICE => new DialogueChoiceNode(),
			NodeType.EVENT => new DialogueEventNode(),
			NodeType.CHECKPOINT => new DialogueCheckpointNode(),
			_ => throw new ArgumentException("Unexpected dialogue node type")
		};
	}
}
