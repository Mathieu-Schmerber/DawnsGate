using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nawlian.Lib.Systems.Pooling
{
	public class BasicPoolableBehaviour : APoolableObject
	{
		public override void Init(object data)
		{

		}

		protected override void OnReleasing()
		{
			base.OnReleasing();
		}
	}
}
