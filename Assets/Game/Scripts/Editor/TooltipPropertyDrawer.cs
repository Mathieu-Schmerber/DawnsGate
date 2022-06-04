using UnityEditor; // Must be in Editor folder!
using UnityEngine;

[CustomPropertyDrawer(typeof(Tooltip))]
public class TooltipPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label);

        Rect btnRect = new Rect(position.position.x + GetLabelSize(label).x, position.position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
        GUI.Button(btnRect, new GUIContent(EditorGUIUtility.IconContent("_Help@2x").image, ((Tooltip)attribute).text), GetStyle());
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

    private Vector2 GetLabelSize(GUIContent label) => GUI.skin.label.CalcSize(label);

    public void GenerateTooltip(string text)
    {
        var propRect = GUILayoutUtility.GetLastRect();
        GUI.Label(propRect, new GUIContent("", text));
    }
}
