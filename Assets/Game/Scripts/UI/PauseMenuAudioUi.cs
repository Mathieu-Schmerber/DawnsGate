using Game.Tools;
using Nawlian.Lib.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class PauseMenuAudioUi : ACloseableMenu 
	{
		private AudioSliderUI[] _selectables;

		protected override void Awake()
		{
			base.Awake();
			_selectables = GetComponentsInChildren<AudioSliderUI>();
		}

		public override void Open()
		{
			base.Open();
			EventSystem.current.SetSelectedGameObject(_selectables[0].gameObject);
			_selectables.ForEach(x => x.Refresh());
		}
	}
}