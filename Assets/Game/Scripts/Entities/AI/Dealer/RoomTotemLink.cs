using UnityEngine;

namespace Game.Entities.AI.Dealer
{
	public class RoomTotemLink : MonoBehaviour
	{
		[SerializeField] private Vector2 _offset;

		private Transform _target;
		private LineRenderer _lr;
		private Material _mat;

		private void Awake()
		{
			_lr = GetComponent<LineRenderer>();
			_mat = _lr.material;
		}

		public void SetTarget(Transform target)
		{
			_target = target;
			if (_lr)
				_lr.enabled = _target != null;
		}

		private void Update()
		{
			if (_target != null)
			{
				_lr.SetPositions(new Vector3[] { transform.position, _target.position });
				_mat.mainTextureOffset += _offset * Time.deltaTime;
			}
		}
	}
}