using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using static AbicraftNodeEditor.NodeEditor;

using AbicraftNodeEditor;
using AbicraftNodes.Action;
using AbicraftMonos;

namespace AbicraftNodes.Editors
{
    [CustomNodeEditor(typeof(AudioNode))]
    public class AudioEditor : NodeEditor
    {
        public Texture2D icon;

        AudioNode node;
        AbicraftObject obj;
        Animator animator;

        int selectedIndex;

        public override void OnHeaderGUI(GUIStyle style)
        {
            // Initialization
            if (node == null)
            {
                node = target as AudioNode;
            }

            base.OnHeaderGUI(style);
            Rect dotRect = GUILayoutUtility.GetLastRect();
            dotRect.size = new Vector2(16, 16);
            dotRect.y += 6;

            if (!node.Clip)
            {
                GUI.color =  Color.red;
                GUI.DrawTexture(dotRect, NodeEditorResources.dot);
                GUI.color = Color.white;
            }
        }

        public override void OnBodyGUI()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;

            node = target as AudioNode;

            GUILayout.BeginHorizontal();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("In"),  new GUIContent("In"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Out"), new GUIContent("Out"));
            GUILayout.EndHorizontal();

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Obj"));

            if (node.SetAudioSourceSettings)
            {
                GuiSpace(5);
                GuiLine(1);
                GuiSpace(5);

                GUILayout.Label("Priority", style);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("priority"), new GUIContent(""));

                GUILayout.Label("Volume", style);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"), new GUIContent(""));

                GUILayout.Label("Pitch", style);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pitch"), new GUIContent(""));

            }

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            GUILayout.Label("Clip", style);
            GuiSpace(5);

            Color bgcol = GUI.backgroundColor;

            if (!node.Clip)
            {
                GUI.backgroundColor = ERRORCOLOR;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Clip"), new GUIContent(""));

            GUI.backgroundColor = bgcol;

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            GUILayout.BeginHorizontal();
           
            node.SetAudioSourceSettings = GUILayout.Toggle(node.SetAudioSourceSettings, new GUIContent(""));
            GUILayout.Label("Set AudioSource Settings", style);
            GUILayout.EndHorizontal();
            GuiSpace(5);
            //base.OnBodyGUI();
        }
    }
}

