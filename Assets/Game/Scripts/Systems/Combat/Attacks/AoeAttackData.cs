using Game.Systems.Combat.Effects;
using Nawlian.Lib.Systems.Pooling;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Combat.Attacks
{
	[CreateAssetMenu(menuName = "Data/Attacks/AOE")]
	public class AoeAttackData : ModularAttackData
	{
		public AudioClip IdleAudio;
		public AEffectBaseData Effect;
		public float EffectDuration;
	}
}
