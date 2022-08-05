using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Combat.Effects;
using Game.VFX;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Entities.Player
{
	public class PlayerIdentity : EntityIdentity
	{
		private PlayerDamageable _damageable;
		private Inventory _inventory;
		private EffectProcessor _effects;
		private PlayerController _controller;

		protected override void Awake()
		{
			base.Awake();
			_damageable = GetComponent<PlayerDamageable>();
			_inventory = GetComponent<Inventory>();
			_effects = GetComponent<EffectProcessor>();
			_controller = GetComponent<PlayerController>();
		}

		private void OnEnable()
		{
			OnHealthChanged += DisplayHealthText;
			OnArmorChanged += DisplayArmorText;
			_damageable.OnPlayerDeath += GameOver;
			GameManager.OnRunMoneyUpdated += DisplayMoneyText;
			RunManager.OnRunEnded += OnRunEnded;
			RunManager.OnBeforeSceneSwitched += ClearState;
		}

		private void OnDisable()
		{
			OnArmorChanged -= DisplayArmorText;
			OnHealthChanged -= DisplayHealthText;
			_damageable.OnPlayerDeath -= GameOver;
			GameManager.OnRunMoneyUpdated -= DisplayMoneyText;
			RunManager.OnRunEnded -= OnRunEnded;
			RunManager.OnBeforeSceneSwitched -= ClearState;
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
			GameManager.Traits.ForEach(x => GameManager.ApplyMultipleTraitUpgrades(x));
		}

		private void OnRunEnded()
		{
			// Order matters, do not refactor into a subscription-based code please !
			ClearEffects();
			ClearInventory();
			ResetStats();
		}

		private void ClearState()
		{
			_controller.SetAnimatorState("IsDead", false);
			_controller.UnRestrict();
			GameManager.Camera.UnlockTarget();
		}

		private void GameOver()
		{
			IEnumerator AnimateGameOver(Action onDone)
			{
				GameManager.Camera.LockTemporaryTarget(transform, 0.7f);
				_controller.Restrict();
				_controller.SetAnimatorState("IsDead", true);
				yield return new WaitForSeconds(5f);
				onDone();
			}
			StartCoroutine(AnimateGameOver(() => RunManager.EndRun()));
		}

		#endregion
	}
}
