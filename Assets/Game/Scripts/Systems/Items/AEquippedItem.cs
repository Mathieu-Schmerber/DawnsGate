using Game.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Items
{
	public abstract class AEquippedItem : MonoBehaviour
	{
		protected EntityIdentity _entity;
		protected int _quality;

		public int Quality => _quality;
		public abstract ItemBaseData Details { get; }

		protected virtual void Awake()
		{
			_entity = GetComponentInParent<EntityIdentity>();
		}

		public virtual void OnEquipped(ItemBaseData data, int quality)
		{
			_quality = quality;
		}

		public virtual void OnUnequipped()
		{
			Destroy(this);
		}

		public virtual void OnUpgrade()
		{
			if (_quality < Databases.Database.Data.Item.Settings.NumberOfUpgrades - 1)
				_quality++;
		}

		public abstract string GetDescription();
	}
}
