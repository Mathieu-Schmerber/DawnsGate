using Nawlian.Lib.Systems.Pooling;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Combat.Attacks
{
	[CreateAssetMenu(menuName = "Data/Attacks/ModularAttack")]
	public class ModularAttackData : AttackBaseData
	{
		[Title("Game feel")]
		[ValidateInput(nameof(EditorValidate), "HitFx needs an IPoolableObject component.")] public GameObject HitFx;
		public float HitYRotation;

		[Title("Settings")]
		public float ActiveTime;
		public bool FollowCaster;
		public float Range;

		private bool EditorValidate() => HitFx != null && HitFx.GetComponent<IPoolableObject>() != null;
	}
}
