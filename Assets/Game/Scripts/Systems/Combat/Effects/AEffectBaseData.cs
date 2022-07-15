using Nawlian.Lib.Utils.Odin;
using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

namespace Game.Systems.Combat.Effects
{
	public abstract class AEffectBaseData : SerializedScriptableObject
	{
		[Title("Info")]
		public string DisplayName;
		public string Description;

		public Type Component;

#if UNITY_EDITOR
		[Title("Effect")]
		[OnValueChanged(nameof(OnScriptChanged)), ValidateInput(nameof(ValidateEditor), "Script needs to inherit AEffect.")]
		public MonoScript Script;
		private bool ValidateEditor() => Script != null && !Script.GetClass().IsAbstract && Script.GetClass().IsSubclassOf(typeof(AEffect));

		[Button("Update script")]
		public void OnScriptChanged()
		{
			if (ValidateEditor())
			{
				Component = Script.GetClass();
				Debug.Log($"{DisplayName} Component set to {Component}");
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
		}
#endif

		[Title("Data")]
		[PropertyTooltip("Is it a reccurent effect")] public InlineToggleableValue<float> Interval;

		private void Awake()
		{
			DisplayName = name;
		}


	}
}
