using Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RouletteFX : MonoBehaviour
    {
        [SerializeField] private AnimationClip _animation;
        private Animator _animator;
		[SerializeField] private Vector3 _screenShakeIntensity;
		[SerializeField] private float _screenShakeDuration;
		public float _totalAnimationTime;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

        public void ScreenShake() => GameManager.Camera.Shake(_screenShakeIntensity, _screenShakeDuration);

		public void Play()
		{
            _animator.Play(_animation.name);
        }
    }
}
