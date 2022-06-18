using Game.Managers;
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

namespace Game.Entities.AI
{
	public class Seeker : EnemyAI, IAnimationEventListener
	{
		[SerializeField] private float _mesurePerMeter;
		[SerializeField] private float _waveAmplitude;
		private Vector3? _sinPos;

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

		protected override void Attack()
		{
			LockMovement = true;
			LockTarget(GameManager.Player.transform);
			_gfxAnim.Play("LoadCharge");
		}

		public void OnAnimationEvent(string animationArg) {}

		public void OnAnimationEnter(AnimatorStateInfo stateInfo) 
		{
			if (stateInfo.IsName("Charge"))
			{
				Vector3 dir = GetAimNormal();

				LockAim = true;
				UnlockTarget();
				Dash(dir, _aiSettings.DashRange, 0.3f, true);
				Awaiter.WaitAndExecute(0.3f, () => {
					_gfxAnim.Play("EndCharge");
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

			Gizmos.DrawWireSphere(transform.position, _aiSettings.AttackRange);

			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.position, _aiSettings.TriggerRange);
		}
	}
}
