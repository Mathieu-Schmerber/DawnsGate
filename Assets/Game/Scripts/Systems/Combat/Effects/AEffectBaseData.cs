using Nawlian.Lib.Utils.Odin;
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
	public abstract class AEffectBaseData : ScriptableObject
	{
		[Title("Info")]
		public string DisplayName;
		public string Description;

		[Title("Effect")]
		[ValidateInput(nameof(ValidateEditor), "Script needs to inherit AEffect.")] 
		public MonoScript Script;

		[Title("Data")]
		[PropertyTooltip("Is it a reccurent effect")] public InlineToggleableValue<float> Interval;

		private void Awake()
		{
			DisplayName = name;
		}

		private bool ValidateEditor() => Script != null && !Script.GetClass().IsAbstract && Script.GetClass().IsSubclassOf(typeof(AEffect));
	}
}
