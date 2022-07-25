using Game.Managers;
using UnityEditor;
using UnityEngine;

namespace Game
{
	public class GameCameraSimulation
	{
		[MenuItem("Tools/Game/Simulate game view")]
		public static void Simulate()
		{
			var view = SceneView.lastActiveSceneView;

			if (view != null)
			{
				var cam = GameManager.Camera.GetComponentInChildren<Camera>();

				view.orthographic = cam.orthographic;
				view.AlignViewToObject(cam.transform);
				view.Repaint();
			}
		}
	}
}
