using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Game.Systems.Combat.Effects
{
	/// <summary>
	/// Processes effects on an entity
	/// </summary>
	[DisallowMultipleComponent]
	public class EffectProcessor : MonoBehaviour
	{
		#region Types
		[System.Serializable] public class EffectDictionary : SerializedDictionary<AEffectBaseData, AEffect> { }
		#endregion

		[ShowInInspector, ReadOnly] private EffectDictionary _activeEffects = new();

		private AEffect AddEffectToActive(AEffectBaseData data)
		{
			AEffect behaviour = gameObject.AddComponent(data.Script.GetClass()) as AEffect;

			_activeEffects[data] = behaviour;
			return behaviour;
		}

		public void ApplyEffect(AEffectBaseData data, float duration)
		{
			AEffect exists = _activeEffects.ContainsKey(data) ? _activeEffects[data] : null;

			if (exists == null)
				exists = AddEffectToActive(data);
			exists.OnStart(duration, data);
		}

		public void RemoveEffect(AEffectBaseData data)
		{
			_activeEffects[data].Delete();
			_activeEffects.Remove(data);
		}
	}
}
