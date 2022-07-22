using Game.Entities.Player;
using Game.Managers;
using Nawlian.Lib.Systems.Interaction;
using Pixelplacement;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
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
			if (GuiManager.IsMenuing)
			{
				Close();
				return;
			}
			else if (!_isOpen && obj != null)
				Open();
			else if (_isOpen || obj == null)
			{
				Close();
				return;
			}
			_interactionTitle.text = obj.InteractionTitle;
		}
	}
}
