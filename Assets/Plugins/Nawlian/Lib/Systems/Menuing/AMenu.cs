using UnityEngine;

namespace Plugins.Nawlian.Lib.Systems.Menuing
{
	public abstract class AMenu : MonoBehaviour, IMenu
	{
		protected bool _isOpen;

		public bool IsOpen => _isOpen;

		public virtual void Close()
		{
			_isOpen = false;
		}

		public virtual void Open()
		{
			_isOpen = true;
		}
	}
}
