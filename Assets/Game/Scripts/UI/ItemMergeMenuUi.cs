using Game.Tools;
using Plugins.Nawlian.Lib.Systems.Menuing;

namespace Game.UI
{
	public class ItemMergeMenuUi : AClosableMenu
	{
		public override void Open()
		{
			if (InventoryUi.IsInUse)
				return;
			base.Open();
			InventoryUi.StartUsing();
		}

		public override void Close()
		{
			base.Close();
			InventoryUi.EndUsing();
		}
	}
}
