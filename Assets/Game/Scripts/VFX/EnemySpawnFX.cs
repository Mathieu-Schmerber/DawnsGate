using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.VFX
{
	public abstract class AMaterialFx : SerializedMonoBehaviour
	{
		protected abstract void ExecuteFx(Action onCompleted, bool isSpawn);

		public void PlaySpawnFX(Action onCompleted) => ExecuteFx(onCompleted, true);
		public void PlayDeathFX(Action onCompleted) => ExecuteFx(onCompleted, false);
	}
}
