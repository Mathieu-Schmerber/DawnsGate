using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StartAssets.PowerfulPreview
{
	public static class PreviewShaders
	{
		public static bool UseSRP { set; get; } = true;

		public const string cLegacyBaseMapField = "_MainTex";
		public const string cBaseMapField = "_BaseMap";
		public const string cHDRPUnlitColorMapField = "_UnlitColorMap";
		public const string cBaseColorField = "_BaseColor";
		public const string cLegacyBaseColorField = "_Color";

		public const string cHDRPUnlitColorField = "_UnlitColor";

		public static bool IsLegacyShaderSet => !UseSRP && string.IsNullOrEmpty(Shader.globalRenderPipeline);
		public static bool IsURP => UseSRP && Shader.globalRenderPipeline.Contains(cURP);
		public static bool IsLWRP => UseSRP &&  Shader.globalRenderPipeline.Contains(cLWRP);
		public static bool IsHDRP => UseSRP &&  Shader.globalRenderPipeline.Contains(cHDRP);

		public static string BaseMapField => IsLegacyShaderSet || IsHDRP ? cLegacyBaseMapField : cBaseMapField;
		public static string BaseColorField => IsLegacyShaderSet || IsHDRP ? cLegacyBaseColorField : cBaseColorField; 

		public static Material LitMaterial
		{
			get
			{
				var mat = new Material(LitShader);
				return mat; 
			}
		}
		public static Material SimpleLitMaterial
		{
			get
			{
				if (IsHDRP) return LitMaterial;

				var mat = new Material(SimpleLitShader);
				return mat; 
			}
		}
		public static Material InvisibleMaterialCopy
		{
			get
			{
				var mat = new Material(InvisibleShader);
				mat.EnableKeyword("_ALPHATEST_ON");
				mat.SetOverrideTag("RenderType", "TransparentCutout");
				mat.SetFloat("_AlphaClip", 1.0f);

				mat.SetColor(cBaseColorField, Color.clear);
				mat.SetColor(cLegacyBaseColorField, Color.clear);
				mat.SetColor(cHDRPUnlitColorField, Color.clear); 

				mat.renderQueue = 2450;
				return mat;
			}
		}

		public static Material GridMaterial
		{
			get
			{
				if (mGridMaterial == null)
				{
					mGridMaterial = new Material(UnlitShader);
					mGridMaterial.mainTexture = Resources.PreviewGrid.Get();
					//mGridMaterial.SetOverrideTag("RenderType", "TransparentCutout");

					mGridMaterial.SetTexture(cBaseMapField, mGridMaterial.mainTexture);
					mGridMaterial.SetTexture(cHDRPUnlitColorMapField, mGridMaterial.mainTexture);

					var textureScale = new Vector2(10.0f, 10.0f);
					mGridMaterial.SetTextureScale(cLegacyBaseMapField, textureScale);
					mGridMaterial.SetTextureScale(cBaseMapField, textureScale);
					mGridMaterial.SetTextureScale(cHDRPUnlitColorMapField, textureScale);

					mGridMaterial.DisableKeyword("_ALPHATEST_ON");
					mGridMaterial.renderQueue = 1000;
				}

				return mGridMaterial;
			}
		}

		public static Shader InvisibleShader => Find("Transparent/Diffuse", "Unlit");
		public static Shader UnlitShader => Find("Unlit/Transparent", "Unlit");
		public static Shader SimpleLitShader => Find("Legacy Shaders/Bumped Diffuse", "Simple Lit");
		public static Shader LitShader => Find("Legacy Shaders/Bumped Specular", "Lit");

		private static Shader Find(string defaultName, string fallbackName)
		{
			var shaderName = defaultName;
			if (Shader.globalRenderPipeline.Contains(cURP)) shaderName = string.Format(cURPShaders, fallbackName);
			else if (Shader.globalRenderPipeline.Contains(cLWRP)) shaderName = string.Format(cLWRPShaders, fallbackName);
			else if (Shader.globalRenderPipeline.Contains(cHDRP)) shaderName = string.Format(cHDRPShaders, fallbackName);
			return Shader.Find(shaderName);
		}

		private const string cURP = "UniversalPipeline";
		private const string cLWRP = "LightweightPipeline";
		private const string cHDRP = "HDRenderPipeline";

		private const string cURPShaders = "Universal Render Pipeline/{0}";
		private const string cLWRPShaders = "Lightweight Render Pipeline/{0}";
		private const string cHDRPShaders = "HDRP/{0}";

		private static Material mGridMaterial;
	}
}


