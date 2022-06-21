using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins.Nawlian.Lib.Systems.Selection
{
	public interface ISelectable
	{
		public void Select();
		public void Deselect();
	}
}
