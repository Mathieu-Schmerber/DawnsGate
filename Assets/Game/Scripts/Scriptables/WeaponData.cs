using Game.Entities.Shared.Attacks;
using Game.Scriptables;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scriptables
{
	[CreateAssetMenu(menuName = "Data/Weapons/Weapon")]
	public class WeaponData : SerializedScriptableObject
	{
		#region Types

		[System.Serializable]
		public class Attack
		{
			[Required]
			public AttackBaseData AttackData;
			public Vector3 StartOffset;
			public Vector3 TravelDistance;
			public bool AimAssist;
			public bool LockAim;
			public bool LockMovement;
			public bool IsHeavyAttack;

			private bool AssertIsAttack(GameObject prefab) => prefab?.GetComponent<AttackBase>() != null;
		}

		[System.Serializable]
		public class Dash
		{
			public bool OnlyWhenMoving;
			public bool OnAnimationEventOnly;
			[Min(0)] public float Distance;
			[Min(0)] public float Duration;
		}

		[System.Serializable]
		public class FX
		{
			public Vector3 CameraShakeForce;
			public float CameraShakeDuration;

			[Range(0, 1)] public float VibrationForce;
			public float VibrationDuration;
		}

		[System.Serializable]
		public class WeaponAttack
		{
			[Required] public AnimationClip AttackAnimation;

			[TabGroup("Attack"), HideLabel] public Attack Attack;
			[TabGroup("Dash"), HideLabel] public Dash Dash;
			[TabGroup("FX"), HideLabel] public FX FX;
			public int OnHitDamage;
		}

		#endregion

		#region Properties


		[Required] public Mesh Mesh;
		public string LocomotionLayer = "DefaultLocomotion";
		[Min(0)] public float AttackSpeed;
		[Min(0)] public float ComboIntervalTime;
		public List<WeaponAttack> AttackCombos;

		#endregion
	}
}