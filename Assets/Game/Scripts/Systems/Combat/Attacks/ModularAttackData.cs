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
		public bool ScaleOverLifetime;
		[ShowIf(nameof(ScaleOverLifetime))] public Vector3 EndScale;
		public bool RotateOverTime;
		[ShowIf(nameof(RotateOverTime))] public Vector3 EndRotation;
		public bool ContinuouslyHit;
		
		[Title("Game feel")]
		[ValidateInput(nameof(EditorValidate), "HitFx needs an IPoolableObject component.")] public GameObject HitFx;
		public float HitYRotation;

		[Title("Settings")]
		public float ActiveTime;
		public bool FollowCaster;
		public float Range;

		public override float AttackRange => Range;
		public override bool StickToCaster => FollowCaster;

		private bool EditorValidate() => HitFx != null && HitFx.GetComponent<IPoolableObject>() != null;
	}
}
