using Game.Systems.Combat.Attacks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Combat.Weapons
{
	public enum WeaponAttackEvent
	{
		Attack,
		Dash
	}

	[CreateAssetMenu(menuName = "Data/Weapons/Weapon")]
	public class WeaponData : SerializedScriptableObject
	{
		#region Types

		[System.Serializable]
		public class Attack
		{
			public AttackBaseData AttackData;
			public Vector3 StartOffset;
			public Vector3 TravelDistance;
			public bool AimAssist;
			public bool LockAim;
			public bool LockMovement;

			private bool AssertIsAttack(GameObject prefab) => prefab?.GetComponent<AttackBase>() != null;
		}

		[System.Serializable]
		public class Dash
		{
			public bool OnlyWhenMoving;
			public bool OnAnimationEventOnly;
			[Min(0)] public float Distance;

#if UNITY_EDITOR

			public string GetInfo() => $"The {nameof(Distance)} field can be set to 0 to disable auto-dashing the attack.";

			public string GetWarning()
			{
				if (Distance > 0)
					return null;
				if (OnlyWhenMoving)
					return $"{nameof(OnlyWhenMoving)} is checked, but the dash distance is 0.";
				if (OnAnimationEventOnly)
					return $"{nameof(OnAnimationEventOnly)} is checked, but the dash distance is 0.";
				return null;
			}

#endif
		}

		[System.Serializable]
		public class FX
		{
			public Vector3 CameraShakeForce;
			public float CameraShakeDuration;

			[Range(0, 1)] public float VibrationForce;
			public float VibrationDuration;

#if UNITY_EDITOR
			public string GetInfo() => $"Any FX triggers when the {WeaponAttackEvent.Attack} event triggers on the animation.";

			public string GetWarning() => null;
#endif
		}

		[System.Serializable]
		public class WeaponAttack
		{
			public AnimationClip AttackAnimation;
			public Attack Attack;
			public Dash Dash;
			public FX FX;

			public WeaponAttack()
			{
				Attack = new();
				Dash = new();
				FX = new();
			}

			public bool ContainsEvent(WeaponAttackEvent @event) => AttackAnimation.events.Any(x => x.stringParameter == @event.ToString());
		}

		#endregion

		#region Properties

		public Mesh Mesh;
		[@Tooltip("Animator layer to use when moving around holding this weapon.")] public string LocomotionLayer = "DefaultLocomotion";
		[Min(0), @Tooltip("Base attack speed of the weapon, affects animation speed.")] public float AttackSpeed;
		[Min(0), @Tooltip("Maximum allowed time between two attacks to not break the combo.")] public float ComboIntervalTime;
		public List<WeaponAttack> AttackCombos;

		#endregion

		public bool IsHeavy(WeaponAttack attack) => attack == AttackCombos[AttackCombos.Count - 1];

#if UNITY_EDITOR



#endif
	}
}