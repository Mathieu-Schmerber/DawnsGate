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
using UnityEngine.SceneManagement;

namespace Game.Systems.Run.Rooms
{
	public class RoomInfoData : SerializedScriptableObject
	{
		public string SceneName;
		[HideInInspector] public bool HasErrors;
		[ReadOnly] public NavMeshData NavMesh;
		[ReadOnly] public float GroundLevel;
		public List<Vector3> SpawnablePositions;
		[ReadOnly] public Vector3 RoomCenter;

#if UNITY_EDITOR

		private void OnValidate()
		{
			Bounds bounds = new();

			foreach (Vector3 item in SpawnablePositions)
				bounds.Encapsulate(item);
			RoomCenter = bounds.center;
		}

		public string ScenePath { 
			get
			{
				string[] guids = AssetDatabase.FindAssets($"{name} t:{typeof(RoomInfoData)}");

				for (int i = 0; i < guids.Length; i++)
				{
					string path = AssetDatabase.GUIDToAssetPath(guids[i]);
					RoomInfoData rid = AssetDatabase.LoadAssetAtPath<RoomInfoData>(path);
					if (rid.GetInstanceID() == this.GetInstanceID())
					{
						string folder = Path.GetDirectoryName(path);
						string scenePath = Directory.GetFiles(folder, "*.unity", SearchOption.TopDirectoryOnly)[0];

						return scenePath;
					}
				}
				return null;
			}
		}

		//[OnInspectorInit]
		public void UpdateSceneName()
		{
			string[] guids = AssetDatabase.FindAssets($"{name} t:{typeof(RoomInfoData)}");

			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				RoomInfoData rid = AssetDatabase.LoadAssetAtPath<RoomInfoData>(path);
				if (rid.GetInstanceID() == this.GetInstanceID())
				{
					string folder = Path.GetDirectoryName(path);
					string scenePath = Directory.GetFiles(folder, "*.unity", SearchOption.TopDirectoryOnly)[0];
					string scene = Path.GetFileNameWithoutExtension(scenePath);
					if (SceneName != scene)
						SceneName = scene;
				}
			}
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}

#endif
	}
}
