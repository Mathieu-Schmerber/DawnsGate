using Game.Entities.AI;
using Game.Managers;
using Game.Systems.Combat.Attacks;
using Game.VFX.Previsualisations;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Utils;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Entities.Lunaris
{
	public class LunarisAI : EnemyAI
	{
		private LunarisStatData _stats;
		private Timer _passiveTimer = new();

		private LunarisStatData.PhaseSettings _currentPhase => _stats.Phases[_phase];

		#region Unity builtins

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Init(object data)
		{
			base.Init(data);
			_stats = _aiSettings as LunarisStatData;
			_passiveTimer.Start(_currentPhase.SpawnRate, true, OnPassiveTick);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			_passiveTimer.Stop();
		}

		protected override void Update()
		{
			base.Update();
		}

		#endregion

		#region Phases

		private readonly LunarisPhase LAST_PHASE = LunarisPhase.STAFF;
		private LunarisPhase _phase = 0;
		internal bool IsLastPhase => _phase == LAST_PHASE;
		internal void SetNextPhase()
		{
			_phase++;
			_passiveTimer.Interval = _currentPhase.SpawnRate;
			_passiveTimer.Restart();
		}

		#endregion

		#region Movement

		protected override bool UsesPathfinding => true;

		#endregion

		#region Passive

		private void OnPassiveTick()
		{
			for (int i = 0; i <= (int)_phase; i++)
				Previsualisation.ShowCircle(
					GameManager.Player.transform.position + Random.insideUnitSphere.WithY(GameManager.Player.transform.position.y) * _currentPhase.PassiveSpread,
					_stats.PassiveAttack.Range,
					_currentPhase.PrevisualisationDuration,
					SpawnPassive);

			if (IsLastPhase)
			{
				for (int i = 0; i < 3; i++)
				{
					Previsualisation.ShowCircle(
					_room.Info.Data.SpawnablePositions.Random(),
					_stats.PassiveAttack.Range,
					_currentPhase.PrevisualisationDuration,
					SpawnPassive);
				}
			}

		}

		private void SpawnPassive(PrevisuParameters obj)
		{
			if (RunManager.RunState == RunState.IN_RUN)
			{
				AttackBase.Spawn(_stats.PassiveAttack, obj.Position, Quaternion.identity, new()
				{
					Caster = _entity,
					Data = _stats.PassiveAttack
				});
			}
		}

		#endregion

		#region Attack

		protected override void Attack()
		{
			OnAttackEnd();
		}

		#endregion
	}
}