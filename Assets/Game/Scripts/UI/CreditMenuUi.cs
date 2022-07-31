using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class CreditMenuUi : AMenu
	{
		[Title("References")]
		[SerializeField] private Button _closeBtn;

		private Canvas _canvas;
		private MainMenuUi _main;

		protected override void Awake()
		{
			base.Awake();
			_canvas = GetComponentInParent<Canvas>();
			_main = _canvas.GetComponentInChildren<MainMenuUi>();
		}

		private void Start()
		{
			_closeBtn.onClick.AddListener(Close);
		}

		public override void Open()
		{
			base.Open();
			EventSystem.current.SetSelectedGameObject(_closeBtn.gameObject);
		}

		public override void Close()
		{
			base.Close();
			_main.Open();
		}
	}
}
