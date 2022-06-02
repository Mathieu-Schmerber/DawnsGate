using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nawlian.Lib.Utils.Odin
{

	/// <summary>
	/// Defines an In-Editor toggleable
	/// </summary>
	[System.Serializable, Toggle(nameof(AToggleable.Enabled), CollapseOthersOnExpand = false)]
	public abstract class AToggleable
	{
		public bool Enabled = true;
	}

	/// <summary>
	/// Defines an In-Editor toggleable hosting a single value
	/// </summary>
	[System.Serializable, InlineProperty]
	public class ToggleableValue<T> : AToggleable
	{
		[SerializeField] private T _value;

		public T Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public static implicit operator ToggleableValue<T>(T b) => new ToggleableValue<T>() { Value = b };
	}

	/// <summary>
	/// Defines an In-Editor toggleable hosting a single value
	/// </summary>
	[System.Serializable, InlineProperty]
	public class InlineToggleableValue<T>
	{
		[HorizontalGroup, HideLabel] public bool Enabled = true;

		[SerializeField, HorizontalGroup, HideLabel, EnableIf(nameof(Enabled))] private T _value;

		public T Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public static implicit operator InlineToggleableValue<T>(T b) => new InlineToggleableValue<T>() { Value = b };
	}
}