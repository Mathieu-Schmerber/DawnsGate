using System;
using UnityEditor.Experimental.GraphView;

namespace Game.Systems.Dialogue
{
	public enum NodeType
	{
		ACTOR,
		PROMPT
	}

	public interface IDialogueElement
	{
		public Guid Id { get; set; }
		public NodeType Type { get; }
	}
}
