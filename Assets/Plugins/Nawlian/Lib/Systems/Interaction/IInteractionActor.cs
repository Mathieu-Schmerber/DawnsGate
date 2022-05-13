using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nawlian.Lib.Systems.Interaction
{
	public interface IInteractionActor
	{
		/// <summary>
		/// Suggests an interaction to the actor.
		/// </summary>
		/// <param name="interactable"></param>
		void SuggestInteraction(IInteractable interactable);

		/// <summary>
		/// Remove the interaction suggestion from the actor.
		/// </summary>
		/// <param name="interactable"></param>
		void UnSuggestInteraction(IInteractable interactable);
	}
}
