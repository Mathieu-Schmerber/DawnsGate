using Game.Systems.Combat.Effects;
using Game.Systems.Items;
using Nawlian.Lib.Extensions;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class SimpleDescriptorUi : MonoBehaviour
	{
		[Title("Required")]
		[SerializeField, Required] private TextMeshProUGUI _nameText;
		[SerializeField, Required] private TextMeshProUGUI _descriptionText;
		[Title("Extra")]
		[SerializeField] private Transform _starList;

		private Image[] _stars;

		private void Awake()
		{
			if (_starList != null)
			{
				_stars = _starList.GetComponentsInChildren<Image>(true);
				_stars.ForEach(x => x.gameObject.SetActive(false));
			}
		}

		public void Describe(ItemBaseData item, int quality)
		{
			if (item != null)
			{
				_nameText.text = item.name;
				_descriptionText.text = item.GetRichDescription(quality);
				if (_stars != null)
				{
					for (int i = 0; i < _stars.Length; i++)
						_stars[i].gameObject.SetActive(i <= quality);
				}
			} else
			{
				_nameText.text = "Empty slot";
				_descriptionText.text = "";
				_stars.ForEach(x => x.gameObject.SetActive(false));

			}
		}

		public void Describe(ItemBaseData item, string forcedDescription)
		{
			if (item != null)
			{
				_nameText.text = item.name;
				_descriptionText.text = forcedDescription;
			}
			else
			{
				_nameText.text = "Empty slot";
				_descriptionText.text = "";
			}
		}

		public void Describe(AEffectBaseData effect)
		{
			if (effect != null)
			{
				_nameText.text = effect.DisplayName;
				_descriptionText.text = effect.Description;
			}
		}
	}
}
