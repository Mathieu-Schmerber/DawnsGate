using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Nawlian.Lib.Utils
{
	/// <summary>
	/// Provides a ScriptableObject with a unique Id
	/// </summary>
	public abstract class IdentifiableSO : ScriptableObject
	{
		[ReadOnly] public string Id = string.Empty;

		/// <summary>
		/// On create/compile
		/// </summary>
		protected virtual void Awake()
		{
			if (string.IsNullOrEmpty(Id))
				GenerateID();
		}

		[Button, ShowIf("@string.IsNullOrEmpty(ID)")]
		private void GenerateID() => Id = Guid.NewGuid().ToString();
	}
}