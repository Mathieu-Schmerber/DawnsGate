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

		private Dictionary<Renderer, Material[]> _baseMaterials = new();
		private Damageable _damageable;

		private void OnEnable()
		{
			RestoreMaterials();
			_damageable.OnDamaged += OnDamageDealt;
		}

		private void OnDisable()
		{
			_damageable.OnDamaged -= OnDamageDealt;
		}

		private void Awake()
		{
			_damageable = GetComponent<Damageable>();
			CacheRenderers();
		}

		private void CacheRenderers()
		{
			foreach (Renderer item in GetComponentsInChildren<Renderer>())
			{
				if (item is ParticleSystemRenderer || item is UnityEngine.UI.ICanvasElement)
					continue;
				_baseMaterials.Add(item, item.materials);
			}
		}

		private void RestoreMaterials()
		{
			foreach (Renderer item in _baseMaterials.Keys)
				item.materials = _baseMaterials[item];
		}

		private void AffectHighlight()
		{
			foreach (Renderer item in _baseMaterials.Keys)
			{
				Material[] highlight = item.materials;
				for (int i = 0; i < highlight.Length; i++)
					highlight[i] = _highlight;
				item.materials = highlight;
			}
		}

		private IEnumerator Highlight()
		{
			AffectHighlight();
			yield return new WaitForSeconds(_highlightTime);
			RestoreMaterials();
		}

		public void OnDamageDealt(float damage)
		{
			if (!_damageable.IsDead)
				StartCoroutine(Highlight());
		}
	}
}