using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public class ScaleOnSelectUi : MonoBehaviour, ISelectHandler, IDeselectHandler
	{
		[SerializeField] private Vector3 _targetScale = new Vector3(1.1f, 1.1f, 1.1f);
		[SerializeField] private float _scaleDuration = .1f;
		private Vector3 _baseScale;

		private void Awake()
		{
			_baseScale = transform.localScale;
		}

		public void OnDeselect(BaseEventData eventData)
		{
			Tween.LocalScale(transform, _baseScale, _scaleDuration, 0, Tween.EaseOut);
		}

		public void OnSelect(BaseEventData eventData)
		{
			Tween.LocalScale(transform, _targetScale, _scaleDuration, 0, Tween.EaseOut);
		}
	}
}
