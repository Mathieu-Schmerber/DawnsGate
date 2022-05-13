using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nawlian.Lib.Systems.Interaction
{
	/// <summary>
	/// Defines an object as interactable.
	/// </summary>
	public interface IInteractable
	{
		/// <summary>
		/// Interacts with the object.
		/// </summary>
		void Interact(IInteractionActor actor);
	}
}
