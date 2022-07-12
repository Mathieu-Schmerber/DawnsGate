using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Plugins.Nawlian.Lib.Systems.Menuing
{
	public interface IMenu
	{
		bool IsOpen { get; }
		bool IsHidden { get; }
		bool RequiresGameFocus { get; }

		public void Open();
		public void Close();
		public void Hide();
		public void Show();
	}
}
