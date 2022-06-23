using Nawlian.Lib.Systems.Interaction;
using Pixelplacement;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Entities.Player.Interaction
{
	public class InteractionUi : AMenu
	{
		[Title("References")]
		[SerializeField] private TextMeshProUGUI _interactionTitle;
		[SerializeField] private Image _inputImage;

		public override bool RequiresGameFocus => false;

		private void OnEnable()
		{
			PlayerInteraction.OnInteractionChanged += OnInteractionProposal;
		}

		private void OnDisable()
		{
			PlayerInteraction.OnInteractionChanged -= OnInteractionProposal;
		}

		private void OnInteractionProposal(IInteractable obj)
		{
			if (!_isOpen && obj != null)
				Open();
			else if (_isOpen && obj == null)
			{
				Close();
				return;
			}
			_interactionTitle.text = obj.InteractionTitle;
		}
	}
}
