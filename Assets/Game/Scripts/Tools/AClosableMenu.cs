using Game.Managers;
using Plugins.Nawlian.Lib.Systems.Menuing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Tools
{
	public abstract class AClosableMenu : AMenu
	{
		protected virtual void OnEnable()
		{
			InputManager.OnCancelPressed += OnCancel;
		}

		protected virtual void OnDisable()
		{
			InputManager.OnCancelPressed -= OnCancel;
		}

		private void OnCancel()
		{
			if (_isOpen)
				Close();
		}
	}
}
