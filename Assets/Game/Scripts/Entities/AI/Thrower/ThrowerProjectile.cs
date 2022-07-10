using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using Pixelplacement;
using UnityEngine;

namespace Game.Entities.AI.Thrower
{
	[System.Serializable]
	public struct ProjectileParameters
	{
		public Vector3 Destination { get; set; }
		public float Lifetime { get; set; }
		public float MaxAltitude { get; set; }
	}

	public class ThrowerProjectile : APoolableObject
	{
		private ProjectileParameters _params;
		private float _speed;
		private float _spawnedTime;
		private (float a, float b, float c) _parabola;

		public override void Init(object data)
		{
			_params = (ProjectileParameters)data;
			_parabola = CalculateParabola(_params.MaxAltitude);
			_spawnedTime = Time.time;

			// Travel from A to B
			Tween.Position(transform, _params.Destination, _params.Lifetime, 0, Tween.EaseIn);
			Invoke(nameof(Explode), _params.Lifetime);
		}

		private void Explode()
		{
			Release();
		}

		/// <see>http://chris35wills.github.io/parabola_python/</see>
		private (float a, float b, float c) CalculateParabola(float maxAltitude)
		{
			Vector2 start = new(0, 0);
			Vector2 end = new(1, 0);
			Vector2 peak = new(.5f, maxAltitude);

			float denom = (start.x - peak.x) * (start.x - end.x) * (peak.x - end.x);

			return (
				a: (end.x * (peak.y - start.y) + peak.x * (start.y - end.y) + start.x * (end.y - peak.y)) / denom,
				b: (end.x * end.x * (start.y - peak.y) + peak.x * peak.x * (end.y - start.y) + start.x * start.x * (peak.y - end.y)) / denom,
				c: (peak.x * end.x * (peak.x - end.x) * start.y + end.x * start.x * (end.x - start.x) * peak.y + start.x * peak.x * (start.x - peak.x) * end.y) / denom
			);
		}

		private void Update()
		{
			float elapsedTime = Time.time - _spawnedTime;
			float x = elapsedTime / _params.Lifetime;
			float altitude = _parabola.a * Mathf.Pow(x, 2) + _parabola.b * x + _parabola.c;

			transform.position = transform.position.WithY(_params.Destination.y + altitude);
		}
	}
}
