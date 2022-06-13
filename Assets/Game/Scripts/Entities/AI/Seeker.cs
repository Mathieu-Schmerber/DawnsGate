using Game.Managers;
using Nawlian.Lib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.AI
{
	public class Seeker : EnemyAI
	{
		protected override bool UsesPathfinding => false;
		protected override Vector3 GetTargetPosition() => GameManager.Player.transform.position;

		protected override void Move()
		{
			Vector3 destination = GetTargetPosition().WithY(transform.position.y);
			Vector3 dir = (destination - transform.position).normalized;

			//// Calculate how fast we should be moving
			//Vector3 targetVelocity = dir;
			//targetVelocity *= _identity.CurrentSpeed;

			//// Apply a force that attempts to reach our target velocity
			//var velocity = _rb.velocity;
			//var velocityChange = targetVelocity - velocity;
			//velocityChange.x = Mathf.Clamp(velocityChange.x, -_identity.CurrentSpeed, _identity.CurrentSpeed);
			//velocityChange.z = Mathf.Clamp(velocityChange.z, -_identity.CurrentSpeed, _identity.CurrentSpeed);
			//velocityChange.y = 0;
			//_rb.AddForce(velocityChange, ForceMode.VelocityChange);

			//// We apply gravity manually for more tuning control
			//_rb.AddForce(new Vector3(0, -14f * _rb.mass, 0));
			transform.Translate(dir * _identity.CurrentSpeed * Time.deltaTime);
		}
	}
}
