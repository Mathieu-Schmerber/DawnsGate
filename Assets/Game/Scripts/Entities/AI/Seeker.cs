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
		[SerializeField] private float _mesurePerMeter;
		[SerializeField] private float _waveAmplitude;
		private Vector3? _sinPos;

		protected override bool UsesPathfinding => false;

		protected override Vector3 GetTargetPosition() => _sinPos ?? GameManager.Player.transform.position;

		protected override Vector3 GetMovementsInputs()
		{
			Vector3 destination = _sinPos ?? GameManager.Player.transform.position;

			return destination - transform.position;
		}

		protected override void Init(object data)
		{
			base.Init(data);
			transform.Translate(new Vector3(0, 0.5f, 0));
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
	}
}
