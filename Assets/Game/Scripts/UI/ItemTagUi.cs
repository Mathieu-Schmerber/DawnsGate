using Game.Systems.Items;
using UnityEngine;

namespace Game.UI
{
	public class ItemTagUi : MonoBehaviour
	{
		[SerializeField] private ItemTag _tag;

		public ItemTag Tag => _tag;
	}
}