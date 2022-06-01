using Game.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.AI
{
	public class IdleController : AController
	{
		protected override Vector3 GetMovementsInputs() => Vector3.zero;

		protected override Vector3 GetTargetPosition() => Vector3.zero;
	}
}
