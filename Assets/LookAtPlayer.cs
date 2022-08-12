using Game.Managers;
using Nawlian.Lib.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LookAtPlayer : MonoBehaviour
    {
        void Update()
        {
            Vector3 dir = (GameManager.Player.transform.position.WithY(transform.position.y) - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.123f);
        }
    }
}
