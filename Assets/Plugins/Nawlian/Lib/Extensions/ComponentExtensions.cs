using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nawlian.Lib.Extensions
{
	public static class ComponentExtensions
	{
		/// <summary>
		/// Gets a list of components in children. <br/>
		/// The current object can be included in the search.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="includeInactive">Include inactive components</param>
		/// <param name="includeThis">Include this object in the search</param>
		/// <returns></returns>
		public static List<T> GetComponentsInChildren<T>(this Component obj, bool includeInactive = false, bool includeThis = false)
		{
			List<T> res = obj.GetComponentsInChildren<T>(includeInactive).ToList();

			if (includeThis)
			{
				T comp = obj.GetComponent<T>();
				if (comp != null)
					res.Add(comp);
			}
			return res;
		}

		/// <summary>
		/// Gets a list of components in parent. <br/>
		/// The current object can be included in the search.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="includeInactive">Include inactive components</param>
		/// <param name="includeThis">Include this object in the search</param>
		/// <returns></returns>
		public static List<T> GetComponentsInParent<T>(this Component obj, bool includeInactive = false, bool includeThis = false)
		{
			List<T> res = obj.GetComponentsInParent<T>(includeInactive).ToList();

			if (includeThis)
			{
				T comp = obj.GetComponent<T>();
				if (comp != null)
					res.Add(comp);
			}
			return res;
		}
	}
}