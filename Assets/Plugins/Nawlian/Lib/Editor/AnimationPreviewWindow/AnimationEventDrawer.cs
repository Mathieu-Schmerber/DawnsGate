using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using StartAssets.PowerfulPreview.Controls;
using UnityEditor;
using Nawlian.Lib.Extensions;

namespace Nawlian.Lib.EditorTools.AnimationPreviewWindow
{
    public class AnimationEventDrawer : Timeline.IDrawer
    {
		public AnimationClip Clip { get; set; }

		/// <summary>
		/// Draws key frame controls on the timeline. 
		/// </summary>
		public void Draw(Rect rect)
        {
            float frameControlWidth = 2.0f;

            if (Clip.events == null || Clip.events.Length == 0)
                return;

            foreach (AnimationEvent keyframe in Clip.events)
            {
                float ratio = keyframe.time / Clip.length;
                int curX = (int)(rect.width * ratio);

                Handles.DrawSolidRectangleWithOutline
                (
                    new Rect(rect.xMin + curX - frameControlWidth / 2, rect.yMin, frameControlWidth, rect.height),
                    ColorExtensions.RandomColor(keyframe.stringParameter),
                    Color.clear
                );
            }
        }               
    }
}