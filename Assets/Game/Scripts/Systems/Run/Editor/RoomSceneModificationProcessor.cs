using Game.Systems.Run.Rooms;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Systems.Run.Editor
{
    public class RoomSceneModificationProcessor : AssetModificationProcessor
    {
        public static string[] OnWillSaveAssets(string[] paths)
        {
            string scenePath = string.Empty;

            foreach (string path in paths)
            {
                if (path.Contains(".unity"))
                    scenePath = path;
            }

            if (scenePath.Length == 0)
                return paths;

            Scene scene = SceneManager.GetSceneByPath(scenePath);
            
            if (scene.IsValid())
            {
                GameObject roomLogic = scene.GetRootGameObjects().FirstOrDefault(x => x.name == Databases.Database.Templates.Editor.RoomLogic.name);

                if (roomLogic != null)
                    roomLogic.GetComponent<RoomInfo>()?.OnSceneSaved();
            }
            return paths;
        }
    }
}