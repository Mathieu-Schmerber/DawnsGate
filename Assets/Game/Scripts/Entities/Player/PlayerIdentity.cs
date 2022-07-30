using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Combat.Effects;
using Game.VFX;
using System;

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
			OnHealthChanged += DisplayHealthText;
			OnArmorChanged += DisplayArmorText;
			_damageable.OnPlayerDeath += OnRunEnded;
			GameManager.OnRunMoneyUpdated += DisplayMoneyText;
			RunManager.OnRunEnded += OnRunEnded;
		}

		private void OnDisable()
		{
			OnArmorChanged -= DisplayArmorText;
			OnHealthChanged -= DisplayHealthText;
			_damageable.OnPlayerDeath -= OnRunEnded;
			GameManager.OnRunMoneyUpdated -= DisplayMoneyText;
			RunManager.OnRunEnded -= OnRunEnded;
		}

		private void DisplayMoneyText(int before, int now)
		{
			QuickText.ShowGoldText(transform.position, now - before);
		}

		private void DisplayHealthText(float before, float now)
		{
			if (before < now)
				QuickText.ShowHealText(transform.position, now - before);
		}

		private void DisplayArmorText(float before, float now)
		{
			QuickText.ShowArmorText(transform.position, now - before);
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
					GameManager.ApplySingleTraitUpgrade(x);
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
