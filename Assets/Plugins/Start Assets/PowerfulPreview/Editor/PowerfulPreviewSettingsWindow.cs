using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace StartAssets
{
	public class PowerfulPreviewSettingsWindow : EditorWindow, IActiveBuildTargetChanged
	{
		public int callbackOrder => 0;

		public void OnGUI()
		{
			if(GUILayout.Button("Switch to Legacy Rendering Pipeline"))
			{
				SetCurrentRenderingPipeline(RenderingPipelineEnum.LegacyRenderingPipeline);
			}
			if( GUILayout.Button( "Switch to LWRP"))
			{
				SetCurrentRenderingPipeline(RenderingPipelineEnum.LWRP);
			}
			if (GUILayout.Button("Switch to URP"))
			{
				SetCurrentRenderingPipeline(RenderingPipelineEnum.URP);
			}
			if (GUILayout.Button("Switch to HDRP"))
			{
				SetCurrentRenderingPipeline(RenderingPipelineEnum.HDRP);
			}
		}
		public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
		{
			var value = EditorPrefs.GetInt(EditorPrefsIntKey, -1);
			if (value < 0) return;

			SetCurrentRenderingPipeline((RenderingPipelineEnum)value);
		}

		private void SetCurrentRenderingPipeline(RenderingPipelineEnum selectedPipeline)
		{
			var selectedRenderingPipeline = selectedPipeline.GetString();
			var currentStoredRenderingPipeline = EditorPrefs.GetString(EditorPrefsStrKey, "");

			var buildGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
			var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup).Split(';').ToList();

			if (!string.IsNullOrEmpty(currentStoredRenderingPipeline)) defines.Remove(currentStoredRenderingPipeline);
			if (!string.IsNullOrEmpty(selectedRenderingPipeline)) defines.Add(selectedRenderingPipeline);

			PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, string.Join(";", defines));
			
			EditorPrefs.SetString(EditorPrefsStrKey, selectedRenderingPipeline);
			EditorPrefs.SetInt(EditorPrefsIntKey, (int)selectedPipeline);
		}

		const string EditorPrefsStrKey = "PowerfulPreview_StoredRenderingPipelineStr";
		const string EditorPrefsIntKey = "PowerfulPreview_StoredRenderingPipelineInt";

		[MenuItem("Window/StartAssets/Powerful Preview Settings")]
		private static void ShowSettings() => GetWindow<PowerfulPreviewSettingsWindow>().Show();

		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			if (!EditorPrefs.HasKey(EditorPrefsStrKey)) ShowSettings();
		}	
	}

	public enum RenderingPipelineEnum { LegacyRenderingPipeline, LWRP, URP, HDRP };
	public static class RenderingPipelinesExtension
	{
		public static string GetString(this RenderingPipelineEnum pipeline) => "POWERFUL_PREVIEW_" + pipeline.GetStringInternal();

		private static string GetStringInternal(this RenderingPipelineEnum pipeline)
		{
			switch (pipeline)
			{
				case RenderingPipelineEnum.LegacyRenderingPipeline: return "LRP";
				case RenderingPipelineEnum.LWRP: return "LWRP";
				case RenderingPipelineEnum.URP: return "URP";
				case RenderingPipelineEnum.HDRP: return "HDRP";
			}
			return "";
		}
	}
}