using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StartAssets.PowerfulPreview
{
    /*
     * The MIT License (MIT)
     * 
     * Copyright (c) 2014-2015, Unity Technologies
     * 
     * Permission is hereby granted, free of charge, to any person obtaining a copy
     * of this software and associated documentation files (the "Software"), to deal
     * in the Software without restriction, including without limitation the rights
     * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
     * copies of the Software, and to permit persons to whom the Software is
     * furnished to do so, subject to the following conditions:
     * 
     * The above copyright notice and this permission notice shall be included in
     * all copies or substantial portions of the Software.
     * 
     * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
     * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
     * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
     * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
     * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
     * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
     * THE SOFTWARE.     
     *        
     */
    ///The only reason, why I modified this class, is cause it uses Screen.width and Screen.height 
    ///to scale for screen size, but I need to use custom size(preview size). 
    [RequireComponent(typeof(Canvas))]
    [ExecuteInEditMode]
    public class PreviewCanvasScaler : UIBehaviour
    {
        [SerializeField] private CanvasScaler.ScaleMode m_UiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        public CanvasScaler.ScaleMode uiScaleMode { get { return m_UiScaleMode; } set { m_UiScaleMode = value; } }

        [SerializeField] protected float m_ReferencePixelsPerUnit = 100;
        public float referencePixelsPerUnit { get { return m_ReferencePixelsPerUnit; } set { m_ReferencePixelsPerUnit = value; } }

        [SerializeField] protected float m_ScaleFactor = 1;
        public float scaleFactor { get { return m_ScaleFactor; } set { m_ScaleFactor = value; } }


        [SerializeField] protected Vector2 m_ReferenceResolution = new Vector2(800, 600);
        public Vector2 referenceResolution { get { return m_ReferenceResolution; } set { m_ReferenceResolution = value; } }

        [SerializeField] protected CanvasScaler.ScreenMatchMode m_ScreenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        public CanvasScaler.ScreenMatchMode screenMatchMode { get { return m_ScreenMatchMode; } set { m_ScreenMatchMode = value; } }

        [SerializeField] protected float m_MatchWidthOrHeight = 0;
        public float matchWidthOrHeight { get { return m_MatchWidthOrHeight; } set { m_MatchWidthOrHeight = value; } }

        private const float kLogBase = 2;

        [SerializeField] protected CanvasScaler.Unit m_PhysicalUnit = CanvasScaler.Unit.Points;
        public CanvasScaler.Unit physicalUnit { get { return m_PhysicalUnit; } set { m_PhysicalUnit = value; } }

        [SerializeField] protected float m_FallbackScreenDPI = 96;
        public float fallbackScreenDPI { get { return m_FallbackScreenDPI; } set { m_FallbackScreenDPI = value; } }

        [SerializeField] protected float m_DefaultSpriteDPI = 96;
        public float defaultSpriteDPI { get { return m_DefaultSpriteDPI; } set { m_DefaultSpriteDPI = value; } }

        [SerializeField] protected float m_DynamicPixelsPerUnit = 1;
        public float dynamicPixelsPerUnit { get { return m_DynamicPixelsPerUnit; } set { m_DynamicPixelsPerUnit = value; } }

        public Vector2 screenSize
        {
            set
            {
                mScreenSize = value;
            }
            get
            {
                return mScreenSize; 
            }
        }
        
        private Canvas m_Canvas;
        [System.NonSerialized]
        private float m_PrevScaleFactor = 1;
        [System.NonSerialized]
        private float m_PrevReferencePixelsPerUnit = 100;
        private Vector2 mScreenSize = new Vector2( 800, 800 );

        protected PreviewCanvasScaler() {}

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Canvas = GetComponent<Canvas>();
            Handle();
        }

        protected override void OnDisable()
        {
            SetScaleFactor(1);
            SetReferencePixelsPerUnit(100);
            base.OnDisable();
        }

        protected virtual void Update()
        {
            Handle();
        }

        protected virtual void Handle()
        {
            if (m_Canvas == null || !m_Canvas.isRootCanvas)
                return;

            if (m_Canvas.renderMode == RenderMode.WorldSpace)
            {
                HandleWorldCanvas();
                return;
            }

            switch (m_UiScaleMode)
            {
                case CanvasScaler.ScaleMode.ConstantPixelSize: HandleConstantPixelSize(); break;
                case CanvasScaler.ScaleMode.ScaleWithScreenSize: HandleScaleWithScreenSize(); break;
                case CanvasScaler.ScaleMode.ConstantPhysicalSize: HandleConstantPhysicalSize(); break;
            }
        }

        protected virtual void HandleWorldCanvas()
        {
            SetScaleFactor(m_DynamicPixelsPerUnit);
            SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit);
        }

        protected virtual void HandleConstantPixelSize()
        {
            SetScaleFactor(m_ScaleFactor);
            SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit);
        }

        protected virtual void HandleScaleWithScreenSize()
        {
            float scaleFactor = 0;
            switch (m_ScreenMatchMode)
            {
                case CanvasScaler.ScreenMatchMode.MatchWidthOrHeight:
                {
                    // We take the log of the relative width and height before taking the average.
                    // Then we transform it back in the original space.
                    // the reason to transform in and out of logarithmic space is to have better behavior.
                    // If one axis has twice resolution and the other has half, it should even out if widthOrHeight value is at 0.5.
                    // In normal space the average would be (0.5 + 2) / 2 = 1.25
                    // In logarithmic space the average is (-1 + 1) / 2 = 0
                    float logWidth = Mathf.Log(screenSize.x / m_ReferenceResolution.x, kLogBase);
                    float logHeight = Mathf.Log(screenSize.y / m_ReferenceResolution.y, kLogBase);
                    float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, m_MatchWidthOrHeight);
                    scaleFactor = Mathf.Pow(kLogBase, logWeightedAverage);
                    break;
                }
                case CanvasScaler.ScreenMatchMode.Expand:
                {
                    scaleFactor = Mathf.Min(screenSize.x / m_ReferenceResolution.x, screenSize.y / m_ReferenceResolution.y);
                    break;
                }
                case CanvasScaler.ScreenMatchMode.Shrink:
                {
                    scaleFactor = Mathf.Max(screenSize.x / m_ReferenceResolution.x, screenSize.y / m_ReferenceResolution.y);
                    break;
                }
            }

            SetScaleFactor(scaleFactor);
            SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit);
        }

        protected virtual void HandleConstantPhysicalSize()
        {
            float currentDpi = Screen.dpi;
            float dpi = (currentDpi == 0 ? m_FallbackScreenDPI : currentDpi);
            float targetDPI = 1;
            switch (m_PhysicalUnit)
            {
                case CanvasScaler.Unit.Centimeters: targetDPI = 2.54f; break;
                case CanvasScaler.Unit.Millimeters: targetDPI = 25.4f; break;
                case CanvasScaler.Unit.Inches:      targetDPI =     1; break;
                case CanvasScaler.Unit.Points:      targetDPI =    72; break;
                case CanvasScaler.Unit.Picas:       targetDPI =     6; break;
            }

            SetScaleFactor(dpi / targetDPI);
            SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit * targetDPI / m_DefaultSpriteDPI);
        }

        protected void SetScaleFactor(float scaleFactor)
        {
			if (scaleFactor == m_PrevScaleFactor)
				return;

			m_Canvas.scaleFactor = scaleFactor;
            m_PrevScaleFactor = scaleFactor;
        }

        protected void SetReferencePixelsPerUnit(float referencePixelsPerUnit)
        {
            if (referencePixelsPerUnit == m_PrevReferencePixelsPerUnit)
                return;

            m_Canvas.referencePixelsPerUnit = referencePixelsPerUnit;
            m_PrevReferencePixelsPerUnit = referencePixelsPerUnit;
        }
    }
}
