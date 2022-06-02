using Game.Entities.Shared;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.Combat.Attacks
{
	public abstract class AttackBase : MonoBehaviour
	{
		protected AttackBaseData _data;

		/// <summary>
		/// Determines if the attack follows the caster (takes it as an anchor point)
		/// </summary>
		public abstract bool FollowCaster { get; }

		/// <summary>
		/// The range of the attack (only informative for the aim assist to act, will not affect the attack behaviour).
		/// </summary>
		public abstract float Range { get; }

		protected EntityIdentity Caster { get; private set; }

		public virtual void Init(AttackBaseData data, EntityIdentity caster)
		{
			_data = data;
			Caster = caster;
		}

		public abstract void OnStart(Vector3 offset, Vector3 travelDistance);

#if UNITY_EDITOR
		protected abstract void OnAttackHit(Collider collider);
#endif
		public abstract (bool isValid, string message) IsAttackEditorValid();

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject == Caster.gameObject)
				return;
			OnAttackHit(other);
		}
	}
}