using Game.Managers;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Systems.Run.Rooms
{
	/// <summary>
	/// Handles room validity
	/// </summary>
	public class RoomInfo : MonoBehaviour
	{
		[SerializeField, OnValueChanged(nameof(OnTypeChanged))] private RoomType _roomType;
		[SerializeField] private Transform _playerSpawn;
		[SerializeField] private RoomDoor[] _doors;
		[SerializeField, ReadOnly] private RoomNavigationData _navigation;

		private Bounds _sceneBounds;
		private ARoom _room;

		public RoomNavigationData NavigationData => _navigation;
		public RoomDoor[] Doors => _doors;
		public RoomType Type { get => _roomType; set 
			{ 
				_roomType = value;
				OnTypeChanged();
			} 
		}

		#region Editor Tools

#if UNITY_EDITOR
		private void OnTypeChanged()
		{
			if (_room != null)
				Destroy(_room);

			switch (Type)
			{
				case RoomType.COMBAT:
					_room = gameObject.AddComponent<CombatRoom>();
					break;
				case RoomType.EVENT:
					break;
				case RoomType.SHOP:
					break;
				case RoomType.LIFE_SHOP:
					break;
				case RoomType.UPGRADE:
					break;
				case RoomType.BOSS:
					break;
				default:
					break;
			}
		}

		[Button]
		private void BakeNavMesh()
		{
			NavMeshHit hit;
			_sceneBounds = new Bounds(Vector3.zero, Vector3.zero);

			BakeNavigationMesh();

			foreach (Renderer r in FindObjectsOfType<Renderer>())
				_sceneBounds.Encapsulate(r.bounds);

			_navigation.SpawnablePositions = new();
			for (int x = 0; x < _sceneBounds.size.x; x += 2)
			{
				for (int z = 0; z < _sceneBounds.size.z; z += 2)
				{
					Vector3 position = new Vector3(_sceneBounds.min.x + x, _sceneBounds.max.y, _sceneBounds.min.z + z);

					if (_navigation.SpawnablePositions.Contains(position))
						continue;
					else if (NavMesh.SamplePosition(position, out hit, 1000, 1))
						_navigation.SpawnablePositions.Add(hit.position);
				}
			}
			EditorUtility.SetDirty(_navigation);
			AssetDatabase.SaveAssets();
		}

		private void BakeNavigationMesh()
		{
			string sceneFolder = Directory.GetParent(gameObject.scene.path).FullName;
			string navmeshFolder = Path.Combine(sceneFolder, Path.GetFileNameWithoutExtension(gameObject.scene.path));


			// Bake NavMesh
			UnityEditor.AI.NavMeshBuilder.BuildNavMesh();

			// Move navmesh data to scene folder
			string assetPath = Path.Combine(sceneFolder, "NavMesh.asset");
			string metaPath = Path.Combine(sceneFolder, "NavMesh.asset.meta");

			if (File.Exists(assetPath))
				File.Delete(assetPath);
			if (File.Exists(metaPath))
				File.Delete(metaPath);
			File.Move(Path.Combine(navmeshFolder, "NavMesh.asset"), assetPath);
			File.Move(Path.Combine(navmeshFolder, "NavMesh.asset.meta"), metaPath);

			// Delete old folder
			Directory.Delete(navmeshFolder, true);
			File.Delete($"{navmeshFolder}.meta");

			// Create RoomNavigationData asset
			RoomNavigationData asset = ScriptableObject.CreateInstance<RoomNavigationData>();
			string soPath = Path.Combine(sceneFolder, "RoomNavigationData.asset").Substring(Application.dataPath.Length);
			string assetRelativePath = $"Assets{soPath}";

			if (File.Exists(soPath))
				File.Delete(soPath);
			AssetDatabase.CreateAsset(asset, assetRelativePath);
			AssetDatabase.SaveAssets();

			// Refresh project directory
			AssetDatabase.Refresh();

			// Unpack prefab, so the _navigation doesn't get overwritten
			if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
				PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

			_navigation = asset;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.gray;
			Gizmos.DrawWireCube(_sceneBounds.center, _sceneBounds.size);

			if (_navigation == null || _navigation.SpawnablePositions == null || _navigation.SpawnablePositions.Count == 0)
				return;

			Gizmos.color = Color.red;
			foreach (var item in _navigation.SpawnablePositions)
				Gizmos.DrawWireSphere(item, 0.5f);
		}

#endif

		#endregion
	}
}
