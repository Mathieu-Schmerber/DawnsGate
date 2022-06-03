using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// This class is a base class for all controls in the Powerful Preview asset. 
    /// It has a list of methods, that should be implemented in the child 
    /// classes. It's some kind of soft version of the interface. 
    /// </summary>
    public abstract class Control : System.IDisposable
    {
        public Rect ControlRect
        {
            set
            {
                Position = value.position;
                Size = value.size;
            }
            get
            {
                return new Rect( Position, Size );
            }
        }

        public Vector2 Position
        {
            set
            {
                PositionX = value.x;
                PositionY = value.y;
            }
            get
            {
                return mPosition;
            }
        }
        public float PositionX
        {
            set
            {
                mPosition.x = Mathf.Clamp( value, 0.0f, Screen.width );
            }
            get
            {
                return mPosition.x;
            }
        }
        public float PositionY
        {
            set
            {
                mPosition.y = Mathf.Clamp( value, 0.0f, Screen.height );
            }
            get
            {
                return mPosition.y;
            }
        }
 
        public Vector2 Size
        {
            set
            {
                Width = value.x;
                Height = value.y;
            }
            get
            {
                return mSize;
            }
        }
        public float Width
        {
            set
            {
                mSize.x = Mathf.Clamp( value, 0.0f, Screen.width );
            }
            get
            {
                return mSize.x;
            }
        }
        public float Height
        {
            set
            {
                mSize.y = Mathf.Clamp( value, 0.0f, Screen.height );
            }
            get
            {
                return mSize.y;
            }
        }

        public Vector2 MinSize
        {
            set
            {
                MinWidth = value.x;
                MinHeight = value.y;
            }
            get
            {
                return mMinSize;
            }
        }
        public float MinWidth
        {
            set
            {
                mMinSize.x = Mathf.Clamp( value, 0.0f, Screen.width );
            }
            get
            {
                return mMinSize.x;
            }

        }
        public float MinHeight
        {
            set
            {
                mMinSize.y = Mathf.Clamp( value, 0.0f, Screen.height );
            }
            get
            {
                return mMinSize.y;
            }
        }

        public Vector2 MaxSize
        {
            set
            {
                MaxWidth = value.x;
                MaxHeight = value.y;
            }
            get
            {
                return mMaxSize;
            }
        }
        public float MaxWidth
        {
            set
            {
                mMaxSize.x = Mathf.Clamp( value, 0.0f, Screen.width );
            }
            get
            {
                return mMaxSize.x;
            }
        }
        public float MaxHeight
        {
            set
            {
                mMaxSize.y = Mathf.Clamp( value, 0.0f, Screen.height );
            }
            get
            {
                return mMaxSize.y;
            }
        }

        public Control( float width = 32.0f, float height = 32.0f )
        {
            MinWidth = MaxWidth = Width = width;
            MinHeight = MaxHeight = Height = height;
        }

        public virtual void Update() { }
        public virtual void Dispose() { }
        public virtual bool HandleEvents() { return false; }
        
        private Vector2 mPosition;
        private Vector2 mSize;
        private Vector2 mMinSize;
        private Vector2 mMaxSize;
    }
}