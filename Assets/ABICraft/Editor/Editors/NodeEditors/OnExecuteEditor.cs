using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AbicraftNodeEditor.NodeEditor;
using AbicraftNodeEditor;
using AbicraftNodes;
using UnityEditor;
using AbicraftCore;

namespace AbicraftNodes.Editors
{
    [CustomNodeEditor(typeof(OnExecuteNode))]
    public class OnExecuteEditor : NodeEditor
    {
        public Texture2D icon;

        OnExecuteNode node;
        public override void OnHeaderGUI(GUIStyle style)
        {
            // Initialization
            if (node == null)
            {
                node = target as OnExecuteNode;
            }

            base.OnHeaderGUI(style);
            Rect dotRect = GUILayoutUtility.GetLastRect();
            dotRect.size = new Vector2(16, 16);
            dotRect.y += 6;
            /*
            GUI.color = Color.red;
            GUI.DrawTexture(dotRect, NodeEditorResources.dot);
            GUI.color = Color.white;*/
        }

        public override void OnBodyGUI()
        {
            if (icon == null)
                icon = Resources.Load("Icons/baseline_play_arrow_white_18dp") as Texture2D;

            if (node == null) node = target as OnExecuteNode;

            // Update serialized object's representation
            serializedObject.Update();


            // if(node.icon != null)
            // GUI.DrawTexture(new Rect(10, 10, 60, 60), node.icon, ScaleMode.ScaleToFit, true, 10.0F);

            GUILayout.Space(25);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Out"), GUIContent.none);
            GUILayout.Space(5);

            Rect dotRect = new Rect(80, 45, 100, 0);
            dotRect.size = new Vector2(50, 50);

            if (node.graph)
            {
                GUIStyle styleName = new GUIStyle();
                styleName.normal.textColor = Color.white;
                styleName.alignment = TextAnchor.MiddleCenter;
                styleName.fontSize = 18;

                GuiSpace(20);
                GuiLine(1);
                GuiSpace(5);

                GUILayout.Label(node.graph.AbilityName, styleName);

                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleLeft;

                GUI.color = Color.white;
                if (node.graph && node.graph.icon)
                {
                    GUI.DrawTexture(dotRect, node.graph.icon);
                }
                else
                {
                    GUI.DrawTexture(dotRect, icon);
                }
                GUI.color = Color.white;

                GuiSpace(5);
                GuiLine(1);
                GuiSpace(5);

                GUIStyle estyle = new GUIStyle(EditorStyles.textField);
                estyle.border = new RectOffset(0, 0, 0, 0);
                estyle.normal.textColor = Color.white;

                if (!AbicraftGlobalContext.HasValidAbicraftInstance())
                {
                    Helpbox("Abicraft component with data file reference in the current scene is required", MessageType.Error);
                    GuiLine(1);
                }

                GUILayout.Label("Ability name", style);
                node.graph.AbilityName = EditorGUILayout.TextField(node.graph.AbilityName);

                GUILayout.Label("Ability icon", style);
                node.graph.icon = EditorGUILayout.ObjectField(node.graph.icon, typeof(Texture2D), false) as Texture2D;

                GUILayout.Label("Ability cooldown seconds", style);
                node.graph.Cooldown = EditorGUILayout.FloatField(node.graph.Cooldown);

                GuiSpace(5);
                GuiLine(1);
                GuiSpace(5);

                GUILayout.Label("Ability is passive", style);
                node.graph.Passive = EditorGUILayout.Toggle(node.graph.Passive);
                GUI.color = Color.white;

                if (node.graph.Passive)
                {
                    GUIStyle gstyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
                    gstyle.normal.textColor = Color.white;

                    Helpbox("Passive abilites are looped until life time ends or passive ability is interupted", MessageType.Info);

                    GuiSpace(5);
                    GUILayout.Label("Default lifetime seconds", style);
                    node.graph.DefaultLifetime = EditorGUILayout.FloatField(node.graph.DefaultLifetime);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
