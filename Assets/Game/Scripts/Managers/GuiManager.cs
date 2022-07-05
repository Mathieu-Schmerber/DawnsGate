using Game.UI;
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
		private InventorySlotSelector _inventoryUi;

		[ShowInInspector, ReadOnly] public static bool IsMenuing => Instance?._menus?.Any(x => x.IsOpen && x.RequiresGameFocus) ?? false;
		public static InventorySlotSelector InventoryUI => Instance._inventoryUi;

		private void Awake()
		{
			_menus = _canvas.GetComponentsInChildren<IMenu>();
			_inventoryUi = _canvas.GetComponentInChildren<InventorySlotSelector>();
		}

		public static T OpenMenu<T>() where T : IMenu
		{
			IMenu menu = Instance._menus.FirstOrDefault(x => x is T);

			if (menu == null)
				return default(T);
			menu.Open();
			return (T)menu;
		}

		public static void CloseMenu<T>() where T : IMenu
		{
			IMenu menu = Instance._menus.FirstOrDefault(x => x is T);

			if (menu == null)
				return;
			menu.Close();
		}
	}
}
