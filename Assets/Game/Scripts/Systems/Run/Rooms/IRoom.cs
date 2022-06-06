using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Run
{
	public interface IRoom
	{
		/// <summary>
		/// Activate the room logic
		/// </summary>
		public void Activate();

		/// <summary>
		/// Clears the room and opens the doors
		/// </summary>
		public void Clear();
	}
}
