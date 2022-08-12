using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
	public static class Bootstrapper
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Execute()
		{
			if (SceneManager.GetActiveScene().name == "Trailer")
				return;

			var boot = SceneManager.GetSceneByName("_Boot");
			if (!boot.IsValid())
				SceneManager.LoadScene("_Boot", LoadSceneMode.Additive);

			var ui = SceneManager.GetSceneByName("_UI");
			if (!ui.IsValid())
				SceneManager.LoadScene("_UI", LoadSceneMode.Additive);
		}
	}
}
