using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using System.Linq;
using Nawlian.Lib.Utils;
using System.Linq.Expressions;
using Nawlian.Lib.Extensions;

namespace Nawlian.Lib.Systems.Pooling
{
	/// <summary>
	/// Object pooling manager class. <br/>
	/// ObjectPooler is a MonoBehaviour Singleton managing IPoolableObject.
	/// </summary>
	public class ObjectPooler : ManagerSingleton<ObjectPooler>
	{
		#region Types

		[System.Serializable] public class IdPoolDictionary : SerializedDictionary<PoolIdEnum, PooledItem> { }
		[System.Serializable] public class NamedPoolDictionary : SerializedDictionary<string, PooledItem> { }

		[System.Serializable]
		public class PooledItem
		{
			[AssetsOnly, Required,
			ValidateInput("@Prefab != null && Prefab.GetComponent<IPoolableObject>() != null", "Prefab has no IPoolableObject implementation")]
			public GameObject Prefab;
			public int InitialCount;
			public List<IPoolableObject> Pool = new List<IPoolableObject>();
		}

		#endregion

		[SerializeField] private IdPoolDictionary _idPool = new IdPoolDictionary();
		[SerializeField] private NamedPoolDictionary _namedPool = new NamedPoolDictionary();

		private void Start() => SpawnPools();

		#region Editor access

		/// <summary>
		/// Custom editor call to add data to the PoolDictionary
		/// </summary>
		/// <param name="id"></param>
		/// <param name="prefab"></param>
		public static void AddToPool(PoolIdEnum id, GameObject prefab) => Instance._idPool.Add(id, new PooledItem() { Prefab = prefab });
		public static string[] GetPoolNames() => Instance._idPool.Where(x => x.Value.Prefab != null).Select(x => x.Value.Prefab.name).ToArray();

		#endregion

		#region Public access

		/// <summary>
		/// Releases an object to the pool.
		/// </summary>
		/// <param name="go"></param>
		public static void Release(IPoolableObject go)
		{
			if (!go.Released)
				go.Release();
		}

		/// <summary>
		/// Releases all objects to the pool
		/// </summary>
		public static void ReleaseAll()
		{
			Instance._idPool.ForEach(x => x.Value.Pool.ForEach(y => y.Release()));
			Instance._namedPool.ForEach(x => x.Value.Pool.ForEach(y => y.Release()));
		}

		/// <summary>
		/// Gets an object from the pool.
		/// </summary>
		/// <returns></returns>
		public static GameObject Get(PoolIdEnum poolId, object initializationData, Action<GameObject> onInititialized = null)
		{
			IPoolableObject poolable = Instance.GetObject(poolId);
			GameObject instance = poolable.Get();

			poolable.InitFromPool(initializationData);
			onInititialized?.Invoke(instance);
			return instance;
		}

		/// <summary>
		/// Gets an object from the pool.
		/// </summary>
		/// <returns></returns>
		public static GameObject Get(PoolIdEnum poolId, Vector3 position, Quaternion rotation, object initializationData, Action<GameObject> onInititialized = null)
		{
			IPoolableObject poolable = Instance.GetObject(poolId);
			GameObject instance = poolable.Get();

			instance.transform.position = position;
			instance.transform.rotation = rotation;
			poolable.InitFromPool(initializationData);
			onInititialized?.Invoke(instance);
			return instance;
		}

		/// <summary>
		/// Gets an object from the pool.
		/// </summary>
		/// <returns></returns>
		public static GameObject Get(GameObject poolPrefab, object initializationData, Action<GameObject> onInititialized = null)
		{
			IPoolableObject poolable = Instance.GetObject(poolPrefab);
			GameObject instance = poolable.Get();

			poolable.InitFromPool(initializationData);
			onInititialized?.Invoke(instance);
			return instance;
		}

		/// <summary>
		/// Gets an object from the pool.
		/// </summary>
		/// <returns></returns>
		public static GameObject Get(GameObject poolPrefab, Vector3 position, Quaternion rotation, object initializationData, Action<GameObject> onInititialized = null)
		{
			IPoolableObject poolable = Instance.GetObject(poolPrefab);
			GameObject instance = poolable.Get();

			instance.transform.position = position;
			instance.transform.rotation = rotation;
			poolable.InitFromPool(initializationData);
			onInititialized?.Invoke(instance);
			return instance;
		}

		#endregion

		#region Private access

		/// <summary>
		/// Pre instantiate pools according to the PoolDictionary.
		/// </summary>
		private void SpawnPools()
		{
			foreach (PooledItem item in _idPool.Values)
			{
				for (int i = 0; i < item.InitialCount; i++)
				{
					GameObject go = Instantiate(item.Prefab);

					go.name = item.Prefab.name;
					item.Pool.Add(go.GetComponent<IPoolableObject>());
					go.SetActive(false);
				}
			}

			foreach (PooledItem item in _namedPool.Values)
			{
				for (int i = 0; i < item.InitialCount; i++)
				{
					GameObject go = Instantiate(item.Prefab);

					go.name = item.Prefab.name;
					item.Pool.Add(go.GetComponent<IPoolableObject>());
					go.SetActive(false);
				}
			}
		}

		private IPoolableObject GetObject(PoolIdEnum poolId)
		{
			if (!_idPool.ContainsKey(poolId))
				Debug.LogError($"ObjectPooler - Requested id {poolId} not found within the pool.");

			PooledItem pooledItem = _idPool[poolId];
			IPoolableObject instance;

			foreach (IPoolableObject item in pooledItem.Pool)
			{
				if (item.Released)
					return item;
			}
			instance = Instantiate(pooledItem.Prefab).GetComponent<IPoolableObject>();
			pooledItem.Pool.Add(instance);
			return instance;
		}

		private IPoolableObject GetObject(GameObject poolObject)
		{
			if (!_namedPool.ContainsKey(poolObject.name))
				_namedPool.Add(poolObject.name, new PooledItem() { Prefab = poolObject, Pool = new() });

			PooledItem pooledItem = _namedPool[poolObject.name];
			IPoolableObject instance;

			foreach (IPoolableObject item in pooledItem.Pool)
			{
				if (item.Released)
					return item;
			}
			instance = Instantiate(pooledItem.Prefab).GetComponent<IPoolableObject>();
			pooledItem.Pool.Add(instance);
			return instance;
		}

		#endregion
	}
}