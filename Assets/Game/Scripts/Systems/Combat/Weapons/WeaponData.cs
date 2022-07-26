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
			[@Tooltip("Where to spawn the attack, relative to the player (Z being in front of the player)")] public Vector3 StartOffset;
			[@Tooltip("The distance that the attack will travel, relative to the attack Z")] public float TravelDistance;
			[@Tooltip("Should this move be aim assisted ?")] public bool AimAssist;
			[@Tooltip("Can the player rotate during the attack ?")] public bool LockAim;
			[@Tooltip("Can the player move during the attack ?")] public bool LockMovement;
		}

		[System.Serializable]
		public class Dash
		{
			[@Tooltip("Should the dash event trigger only when moving ?")] public bool OnlyWhenMoving;
			[Min(0)] public float Distance;

#if UNITY_EDITOR

			public string GetInfo() => null;

			public string GetWarning() => null;

#endif
		}

		[System.Serializable]
		public class FX
		{
			[Range(0, 2)] public float CameraShakeForce;
			[Min(0)] public float CameraShakeDuration;

			[Range(0, 1)] public float VibrationForce;
			[Min(0)] public float VibrationDuration;

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

			public bool ContainsEvent(WeaponAttackEvent @event) => AttackAnimation != null && AttackAnimation.events.Any(x => x.stringParameter == @event.ToString());
			public bool HasError() => !ContainsEvent(WeaponAttackEvent.Attack);
		}

		#endregion

		#region Properties

		public Material Material;
		public Mesh Mesh;
		public Vector3 InHandPosition;
		public Vector3 InHandRotation;
		[@Tooltip("Animator layer to use when moving around holding this weapon.")] 
		public string LocomotionLayer = "DefaultLocomotion";
		[@Tooltip("Base attack speed of the weapon, affects animation speed.")]
		[Min(0)] public float AttackSpeed;
		[@Tooltip("Maximum allowed time to perform an attack input while not breaking a combo.")] 
		[Min(0)] public float ComboIntervalTime;
		public List<WeaponAttack> AttackCombos;

		#endregion

		public bool IsHeavy(WeaponAttack attack) => attack == AttackCombos[AttackCombos.Count - 1];

#if UNITY_EDITOR



#endif
	}
}