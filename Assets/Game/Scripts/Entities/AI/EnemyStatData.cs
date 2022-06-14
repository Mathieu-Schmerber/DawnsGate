using Game.Entities.Shared;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.AI
{
	[CreateAssetMenu(menuName = "Data/Entity/Stats/Enemy")]
	public class EnemyStatData : BaseStatData
	{
		[Title("AI")]
		[@Tooltip("The range at which the enemy spots the player")]
		public float TriggerRange;

		[@Tooltip("The range at which the enemy returns to a patrol state")]
		public float UntriggerRange;
	}
}
