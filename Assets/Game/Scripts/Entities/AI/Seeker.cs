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

		protected override void Init(object data)
		{
			base.Init(data);
			transform.Translate(new Vector3(0, 0.5f, 0));
		}

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
			LockTarget(GameManager.Player.transform, true);
			_gfxAnim.Play("LoadCharge");
		}

		public void OnAnimationEvent(string animationArg) {}

		public void OnAnimationEnter(AnimatorStateInfo stateInfo) 
		{
			if (stateInfo.IsName("Charge"))
			{
				Vector3 dir = GetAimNormal();

				UnlockTarget();
				IsAimLocked = true;
				Dash(dir, _aiSettings.AttackRange, 0.3f);
				Awaiter.WaitAndExecute(0.3f, () => {
					_gfxAnim.Play("EndCharge");
				});
			}
		}

		public void OnAnimationExit(AnimatorStateInfo stateInfo)
		{
			if (stateInfo.IsName("EndCharge"))
			{
				State = Shared.EntityState.IDLE;
				LockMovement = false;
				IsAimLocked = false;
			}
		}

		#endregion
	}
}
