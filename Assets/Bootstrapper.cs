using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
	public static class Bootstrapper
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Execute()
		{
			var boot = SceneManager.GetSceneByName("_Boot");

			if (!boot.IsValid())
				SceneManager.LoadScene("_Boot", LoadSceneMode.Additive);
		}
	}
}
