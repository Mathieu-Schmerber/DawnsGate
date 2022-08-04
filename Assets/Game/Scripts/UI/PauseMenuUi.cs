using Game.Managers;
using Game.Tools;
using Pixelplacement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
	public class PauseMenuUi : ACloseableMenu
	{
		[Title("References")]
		[SerializeField] private RectTransform _moveableMenuPart;
		[SerializeField] private Button _resumeBtn;
		[SerializeField] private Button _audioBtn;
		[SerializeField] private Button _menuBtn;

		#region Unity builtins

		protected override void Awake()
		{
			base.Awake();
			_rect = _moveableMenuPart;
		}

		private void Start()
		{
			_resumeBtn.onClick.AddListener(ResumeBtn);
			_audioBtn.onClick.AddListener(AudioBtn);
			_menuBtn.onClick.AddListener(MenuBtn);
		}

		private void OnEnable()
		{
			InputManager.OnEscapePressed += OnCancel;
		}

		private void OnDisable()
		{
			InputManager.OnEscapePressed -= OnCancel;
		}

		#endregion

		#region Buttons

		private void ResumeBtn() => Close();
		private void AudioBtn() => GuiManager.OpenMenu<PauseMenuAudioUi>();
		private void MenuBtn() => GameManager.StopGame();

		#endregion

		#region Open / Close

		protected override void OnCancel()
		{
			if (!_isOpen)
				Open();
			else if (GuiManager.Get<PauseMenuAudioUi>().IsOpen)
			{
				GuiManager.CloseMenu<PauseMenuAudioUi>();
				EventSystem.current.SetSelectedGameObject(_resumeBtn.gameObject);
			}
			else if (IsOpen)
				Close();
		}

		public override void Open()
		{
			EventSystem.current.SetSelectedGameObject(_resumeBtn.gameObject);
			Show();
			PlayOpenSound();
			_isOpen = true;
		}

		public override void Close()
		{
			Hide();
			_isHidden = false;
			_isOpen = false;
			PlayCloseSound();
			GuiManager.CloseMenu<PauseMenuAudioUi>();
			EventSystem.current.SetSelectedGameObject(null);
		}

		#endregion

		#region Editor

#if UNITY_EDITOR

		public override void OpenEditorButton()
		{
			_isOpen = true;
			_grp = GetComponent<CanvasGroup>();
			if (_grp != null)
				_grp.alpha = 1;
		}

		public override void CloseEditorButton()
		{
			_isOpen = false;
			_grp = GetComponent<CanvasGroup>();
			if (_grp != null)
				_grp.alpha = 0;
		}

#endif

		#endregion
	}
}