using Game.Managers;
using Game.Systems.Combat.Attacks;
using Game.VFX;
using Game.VFX.Preview;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Animations;
using Nawlian.Lib.Systems.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using static Game.Systems.Combat.Attacks.AttackBase;

namespace Game.Entities.AI.Thrower
{
	public class Thrower : EnemyAI, IAnimationEventListener
	{
		private ThrowerStatData _stats;
		private NavMeshPath _previsionPath;
		private Vector3 _lastHitPos;
		private AEnemySpawnFX _spawnFx;

		protected override void Init(object data)
		{
			base.Init(data);
			_stats = _entity.Stats as ThrowerStatData;
		}

		protected override void Awake()
		{
			base.Awake();
			_previsionPath = new();
			_spawnFx = GetComponentInChildren<AEnemySpawnFX>();
		}

		protected override void OnInitState() => _spawnFx.PlaySpawnFX(() => base.OnInitState());

		#region Movement

		protected override bool UsesPathfinding => true;

		private bool PathIsDangerous(Vector3 destination)
		{
			NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, _previsionPath);

			if (_previsionPath.corners.Length < 2)
				return true;

			Vector3 dirToPoint = _previsionPath.corners[1] - transform.position;
			Vector3 dirToPlayer = GameManager.Player.transform.position - transform.position;
			float angle = Vector3.Angle(dirToPlayer, dirToPoint);

			return angle < 40f;
		}

		protected override Vector3 CalculateNextAggressivePoint()
		{
			var center = GameManager.Player.transform.position;
			var aroundPos = _room.Info.GetPositionsAround(center, _stats.AttackRange);

			if (aroundPos?.Length == 0)
				return _room.Info.Data.SpawnablePositions.Random();

			return aroundPos.Where(x => !PathIsDangerous(x)).Random();
		}

		#endregion

		#region Attack

		protected override void Attack() => _gfxAnim.Play("Attack");

		private void SpawnAOE()
		{
			if (RunManager.RunState != RunState.IN_RUN)
				return;

			InitData init = new InitData()
			{
				Caster = _entity,
				Data = _stats.AoeAttack
			};

			ObjectPooler.Get(_stats.AoeAttack.Prefab.gameObject, _lastHitPos, Quaternion.identity, init,
				(go) =>
				{
					var attack = go.GetComponent<AttackBase>();
					attack.OnStart(Vector3.zero, 0);
				});
		}

		public void OnAnimationEvent(string animationArg)
		{
			if (animationArg == "Attack")
			{
				_lastHitPos = GameManager.Player.transform.position;
				ObjectPooler.Get(_stats.Projectile, transform.position, Quaternion.identity, new ProjectileParameters()
				{
					Lifetime = _stats.TravelTime,
					MaxAltitude = _stats.MaxAltitude,
					Destination = _lastHitPos
				}, null);
				Preview.Show(_stats.AoeAttack.Previsualisation, _lastHitPos, _stats.AoeAttack.Range / 2, _stats.TravelTime);
				Invoke(nameof(SpawnAOE), _stats.TravelTime);
			}
		}

		public void OnAnimationEnter(AnimatorStateInfo stateInfo)
		{
			if (stateInfo.IsName("Attack"))
			{
				LockMovement = true;
				LockAim = true;
			}
		}

		public void OnAnimationExit(AnimatorStateInfo stateInfo)
		{
			if (stateInfo.IsName("Attack"))
			{
				LockMovement = false;
				LockAim = false;
				OnAttackEnd();
			}
		}

		#endregion
	}
}
