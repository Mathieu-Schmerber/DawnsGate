using System;

namespace Game.Systems.Dialogue.Data
{
	[Serializable]
	public class DialogueCheckpointNode : ADialogueNode
	{
		public override NodeType Type => NodeType.CHECKPOINT;

		public string CheckPointName;
		public string NextNodeId;
	}
}
