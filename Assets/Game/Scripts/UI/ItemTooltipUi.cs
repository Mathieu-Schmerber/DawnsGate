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

		private Camera _cam;
		private ItemTagUi[] _tags;

		public override bool RequiresGameFocus => false;

		protected override void Awake()
		{
			base.Awake();
			_cam = GameManager.Camera.GetComponentInChildren<Camera>();
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
			_nameTxt.text = item.Data.name;
			foreach (var tag in _tags)
				tag.gameObject.SetActive((item.Data.Tags & tag.Tag) == tag.Tag);
		}

		private void PlaceOnCanvas(Transform item)
		{
			Vector3 canvasPos = WorldToViewPosition(item.position + item.up, _cam, _rect);
			_rect.anchoredPosition = canvasPos;
		}

		private Vector3 WorldToViewPosition(Vector3 worldPos, Camera camera, RectTransform viewport)
		{
			Vector2 screenOffset = new Vector2(viewport.sizeDelta.x / 2f, viewport.sizeDelta.y / 2f);
			Vector2 ViewportPosition = camera.WorldToViewportPoint(worldPos);
			Vector2 proportionalPosition = new Vector2(ViewportPosition.x * viewport.sizeDelta.x, ViewportPosition.y * viewport.sizeDelta.y);

			return proportionalPosition - screenOffset;
		}
	}
}