using Game.Managers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game
{
	public class GameCameraSimulation
	{
		[MenuItem("Tools/Game/Simulate game view")]
		public static void Simulate()
		{
			bool needBoot = true;
			var view = SceneView.lastActiveSceneView;

			if (view != null)
			{
				var boot = EditorSceneManager.GetSceneByPath(RunManager.RunSettings.BootScenePath);
				if (!boot.IsValid())
				{
					needBoot = false;
					boot = EditorSceneManager.OpenScene(RunManager.RunSettings.BootScenePath, OpenSceneMode.Additive);
				}

				var cam = GameManager.Camera.GetComponentInChildren<Camera>();

				view.orthographic = cam.orthographic;
				view.AlignViewToObject(cam.transform);
				view.Repaint();

				if (!needBoot)
					EditorSceneManager.UnloadSceneAsync(boot);
			}
		}
	}
}
