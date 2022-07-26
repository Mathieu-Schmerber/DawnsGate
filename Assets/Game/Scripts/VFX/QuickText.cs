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
			Tween.Position(transform, transform.position + Vector3.up, _lifetime, 0, Tween.EaseBounce);
			Tween.Value(1, 0, (value) => _text.color.Alpha(value), _lifetime / 2f, _lifetime / 2f);
			Invoke(nameof(Release), _lifetime);
		}

		public static void ShowDamageText(Vector3 position, float amount)
			=> ObjectPooler.Get(PoolIdEnum.QUICK_TEXT, position, Quaternion.identity, new TextData() { Text = $"-{amount}", Color = Color.red });

		public static void ShowHealText(Vector3 position, float amount)
			=> ObjectPooler.Get(PoolIdEnum.QUICK_TEXT, position, Quaternion.identity, new TextData() { Text = $"+{amount}", Color = Color.green });

		public static void ShowGoldText(Vector3 position, float amount)
			=> ObjectPooler.Get(PoolIdEnum.QUICK_TEXT, position, Quaternion.identity, new TextData() { Text = $"+{amount}<sprite=\"money\" index=0>", Color = Color.yellow });
	}
}
