using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nawlian.Lib.Extensions
{
	/// <summary>
	/// Extends any type of UnityEngine.Vector
	/// </summary>
	public static class VectorExtension
	{
		private static readonly Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

		/// <summary>
		/// Rotates a Vector3 by the given eulers
		/// </summary>
		/// <param name="input"></param>
		/// <param name="eulers"></param>
		/// <returns></returns>
		public static Vector3 Rotate(this Vector3 input, Vector3 eulers)
		{
			Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(eulers));

			return matrix.MultiplyPoint3x4(input);
		}

		/// <summary>
		/// Converts a Vector3 as isometric
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static Vector3 ToIsometric(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);

		/// <summary>
		/// Return the array's closest position to the given point.
		/// </summary>
		public static Vector3 Closest(this Vector3[] input, Vector3 point)
		{
			Vector3 best = Vector3.zero;
			float bestDistance = Mathf.Infinity;
			float distance;

			if (input == null || input.Length == 0)
				throw new ArgumentNullException("input was null or empty");
			for (int i = 0; i < input.Length; i++)
			{
				distance = Vector3.Distance(point, input[i]);

				if (distance < bestDistance)
				{
					best = input[i];
					bestDistance = distance;
				}
			}
			return best;
		}

		/// <summary>
		/// Return the list closest position to the given point.
		/// </summary>
		public static Vector3 Closest(this IList<Vector3> input, Vector3 point) => input.ToArray().Closest(point);

		/// <summary>
		/// Converts a Vector2 to a Vector3, translating to X and Z
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Vector3 ToVector3XZ(this Vector2 vector, float y = 0) => new Vector3(vector.x, y, vector.y);
		public static Vector3 ToVector3XZ(this Vector2Int vector, float y = 0) => new Vector3(vector.x, y, vector.y);

		/// <summary>
		/// Converts a Vector2 to a Vector3, translating to X and Y
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Vector3 ToVector3XY(this Vector2 vector, float z = 0) => new Vector3(vector.x, vector.y, z);
		public static Vector3 ToVector3XY(this Vector2Int vector, float z = 0) => new Vector3(vector.x, vector.y, z);

		/// <summary>
		/// Returns the vector setting its X
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static Vector3 WithX(this Vector3 vector, float x) => new Vector3(x, vector.y, vector.z);

		/// <summary>
		/// Returns the vector setting its Y
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Vector3 WithY(this Vector3 vector, float y) => new Vector3(vector.x, y, vector.z);

		/// <summary>
		/// Returns the vector setting its Z
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public static Vector3 WithZ(this Vector3 vector, float z) => new Vector3(vector.x, vector.y, z);

		public static Vector2 ToVector2XY(this Vector3 vector) => new Vector2(vector.x, vector.y);
		public static Vector2 ToVector2XZ(this Vector3 vector) => new Vector2(vector.x, vector.z);
		public static Vector2 ToVector2YZ(this Vector3 vector) => new Vector2(vector.y, vector.z);

		/// <summary>
		/// Switch a vector's X and Y
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static Vector2 InverseXY(this Vector2 vector) => new Vector2(vector.y, vector.x);
		public static Vector2 InverseXY(this Vector2Int vector) => new Vector2(vector.y, vector.x);
	}
}