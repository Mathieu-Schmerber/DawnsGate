using Nawlian.Lib.Utils;
using Pixelplacement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.Nawlian.Lib.Systems.Menuing
{
	public abstract class AMenu : MonoBehaviour, IMenu
	{
		protected bool _isOpen;
		protected bool _isHidden;
		protected CanvasGroup _grp;
		protected RectTransform _rect;

		[Title("Animation")]
		[SerializeField] protected float _duration;
		[SerializeField] protected Vector2 _openPosition;
		[SerializeField] protected Vector2 _closePosition;

		[Title("Audio")]
		[SerializeField] protected AudioClip _openAudio;
		[SerializeField] protected bool _reverseClipForClose;
		[SerializeField, ShowIf("@_reverseClipForClose == false")] protected AudioClip _closeAudio;
		protected AudioSource _source;

		public bool IsOpen => _isOpen;
		public bool IsHidden => _isHidden;

		public virtual bool RequiresGameFocus => true;

		protected virtual void Awake()
		{
			_grp = GetComponent<CanvasGroup>();
			_rect = GetComponent<RectTransform>();
			_source = GetComponent<AudioSource>();
		}

		protected virtual void PlayOpenSound()
		{
			if (_source)
			{
				_source.pitch = 1;
				if (_openAudio)
					_source.PlayOneShot(_openAudio);
			}
		}

		protected virtual void PlayCloseSound()
		{
			if (_source != null)
			{
				if (_reverseClipForClose == false && _closeAudio != null)
					_source.PlayOneShot(_closeAudio);
				else
				{
					if (_openAudio == null)
						return;
					_source.clip = _openAudio;
					_source.pitch = -1;
					_source.time = _openAudio.length - 0.01f;
					_source.Play();
				}
			}
		}

		protected virtual void ExecuteHideMovement()
		{
			if (_rect != null)
				Tween.AnchoredPosition(_rect, _openPosition, _closePosition, _duration, 0, Tween.EaseIn);
			if (_grp != null)
			{
				Tween.CanvasGroupAlpha(_grp, 1, 0, _duration, 0, Tween.EaseIn);
				_grp.interactable = false;
				_grp.blocksRaycasts = false;
			}
		}

		protected virtual void ExecuteShowMovement()
		{
			if (_rect != null)
				Tween.AnchoredPosition(_rect, _closePosition, _openPosition, _duration, 0, Tween.EaseOut);
			if (_grp != null)
			{
				Tween.CanvasGroupAlpha(_grp, 0, 1, _duration, 0, Tween.EaseOut);
				_grp.interactable = true;
				_grp.blocksRaycasts = true;
			}
		}

		public virtual void Close()
		{
			Hide();
			_isOpen = false;
			_isHidden = false;
			PlayCloseSound();
		}

		public virtual void Open()
		{
			Show();
			PlayOpenSound();
			_isOpen = true;
		}

		public virtual void Hide()
		{
			_isHidden = true;
			ExecuteHideMovement();
		}

		public virtual void Show()
		{
			_isHidden = false;
			ExecuteShowMovement();
		}

#if UNITY_EDITOR

		private void OnValidate()
		{
			if ((_openAudio || _closeAudio) && GetComponent<AudioSource>() == null)
				gameObject.AddComponent<AudioSource>();
		}

		[Button("Open")]
		public virtual void OpenEditorButton()
		{
			_isOpen = true;
			_rect = GetComponent<RectTransform>();
			_grp = GetComponent<CanvasGroup>();
			_grp.blocksRaycasts = true;
			_grp.interactable = true;
			if (_rect != null)
				_rect.anchoredPosition = _openPosition;
			if (_grp != null)
				_grp.alpha = 1;
		}

		[Button("Close")]
		public virtual void CloseEditorButton()
		{
			_isOpen = false;
			_rect = GetComponent<RectTransform>();
			_grp = GetComponent<CanvasGroup>();
			_grp.blocksRaycasts = false;
			_grp.interactable = false;
			if (_rect != null)
				_rect.anchoredPosition = _closePosition;
			if (_grp != null)
				_grp.alpha = 0;
		}

#endif
	}
}
