using Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Player
{
	public class Footstep : MonoBehaviour
	{
		private ParticleSystem _ps;
		private Animator _animator;
		[SerializeField] private float _minSpeedToPlay;
		[SerializeField] private float _maxSpeedToPlay;

		private void Awake()
		{
			_ps = GetComponent<ParticleSystem>();
			_animator = GameManager.Player.GetComponentInChildren<Animator>();
		}

		private void OnTriggerEnter(Collider other)
		{
			float speed = _animator.GetFloat("Speed");

			if (speed >= _minSpeedToPlay && speed <= _maxSpeedToPlay)
				_ps.Play(true);
		}
	}
}