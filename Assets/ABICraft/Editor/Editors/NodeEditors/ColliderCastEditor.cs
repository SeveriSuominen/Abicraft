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
    [CustomNodeEditor(typeof(ColliderCastNode))]
    public class ColliderCastNodeEditor : NodeEditor
    {
        public Texture2D icon;

        ColliderCastNode node;
        AbicraftObject obj;
        Animator animator;

        int selectedIndex;

        bool noColliders = false, colliderHasTrigger = false;

        public override void OnHeaderGUI(GUIStyle style)
        {
            // Initialization
            if (node == null)
            {
                node = target as ColliderCastNode;
            }

            base.OnHeaderGUI(style);
            Rect dotRect = GUILayoutUtility.GetLastRect();
            dotRect.size = new Vector2(16, 16);
            dotRect.y += 6;

            if(node.collider != null)
            {
                var colliders = node.collider.GetComponents<Collider>();

                if (colliders.Length == 0)
                {
                    noColliders = true;
                }
                else
                {
                    noColliders = false;
                }

                if (colliders.Length > 0)
                {
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (colliders[i].isTrigger)
                            colliderHasTrigger = true;
                    }
                }
            }
            if (!node.collider || noColliders || (!noColliders && !colliderHasTrigger))
            {
                GUI.color = Color.red;
                GUI.DrawTexture(dotRect, NodeEditorResources.dot);
                GUI.color = Color.white;
            }
        }

        public override void OnBodyGUI()
        {
            node = target as ColliderCastNode;

            obj = node.GetInputValue<AbicraftObject>(null, "Obj");

            //base.OnBodyGUI();
            // Update serialized object's representation
            serializedObject.Update();

            GUIStyle gstyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            gstyle.normal.textColor = Color.white;
            // if(node.icon != null)
            // GUI.DrawTexture(new Rect(10, 10, 60, 60), node.icon, ScaleMode.ScaleToFit, true, 10.0F);
            GUILayout.BeginVertical();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("In"),  new GUIContent("In"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Out"), new GUIContent("Out"));
            GUILayout.EndVertical();

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);
            
            GUILayout.BeginVertical();
            Color col = GUI.color;

            if (!node.collider)
                GUI.color = ERRORCOLOR;
            if (noColliders)
                GUI.color = ERRORCOLOR;
            if (!noColliders && !colliderHasTrigger)
                GUI.color = ERRORCOLOR;

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("collider"), new GUIContent("Collider"));

            GUI.color = col;
            GUILayout.EndVertical();

            if (noColliders)
            {
                GuiSpace(5);
                GUILayout.Label("Selected Collinder object dont have any Unity collinder components attached. At least one collinder component is required to cast area.", gstyle);
            }

            if (!noColliders && !colliderHasTrigger)
            {
                GuiSpace(5);

                GUILayout.Label("At least one of the colliders need to have IsTrigger setting true", gstyle);
                GuiSpace(5);

                if (GUILayout.Button("Fix all collinders")){
                    var colliders = node.collider.GetComponents<Collider>();
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        colliders[i].isTrigger = true;
                    }
                }
            }

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            GUILayout.BeginVertical();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Casted"), new GUIContent("Casted"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Collisions"), new GUIContent("Collisions"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Position"), new GUIContent("Position"));
            GUILayout.EndVertical();
        }
    }
}

