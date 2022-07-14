using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Systems.Items
{
	public abstract class UpgradableItemBaseData<T> : ItemBaseData
	{
		[Space]
		public T[] Stages;

#if UNITY_EDITOR

		[OnInspectorInit]
		public void InitStages()
		{
			int nbStage = Databases.Database.Data.Item.Settings.NumberOfUpgrades;

			if (Stages == null || Stages.Length == 0)
				Stages = new T[nbStage];
			else if (Stages.Length != nbStage)
				Array.Resize(ref Stages, nbStage);
		}

#endif
	}
}
