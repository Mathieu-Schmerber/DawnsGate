using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageProcessor
{
	bool IsDead { get; }

	void ApplyDamage(GameObject attacker, int amount);

	void ApplyKnockback(GameObject attacker, Vector3 force, float knockbackTime = 0.2f);
}