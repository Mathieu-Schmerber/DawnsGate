using UnityEngine;

public class Tooltip : PropertyAttribute
{
    public string text;

    public Tooltip(string tooltipText)
    {
        this.text = tooltipText;
    }
}