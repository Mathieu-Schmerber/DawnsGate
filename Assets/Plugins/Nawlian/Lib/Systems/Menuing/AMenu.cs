using Sirenix.OdinInspector;
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

		[Button("Close")]
		public virtual void CloseEditor()
		{
			_isOpen = false;
		}

		[Button("Open")]
		public virtual void OpenEditor()
		{
			_isOpen = true;
		}
	}
}
