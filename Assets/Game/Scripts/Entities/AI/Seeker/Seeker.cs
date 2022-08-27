using Game.Entities.Shared.Health;
using Game.Managers;
using Game.Systems.Combat.Attacks;
using Game.VFX;
using Game.VFX.Preview;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Animations;
using Nawlian.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.AI.Seeker
{
	public class Seeker : EnemyAI, IAnimationEventListener
	{
		[SerializeField] private float _mesurePerMeter;
		[SerializeField] private float _waveAmplitude;

		private SeekerStatData _stats;
		private Vector3? _sinPos;
		private bool _startDamageCheck = false;
		private AEnemySpawnFX _spawnFx;

		protected override void Init(object data)
		{
			base.Init(data);
			_stats = _entity.Stats as SeekerStatData;
		}

		protected override void Awake()
		{
			base.Awake();
			_spawnFx = GetComponentInChildren<AEnemySpawnFX>();
		}
		//protected override void OnInitState() => _spawnFx?.PlaySpawnFX(() => base.OnInitState());

		#region Movement

		protected override bool UsesPathfinding => false;

		protected override Vector3 GetTargetPosition() => _sinPos ?? GameManager.Player.transform.position;

		protected override Vector3 GetMovementsInputs()
		{
			Vector3 destination = _sinPos ?? GameManager.Player.transform.position;

			return destination - transform.position;
		}

		protected override void Update()
		{
			GenerateSinPath();
			if (_startDamageCheck)
				ApplyDashDamage();
			base.Update();
		}

		private void GenerateSinPath()
		{
			Vector3 destination = GetPathfindingDestination();
			float distance = Vector3.Distance(transform.position, destination);
			float precision = distance * _mesurePerMeter;
			Vector3 dir = (destination - transform.position).normalized;
			Vector3 right = Quaternion.AngleAxis(90, Vector3.up) * dir;

			_sinPos = transform.position + dir * (distance / precision) + right * Mathf.Sin(Time.time) * _waveAmplitude;
		}

		#endregion

		#region Attack

		private void ApplyDashDamage()
		{
			if (Vector3.Distance(transform.position, GameManager.Player.transform.position) <= _stats.AttackRadius)
				Attack(GameManager.Player.gameObject);
		}

		private void Attack(GameObject obj)
		{
			if (!_startDamageCheck)
				return;

			Damageable damageable = obj.GetComponent<Damageable>();

			if (damageable != null)
			{
				AttackBase.ApplyDamageLogic(_entity, damageable, KnockbackDirection.FROM_CENTER, _stats.BaseDamage, _stats.BaseKnockBackForce, _stats.HitFx);
				_startDamageCheck = false;
			}
		}

		protected override void Attack()
		{
			LockMovement = true;
			LockTarget(GameManager.Player.transform);
			_gfxAnim.Play("LoadCharge");
		}

		public void OnAnimationEvent(string animationArg) { }

		public void OnAnimationEnter(AnimatorStateInfo stateInfo)
		{
			if (stateInfo.IsName("Charge"))
			{
				Vector3 dir = GetAimNormal();

				LockAim = true;
				UnlockTarget();
				Dash(dir, _stats.DashRange, 0.3f, true, true);
				_startDamageCheck = true;
				Awaiter.WaitAndExecute(0.3f, () =>
				{
					_gfxAnim.Play("EndCharge");
					_startDamageCheck = false;
				});
			}
			else if (stateInfo.IsName("LoadCharge"))
			{
				Preview.Show(_stats.DashPreview, transform.position, Quaternion.LookRotation(GetAimNormal()), _stats.AttackRadius, .5f,
				OnUpdate: (param) =>
				{
					param.Transform.localScale = new Vector3(1, 1, Vector3.Distance(transform.position, transform.position + GetAimNormal() * _stats.DashRange));
					param.Transform.rotation = Quaternion.LookRotation(GetAimNormal());
				});
			}
		}

		public void OnAnimationExit(AnimatorStateInfo stateInfo)
		{
			if (stateInfo.IsName("EndCharge"))
			{
				LockMovement = false;
				LockAim = false;
				UnlockTarget();
				OnAttackEnd();
			}
		}

		#endregion

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
				return;
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(NextAggressivePosition, 0.2f);

			Gizmos.color = Color.green;
			Gizmos.DrawSphere(NextPassivePosition, 0.2f);

			Gizmos.DrawWireSphere(transform.position, _stats.AttackRange);

			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.position, _stats.TriggerRange);
		}
	}
}
