using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
    public class EntityCircle : MonoBehaviour
    {
        [SerializeField] private LayerMask _layers;

        void Update()
        {
            if (Physics.Raycast(transform.parent.position, Vector3.down, out RaycastHit hit, 1000f, _layers))
                transform.position = hit.point + Vector3.up * 0.1f;
        }
    }
}
