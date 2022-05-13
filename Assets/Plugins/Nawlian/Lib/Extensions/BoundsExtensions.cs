using UnityEngine;

namespace Nawlian.Lib.Extensions
{
    public static class BoundsExtensions
    {
        /// <summary>
        /// Returns a random point inside the bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Vector3 GetRandomPoint(this Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }
    }
}