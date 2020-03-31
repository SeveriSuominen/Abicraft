using AbicraftCore;
using AbicraftMonos;
using AbicraftNodeEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AbicraftNodes.Editors
{
    [CustomEditor(typeof(AbicraftAttribute))]
    public class AbicraftAttributeEditor : AbicratInspectorEditor
    {
        int instantiatePoolAmount;
        Texture2D iconUP, iconDOWN;


        public void Swap<T>(ref List<T> list, int from, int to)
        {
            if (to < 0 || to > list.Count - 1)
                return;

            var swaptemp = list[to];
            list[to] = list[from];
            list[from] = swaptemp;
        }

        public override void OnInspectorGUI()
        {
            if (!iconUP)
                iconUP   = Resources.Load<Texture2D>("Icons/baseline_keyboard_arrow_up_black_36dp");
            if (!iconDOWN)
                iconDOWN = Resources.Load<Texture2D>("Icons/baseline_keyboard_arrow_down_black_36dp");


            var attribute = target as AbicraftAttribute;

            GUIStyle gstyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            
            attribute.AttributeName = GUILayout.TextField(attribute.AttributeName);
            attribute.AttributeIcon = (Texture2D)EditorGUILayout.ObjectField(attribute.AttributeIcon, typeof(Texture2D), false);
            attribute.Category      = (AbicraftAttribute.AttributeCategory)EditorGUILayout.EnumPopup(attribute.Category);

            GuiSpace(5);
            GuiLine (1);
            GuiSpace(5);

            if (!AbicraftGlobalContext.abicraft.dataFile)
                return;

            GUIStyle style = new GUIStyle(EditorStyles.helpBox);
            style.normal.textColor = Color.blue;
            style.fontSize = 12;

            List<AbicraftAttribute> attributes = AbicraftGlobalContext.abicraft.dataFile.GlobalAttributes;
            List<GUIContent> attr_contents = new List<GUIContent>();
            for (int i = 0; i < attributes.Count; i++)
            {
                attr_contents.Add(new GUIContent(attributes[i].AttributeName, attributes[i].AttributeIcon));
            }

            for (int i = 0; i < attribute.effects.Count; i++)
            {
                GuiSpace(10);
                GUILayout.BeginVertical(gstyle);
                GUILayout.BeginHorizontal();

                attribute.effects[i].effect =  (AbicraftAttribute.AttributeEffect.Effect)EditorGUILayout.EnumPopup(attribute.effects[i].effect, GUILayout.Width(100));

                if(attribute.effects[i].effect == AbicraftAttribute.AttributeEffect.Effect.Regenerate)
                {
                    GUILayout.Label("Per Second", style ,GUILayout.Width(75));
                }

                GuiSpace(10);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    attribute.effects.RemoveAt(i);
                }

                GUILayout.EndHorizontal();

                GuiSpace(5);
                GuiLine(1);
                GuiSpace(5);

               // EditorGUI.indentLevel++;

                switch (attribute.effects[i].effect)
                {
                    case AbicraftAttribute.AttributeEffect.Effect.Max:
                        for (int j = 0; j < attribute.effects[i].options.Count; j++)
                        {
                            GUILayout.BeginHorizontal(gstyle);

                            var effectoption = attribute.effects[i].options[j];

                            effectoption.option = (AbicraftAttribute.AttributeEffect.EffectOption)EditorGUILayout.EnumPopup(attribute.effects[i].options[j].option, GUILayout.Width(100));
                            effectoption.attribute = attributes[effectoption.attribute_index = EditorGUILayout.Popup(effectoption.attribute_index, attr_contents.ToArray())];

                            GuiSpace(5);
                            EditorGUIUtility.SetIconSize(new Vector2(12, 12));

                            if (GUILayout.Button(new GUIContent(iconUP)))
                                Swap(ref attribute.effects[i].options, j, j-1);

                            if (GUILayout.Button(new GUIContent(iconDOWN)))
                                Swap(ref attribute.effects[i].options, j, j+1);

                            if (GUILayout.Button("X", GUILayout.Width(15), GUILayout.Height(15)))
                                attribute.effects[i].options.RemoveAt(j);
                            GUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button("+"))
                            attribute.effects[i].options.Add(new AbicraftAttribute.AttributeEffect.AttributeEffectOption());
                        break;

                    case AbicraftAttribute.AttributeEffect.Effect.OnCast:
                        for (int j = 0; j < attribute.effects[i].options.Count; j++)
                        {
                            GUILayout.BeginHorizontal(gstyle);

                            var effectoption = attribute.effects[i].options[j];
                            effectoption.operation = (AbicraftAttribute.AttributeEffect.OperationOption)EditorGUILayout.EnumPopup(effectoption.operation, GUILayout.Width(80));
                            effectoption.targetOption = (AbicraftAttribute.AttributeEffect.TargetOption)EditorGUILayout.EnumPopup(effectoption.targetOption, GUILayout.Width(80));
                            effectoption.attribute = attributes[effectoption.attribute_index = EditorGUILayout.Popup(effectoption.attribute_index, attr_contents.ToArray())];
                            effectoption.option = (AbicraftAttribute.AttributeEffect.EffectOption)EditorGUILayout.EnumPopup(effectoption.option, GUILayout.Width(100));
                            effectoption.amount = EditorGUILayout.FloatField(effectoption.amount);
                            GUILayout.Label("Per point");

                            GuiSpace(5);
                            EditorGUIUtility.SetIconSize(new Vector2(12, 12));
                            if (GUILayout.Button(new GUIContent(iconUP)))
                                Swap(ref attribute.effects[i].options, j, j - 1);

                            if (GUILayout.Button(new GUIContent(iconDOWN)))
                                Swap(ref attribute.effects[i].options, j, j + 1);


                            if (GUILayout.Button("X", GUILayout.Width(15), GUILayout.Height(15)))
                                attribute.effects[i].options.RemoveAt(j);
                            GUILayout.EndHorizontal();
                        }

                        GuiSpace(5);
                        GuiLine(1);
                        GuiSpace(1);

                        if (GUILayout.Button("+"))
                            attribute.effects[i].options.Add(new AbicraftAttribute.AttributeEffect.AttributeEffectOption());
                        break;

                    case AbicraftAttribute.AttributeEffect.Effect.Regenerate:
                        for (int j = 0; j < attribute.effects[i].options.Count; j++)
                        {
                            GUILayout.BeginHorizontal(gstyle);

                            var effectoption = attribute.effects[i].options[j];
                            effectoption.attribute = attributes[effectoption.attribute_index = EditorGUILayout.Popup(effectoption.attribute_index, attr_contents.ToArray())];
                            effectoption.option = (AbicraftAttribute.AttributeEffect.EffectOption)EditorGUILayout.EnumPopup(effectoption.option, GUILayout.Width(100));
                            effectoption.amount = EditorGUILayout.FloatField(effectoption.amount);
                            GUILayout.Label("Per point");

                            GuiSpace(5);
                            EditorGUIUtility.SetIconSize(new Vector2(12, 12));
                            if (GUILayout.Button(new GUIContent(iconUP)))
                                Swap(ref attribute.effects[i].options, j, j - 1);

                            if (GUILayout.Button(new GUIContent(iconDOWN)))
                                Swap(ref attribute.effects[i].options, j, j + 1);


                            if (GUILayout.Button("X", GUILayout.Width(15), GUILayout.Height(15)))
                                attribute.effects[i].options.RemoveAt(j);
                            GUILayout.EndHorizontal();
                        }

                        GuiSpace(5);
                        GuiLine(1);
                        GuiSpace(1);

                        if (GUILayout.Button("+"))
                            attribute.effects[i].options.Add(new AbicraftAttribute.AttributeEffect.AttributeEffectOption());
                        break;
                }
                //EditorGUI.indentLevel--;
                GuiSpace(1);
                GUILayout.EndVertical();
            }
        
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                attribute.effects.Add(new AbicraftAttribute.AttributeEffect());
            }
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
