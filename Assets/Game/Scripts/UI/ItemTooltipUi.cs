using Game.Entities.Player;
using Game.Managers;
using Game.Systems.Items;
using Nawlian.Lib.Systems.Interaction;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.UI
{
	public class ItemTooltipUi : AMenu
	{
		[Title("References")]
		[SerializeField] private TextMeshProUGUI _nameTxt;
		[SerializeField] private RectTransform _panelRect;
		[SerializeField] private TextMeshProUGUI _descriptionText;

 		private ItemTagUi[] _tags;

		public override bool RequiresGameFocus => false;

		protected override void Awake()
		{
			base.Awake();
			_tags = GetComponentsInChildren<ItemTagUi>(true);
		}

		private void OnEnable()
		{
			PlayerInteraction.OnInteractionChanged += OnInteractionChanged;
		}

		private void OnDisable()
		{
			PlayerInteraction.OnInteractionChanged -= OnInteractionChanged;
		}

		private void OnInteractionChanged(IInteractable obj)
		{
			LootedItem item;

			if (obj == null || (item = obj as LootedItem) == null)
			{
				if (_isOpen)
					Close();
				return;
			}

			Open();
			_nameTxt.text = item.Item.name;
			if (item.Item.IsLifeItem)
			{
				_descriptionText.gameObject.SetActive(true);
				_descriptionText.text = item.Item.GetRichDescription(item.Quality);
			}
			else
				_descriptionText.gameObject.SetActive(false);
			foreach (var tag in _tags)
			{
				if (item.Item.IsLifeItem)
					tag.gameObject.SetActive(false);
				else
				{
					tag.gameObject.SetActive((item.Item.Tags & tag.Tag) == tag.Tag);
					if (!tag.gameObject.activeSelf && item.Summary.isMerged)
						tag.gameObject.SetActive((item.Summary.Merge.Data.Tags & tag.Tag) == tag.Tag);
				}
			}
		}
	}
}