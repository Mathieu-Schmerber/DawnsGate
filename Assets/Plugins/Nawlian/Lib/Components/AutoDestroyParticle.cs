using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using Sirenix.OdinInspector;

namespace Nawlian.Lib.Components
{
	/// <summary>
	/// Automatically destroys this gameobject after every particle system have been played.
	/// </summary>
	public class AutoDestroyParticle : MonoBehaviour
	{
		public enum Mode
		{
			POOL_RELEASE,
			DESTROY
		}

		[ValidateInput(nameof(Validate), "Missing an IPoolableObject component.")]
		[SerializeField] private Mode _destructionMode;

		private List<ParticleSystem> _ps;

		private void Awake()
		{
			_ps = transform.GetComponentsInChildren<ParticleSystem>(includeThis: true).ToList();
		}

		private void OnEnable()
		{
			float time = _ps.Max(x => x.main.startLifetime.constantMax) + _ps.Max(x => x.main.startDelay.constantMax);

			switch (_destructionMode)
			{
				case Mode.POOL_RELEASE:
					Invoke(nameof(Release), time);
					break;
				case Mode.DESTROY:
					Destroy(gameObject, time);
					break;
			}
		}

		private bool Validate() 
			=> _destructionMode == Mode.DESTROY || (_destructionMode == Mode.POOL_RELEASE && GetComponent<IPoolableObject>() != null);

		private void Release()
		{
			if (!Validate())
				Debug.LogError($"{gameObject}.AutoDestroyParticle is declared POOL_RELEASE but has no IPoolableObject component.");
			else
				GetComponent<IPoolableObject>().Release();
		}
	}
}
