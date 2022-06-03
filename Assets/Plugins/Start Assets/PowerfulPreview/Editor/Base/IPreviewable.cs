using StartAssets.PowerfulPreview;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// An interface for all the events that might be previewable inside the animation move editor.
    /// </summary>
    public interface IPreviewable 
    {
        /// <summary>
        /// A call which is called any time the preview needs to be updated. 
        /// </summary>
        /// <param name="parent">Parent of the event game object inside the preview scene. Basically the origin of the event.</param>
        /// <param name="preview">Preview the event should be drawn inside.</param>
        /// <param name="currentTime">Current time of the event preview animation (from 0 to 1).</param>
        /// <param name="playing">Is the event animation preview playing?</param>
        void DrawPreview( GameObject parent, Preview preview, float currentTime, bool playing );
    }
}