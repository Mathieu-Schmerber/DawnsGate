using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StartAssets.PowerfulPreview.Preview;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// Possible user interaction states of the preview.
    /// </summary>
    public enum PreviewCameraStates
    {
        None = -1,
        Dragging = 0,
        Rotating = 1,
        Zooming = 2,
    }

    /// <summary>
    /// A controller for the preview camera, which
    /// is listening for the UI events and reacts accordingly. 
    /// </summary>
    public class PreviewCameraController 
    {
        /// <summary>
        /// How fast is the camera moving.
        /// </summary>
        public float MoveSpeed
        {
            set;get;
        }
        /// <summary>
        /// How fast is the camera rotating.
        /// </summary>
        public float RotateSpeed
        {
            set;get;
        }
        /// <summary>
        /// How fast is the camera zooming. 
        /// </summary>
        public float ZoomSpeed
        {
            set;get;
        }

        /// <summary>
        /// Current state of the camera controller.
        /// </summary>
        public PreviewCameraStates CurrentState
        {
            private set;
            get;
        }
        /// <summary>
        /// Radius of the camera orbit.
        /// </summary>
        public float OrbitRadius
        {
            set;get;
        }
        /// <summary>
        /// Target of the camera.
        /// </summary>
        public Vector3 Target
        {
            set;get;
        }

        /// <summary>
        /// Create an instance of the preview camera controller for some specific preview instance.
        /// </summary>
        public PreviewCameraController( Preview preview )
        {
            mCamera = preview.Camera;
            
            mPreview = preview;
            mPreview.OnMouseDown += OnPreviewMouseDown;
            mPreview.OnMouseDrag += OnPreviewMouseDrag;
            mPreview.OnMouseUp += OnPreviewMouseUp;
            mPreview.OnScrollWheel += OnPreviewScrollWheel;

            Target = Vector3.zero;
            OrbitRadius = 1.0f;

            MoveSpeed = 0.0065f;
            RotateSpeed = 0.1f;
            ZoomSpeed = 0.1f;
        }

        /// <summary>
        /// Resets the state of the camera.
        /// </summary>
        public void ResetState()
        {
            CurrentState = PreviewCameraStates.None;
        }

        /// <summary>
        /// Sets all PreviewCameraStates to be enabled/disabled. 
        /// </summary>
        /// <param name="value">Enabled/Disabled</param>
        public void SetStatesEnabled(bool value)
        {
            SetStateEnabled(PreviewCameraStates.Dragging, value);
            SetStateEnabled(PreviewCameraStates.Rotating, value);
            SetStateEnabled(PreviewCameraStates.Zooming, value);
        }
        /// <summary>
        /// Sets some specific state to be enabled/disabled.  
        /// </summary>
        /// <param name="state"></param>
        /// <param name="value"></param>
        public void SetStateEnabled(PreviewCameraStates state, bool value)
        {
            var index = (int)state;
            index = Mathf.Clamp(index, (int)PreviewCameraStates.Dragging, (int)PreviewCameraStates.Zooming);
            mStates[index] = value;
        }
        /// <param name="state">State to check.</param>
        /// <returns>true if the state is enabled; false otherwise.</returns>
        public bool IsStateEnabled(PreviewCameraStates state)
        {
            var index = Mathf.Clamp((int)state, (int)PreviewCameraStates.Dragging, (int)PreviewCameraStates.Zooming);
            return mStates[index];
        }

        private void OnPreviewMouseDown(Event mouseEvent)
        {
            switch (mouseEvent.button)
            {
                case (int)Preview.Buttons.Left:
                case (int)Preview.Buttons.Middle:
                {
                    if (IsStateEnabled(PreviewCameraStates.Dragging))
                    {
                        CurrentState = PreviewCameraStates.Dragging;
                    }
                    break;
                }
                case (int)Preview.Buttons.Right:
                {
                    if (IsStateEnabled(PreviewCameraStates.Rotating))
                    {
                        CurrentState = PreviewCameraStates.Rotating;
                    }
                    break;
                }
                default: break;
            }
        }
        private void OnPreviewMouseDrag(Event mouseEvent)
        {
            switch (CurrentState)
            {
                case PreviewCameraStates.Dragging:
                {
                    MoveCamera(mouseEvent);
                    break;
                }
                case PreviewCameraStates.Rotating:
                {
                    RotateCamera(mouseEvent);
                    break;
                }
            }
        }
        private void OnPreviewMouseUp(Event mouseEvent)
        {
            ResetState();
        }
        private void OnPreviewScrollWheel(Event mouseEvent)
        {
            if (!IsStateEnabled(PreviewCameraStates.Zooming))
            {
                return;
            }

            ZoomCamera(mouseEvent);
        }
        
        private void MoveCamera(Event e)
        {
            Vector3 center = mPreview.SurfaceRect.center;
            var worldCenter = mCamera.ScreenToWorldPoint(center);
            var worldOffsetPosition = mCamera.ScreenToWorldPoint(center + new Vector3(-e.delta.x, e.delta.y));
            var worldDelta = worldOffsetPosition - worldCenter;
            Vector3 offset = mCamera.transform.TransformDirection(new Vector3(-e.delta.x, e.delta.y) * MoveSpeed);
            Target += offset;
            if (IsStateEnabled(PreviewCameraStates.Rotating))
            {
                UpdateCameraPosition();
            }
            else
            {
                mCamera.transform.position += worldDelta;
            }
        }
        private void RotateCamera(Event e)
        {
            Vector2 eulerAngles = mCamera.transform.eulerAngles;
            eulerAngles.y += RotateSpeed * e.delta.x;
            eulerAngles.x += RotateSpeed * e.delta.y;
            if (eulerAngles.x > 85 && eulerAngles.x < 110)
            {
                eulerAngles.x = 85;
            }
            mCamera.transform.eulerAngles = eulerAngles;
            UpdateCameraPosition();
        }
        private void ZoomCamera(Event e)
        {
            if (mCamera.orthographic)
            {
                const float MaxOrthographicSize = 1000.0f;
                const float MinOrthographicSize = 0.01f;

                mCamera.orthographicSize += e.delta.y * ZoomSpeed;
                mCamera.orthographicSize = Mathf.Min(Mathf.Max(MinOrthographicSize, mCamera.orthographicSize), MaxOrthographicSize);
            }
            else
            {
                float deltaValue = e.delta.y * ZoomSpeed;
                OrbitRadius += deltaValue;
                UpdateCameraPosition();
            }
        }

        private void UpdateCameraPosition()
        {
            var negDistance = new Vector3(0.0f, 0.0f, -OrbitRadius);
            mCamera.transform.position = mCamera.transform.rotation * negDistance + Target;
        }
        
        private Preview mPreview; 
        private PreviewCamera mCamera;

        private bool[] mStates = new bool[3] { false, false, false };
    }
}