using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.Dialogue
{
	[CreateAssetMenu(menuName = "Data/Dialogue")]
	public class DialogueData : ScriptableObject
	{
		public Dictionary<Guid, DialoguePrompt> Prompts = new();
	}
}
