using Game.Entities.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
    public class SpottedEffect : MonoBehaviour
    {
		[SerializeField] private Material _patrolMaterial;
		[SerializeField] private Material _chaseMaterial;
		private Renderer _renderer;
		private EnemyAI _ai;

		private void Awake()
		{
			_ai = GetComponentInParent<EnemyAI>();
			_renderer = GetComponent<Renderer>();
			_ai_OnStateChanged(EnemyState.PASSIVE);
		}

		private void OnEnable()
		{
			_ai.OnStateChanged += _ai_OnStateChanged;
		}

		private void OnDisable()
		{
			_ai.OnStateChanged -= _ai_OnStateChanged;
		}

		private void _ai_OnStateChanged(EnemyState obj)
		{
			_renderer.material = obj == EnemyState.PASSIVE ? _patrolMaterial : _chaseMaterial;
		}
	}
}
