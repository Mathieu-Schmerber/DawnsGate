using UnityEngine;
using UnityEditor;
using System.Linq;
using StartAssets.PowerfulPreview.Drawers;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// The control which draws objects into itself. 
    /// It contains grid and two cameras: main and gizmo's. 
    /// Main camera draws all previews scene, and gizmo's camera draws only gizmo's.  
    /// To use this control, you should get instance for the current editor. And 
    /// after it's disabled release it.
    /// </summary>
    public partial class Preview : Control, System.IDisposable
    {
        /// <summary>
        /// Prefix of every preview object. 
        /// </summary>
        public const string PreviewPrefix = "PREVIEW__";

        /// <summary>
        /// Create an instance of the preview with the <code>editor</code> as the owner. 
        /// </summary>
        /// <param name="editor">Owner of the created preview instance.</param>
        /// <returns>An instance of the preview.</returns>
        public static Preview Create(Editor editor)
        {
            bool isObjectPicker = EditorGUIUtility.GetObjectPickerControlID() != 0 &&
                                  EditorGUIUtility.GetObjectPickerObject() == editor.target;
            if( isObjectPicker )
            {
                return null;
            }
            return new Preview(); 
        }

        /// <summary>
        /// Buttons that are used by the preview
        /// </summary>
        public enum Buttons
        {
            Left = 0,
            Right = 1,
            Middle = 2,
        }

        /// <summary>
        /// A delegate for any drawing event fired by the preview.
        /// </summary>
        /// <param name="surfaceRect">Surface rect of the preview.</param>
        public delegate void DrawingEvent(Rect surfaceRect);
        /// <summary>
        /// A delegate for any mouse event fired by the preview. 
        /// </summary>
        /// <param name="mouseEvent">Mouse event of the preview.</param>
        public delegate void MouseEvent(Event mouseEvent);

        /// <summary>
        /// Action that is invoked before the main camera render.
        /// There is no much sense to draw anything at this state.
        /// But, it can be useful if you need to handle pre-render moment. 
        /// </summary>
        public event DrawingEvent OnBeforeDraw;
        /// <summary>
        /// Action that is invoked after the main camera render.  
        /// Anything is drawn on this layer will be overlapped by the gizmo layer. 
        /// </summary>
        public event DrawingEvent OnDraw;
        /// <summary>
        /// Action that is invoked after the gizmo camera render. 
        /// Anything is drawn on this layer will be on top of everything. 
        /// </summary>
        public event DrawingEvent OnAfterDraw;

        /// <summary>
        /// Action that is invoked, when user 
        /// push some mouse button over the preview rect. 
        /// </summary>
        public event MouseEvent OnMouseDown;
        /// <summary>
        /// Action that is invoked, when user pressed 
        /// some mouse button inside the preview rect. 
        /// </summary>
        public event MouseEvent OnMouseUp;
        /// <summary>
        /// Action that is invoked, when user pressed
        /// some mouse button and moved the mouse over the preview rect. 
        /// </summary>
        public event MouseEvent OnMouseDrag;
        /// <summary>
        /// Action that is invoked, when user scrolls 
        /// the mouse wheel. 
        /// </summary>
        public event MouseEvent OnScrollWheel;

        /// <summary>
        /// Called when the surface rect of the preview has been updated. 
        /// </summary>
        public event Action<Rect> OnSurfaceRectUpdated;

		/// <summary>
		/// Preview camera. 
		/// </summary>
		public PreviewCamera Camera => Scene.Camera;
        
        /// <summary>
        /// Preview camera controller, it's used to handle user input events.
        /// </summary>
        public PreviewCameraController CameraController
        {
            private set;
            get;
        }
        /// <summary>
        /// A scene of the preview, use this to instantiate or add new objects to the scene. 
        /// </summary>
        public PreviewScene Scene
        {
            private set;
            get;
        }

        /// <summary>
        /// Surface or content rect of the preview. 
        /// </summary>
        public Rect SurfaceRect
        {
            private set;
            get;
        }
        /// <summary>
        /// Returns true, if editor is not changing playmode right now; false otherwise. 
        /// </summary>
        public bool CanInstantiate
        {
            get
            {
                return !(EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying);
            }
        }
        /// <summary>
        /// Should the preview handle mouse scroll wheel events? 
        /// </summary>
        public bool ScrollWheelEnabled { set; get; }
		public bool ShouldUseSRP { set; get; }
		
        /// <summary>
        /// Sets all the buttons enabled state.
        /// </summary>
        /// <param name="value">Enabled/Disabled</param>
        public void SetButtonsEnabled(bool value)
        {
            SetButtonEnabled(Buttons.Left, value);
            SetButtonEnabled(Buttons.Middle, value);
            SetButtonEnabled(Buttons.Right, value);
            ScrollWheelEnabled = value;
        }
        /// <summary>
        /// Sets some specific button enabled state. 
        /// </summary>
        /// <param name="button">Button to set.</param>
        /// <param name="value">Enabled/Disabled</param>
        public void SetButtonEnabled(Buttons button, bool value)
        {
            var index = (int)button;
            index = Mathf.Clamp(index, (int)Buttons.Left, (int)Buttons.Middle);
            mButtons[index] = value;
        }
        /// <param name="button">The button to check.</param>
        /// <returns>true if the button is enabled; false otherwise.</returns>
        public bool IsButtonEnabled(Buttons button)
        {
            return IsButtonEnabled((int)button);
        }
        /// <summary>
        /// This method is used for unity events, because the buttons are represented
        /// as integer indices there.
        /// </summary>
        /// <param name="index">Index of the button to check</param>
        /// <returns>true if the button is enabled; false otherwise.</returns>
        public bool IsButtonEnabled(int index)
        {
            index = Mathf.Clamp(index, (int)Buttons.Left, (int)Buttons.Middle);
            return mButtons[index];
        }

        /// <summary>
        /// Create a unique name of the game object bases on the preview prefix 
        /// and it's UUID, It's unique only among the other previews, but the name 
        /// itself must be unique to be unique amongh the objects of this specific 
        /// preview instance. 
        /// </summary>
        /// <param name="gameObjectName">Original game object name.</param>
        /// <returns>Preview prefix + game object name + UUID</returns>
        public string GetPreviewGameObjectUniqueName( string gameObjectName )
        {
            return PreviewPrefix + gameObjectName + UUID;
        }

        /// <summary>
        /// Updates the preview. 
        /// </summary>
        public override void Update()
        {
            if (Mathf.Approximately(SurfaceRect.width, 0.0f) ||
                Mathf.Approximately(SurfaceRect.height, 0.0f))
            {
                return;
            }
            foreach (var control in mControls)
            {
                control.Update();
            }

            Draw();

            if (Event.current.type != EventType.Layout)
            {
                HandleEvents();
            }
        }
        /// <summary>
        /// Sets current surface rect. It's used to calculate control
        /// positions and initiate recreating of the surfaces. 
        /// </summary>
        /// <param name="rect">Surface rect of the preview</param>
        public void SetSurfaceRect( Rect rect )
        {
			if ( rect.x < 0 || rect.y < 0 || rect.height < 2 || rect.width < 2 )
            {
                return; 
            }
            if( Mathf.RoundToInt( SurfaceRect.x ) == Mathf.RoundToInt( rect.x ) &&
                Mathf.RoundToInt( SurfaceRect.y ) == Mathf.RoundToInt( rect.y ) &&
                Mathf.RoundToInt( SurfaceRect.width ) == Mathf.RoundToInt( rect.width ) &&
                Mathf.RoundToInt( SurfaceRect.height ) == Mathf.RoundToInt( rect.height ) )
            {
                return;
            }

            var currentOffset = 0.0f;
            foreach (var control in mControls)
            {
                control.PositionX = rect.x;
                control.PositionY = rect.y + currentOffset;
                control.Width = rect.width;
                currentOffset += control.Height;
            }
            rect.yMin += currentOffset;
            SurfaceRect = rect;
            Camera.SetSurfaceRect(SurfaceRect);
            OnSurfaceRectUpdated?.Invoke(SurfaceRect);
        }
        /// <summary>
        /// Handles the input events.
        /// </summary>
        /// <returns>True if some event has been handled, false otherwise.</returns>
        public override bool HandleEvents()
        {
			foreach (var control in mControls)
            {
                if (control.HandleEvents())
                {
                    CameraController.ResetState();
                    return true;
                }
            }

            var e = new Event(Event.current);
            var editorEvent = new Event(e);

            Vector2 realMousePos = editorEvent.mousePosition;
            if (SurfaceRect.Contains(e.mousePosition))
            {
                realMousePos.y = SurfaceRect.height - e.mousePosition.y + SurfaceRect.y;
                editorEvent.mousePosition = realMousePos;
            }
            EditorInput.Event = editorEvent;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (IsButtonEnabled(e.button) && SurfaceRect.Contains(e.mousePosition))
                    {
                        mMouseButtonPressed = true;
                        OnMouseDown?.Invoke(e);
                    }
                    break;
                case EventType.MouseDrag:
                    if (mMouseButtonPressed && IsButtonEnabled(e.button))
                    {
                        OnMouseDrag?.Invoke(e);
                    }
                    break;
                case EventType.MouseUp:
                    if (mMouseButtonPressed && IsButtonEnabled(e.button))
                    {
                        mMouseButtonPressed = false;
                        OnMouseUp?.Invoke(e);
                    }
                    break;
                case EventType.ScrollWheel:
                    if (ScrollWheelEnabled && SurfaceRect.Contains(e.mousePosition))
                    {
                        OnScrollWheel?.Invoke(e);
                    }
                    break;
            }

            switch (CameraController.CurrentState)
            {
                case PreviewCameraStates.Dragging:
                    EditorGUIUtility.AddCursorRect(SurfaceRect, MouseCursor.Pan);
                    break;
                case PreviewCameraStates.Rotating:
                    EditorGUIUtility.AddCursorRect(SurfaceRect, MouseCursor.Orbit);
                    break;
            }
            return true;
        }
        /// <summary>
        /// Releases all the preview resources. 
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Adds control that is drawn as a child of the preview: 
        /// on top of preview surface rect. 
        /// </summary>
        /// <param name="control">Control to add.</param>
        public virtual void AddControl( Control control )
        {
            mControls.Add( control );
        }
        /// <summary>
        /// Removes existing preview control. 
        /// </summary>
        /// <param name="control">Control to remove.</param>
        public virtual void RemoveControl( Control control )
        {
            for( int i = 0; i < mControls.Count; i++ )
            {
                if( mControls[ i ].Equals( control ) )
                {
                    mControls.RemoveAt( i );
                    return; 
                }
            }
        }
        
        /// <summary>
        /// Adds a handler for the preview. Look at IPreviewHandler.cs for more information.
        /// </summary>
        /// <param name="handler">Handler to add.</param>
        public void AddHandler( IPreviewHandle handler )
        {
            mPreviewHandles.Add(handler);
        }
        /// <summary>
        /// Removes existing preview handler.
        /// </summary>
        /// <param name="handler">Handler to remove.</param>
        public void RemoveHandler( IPreviewHandle handler )
        {
            mPreviewHandles.Remove(handler);
        }

        protected Preview()
        {
            UUID = "_" + System.Guid.NewGuid().ToString();

            mControls = new List<Control>();
            mPreviewHandles = new List<IPreviewHandle>();

            Scene = new PreviewScene(this);
            CameraController = new PreviewCameraController(this);
        }
		~Preview()
        {
            Dispose(false);
		}
		
        /// <summary>
        /// Draws the preview. 
        /// </summary>
        protected virtual void Draw()
        {
			Scene.PrepareScene();
			Camera.Render();

            OnBeforeDraw?.Invoke(SurfaceRect);
			GUI.DrawTexture(SurfaceRect, Camera.MainSurface);
			OnDraw?.Invoke(SurfaceRect);
			GUI.DrawTexture(SurfaceRect, Camera.GizmoSurface);
			OnAfterDraw?.Invoke(SurfaceRect);
			Camera.RenderHandles(mPreviewHandles);

			Handles.BeginGUI();
			foreach (var control in mControls)
			{
				control.Update();
			}

			Scene.RestoreScene();
		}
        /// <summary>
        /// See `Dispose`pattern 
        /// https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (mDisposed)
            {
                return;
            }
            if (disposing)
            {
                Camera.Dispose();
                Scene.Dispose();
			}
            mDisposed = true;
        }

		private string UUID;
        private bool mDisposed;
        
        private bool[] mButtons = new bool[ 3 ] { false, false, false };
        private bool mMouseButtonPressed;

        private List<Control> mControls;
        private List<IPreviewHandle> mPreviewHandles;
    }
}