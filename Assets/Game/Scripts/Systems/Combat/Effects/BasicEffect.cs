using Game.Entities.Shared;
using Nawlian.Lib.Systems.Pooling;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.Combat.Effects
{
	public class BasicEffect : AEffect
	{
		private BasicEffectData _data;
		private IDamageProcessor _damageProcessor;
		protected EntityIdentity _identity;
		protected List<GameObject> _spawned = new();

		private int _activations { get; set; }

		protected virtual void Awake()
		{
			_damageProcessor = GetComponentInParent<IDamageProcessor>();
			_identity = GetComponentInParent<EntityIdentity>();
		}

		public override void OnStart(float duration, AEffectBaseData data)
		{
			_data = data as BasicEffectData;
			base.OnStart(duration, data);
		}

		protected override void OnEnd()
		{
			foreach (BasicEffectData.ActionDescriptor action in _data.Actions)
			{
				if (action.Action == EffectAction.APPLY_MODIFIER)
					RemoveModifiers(action.Modifiers);
			}
			_spawned.ForEach(x => x.GetComponent<IPoolableObject>()?.Release());
		}

		private void ApplyModifiers(StatDictionary modifiers)
		{
			foreach (var item in modifiers)
				_identity.Stats.Modifiers[item.Key].TemporaryModifier += item.Value.Value;
		}

		private void RemoveModifiers(StatDictionary modifiers)
		{
			foreach (var item in modifiers)
				_identity.Stats.Modifiers[item.Key].TemporaryModifier -= item.Value.Value * _activations;
		}

		protected override void OnActivation()
		{
			foreach (BasicEffectData.ActionDescriptor action in _data.Actions)
			{
				switch (action.Action)
				{
					case EffectAction.APPLY_DAMAGE:
						_damageProcessor?.ApplyPassiveDamage(action.DamageAmount);
						break;
					case EffectAction.APPLY_MODIFIER:
						ApplyModifiers(action.Modifiers);
						break;
					case EffectAction.SPAWN_OBJECT:
						if (!action.AllowDuplicates && _activations > 0)
							return;
						ObjectPooler.Get(action.Prefab, null, (GameObject go) => {
							if (action.StickToEntity)
							{
								go.transform.parent = transform;
								go.transform.localPosition = Vector3.zero;
							}
							_spawned.Add(go);
						});
						break;
					default:
						break;
				}
			}
			_activations++;
		}
	}
}
