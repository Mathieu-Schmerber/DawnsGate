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
	[RequireComponent(typeof(AudioSource))]
	public class ItemStand : LootedItem
	{
		[SerializeField] private SpriteRenderer _itemRenderer;
		[SerializeField] private AudioClip _purchaseAudio;
		[SerializeField] private AudioClip _errorAudio;

		private AudioSource _source;

		public bool WasBought { get; set; }
		public int Cost { get; set; }
		public bool IsBuyable => GameManager.CanRunMoneyAfford(Cost) || Item.IsLifeItem;

		public override string InteractionTitle => $"Buy for <color={(IsBuyable ? "white" : "red")}>{Cost} {(Item.IsLifeItem ? "<color=red>♥</color>" : "<sprite=\"money\" index=0>")}";
		public event Action OnItemPaid;

		private bool _canBeSuggested = true;

		protected override void Awake()
		{
			_spr = _itemRenderer;
			_source = GetComponent<AudioSource>();
		}

		public override void Interact(IInteractionActor actor)
		{

			if (IsBuyable && !WasBought)
			{
				actor.UnSuggestInteraction(this);
				PayItem();
			}
			else
				_source.PlayOneShot(_errorAudio);
			if (WasBought)
			{
				LootedItem.Create(_spr.transform.position, _item, -transform.forward * 2);
				SetItem(null);
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
			_source.PlayOneShot(_purchaseAudio);
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
