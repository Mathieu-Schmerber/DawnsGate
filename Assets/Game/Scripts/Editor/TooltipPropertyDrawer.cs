using UnityEditor; // Must be in Editor folder!
using UnityEngine;

[CustomPropertyDrawer(typeof(Tooltip))]
public class TooltipPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string tooltip = ((Tooltip)attribute).text;
        Vector2 labelSize = GetLabelSize(property.name);
        Rect btnRect = new Rect(position.position.x + labelSize.x, position.position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

        label.tooltip = tooltip;
        EditorGUI.PropertyField(position, property, label);
        GUI.Button(btnRect, new GUIContent(EditorGUIUtility.IconContent("_Help@2x").image, tooltip), GetStyle());
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label);
    }

    static GUIStyle GetStyle()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.margin = new RectOffset();
        return style;
    }

    private Vector2 GetLabelSize(string label) => GUI.skin.label.CalcSize(new GUIContent(label));
}
