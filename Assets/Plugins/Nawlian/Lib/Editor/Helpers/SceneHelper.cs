using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nawlian.Lib.EditorTools.Helpers
{
	public static class SceneHelper
	{
		public struct SceneInfo
		{
			public string Name;
			public string Path;
			public int BuildIndex;
			public bool IsOpen => Scene.IsValid();
			public Scene Scene;

			public SceneInfo(string name, string path, Scene scene)
			{
				Scene = scene;
				Name = name;
				Path = path;
				Scene = scene;
				BuildIndex = scene.IsValid() ? scene.buildIndex : -1;
			}
		} 

		public static SceneInfo[] GetAllSceneInProject()
		{
			string[] pathes = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);

			return pathes.Select(x => GetSceneAtPath(x)).ToArray();
		}

		public static SceneInfo[] GetAllBuiltScene() => EditorBuildSettings.scenes.Select(x => GetSceneAtPath(x.path)).ToArray();

		public static SceneInfo GetSceneAtPath(string path)
		{
			var scene = EditorSceneManager.GetSceneByPath(path);

			return new SceneInfo(Path.GetFileNameWithoutExtension(path), path, scene);
		}
	}
}