using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace StartAssets.PowerfulPreview.Controls
{
    /// <summary>
    /// A class that implements timeline control. 
    /// It helps to select specifics time of the animations. 
    /// Also it can play animations automatically. 
    /// </summary>
	public class Timeline : Control
	{
        /// <summary>
        /// An interface for custom drawers for the timeline 
        /// </summary>
        public interface IDrawer
        {
            /// <summary>
            /// Implement that to draw something over the timeline.
            /// </summary>
            /// <param name="rect">The rect of the timeline.</param>
            void Draw( Rect rect );
        }

        /// <summary>
        /// Is the animation playing.
        /// </summary>
        public bool Playing
		{
            set;get;
		}
        /// <summary>
        /// Current time.
        /// </summary>
        public float CurTime
		{
			set
			{
				mCurTime = value;
				if( mCurTime >= mEndTime )
				{
					mCurTime = 0.0f;
				}

				if( mCurTime < 0.0f )
				{
					mCurTime = mEndTime;
				}
			}
			get
			{
				return mCurTime;
			}
		}
        /// <summary>
        /// Current normalized time. 
        /// </summary>
		public float CurNormalTime
		{
            set
            {
                mNormTime = value;
                mCurTime = mNormTime * mEndTime;
            }
			get
			{
                return mNormTime;
			}
		}
        /// <summary>
        /// Current frame.
        /// </summary>
        public int CurFrame
		{
			set
			{
				mCurTime = value / Framerate;
			}
			get
			{
                if( Visible )
                {
				    return ( int )( mCurTime * Framerate );
                }
                return 0; 
			}
		}
        /// <summary>
        /// Animation's last frame. 
        /// </summary>
		public int EndFrame
		{
			get
			{
				return ( int )( mEndTime * Framerate );
			}
		}
        /// <summary>
        /// Animation length. 
        /// </summary>
        public float EndTime
		{
			set
			{
				mEndTime = Mathf.Max( 0.0f, value );
			}
			get
			{
				return mEndTime;
			}
		}
        /// <summary>
        /// Animation's framerate. 
        /// </summary>
        public float Framerate
		{
			set
			{
				mFramerate = Mathf.Max( 0.0f, value );
			}
            get
            {
                return mFramerate;
            }
		}
        /// <summary>
        /// Current animation play speed.
        /// </summary>
        public float Speed
		{
            set;get;
		}

        /// <summary>
        /// Is the timeline is visible. 
        /// </summary>
        public bool Visible
        {
            set;
            get;
        }

		public Timeline()
		{
            mDrawersRect = new Rect(0, 0, 0, 0);
            mDrawers = new SortedList<int, IDrawer>();

            if( EditorGUIUtility.isProSkin )
            {
                mPlayIcon = EditorGUIUtility.Load( "icons/d_PlayButton.png" ) as Texture2D;
                mPauseIcon = EditorGUIUtility.Load( "icons/d_PauseButton.png" ) as Texture2D;
            }
            else
            {
                mPlayIcon = EditorGUIUtility.Load( "icons/PlayButton.png" ) as Texture2D;
                mPauseIcon = EditorGUIUtility.Load( "icons/PauseButton.png" ) as Texture2D;
            }

			mCurTime = 0.0f;
			mEndTime = 0.0f;

			Playing = false;

			mCurSliderPos = 0.0f;
			mNormTime = 0.0f;

			Framerate = 30;

			Speed = 1.0f;

            Height = mPlayIcon.height;
            Width = Screen.width; 

			mStartTranslating = false;

            InitializeTimer();
        }

        /// <summary>
        /// Adds a custom timeline drawer on some specific layer.
        /// </summary>
        /// <param name="drawer">Custom drawer</param>
        /// <param name="layer">The layer of the drawer</param>
        public void AddDrawer( IDrawer drawer, int layer )
        {
            if( mDrawers.ContainsKey( layer ) )
            {
                throw new ArgumentException(string.Format("{0} drawer already is at {1} layer. Please change it.",
                    mDrawers[layer], layer) );
            }

            mDrawers.Add(layer, drawer);
        }
        /// <summary>
        /// Removed a drawer from the layer. 
        /// </summary>
        /// <param name="layer"></param>
        public void RemoveDrawer( int layer )
        {
            mDrawers.RemoveAt(layer);
        }

        /// <summary>
        /// Updates the timeline. 
        /// </summary>
        public override void Update()
		{
            if( mButtonStyle == null )
            {
                mButtonStyle = new GUIStyle( ( GUIStyle )"TimeScrubberButton" );
                mButtonActiveStyle = new GUIStyle( ( GUIStyle )"TimeScrubberButton" );
                mButtonActiveStyle.normal = mButtonActiveStyle.active;
            }

			mButtonRect = new Rect( PositionX, PositionY, 34, 22 );
			mTimelineRect = new Rect( mButtonRect.xMax, PositionY, Width - mButtonRect.width, mButtonRect.height );
            mBackgroundRect = mTimelineRect;
            mBackgroundRect.xMin = mButtonRect.xMin; 

			mNormTime = mCurTime / mEndTime;
			mCurSliderPos = mTimelineRect.xMin + mTimelineRect.width * mNormTime;

            float deltaTime = 0.0f;
            UpdateTimer( out deltaTime );
            if ( Playing )
            {
                CurTime += ( float )deltaTime * Speed;
            }

            Draw();
        }
        /// <summary>
        /// Draws the timeline. 
        /// </summary>
        public void Draw()
		{
            if ( Visible )
            {
                GUI.Box( mBackgroundRect, "", ( GUIStyle )"TimeScrubber" );

                if ( GUI.Button( mButtonRect, "", Playing ? mButtonActiveStyle : mButtonStyle ) )
                {
                    Playing = !Playing;
                }
                GUI.DrawTexture( new Rect( mButtonRect.xMin + 6, mButtonRect.yMin + 1, 21, 20 ), PlayButton );

                foreach (var item in mDrawers)
                {
                    item.Value.Draw(mDrawersRect);
                }

                if ( EditorGUIUtility.isProSkin )
                {
                    EditorGUI.DrawRect( new Rect( mCurSliderPos, mTimelineRect.yMin, 1, mTimelineRect.height - 2 ), new Color( 0.72f, 0.72f, 0.72f ) );
                    EditorGUI.DrawRect( new Rect( mCurSliderPos + 1, mTimelineRect.yMin, 1, mTimelineRect.height - 2 ), new Color( 0.62f, 0.62f, 0.62f ) );
                }
                else
                {
                    EditorGUI.DrawRect( new Rect( mCurSliderPos, mTimelineRect.yMin, 2, mTimelineRect.height - 2 ), Color.white );
                }

                if( Event.current.type == EventType.Repaint )
                {
                    mDrawersRect = mTimelineRect;
                    mDrawersRect.height -= 2.0f;
                }
            }
		}
        /// <summary>
        /// Handles user input events. 
        /// </summary>
        /// <returns></returns>
        public override bool HandleEvents()
		{
            if( !Visible )
            {
                return false; 
            }
            var e = Event.current;
            var returnValue = false; 
            if( e.IsButtonDown( (int)Preview.Buttons.Left ) )
			{
				if( mTimelineRect.Contains( e.mousePosition ) )
				{
					mStartTranslating = true;
					var curTime = mEndTime * ( ( e.mousePosition.x - mTimelineRect.x ) / mTimelineRect.width );
					curTime = curTime % EndTime;
					if( curTime < 0.0f )
					{
						curTime = EndTime + curTime;
					}
					CurTime = curTime;
                    returnValue = true;
				}
				else
				{
					mStartTranslating = false;
				}
			}
			else if( e.IsButton((int)Preview.Buttons.Left) && mStartTranslating )
			{
                var curTime = mEndTime * ( ( e.mousePosition.x - mTimelineRect.x ) / mTimelineRect.width );
				curTime = curTime % EndTime;
				if( curTime < 0.0f )
				{
					curTime = EndTime + curTime;
				}
				CurTime = curTime;
                returnValue = true;
			}
			else if( Event.current.type == EventType.MouseMove )
			{
				mStartTranslating = false;
			}

            if( e.IsButtonUp((int)Preview.Buttons.Left) )
            {
                mStartTranslating = false;
            }

            return returnValue; 
        }
        /// <summary>
        /// Returns the time of the animation's frame. 
        /// </summary>
        /// <param name="frame">Animation's frame. </param>
        /// <returns>The time of the animation's frame.</returns>
		public float GetTime( int frame )
		{
			return frame / Framerate; 
		}

        /// <summary>
        /// Initializes the timer, that will update the animation in proper way.
        /// </summary>
        protected void InitializeTimer()
        {
            if( EditorApplication.isPlaying )
            {
                mPrevTime = Time.time;
            }
            else
            {
                mPrevTime = ( float )EditorApplication.timeSinceStartup;
            }
        }
        
        /// <summary>
        /// Updates the animation timer.
        /// </summary>
        /// <param name="deltaTime">Current delta time, to move the animation time. </param>
        /// <returns>Returns true, if current time is the time for new frame; false otherwise. </returns>
        protected bool UpdateTimer( out float deltaTime )
        {
            var currentTime = EditorApplication.isPlaying ? Time.time : (float)EditorApplication.timeSinceStartup;
            deltaTime = currentTime - mPrevTime;
            mPrevTime = currentTime;
            return true;
        }
        
        /// <summary>
        /// Returns a button texture, based on the current timeline state. 
        /// </summary>
        protected Texture2D PlayButton
		{
			get
			{
				if( Playing )
				{
					return mPauseIcon;
				}
				return mPlayIcon;
			}
		}

        private float mPrevTime; 

		private bool mStartTranslating;
		private float mFramerate;
		private float mNormTime;
		private float mCurTime;
		private float mEndTime;
		private float mCurSliderPos;

        private Rect mBackgroundRect; 
        private Rect mButtonRect;
        private Rect mTimelineRect;

		private Texture2D mPlayIcon;
		private Texture2D mPauseIcon;

        private GUIStyle mButtonStyle;
        private GUIStyle mButtonActiveStyle;

        private SortedList<int, IDrawer> mDrawers;

        private Rect mDrawersRect; 
	}
}