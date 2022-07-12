using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Game
{
    public class SpriteAssetCreator : OdinEditorWindow
    {
		[MenuItem("Tools/Game/Sprite Asset Creator")]
        public static void OpenWindow() => GetWindow<SpriteAssetCreator>().Show();

		public List<Sprite> _sprites = new();
		public List<TMP_SpriteAsset> _tmpSprites = new();

		[Button]
		private void CreateTmpAssets()
		{
			foreach (Sprite sprite in _sprites)
			{
				Selection.SetActiveObjectWithContext(sprite.texture, sprite);
				TMP_SpriteAssetMenu.CreateSpriteAsset();
			}
		}

		[Button]
		private void AdjustTMP(Vector2Int offset, float scale)
		{
			foreach (var asset in _tmpSprites)
			{
				asset.spriteGlyphTable[0].scale = scale;
				asset.spriteGlyphTable[0].metrics = new(
					asset.spriteGlyphTable[0].metrics.width,
					asset.spriteGlyphTable[0].metrics.height,
					offset.x,
					offset.y,
					asset.spriteGlyphTable[0].metrics.horizontalAdvance);
				EditorUtility.SetDirty(asset);
			}
			AssetDatabase.SaveAssets();
		}
	}
}
