using UnityEngine;

namespace Nawlian.Lib.Systems.Pooling
{
	/// <summary>
	/// Defines an object that can be pooled. <br/>
	/// IPoolableObject is self-sufficient in its way of declaring itself Released. <br/>
	/// It is up to the object itself to determine its state.
	/// </summary>
	public interface IPoolableObject
	{
		/// <summary>
		/// Is the poolable object release ? <br/>
		/// eg. Can we Get() it to spawn it ?
		/// </summary>
		public bool Released { get; }

		/// <summary>
		/// Called whenever the ObjectPooler get the object.
		/// </summary>
		/// <param name="data"></param>
		void InitFromPool(object data);

		/// <summary>
		/// Spawns the object from the pool
		/// </summary>
		GameObject Get();

		/// <summary>
		/// Releases the current object to the ObjectPooler.
		/// </summary>
		void Release();
	}
}
