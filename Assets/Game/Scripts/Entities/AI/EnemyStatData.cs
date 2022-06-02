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
		/// <summary>
		/// Attack range of an entity
		/// </summary>
		[SuffixLabel("In percent", true), TitleGroup("Attack")] public StatLine AttackRange;
	}
}
