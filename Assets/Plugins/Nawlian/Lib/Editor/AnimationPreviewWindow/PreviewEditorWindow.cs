using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using System.Threading.Tasks;
using StartAssets.PowerfulPreview;

namespace Nawlian.Lib.EditorTools.AnimationPreviewWindow
{
    /// <summary>
    /// Defines an editor window hosting a preview
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public abstract class PreviewEditorWindow<T, U> : OdinEditorWindow
        where T : EmbeddedPreview<U>
        where U : Object
    {
        #region Properties

        public T PreviewEditor { get; set; }
        public U DataPreview { get; set; }
        public GameObject PreviewGameObject { get; set; }

        #endregion

        private void CreateIfNull()
		{
            if (PreviewEditor == null && DataPreview != null)
            {
                PreviewEditor = UnityEditor.Editor.CreateEditor(DataPreview, typeof(T)) as T;
                PreviewEditor.OnCreate(DataPreview, PreviewGameObject);
                PreviewEditor.HasPreviewGUI();
            }
        }

        protected void RenderPreview(Rect rect)
		{
            CreateIfNull();
            PreviewEditor.OnInteractivePreviewGUI(rect, EditorStyles.whiteLabel);
        }
	}
}