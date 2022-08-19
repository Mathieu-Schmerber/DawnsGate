using Game.Managers;
using Game.Systems.Run.GPE;
using Nawlian.Lib.Extensions;
using Sirenix.OdinInspector;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor.SceneManagement;
using Game.Scripts.Tools;
#endif

namespace Game.Systems.Run.Rooms
{
	/// <summary>
	/// Handles room validity
	/// </summary>
	public class RoomInfo : MonoBehaviour
	{
		[Space]
		[Title("General")]
		[SerializeField, OnValueChanged("OnTypeChanged")] private RoomType _roomType;
		[SerializeField] private Transform _playerSpawn;
		[SerializeField] private RoomDoor[] _doors;

		[Title("Navigation")]
		[SerializeField, ShowIf(nameof(_requiresNav))] private bool _drawGizmos = true;
		[ReadOnly, ShowIf(nameof(_requiresNav))] public RoomInfoData Data;

		private Bounds _sceneBounds;
		private ARoom _room;

		private bool _requiresNav => GetRoom()?.RequiresNavBaking ?? false;
		public RoomDoor[] Doors => _doors;
		public RoomType Type { get => _roomType; set 
			{ 
				_roomType = value;
			} 
		}

		#region Editor Tools

		private ARoom GetRoom() => _room ?? GetComponent<ARoom>();

#if UNITY_EDITOR

		private void OnTypeChanged()
		{
			_room = GetRoom();
			if (_room != null)
				DestroyImmediate(_room);

			switch (Type)
			{
				case RoomType.COMBAT:
					_room = gameObject.AddComponent<CombatRoom>();
					break;
				case RoomType.SHOP:
				case RoomType.LIFE_SHOP:
					_room = gameObject.AddComponent<ShopRoom>();
					break;
				default:
					_room = gameObject.AddComponent<AutoClearRoom>();
					break;
			}
		}

		[Button, ShowIf(nameof(_requiresNav))]
		private void BakeNavMesh()
		{
			NavMeshHit hit;
			float lowerGround = float.MaxValue;

			_sceneBounds = new Bounds(Vector3.zero, Vector3.zero);

			BakeNavigationMesh();

			foreach (Renderer r in FindObjectsOfType<Renderer>())
				_sceneBounds.Encapsulate(r.bounds);

			Data.SpawnablePositions = new();
			for (int x = 0; x < _sceneBounds.size.x; x += 2)
			{
				for (int z = 0; z < _sceneBounds.size.z; z += 2)
				{
					Vector3 position = new Vector3(_sceneBounds.min.x + x, _sceneBounds.max.y, _sceneBounds.min.z + z);

					if (Data.SpawnablePositions.Contains(position))
						continue;
					else if (NavMesh.SamplePosition(position, out hit, 1000, 1) && Vector3.Distance(hit.position.WithY(position.y), position) == 0)
					{
						if (lowerGround > hit.position.y)
							lowerGround = hit.position.y;
						Data.SpawnablePositions.Add(hit.position);
					}
				}
			}
			Data.SpawnablePositions.RemoveAll(pos => pos.y > lowerGround);
			Data.GroundLevel = lowerGround;
			_drawGizmos = true;
			EditorUtility.SetDirty(Data);
			AssetDatabase.SaveAssets();
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		}

		private void BakeNavigationMesh()
		{
			string sceneFolder = Directory.GetParent(gameObject.scene.path).FullName;
			string navmeshFolder = Path.Combine(sceneFolder, Path.GetFileNameWithoutExtension(gameObject.scene.path));

			// Bake NavMesh
			UnityEditor.AI.NavMeshBuilder.BuildNavMesh();

			if (!Directory.Exists(navmeshFolder))
			{
				Debug.LogError("[RoomInfo] The NavMesh couldn't be baked, ensure that your walkable surfaces are large enough.");
				Data.NavMesh = null;
				return;
			}

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
			RoomInfoData asset = ScriptableObject.CreateInstance<RoomInfoData>();
			string soPath = Path.Combine(sceneFolder, $"{Path.GetFileNameWithoutExtension(sceneFolder)}.asset").Substring(Application.dataPath.Length);
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

			Data = asset;
			Data.NavMesh = AssetDatabase.LoadAssetAtPath<NavMeshData>($"Assets{assetPath.Substring(Application.dataPath.Length)}");
			Debug.Log("[RoomInfo] NavMesh successfully baked !");
		}

		public void OnSceneSaved()
		{
			bool state = GetError() != null;

			if (Data != null && state != Data.HasErrors)
			{
				Data.HasErrors = state;
				EditorUtility.SetDirty(Data);
				AssetDatabase.SaveAssets();
				RunManager.RunSettings.RoomFolders.Refresh();
			}
		}

		public string GetError()
		{
			if (_requiresNav &&
				FindObjectsOfType<GameObject>().Count(x => x.isStatic && x.GetComponent<Collider>() != null && x.GetComponentInParent<RoomInfo>() == null) == 0)
			{
				Data?.SpawnablePositions.Clear();
				return "No walkalble surface found. Walkable surfaces are defined by making objects static.";
			}
			if (_requiresNav && (Data == null || Data.SpawnablePositions.Count == 0 || Data.NavMesh == null || !NavMesh.SamplePosition(Vector3.zero, out var hit, 1000.0f, 1)))
				return "The room navmesh needs to be baked.";
			return null;
		}

		[OnInspectorGUI, PropertyOrder(int.MinValue)]
		private void DisplayErrorBox()
		{
			string error = GetError();

			if (error == null)
			{
				SuccessMessageBox.Show("Everything is OK !");
				return;
			}
			SirenixEditorGUI.ErrorMessageBox(error);
		}

#endif

		#endregion

		public Vector3[] GetPositionsAround(Vector3 center, float radius) => Data.SpawnablePositions.Where(pos => pos != center && Vector3.Distance(center, pos) <= radius).ToArray();

		private void OnDrawGizmos()
		{
			if (!_requiresNav || !_drawGizmos)
				return;

			Gizmos.color = Color.gray;
			Gizmos.DrawWireCube(_sceneBounds.center, _sceneBounds.size);

			if (Data == null || Data.SpawnablePositions == null || Data.SpawnablePositions.Count == 0)
				return;

			Gizmos.color = Color.red;
			foreach (var item in Data.SpawnablePositions)
				Gizmos.DrawSphere(item, 0.1f);

			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(Data.RoomCenter, 0.2f);
		}
	}
}
