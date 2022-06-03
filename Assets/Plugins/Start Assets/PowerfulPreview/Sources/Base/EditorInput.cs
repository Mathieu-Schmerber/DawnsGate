namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// This class is used to translate events from the editor to the engine.
    /// As they are two different assemblies but sometimes the preview objects 
    /// might act like a handles or gizmos that should know about the preview input. 
    /// </summary>
	public static class EditorInput
	{
        public enum MouseButton { Left = 0, Right = 1, Middle = 2 }

        public static UnityEngine.Event Event
        {
            set; 
            get;
        }

        public static bool IsButton( this UnityEngine.Event e, MouseButton button )
        {
            return e.button == ( int )button;
        }
        public static bool IsButtonDown( this UnityEngine.Event e, MouseButton button )
        {
            return e.type == UnityEngine.EventType.MouseDown && e.button == ( int )button;
        }
        public static bool IsButtonUp( this UnityEngine.Event e, MouseButton button )
        {
            return e.type == UnityEngine.EventType.MouseUp && e.button == ( int )button;
        }
        public static bool IsDragging( this UnityEngine.Event e )
        {
            return e.type == UnityEngine.EventType.MouseDrag;
        }
        public static bool IsDragging( this UnityEngine.Event e, MouseButton button )
        {
            return e.type == UnityEngine.EventType.MouseDrag && e.button == ( int )button;
        }
    }
}