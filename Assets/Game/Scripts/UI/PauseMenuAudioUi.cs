using Game.Tools;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class PauseMenuAudioUi : ACloseableMenu 
	{
		private Selectable[] _selectables;

		protected override void Awake()
		{
			base.Awake();
			_selectables = GetComponentsInChildren<Selectable>();
		}

		public override void Open()
		{
			base.Open();
			EventSystem.current.SetSelectedGameObject(_selectables[0].gameObject);
		}
	}
}