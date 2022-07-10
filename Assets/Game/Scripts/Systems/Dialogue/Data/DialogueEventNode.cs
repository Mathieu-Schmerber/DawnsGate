using Game.Systems.Dialogue;
using System;

namespace Game.Systems.Dialogue.Data
{
	[Serializable]
	public class DialogueEventNode : ADialogueNode
	{
		public override NodeType Type => NodeType.EVENT;

		public string FunctionName;
	}
}
