using Game.Entities.Shared.Health;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
	public class HighlightOnDamage : MonoBehaviour
	{
		[SerializeField] private Material _highlight;
		[SerializeField] private float _highlightTime;

		private Material[] _baseMaterials;
		private Renderer _renderer;
		private Damageable _damageable;

		private void OnEnable()
		{
			_renderer.materials = _baseMaterials;
			_damageable.OnDamaged += OnDamageDealt;
		}

		private void OnDisable()
		{
			_damageable.OnDamaged -= OnDamageDealt;
		}

		private void Awake()
		{
			_renderer = GetComponentInChildren<Renderer>();
			_damageable = GetComponent<Damageable>();
			_baseMaterials = _renderer.materials;
		}

		private IEnumerator Highlight()
		{
			Material[] highlightMaterials = _renderer.materials;
			for (int i = 0; i < highlightMaterials.Length; i++)
				highlightMaterials[i] = _highlight;
			_renderer.materials = highlightMaterials;
			yield return new WaitForSeconds(_highlightTime);
			_renderer.materials = _baseMaterials;
		}

		public void OnDamageDealt(float damage)
		{
			if (!_damageable.IsDead)
				StartCoroutine(Highlight());
		}
	}
}