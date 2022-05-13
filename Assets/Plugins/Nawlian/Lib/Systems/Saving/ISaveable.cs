namespace Nawlian.Lib.Systems.Saving
{
	/// <summary>
	/// Defines a component as saveable.
	/// </summary>
	public interface ISaveable
	{
		/// <summary>
		/// Loads previously saved data.
		/// </summary>
		/// <param name="data"></param>
		public void Load(object data);

		/// <summary>
		/// Saves current data.
		/// </summary>
		/// <returns></returns>
		public object Save();
	}
}