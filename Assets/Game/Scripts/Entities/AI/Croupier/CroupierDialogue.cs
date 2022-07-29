using Game.Entities.Player;
using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Dialogue;
using Game.Systems.Dialogue.Data;
using Game.Systems.Items;
using Game.Systems.Run;
using Game.Systems.Run.Rooms;
using Game.UI;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Interaction;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Entities.AI.Croupier
{
	public class CroupierDialogue : ADialogueInterpreter, IInteractable
	{
		#region Types

		public class BetInfo
		{
			public int MoneyBet { get; set; }
			public int MoneyReward => MoneyBet * 2;
			public AEquippedItem ItemBet { get; set; }
		}

		#endregion

		[SerializeField] private ARoom _room;
		[Title("Roulette animation")]
		[SerializeField] private RouletteFX _roulette;

		private const string ON_BET_WON = "Won";
		private const string ON_BET_LOST = "Lost";
		private const string ON_NOTHING_TO_BET = "CannotBet";

		private const string ANIM_ACTIVE = "IsActive";
		private const string ANIM_PROMPT = "IsTalking";
		private const string ANIM_BET_OCCURING = "IsRouletteRolling";
		private const string ANIM_BET_LOST = "Lost";

		private int _numberOfBets = 0;
		private CroupierStatData _npc;
		private Inventory _inventory;
		private BetInfo _currentReward = new();
		private Animator _animator;

		private RoomRewardType _betType => _room.RoomData.Reward;

		#region Unity builtins

		private void Awake()
		{
			_npc = GetComponent<EntityIdentity>().Stats as CroupierStatData;
			_inventory = GameManager.Player.GetComponent<Inventory>();
			_animator = GetComponentInChildren<Animator>();
		}

		private void Start()
		{
			_npc = GetComponent<EntityIdentity>().Stats as CroupierStatData;
			ProcessBetReward();
		}

		#endregion

		#region Interaction

		public string InteractionTitle => "Talk with the croupier";

		public void Interact(IInteractionActor actor)
		{
			if (_npc.DialogueData == null || _numberOfBets > 0)
				return;
			actor.UnSuggestInteraction(this);
			OpenAndProcessDialogue(_npc.DialogueData);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (_numberOfBets == 0)
				other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);
		}

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);

		#endregion

		#region Dialogue events

		private void Bet()
		{
			if (!CanPlayerBet())
			{
				OnCannotBet();
				return;
			}

			bool isWin = Random.Range(0, 2) == 0;

			StartCoroutine(PrepareBet(isWin, onPreparationDone: () =>
			{
				if (isWin)
					OnBetWon();
				else
					OnBetLost();
				_numberOfBets++;
				ProcessBetReward();
			}));
		}

		protected override void CloseDialogue()
		{
			base.CloseDialogue();
			_animator.SetBool(ANIM_PROMPT, false);
			_animator.SetBool(ANIM_ACTIVE, false);
			_room.Clear();
		}

		private string GetItemBetChoiceText()
		{
			if (CanPlayerBet())
			{
				return $"Bet {_currentReward.ItemBet.Details.name}" +
				$"<sprite=\"{_currentReward.ItemBet.Details.Graphics.name}\" index=0>" +
				$"{string.Concat(Enumerable.Repeat("<sprite=\"star\" index=0>", _currentReward.ItemBet.Quality + 1))}";
			}
			return "Bet nothing";
		}

		private string GetMoneyBetChoiceText()
		{
			int amount = CanPlayerBet() ? _currentReward.MoneyBet : _numberOfBets == 0 ? _npc.MinimumBet : _currentReward.MoneyBet;
			string format = string.Format("{0:n0}", amount);

			return $"Bet {format} <sprite=\"money\" index=0>";
		}

		protected override string GetFormattedChoice(string choiceText)
		{
			switch (_betType)
			{
				case RoomRewardType.GOLD:
					return choiceText.Replace("{BET}", GetMoneyBetChoiceText());
				case RoomRewardType.ITEM:
					return choiceText.Replace("{BET}", GetItemBetChoiceText());
			}
			return choiceText;
		}

		protected override void OpenDialogue(DialogueData dialogue)
		{
			base.OpenDialogue(dialogue);
			_animator.SetBool(ANIM_ACTIVE, true);
		}

		protected override void OnPromptShowing()
		{
			if (_animator.GetBool(ANIM_PROMPT))
			{
				_animator.SetBool(ANIM_PROMPT, false);
				Awaiter.WaitAndExecute(0.1f, () => _animator.SetBool(ANIM_PROMPT, true));
			}
			else
				_animator.SetBool(ANIM_PROMPT, true);
		}

		protected override void OnChoiceShowing() => _animator.SetBool(ANIM_PROMPT, false);

		#endregion

		#region Betting

		#region RunMoney reward

		private int GetMoneyBet()
			=> _numberOfBets == 0 ? Mathf.RoundToInt(GameManager.RunMoney * (_npc.InitialBetRatio / 100f)) : _currentReward.MoneyBet * 2;

		#endregion

		#region Item reward

		private AEquippedItem GetItemBet()
		{
			AEquippedItem[] items = _inventory.Items.Where(x => x.HasUpgrade).ToArray();

			if (items.Length == 0)
				return null;
			return items.Random();
		}

		#endregion

		private bool CanPlayerBet()
		{
			switch (_betType)
			{
				case RoomRewardType.GOLD:
					return _currentReward.MoneyBet >= _npc.MinimumBet && GameManager.CanRunMoneyAfford(_currentReward.MoneyBet);
				case RoomRewardType.ITEM:
					return _currentReward.ItemBet != null;
			}
			return false;
		}

		private void ProcessBetReward()
		{
			switch (_betType)
			{
				case RoomRewardType.GOLD:
					_currentReward.MoneyBet = GetMoneyBet();
					break;
				case RoomRewardType.ITEM:
					_currentReward.ItemBet = GetItemBet();
					break;
			}
		}

		private IEnumerator PrepareBet(bool isWinningBet, Action onPreparationDone)
		{
			HideDialogue();

			// Visually remove the money to bet
			if (_betType == RoomRewardType.GOLD)
				GameManager.PayWithRunMoney(_currentReward.MoneyBet);

			yield return new WaitForSeconds(0.5f);

			// Roulette roll
			_animator.SetBool(ANIM_PROMPT, false);
			_animator.SetBool(ANIM_BET_OCCURING, true);
			_animator.SetBool(ANIM_BET_LOST, !isWinningBet);
			_roulette.Play();

			// Quick state reset
			yield return new WaitForSeconds(0.1f);
			_animator.SetBool(ANIM_BET_OCCURING, false);

			yield return new WaitForSeconds(_roulette._totalAnimationTime);
			onPreparationDone?.Invoke();
		}

		private void OnBetWon()
		{
			switch (_betType)
			{
				case RoomRewardType.GOLD:
					GameManager.RewardWithRunMoney(_currentReward.MoneyReward);
					break;
				case RoomRewardType.ITEM:
					_currentReward.ItemBet.Upgrade();
					break;
			}
			ProcessCheckpoint(ON_BET_WON);
		}

		private void OnBetLost()
		{
			if (_betType == RoomRewardType.ITEM)
				_inventory.RemoveItemFromInventory(_currentReward.ItemBet.Details, true);
			ProcessCheckpoint(ON_BET_LOST);
		}

		private void OnCannotBet() => ProcessCheckpoint(ON_NOTHING_TO_BET);

		#endregion
	}
}
