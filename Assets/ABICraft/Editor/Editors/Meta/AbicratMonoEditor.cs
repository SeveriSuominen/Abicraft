using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbicratInspectorEditor : Editor
{
    protected void GuiLine(int i_height = 1)
    {
        Rect rect = default;
        try
        {
            rect = EditorGUILayout.GetControlRect(false, i_height);
        }
        catch { }

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1));
    }

    protected void GuiSpace(int i_height = 1)
    {
        Rect rect = default;
        try
        {
            rect = EditorGUILayout.GetControlRect(false, i_height);
        }
        catch { }

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0, 0, 0, 0));
    }
}
