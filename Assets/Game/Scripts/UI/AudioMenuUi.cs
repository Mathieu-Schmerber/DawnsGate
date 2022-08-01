using Game.Tools;
using Nawlian.Lib.Extensions;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class AudioMenuUi : ACloseableMenu
	{
		[Title("References")]
		[SerializeField] private Button _closeBtn;

		private AudioSliderUI[] _sliders;
		private Canvas _canvas;
		private MainMenuUi _main;

		protected override void Awake()
		{
			base.Awake();
			_canvas = GetComponentInParent<Canvas>();
			_main = _canvas.GetComponentInChildren<MainMenuUi>();
			_sliders = GetComponentsInChildren<AudioSliderUI>();
		}

		private void Start()
		{
			_sliders.ForEach(x => x.Refresh());
			_closeBtn.onClick.AddListener(Close);
		}

		public override void Open()
		{
			base.Open();
			EventSystem.current.SetSelectedGameObject(_sliders[0].gameObject);
		}

		public override void Close()
		{
			base.Close();
			_main.Open();
		}
	}
}
