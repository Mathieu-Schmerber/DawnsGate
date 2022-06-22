using Nawlian.Lib.Systems.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.VFX
{
	public class BloodChainFX : APoolableObject
	{
		[SerializeField] private float _lifetime;
		private LineRenderer _lr;

		public Transform Origin { get; set; }
		public Transform Destination { get; set; }

		private void Awake()
		{
			_lr = GetComponent<LineRenderer>();
		}

		public override void Init(object data)
		{
			Invoke(nameof(Release), _lifetime);
		}

		private void Update()
		{
			_lr.SetPosition(0, Origin.position);
			_lr.SetPosition(1, Destination.position);
		}
	}
}
