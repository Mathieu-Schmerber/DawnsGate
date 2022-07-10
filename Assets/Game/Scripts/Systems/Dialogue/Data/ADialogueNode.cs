using Sirenix.OdinInspector;
using System;

namespace Game.Systems.Dialogue.Data
{
	[System.Serializable]
	public abstract class ADialogueNode
	{
		[ReadOnly] public string Id;
		public bool IsStartNode;
		public bool IsEndNode;

		public abstract NodeType Type { get; }

		public ADialogueNode()
		{
			Id = Guid.NewGuid().ToString();
		}
	}
}