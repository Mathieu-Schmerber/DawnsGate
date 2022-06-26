using Game.Entities.Shared;
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
		public bool IsBuyable => GameManager.CanRunMoneyAfford(Cost) || Item.IsLifeItem;

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
			if (!Item.IsLifeItem)
				GameManager.PayWithRunMoney(Cost);
			else
			{
				var entity = GameManager.Player.GetComponent<EntityIdentity>();
				float healthRatio = entity.CurrentHealth / entity.MaxHealth;

				entity.Stats.Modifiers[StatModifier.MaxHealth].BonusModifier -= Item.LifeCost;
				entity.CurrentHealth = Mathf.Max(1, entity.MaxHealth * healthRatio);
			}
			OnItemPaid?.Invoke();
			WasBought = true;
		}

		protected override void OnSuggesting(IInteractionActor actor)
		{
			if (_canBeSuggested && Item != null)
				base.OnSuggesting(actor);
		}
	}
}
