using Game.Entities.AI;
using Game.Systems.Combat.Attacks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.AI
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Tank")]
	public class TankStatData : EnemyStatData
	{
		[SuffixLabel("in %", true)] public float ArmorGain;
		[SuffixLabel("in %", true)] public float KnockbackResistanceGain;
		[Range(0, 1)] public float VibrationForce;
		public float VibrationDuration;
	}
}
