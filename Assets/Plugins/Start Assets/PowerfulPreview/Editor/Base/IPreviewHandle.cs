using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StartAssets
{
    /// <summary>
    /// An interface for custom preview handles implementations. 
    /// Preview handle is a small control/view which is drawn 
    /// on top of the preview, to emulate the same functionality 
    /// as Unity Drag/Rotate/Scale tools, or camera orientation 
    /// view at the right top corner of the Unity scene view. 
    /// </summary>
    public interface IPreviewHandle
    {
        /// <summary>
        /// Is the handle visible?
        /// </summary>
        bool Visible { set; get; } 

        /// <summary>
        /// Implement it to draw the handle. 
        /// </summary>
        /// <param name="handleCamera">It's a special camera of the preview, which is utilized for drawing the handles!</param>
        /// <param name="surfaceRect">Visible rect of the preview.</param>
        void Draw(Camera handleCamera, Rect surfaceRect);
    }
}
