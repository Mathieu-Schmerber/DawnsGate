using Game.Entities.Player;
using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Dialogue;
using Game.Systems.Items;
using Game.Systems.Run;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Interaction;
using System.Linq;
using UnityEngine;

namespace Game.Entities.AI.Dealer
{
	public class DealerDialogue : ADialogueInterpreter, IInteractable
	{
		#region Types

		public class DealSummary
		{
			public int MoneyAmount;
			public ItemSummary Item;
		}

		#endregion

		private const string ON_CANNOT_DEAL = "OnCannotDeal";
		private const string ON_DEAL_DONE = "OnDealDone";

		private bool _dealDone;
		private DealerStatData _stats;
		private DealerAI _dealer;
		private Inventory _inventory;
		private DealSummary _deal;
		private RoomRewardType _dealType => RunManager.CurrentRoom.Reward;

		public DealSummary CurrentDeal => _deal;

		private void Awake()
		{
			_stats = GetComponentInParent<EntityIdentity>().Stats as DealerStatData;
			_dealer = GetComponentInParent<DealerAI>();
			_inventory = GameManager.Player.GetComponent<Inventory>();
		}

		private void OnEnable()
		{
			ProcessNewDeal();
			_dealDone = false;
			AudioManager.PlayTheme(_stats.DealTheme);
		}

		#region Interaction

		public string InteractionTitle => "Talk";

		public void Interact(IInteractionActor actor)
		{
			if (_stats.Dialogue == null || _dealDone)
				return;
			actor.UnSuggestInteraction(this);
			OpenAndProcessDialogue(_stats.Dialogue);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!_dealDone)
				other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);
		}

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);

		#endregion

		#region Dialogue events

		private void Deal()
		{
			if (CanDeal())
				OnDealDone();
			else
				OnCannotDeal();
		}

		private void OnRefused()
		{
			_dealDone = true;
			_dealer.TakeAction();
			AudioManager.PlayTheme(_stats.FightTheme);
		}

		private string GetBetText()
		{
			string format = string.Format("{0:n0}", _deal.MoneyAmount);

			switch (_dealType)
			{
				case RoomRewardType.GOLD:
					if (!CanDeal())
						return "Try to hide that you have no item to sell.";
					return $"Deal <sprite=\"{_deal.Item.Data.Graphics.name}\" index=0>" +
						   $"{string.Concat(Enumerable.Repeat("<sprite=\"star\" index=0>", _deal.Item.Quality + 1))}" +
						   $" for {format}";
				case RoomRewardType.ITEM:
					return $"Deal {format} for <sprite=\"{_deal.Item.Data.Graphics.name}\" index=0>" +
						   $"{string.Concat(Enumerable.Repeat("<sprite=\"star\" index=0>", _deal.Item.Quality + 1))}";
			}
			return string.Empty;
		}

		protected override string GetFormattedChoice(string choiceText)
			=> choiceText.Replace("{DEAL}", GetBetText());

		#endregion

		#region Deal

		private ItemSummary GetRandomItem()
		{
			ItemSummary result = new();

			result.Data = _inventory.GetRandomUnEquippedItem(false);
			result.Merge = null;
			result.Quality = 0;
			return result;
		}

		private void ProcessNewDeal()
		{
			ItemSummary item = null;
			int amount = 0;
			int cost;

			switch (_dealType)
			{
				case RoomRewardType.GOLD:
					if (_inventory.Items.Length == 0)
						item = null;
					else
					{
						AEquippedItem equipped = _inventory.Items.Random();

						item = equipped.Summary;
						cost = equipped.NextUpgradePrice;
						amount = cost - Mathf.RoundToInt(cost * (_stats.PriceDiscount / 100f));
					}
					break;
				case RoomRewardType.ITEM:
					item = GetRandomItem();
					cost = Databases.Database.Data.Item.Settings.ItemCosts[item.Data.Type].x;
					amount = cost - Mathf.RoundToInt(cost * (_stats.PriceDiscount / 100f));
					break;
			}
			_deal = new() { Item = item, MoneyAmount = amount };
		}

		private bool CanDeal()
		{
			switch (_dealType)
			{
				case RoomRewardType.GOLD:
					return _deal.Item != null;
				case RoomRewardType.ITEM:
					return GameManager.CanRunMoneyAfford(_deal.MoneyAmount);
			}
			return false;
		}

		private void OnDealDone()
		{
			switch (_dealType)
			{
				case RoomRewardType.GOLD:
					_inventory.RemoveItemFromInventory(_deal.Item.Data, true);
					GameManager.RewardWithRunMoney(_deal.MoneyAmount);
					break;
				case RoomRewardType.ITEM:
					_inventory.EquipItem(_deal.Item);
					GameManager.PayWithRunMoney(_deal.MoneyAmount);
					break;
			}
			_dealDone = true;
			ProcessCheckpoint(ON_DEAL_DONE);
			RunManager.CurrentRoomInstance.Clear();
		}

		private void OnCannotDeal() => ProcessCheckpoint(ON_CANNOT_DEAL);

		#endregion
	}
}
