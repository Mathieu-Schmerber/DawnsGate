﻿using Game.Entities.Miscellaneous;
using Nawlian.Lib.Systems.Pooling;
using UnityEngine;

namespace Game.Systems.Items.Passive
{
	public class SolarisOrb : ASpecialItem
	{
		private OrbPivot _orbPivot;

		public override void OnEquipped(ItemBaseData data, int quality)
		{
			base.OnEquipped(data, quality);

			// Here we instantiate, because we don't want this to be released on scene change
			_orbPivot = Instantiate(_data.SpawnPrefab, _entity.transform).GetComponent<OrbPivot>();
			_orbPivot.SetOrbCount((int)_data.Stages[Quality].Amount, _data.Stages[Quality].Damage);
		}

		public override void OnUnequipped()
		{
			Destroy(_orbPivot.gameObject);
			base.OnUnequipped();
		}

		public override void OnUpgrade()
		{
			base.OnUpgrade();
			_orbPivot.SetOrbCount((int)_data.Stages[Quality].Amount, _data.Stages[Quality].Damage);
		}
	}
}