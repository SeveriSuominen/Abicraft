using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbicratInspectorEditor : Editor
{
    protected void Helpbox(string text, MessageType type)
    {
        try
        {
            GuiSpace(5);

            GUIStyle style = GUI.skin.GetStyle("helpbox");
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperLeft;

            EditorGUIUtility.SetIconSize(new Vector2(15, 15));
            EditorGUILayout.HelpBox(text, type);
            GuiSpace(5);
        }
        catch (ArgumentException e) { }
    }

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
