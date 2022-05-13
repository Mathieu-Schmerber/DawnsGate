using Game.Scriptables;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Game.Entities.Shared
{
	public class EntityIdentity : MonoBehaviour
	{
		[SerializeField] private EntityStatData _stats;
		private int _currentHealth;

		public int CurrentHealth { get => _currentHealth; set => _currentHealth = Mathf.Clamp(value, 0, _stats.MaxHealth.Value); }

		public EntityStatData Stats => _stats;


		private void Awake()
		{
			CurrentHealth = Stats.MaxHealth.Value;
		}
	}
}