using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nawlian.Lib.Systems.Health
{
	/// <summary>
	/// Provides with the ability to get damaged.
	/// </summary>
	public interface IDamageProcessor
	{
		void Damage(GameObject attacker, float amount);
		void Kill(GameObject attacker);
	}
}