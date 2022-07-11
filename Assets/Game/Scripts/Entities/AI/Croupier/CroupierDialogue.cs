using Game.Entities.AI;
using Game.Entities.Shared;
using Game.Systems.Dialogue;
using Game.Systems.Run.Rooms;
using Nawlian.Lib.Systems.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class CroupierDialogue : ADialogueInterpreter, IInteractable
	{
		[SerializeField] private ARoom _room;
		private NpcStatData _npc;

		private void Awake()
		{
			_npc = GetComponent<EntityIdentity>()?.Stats as NpcStatData;
		}

		#region Interaction

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

		#endregion

		#region Dialogue events

		protected override void CloseDialogue()
		{
			base.CloseDialogue();
			_room.Clear();
		}

		private void Bet()
		{
			if (Random.Range(0, 2) == 0)
				OnBetWon();
			else
				OnBetLost();
		}

		#endregion

		#region Betting

		private void OnBetWon()
		{
			ProcessCheckpoint("Won");
		}

		private void OnBetLost()
		{
			ProcessCheckpoint("Lost");
		}

		#endregion
	}
}
