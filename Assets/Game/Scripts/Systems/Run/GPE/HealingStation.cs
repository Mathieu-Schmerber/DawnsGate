using Game.Entities.Player;
using Game.Entities.Shared;
using Game.Managers;
using Nawlian.Lib.Systems.Interaction;
using System;
using UnityEngine;

namespace Game.Systems.Run.GPE
{
	public class HealingStation : ATriggerInteractable
	{
		[SerializeField, Range(0, 1)] private float _healAmount;
		private bool _used = false;

		public event Action OnInteracted;
		public override string InteractionTitle => $"Praise the sun (+ {HealAmount}<color=red>♥</color>)";
		public int HealAmount => Mathf.CeilToInt(_player.MaxHealth * _healAmount);

		private EntityIdentity _player;

		private void Start()
		{
			_player = GameManager.Player.GetComponent<EntityIdentity>();
		}

		public override void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
			_used = true;
			OnInteracted?.Invoke();
		}

		public void Heal()
		{
			_player.CurrentHealth += HealAmount;
		}

		protected override void OnSuggesting(IInteractionActor actor)
		{
			if (!_used)
				base.OnSuggesting(actor);
		}
	}
}
