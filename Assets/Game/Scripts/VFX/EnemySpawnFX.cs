using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.VFX
{
	public abstract class AEnemySpawnFX : MonoBehaviour
	{
		protected abstract void ExecuteFx(Action onCompleted);

		public void PlaySpawnFX(Action onCompleted) => ExecuteFx(onCompleted);
	}
}
