using Nawlian.Lib.Utils;
using System;
using System.Linq;

namespace Game.Entities.Shared
{
	[Serializable]
	public class StatDictionary : SerializedDictionary<StatModifier, StatLineModifier>, ICloneable
	{
		public object Clone() => this.ToDictionary(entry => entry.Key, entry => entry.Value);
	}
}