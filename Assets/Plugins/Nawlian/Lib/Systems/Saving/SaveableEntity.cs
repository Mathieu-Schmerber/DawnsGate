using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nawlian.Lib.Systems.Saving
{
	/// <summary>
	/// Marks a GameObject as Saveable. <br/>
	/// This MonoBehaviour should be attach to any object containing ISaveable components.
	/// </summary>
	/// <see cref="ISaveable"/>
	public class SaveableEntity : MonoBehaviour
	{
		/// <summary>
		/// Unique Identifier used by the SaveSystem.
		/// </summary>
		/// <see cref="SaveSystem.SaveSceneState(Dictionary{string, object})"/>
		/// <see cref="SaveSystem.LoadSceneState(Dictionary{string, object})"/>
		[ReadOnly] public string Id;

		/// <summary>
		/// Generates the unique Id <br/>
		/// This will be called whenever the script is compiled by the editor.
		/// </summary>
		private void OnValidate()
		{
			if (string.IsNullOrEmpty(Id))
				Id = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Saves all ISaveable components states
		/// </summary>
		/// <returns></returns>
		public object Save()
		{
			Dictionary<string, object> state = new Dictionary<string, object>();

			foreach (var saveable in GetComponents<ISaveable>())
				state.Add(saveable.GetType().ToString(), saveable.Save());
			return state;
		}

		/// <summary>
		/// Loads all ISaveable components from save
		/// </summary>
		/// <returns></returns>
		public void Load(object data)
		{
			Dictionary<string, object> state = (Dictionary<string, object>)data;

			foreach (var saveable in GetComponents<ISaveable>())
			{
				string typename = saveable.GetType().ToString();

				if (state.TryGetValue(typename, out object value))
					saveable.Load(value);
			}
		}
	}
}