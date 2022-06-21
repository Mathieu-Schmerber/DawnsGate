using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.VFX
{
	public class FloatingEffect : MonoBehaviour
	{
		[SerializeField] private float _amplitude;
		private float customTime;

		private void Start()
		{
			customTime = UnityEngine.Random.Range(0, 10);
		}

		private void Update()
		{
			transform.position += Vector3.up * Mathf.Sin(Time.time + customTime) * (_amplitude / 100);
		}
	}
}
