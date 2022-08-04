using Game.Managers;
using Plugins.Nawlian.Lib.Systems.Menuing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Tools
{
	public abstract class ACloseableMenu : AMenu
	{
		protected virtual void OnCancel()
		{
			if (_isOpen)
				Close();
		}

		protected virtual void OnSubmit()
		{

		}

		public override void Open()
		{
			base.Open();
			//InputManager.OnCancelPressed += OnCancel;
			InputManager.OnSubmitPressed += OnSubmit;
		}

		public override void Close()
		{
			base.Close();
			//InputManager.OnCancelPressed -= OnCancel;
			InputManager.OnSubmitPressed -= OnSubmit;
		}
	}
}
