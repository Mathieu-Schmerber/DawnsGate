using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Combat.Effects;

namespace Game.Entities.Player
{
	public class PlayerIdentity : EntityIdentity
	{
		private PlayerDamageable _damageable;
		private Inventory _inventory;
		private EffectProcessor _effects;

		protected override void Awake()
		{
			base.Awake();
			_damageable = GetComponent<PlayerDamageable>();
			_inventory = GetComponent<Inventory>();
			_effects = GetComponent<EffectProcessor>();
		}

		private void OnEnable()
		{
			_damageable.OnPlayerDeath += OnRunEnded;
		}

		private void OnDisable()
		{
			_damageable.OnPlayerDeath -= OnRunEnded;
		}

		#region Run Ending management 

		private void ClearEffects() => _effects.ClearAllEffects();

		private void ClearInventory() => _inventory.RemoveAllItems();

		public override void ResetStats()
		{
			base.ResetStats();
			GameManager.Traits.ForEach(x =>
			{
				for (int i = 0; i < GameManager.GetTraitUpgradeCount(x); i++)
					GameManager.ApplyTrait(x);
			});
		}

		private void OnRunEnded()
		{
			// Order matters, do not refactor into a subscription-based code please !
			ClearEffects();
			ClearInventory();
			ResetStats();
		}

		#endregion
	}
}
