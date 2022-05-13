using Game.Entities.Shared.Attacks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Scriptables
{
	public abstract class AttackBaseData : ScriptableObject
	{
		public const string ATTACK_PREFAB_FOLDER = "Assets/Game/Resources/Prefabs/Attacks";

		public enum KnockbackDirection
		{
			FROM_CENTER,
			FORWARD
		}

		[AssetsOnly, AssetSelector(Paths = ATTACK_PREFAB_FOLDER), ValidateInput("@Validate()", "@GetError()")]
		public AttackBase Prefab;

		[Title("Attack stats")]
		public int BaseDamage;
		public int BaseKnockbackForce;
		public KnockbackDirection KnockbackDir;

#if UNITY_EDITOR

		private string GetError()
		{
			if (Prefab == null)
				return "Prefab is null";
			else
				return Prefab.IsAttackEditorValid().message;
		}

		public bool Validate()
		{
			if (Prefab == null || !Prefab.IsAttackEditorValid().isValid)
				return false;
			return true;
		}

#endif
	}
}
