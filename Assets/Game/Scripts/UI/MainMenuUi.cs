using Game.Managers;
using Nawlian.Lib.Systems.Saving;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
	public class MainMenuUi : AMenu
	{
		[Title("References")]
		[SerializeField] private Button _startBtn;
		[SerializeField] private Button _audioBtn;
		[SerializeField] private Button _creditBtn;
		[SerializeField] private Button _quitBtn;

		private Canvas _canvas;
		private AudioMenuUi _audio;
		private CreditMenuUi _credit;

		protected override void Awake()
		{
			base.Awake();
			_canvas = GetComponentInParent<Canvas>();
			_audio = _canvas.GetComponentInChildren<AudioMenuUi>();
			_credit = _canvas.GetComponentInChildren<CreditMenuUi>();
		}

		private void Start()
		{
			EventSystem.current.SetSelectedGameObject(_startBtn.gameObject);
			_startBtn.onClick.AddListener(StartClicked);
			_audioBtn.onClick.AddListener(AudioClicked);
			_creditBtn.onClick.AddListener(CreditClicked);
			_quitBtn.onClick.AddListener(QuitGame);
		}

		private void StartClicked()
		{
			GameManager.StartGame();
			//SceneManager.LoadScene(RunManager.RunSettings.LobbySceneName); 
			//SceneManager.LoadScene("_Boot", LoadSceneMode.Additive);
			//SceneManager.LoadScene("_UI", LoadSceneMode.Additive);
		}

		private void AudioClicked()
		{
			Close();
			_audio.Open();
		}

		private void CreditClicked()
		{
			Close();
			_credit.Open();
		}

		private void QuitGame()
		{
			Application.Quit();
		}

		public override void Open()
		{
			base.Open();
			EventSystem.current.SetSelectedGameObject(_startBtn.gameObject);
		}
	}
}
