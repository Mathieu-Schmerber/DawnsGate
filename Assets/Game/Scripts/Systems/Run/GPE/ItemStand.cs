using Game.Managers;
using Game.Systems.Items;
using Nawlian.Lib.Systems.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class ItemStand : LootedItem
	{
		[SerializeField] private SpriteRenderer _itemRenderer;

		public bool WasBought { get; set; }
		public int Cost { get; set; }
		public bool IsBuyable => GameManager.CanMoneyAfford(Cost);

		public override string InteractionTitle => $"Buy";
		public event Action OnItemPaid;

		private bool _canBeSuggested = true;

		protected override void Awake()
		{
			_spr = _itemRenderer;
		}

		public override void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);

			if (IsBuyable && !WasBought)
				PayItem();
			if (WasBought && TryEquipItem(actor))
			{
				_canBeSuggested = false;
				actor.UnSuggestInteraction(this);
			}

		}

		private void PayItem()
		{
			GameManager.PayWithMoney(Cost);
			OnItemPaid?.Invoke();
			WasBought = true;
		}

		protected override void OnSuggesting(IInteractionActor actor)
		{
			if (_canBeSuggested && _data != null)
				base.OnSuggesting(actor);
		}
	}
}
