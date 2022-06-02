using Game.Entities.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageProcessor
{
	bool IsDead { get; }

	/// <summary>
	/// Applies direct damages
	/// </summary>
	/// <param name="attacker"></param>
	/// <param name="amount"></param>
	void ApplyDamage(EntityIdentity attacker, float amount);

	/// <summary>
	/// Applies indirect damages
	/// </summary>
	/// <param name="amount"></param>
	void ApplyPassiveDamage(float amount);

	void ApplyKnockback(EntityIdentity attacker, Vector3 force, float knockbackTime = 0.2f);
}