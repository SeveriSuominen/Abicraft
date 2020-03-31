using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AbicraftNodeEditor.NodeEditor;
using AbicraftNodeEditor;
using AbicraftNodes.Action;
using UnityEditor;
using AbicraftCore;
using AbicraftMonos;
using System;

namespace AbicraftNodes.Editors
{
    [CustomNodeEditor(typeof(HasAttributeAmountNode))]
    public class HasAttributeAmountEditor : NodeEditor
    {

        HasAttributeAmountNode node;
        AbicraftObject obj;

        int selectedIndex;

        public override void OnHeaderGUI(GUIStyle style)
        {
            // Initialization
            if (node == null)
            {
                node = target as HasAttributeAmountNode;
            }

            base.OnHeaderGUI(style);
            Rect dotRect = GUILayoutUtility.GetLastRect();
            dotRect.size = new Vector2(16, 16);
            dotRect.y += 6;

            /*GUI.color =  node.data.statusColor;
            GUI.DrawTexture(dotRect, NodeEditorResources.dot);
            GUI.color = Color.white;*/
        }

        public override void OnBodyGUI()
        {

            node = target as HasAttributeAmountNode;

            if (!node.abicraft)
                node.abicraft = AbicraftGlobalContext.abicraft;

            var abicraft = node.abicraft;

            List<GUIContent> avaibleAttrContents = new List<GUIContent>();
            List<AbicraftAttribute> attrs = new List<AbicraftAttribute>();

            Texture2D icon = null;

            avaibleAttrContents.Add(new GUIContent("None"));
            attrs.Add(null);

            if (AbicraftGlobalContext.HasValidAbicraftInstance())
            {
                for (int i = 0; i < abicraft.dataFile.GlobalAttributes.Count; i++)
                {
                    AbicraftAttribute attr = abicraft.dataFile.GlobalAttributes[i];
                    avaibleAttrContents.Add(new GUIContent(attr.AttributeName, attr.AttributeIcon));
                    attrs.Add(attr);
                }
            }

            base.OnBodyGUI();

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);
            EditorGUIUtility.SetIconSize(new Vector2(16, 16));

            GUIStyle gstyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));

            GUIStyle hstyle = new GUIStyle();
            hstyle.fontSize = 11;
            hstyle.normal.textColor = Color.white;

            if (AbicraftGlobalContext.HasValidAbicraftInstance())
            {
                for (int i = 0; i < node.allSelectedIndices.Count; i++)
                {
                    try
                    {
                        GUILayout.BeginVertical(gstyle);
                        GUILayout.BeginHorizontal();

                        GUIStyle style = new GUIStyle();
                        style.fontSize = 12;
                        style.normal.textColor = Color.white;

                        GUIContent content = null;

                        for (int j = 0; j < attrs.Count; j++)
                        {
                            if (attrs[j] == node.allSelectedIndices[i].attribute)
                            {
                                content = avaibleAttrContents[j];
                            }
                        }

                        if (content != null)
                        {
                            GUILayout.Label(content, style);

                            if (GUILayout.Button("X", GUILayout.MaxWidth(15), GUILayout.MaxHeight(15)))
                                node.allSelectedIndices.RemoveAt(i);
                        }

                        GUILayout.EndHorizontal();
                        GuiLine(1);

                        GUILayout.Label("Mode", hstyle);
                        node.allSelectedIndices[i].conditionMode = (HasAttributeAmountNode.ConditionMode)EditorGUILayout.EnumPopup(node.allSelectedIndices[i].conditionMode);
                        GUILayout.Label("Amount", hstyle);
                        node.allSelectedIndices[i].amount = EditorGUILayout.IntField(node.allSelectedIndices[i].amount);
                        GUILayout.EndVertical();

                        if (i != node.allSelectedIndices.Count - 1)
                        {
                            GuiSpace(1);
                            GuiLine(1);
                            GuiSpace(1);
                        }
                    }
                    catch (ArgumentException e){}
                }
            }
            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);
            EditorGUIUtility.SetIconSize(Vector2.zero);

            if (!AbicraftGlobalContext.HasValidAbicraftInstance())
            {
                Helpbox("Could not fetch attributes, abicraft data file reference missing", MessageType.Error);
            }

            GUILayout.Label("Available Attributes", NodeEditorGUILayout.GetFieldStyle("In"));
            node.selectedIndex = EditorGUILayout.Popup(node.selectedIndex, avaibleAttrContents.ToArray()); //selectedIndex = EditorGUILayout.Popup(selectedIndex, strings);

            if (node.selectedIndex != node.lastIndex)
            {
                if (node.selectedIndex != 0)
                {
                    if (attrs[node.selectedIndex] != null && !node.allSelectedIndices.Contains(new HasAttributeAmountNode.AttributeAmount(attrs[node.selectedIndex])))
                        node.allSelectedIndices.Add(new HasAttributeAmountNode.AttributeAmount(attrs[node.selectedIndex]));

                    node.lastIndex = node.selectedIndex;
                }
            }
        }
    }
}

