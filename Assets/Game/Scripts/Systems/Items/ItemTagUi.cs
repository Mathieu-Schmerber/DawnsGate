using UnityEngine;

namespace Game.Systems.Items
{
	public class ItemTagUi : MonoBehaviour
	{
		[SerializeField] private ItemTag _tag;

		public ItemTag Tag => _tag;
	}
}