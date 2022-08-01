using Game.Managers;
using Nawlian.Lib.Utils;
using Pixelplacement;
using Plugins.Nawlian.Lib.Systems.Menuing;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class SceneTransition : AMenu
    {
		private Material _material;

		public override bool RequiresGameFocus => true;

		protected override void Awake()
		{
			base.Awake();
			_material = GetComponentInChildren<Image>().material;
		}

		private void Fade(float duration, bool toOpen)
		{
			float init = toOpen ? 0 : 1;
			float end = toOpen ? 1 : 0;

			Tween.Value(init, end, (value) => _material.SetFloat("_Progress", value), duration, 0);
		}

		public override void Open()
		{
			GuiManager.CloseAll();
			Fade(_duration, true);
			base.Open();
		}

		public override void Close()
		{
			GuiManager.OpenMenu<PlayerUi>();
			Fade(_duration, false);
			base.Close();
		}

#if UNITY_EDITOR

		public override void CloseEditorButton()
		{
			base.CloseEditorButton();
			GetComponentInChildren<Image>().material.SetFloat("_Progress", 0);
		}

		public override void OpenEditorButton()
		{
			base.OpenEditorButton();
			GetComponentInChildren<Image>().material.SetFloat("_Progress", 1);
		}

		private void OnValidate()
		{
			CloseEditorButton();
		}

#endif

		public static void StartTransition(Action onReady)
		{
			var menu = GuiManager.OpenMenu<SceneTransition>();
			Awaiter.WaitAndExecute(menu._duration, onReady);
		}

		public static void EndTransition(Action onReady)
		{
			var menu = GuiManager.CloseMenu<SceneTransition>();
			Awaiter.WaitAndExecute(menu._duration, onReady);
		}
	}
}
