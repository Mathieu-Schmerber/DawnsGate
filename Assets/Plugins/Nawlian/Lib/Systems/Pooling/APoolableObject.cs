using System;
using UnityEngine;

namespace Nawlian.Lib.Systems.Pooling
{
	/// <summary>
	/// Basic implementation of IPoolableObject. <br/>
	/// This implementation declares <see cref="IPoolableObject.Released"/> if gameObject.activeSelf is false.
	/// </summary>
	[DisallowMultipleComponent]
	public abstract class APoolableObject : MonoBehaviour, IPoolableObject
	{
		#region IPoolableObject

		/// <inheritdoc/>
		public bool Released => !gameObject.activeSelf;

		/// <inheritdoc/>
		public void InitFromPool(object data) => Init(data);

		/// <inheritdoc/>
		public GameObject Get()
		{
			gameObject.SetActive(true);
			return gameObject;
		}

		/// <inheritdoc/>
		public void Release()
		{
			OnReleasing();
			gameObject.SetActive(false);
		}

		#endregion

		#region abstract

		/// <summary>
		/// Triggers just before being released.
		/// </summary>
		public event Action OnPoolReleasing;

		/// <summary>
		/// Called when spawned from the pool.
		/// </summary>
		/// <param name="data"></param>
		public abstract void Init(object data);
		protected virtual void OnReleasing() => OnPoolReleasing?.Invoke();

		#endregion
	}
}
