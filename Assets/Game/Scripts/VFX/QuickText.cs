using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.VFX
{
	public class QuickText : APoolableObject
	{
		internal struct TextData
		{
			public Color Color { get; set; }
			public string Text { get; set; }
		}

		[SerializeField] private float _lifetime;
		[SerializeField] private Vector2 _horizontalMinMax;
		[SerializeField] private Vector2 _verticalMinMax;
		private TextMeshPro _text;

		private void Awake()
		{
			_text = GetComponentInChildren<TextMeshPro>();
		}

		public override void Init(object data)
		{
			var textData = (TextData)data;

			_text.text = textData.Text;
			_text.color = textData.Color;

			var offset = (Vector3.up * Random.Range(_verticalMinMax.x, _verticalMinMax.y)) + (Vector3.forward * Random.Range(_horizontalMinMax.x, _horizontalMinMax.y));

			Tween.Position(transform, transform.position + offset, _lifetime, 0, Tween.EaseSpring);
			Tween.Value(1f, 0f, (value) => _text.color = _text.color.Alpha(value), _lifetime / 2f, _lifetime / 2f, Tween.EaseOut);
			Invoke(nameof(Release), _lifetime);
		}

		public static void ShowDamageText(Vector3 position, float amount)
			=> ObjectPooler.Get(PoolIdEnum.QUICK_TEXT, position, Quaternion.identity, new TextData() { Text = $"-{amount}", Color = Color.red });

		public static void ShowHealText(Vector3 position, float amount)
			=> ObjectPooler.Get(PoolIdEnum.QUICK_TEXT, position, Quaternion.identity, new TextData() { Text = $"+{amount}", Color = Color.green });

		public static void ShowArmorText(Vector3 position, float amount)
		{
			if (amount == 0)
				return;
			ObjectPooler.Get(PoolIdEnum.QUICK_TEXT, position, Quaternion.identity, new TextData() { Text = $"{amount}", Color = Color.yellow });
		}

		public static void ShowGoldText(Vector3 position, float amount)
			=> ObjectPooler.Get(PoolIdEnum.QUICK_TEXT, position, Quaternion.identity, new TextData() { Text = $"{amount}<sprite=\"money\" index=0>", Color = Color.yellow });
	}
}
