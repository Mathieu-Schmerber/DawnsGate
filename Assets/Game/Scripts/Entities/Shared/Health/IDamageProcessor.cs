using Game.Entities.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageProcessor
{
	bool IsDead { get; }

	void ApplyDamage(EntityIdentity attacker, float amount);

	void ApplyKnockback(EntityIdentity attacker, Vector3 force, float knockbackTime = 0.2f);
}