using StartAssets.PowerfulPreview.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// It's an extension of the original preview editor, in case if you need 
    /// to use the timeline control for your editor. It implements the settings 
    /// for the preview to control the timeline. 
    /// </summary>
    public abstract class TimelinePreviewEditor<T> : PreviewEditor<T> where T : UnityEngine.Object 
    {
        /// <summary>
        /// Timeline assigned to this editor.
        /// </summary>
        public Timeline Timeline
        {
            private set;
            get;
        }

        /// <summary>
        /// Sets up settings for the timeline.
        /// </summary>
        public override void OnPreviewSettings()
        {
            base.OnPreviewSettings();
            if( Timeline != null )
            {
                var style = new GUIStyle(EditorStyles.boldLabel);
                style.normal.textColor = Color.white;
                style.fontSize = 11;

                float playSpeed = Timeline.Speed;
                EditorGUILayout.LabelField("Speed: ", style, GUILayout.Width(50));
                playSpeed = GUILayout.HorizontalSlider(playSpeed, 0.0f, 2.0f, GUILayout.Width(100));
                EditorGUILayout.LabelField(playSpeed.ToString("F2"), style, GUILayout.Width(35));
                Timeline.Speed = playSpeed;
            }
        }
        
        protected override void OnEnable()
        {
            Timeline = new Timeline();
            base.OnEnable();
            preview.AddControl(Timeline);
        }
    }
}


