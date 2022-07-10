using Game.Entities.AI;
using Game.Entities.Shared;
using Game.Systems.Dialogue;
using Nawlian.Lib.Systems.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class CroupierDialogue : ADialogueInterpreter, IInteractable
	{
		private NpcStatData _npc;

		private void Awake()
		{
			_npc = GetComponent<EntityIdentity>()?.Stats as NpcStatData;
		}

		public string InteractionTitle => "Talk with the croupier";

		public void Interact(IInteractionActor actor)
		{
			if (_npc.DialogueData == null)
				return;
			actor.UnSuggestInteraction(this);
			OpenAndProcessDialogue(_npc.DialogueData);
		}

		private void OnTriggerEnter(Collider other) => other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);

		private void Log()
		{
			Debug.Log("Dialogue event !");
		}
	}
}
