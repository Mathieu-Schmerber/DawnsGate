using Game.UI;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Utils;
using Pixelplacement;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Managers
{
	public class GuiManager : ManagerSingleton<GuiManager>
	{
		[SerializeField] private Canvas _mainCanvas;
		[SerializeField] private Canvas _sceneTransitionCanvas;

		private CanvasGroup _group;
		private Dictionary<Type, IMenu> _menus = new();
		private InventorySlotSelector _inventoryUi;

		[ShowInInspector, ReadOnly] public static bool IsMenuing => Instance?._menus?.Any(x => x.Value.IsOpen && x.Value.RequiresGameFocus) ?? false;
		public static InventorySlotSelector InventoryUI => Instance._inventoryUi;

		private void Awake()
		{
			_mainCanvas.GetComponentsInChildren<IMenu>().ForEach(x => _menus.Add(x.GetType(), x));
			_sceneTransitionCanvas.GetComponentsInChildren<IMenu>().ForEach(x => _menus.Add(x.GetType(), x));
			_inventoryUi = _mainCanvas.GetComponentInChildren<InventorySlotSelector>();
			_group = _mainCanvas.GetComponent<CanvasGroup>();
		}

		public static T Get<T>() where T : IMenu
		{
			IMenu menu = Instance._menus[typeof(T)];

			if (menu == null)
				return default(T);
			return (T)menu;
		}

		public static T OpenMenu<T>() where T : IMenu
		{
			IMenu menu = Instance._menus[typeof(T)];

			if (menu == null)
				return default(T);
			if (!menu.IsOpen)
				menu.Open();
			return (T)menu;
		}

		public static T CloseMenu<T>() where T : IMenu
		{
			IMenu menu = Instance._menus[typeof(T)];

			if (menu == null)
				return default(T);
			if (menu.IsOpen)
				menu.Close();
			return (T)menu;
		}

		public static void CloseAll()
		{
			Instance._menus.ForEach(x => { 	
				if (x.Value.IsOpen)
					x.Value.Close(); 
			});
		}

		public static void Hide(float duration)
		{
			Instance._group.interactable = false;
			Tween.CanvasGroupAlpha(Instance._group, 0, duration, 0);
		}

		public static void Show(float duration)
		{
			Instance._group.interactable = true;
			Tween.CanvasGroupAlpha(Instance._group, 1, duration, 0);
		}
	}
}
