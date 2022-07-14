using UnityEngine;

namespace Game.Systems.Combat.Attacks
{
	public class AttackSubPart : MonoBehaviour
	{
		private AttackBase _base;

		protected virtual void Awake()
		{
			_base = GetComponentInParent<AttackBase>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject == _base.Caster.gameObject)
				return;
			_base.OnAttackHit(other);
		}
	}
}