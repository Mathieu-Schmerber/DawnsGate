using Nawlian.Lib.Utils;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

namespace Game.Managers
{
	public class GuiManager : ManagerSingleton<GuiManager>
	{
		[SerializeField] private Canvas _canvas;

		private IMenu[] _menus;

		public static event Action<IMenu> OnMenuOpened;
		public static event Action<IMenu> OnMenuClosed;

		[ShowInInspector, ReadOnly] public static bool IsMenuing => Instance?._menus?.Any(x => x.IsOpen && x.RequiresGameFocus) ?? false;

		private void Awake()
		{
			_menus = _canvas.GetComponentsInChildren<IMenu>();
		}

		public static void OpenMenu<T>() where T : IMenu
		{
			IMenu menu = Instance._menus.FirstOrDefault(x => x is T);

			if (menu == null)
				return;
			menu.Open();
			OnMenuOpened?.Invoke(menu);
		}

		public static void CloseMenu<T>() where T : IMenu
		{
			IMenu menu = Instance._menus.FirstOrDefault(x => x is T);

			if (menu == null)
				return;
			menu.Close();
			OnMenuClosed?.Invoke(menu);
		}
	}
}
