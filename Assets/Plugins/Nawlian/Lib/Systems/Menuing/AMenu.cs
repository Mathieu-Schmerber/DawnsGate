using Pixelplacement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.Nawlian.Lib.Systems.Menuing
{
	public abstract class AMenu : MonoBehaviour, IMenu
	{
		protected bool _isOpen;
		protected CanvasGroup _grp;
		protected RectTransform _rect;

		[Title("Animation")]
		[SerializeField] protected float _duration;
		[SerializeField] protected Vector2 _openPosition;
		[SerializeField] protected Vector2 _closePosition;

		public bool IsOpen => _isOpen;

		public virtual bool RequiresGameFocus => true;

		protected virtual void Awake()
		{
			_grp = GetComponent<CanvasGroup>();
			_rect = GetComponent<RectTransform>();
		}

		public virtual void Close()
		{
			_isOpen = false;
			if (_rect != null)
				Tween.AnchoredPosition(_rect, _openPosition, _closePosition, _duration, 0, Tween.EaseIn);
			if (_grp != null)
				Tween.CanvasGroupAlpha(_grp, 1, 0, _duration, 0, Tween.EaseIn);
		}

		public virtual void Open()
		{
			_isOpen = true;
			if (_rect != null)
				Tween.AnchoredPosition(_rect, _closePosition, _openPosition, _duration, 0, Tween.EaseOut);
			if (_grp != null)
				Tween.CanvasGroupAlpha(_grp, 0, 1, _duration, 0, Tween.EaseOut);
		}

#if UNITY_EDITOR

		[Button("Open")]
		public virtual void OpenEditorButton()
		{
			_isOpen = true;
			_rect = GetComponent<RectTransform>();
			_grp = GetComponent<CanvasGroup>();
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
			if (_rect != null)
				_rect.anchoredPosition = _closePosition;
			if (_grp != null)
				_grp.alpha = 0;
		}

#endif
	}
}
