
using Sirenix.OdinInspector;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.Dialogue.Data
{
	[CreateAssetMenu(menuName = "Data/Dialogue")]
	public class DialogueData : SerializedScriptableObject
    {
		[DictionaryDrawerSettings(IsReadOnly = true)]
		public Dictionary<string, ADialogueNode> Nodes;

		[OnInspectorInit]
		private void Init()
		{
			if (Nodes == null)
				Nodes = new();
		}

		[Button(Style = ButtonStyle.Box)]
		private void AddNode(NodeType type)
		{
			ADialogueNode node = DialogueNodeFactory.Create(type);

			Nodes.Add(node.Id, node);
		}

		[Button(Style = ButtonStyle.Box)]
		private void Clean()
		{
			foreach (var item in Nodes.ToList())
			{
				if (item.Value == null)
					Nodes.Remove(item.Key);
			}
		}
	}
}