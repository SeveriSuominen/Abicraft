using AbicraftCore;
using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AbicraftNodes.Editors
{
    [CustomEditor(typeof(AbicraftObjectProfile))]
    public class AbicraftObjectProfileEditor : AbicratInspectorEditor
    {
        int instantiatePoolAmount;

        public override void OnInspectorGUI()
        {
            var profile = target as AbicraftObjectProfile;

        
            GUIStyle gstyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            GUIStyle bstyle = new GUIStyle(EditorStyles.boldLabel);
            GUIStyle b2style = new GUIStyle(EditorStyles.boldLabel);

            b2style.fontSize = 14;
            bstyle.fontSize = 12;

            GUILayout.BeginVertical();
            GUILayout.Label("Name", EditorStyles.boldLabel);
            GuiLine(1);
            GuiSpace(5);
            profile.TypeName = GUILayout.TextField(profile.TypeName);

            GuiSpace(5);
            

            GUILayout.EndVertical();
            GuiSpace(5);

            GUILayout.BeginVertical();
            GUILayout.Label("Attributes", b2style);
            GuiLine(1);
            GuiSpace(5);
            GUILayout.Label("Base", bstyle);
            GUILayout.Label("Base attributes are automatically included in every Abicraft object.", gstyle);
            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", bstyle, GUILayout.Width(125));
            GUILayout.Label("Base value", bstyle, GUILayout.Width(135));
            GUILayout.Label("Scaling", bstyle, GUILayout.Width(125));
            GUILayout.EndHorizontal();
            GuiSpace(5);

            foreach (var attr in AbicraftGlobalContext.abicraft.dataFile.GlobalAttributes)
            {
                if(attr.Category == AbicraftAttribute.AttributeCategory.Base)
                {
                    AbicraftAttribute.AbicraftObjectAttribute attrObj = null;

                    if ((attrObj = profile.GetAttributeObject(attr)) == null)
                        attrObj = profile.AddAttributeObject(attr);

                    GUILayout.BeginHorizontal();
                    
                    GUILayout.Label(attr.AttributeName, GUILayout.Width(125));
                    attrObj.baseValue = EditorGUILayout.IntField(attrObj.baseValue, GUILayout.Width(125));
                    GUILayout.Space(10);
                    attrObj.scaling = GUILayout.Toggle(attrObj.scaling, "", GUILayout.Width(15));
                    EditorGUILayout.CurveField(attrObj.powerCurve, GUILayout.Width(250));
                    GUILayout.EndHorizontal();
                }
            }

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);
            GUILayout.Label("Special", bstyle);
            GuiSpace(5);
            GuiLine(1);

            foreach (var attr in AbicraftGlobalContext.abicraft.dataFile.GlobalAttributes)
            {
                if (attr.Category == AbicraftAttribute.AttributeCategory.Special)
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Toggle(true, "", GUILayout.Width(11)))
                    {

                    }
                    GUILayout.Label(attr.AttributeName);
                    GUILayout.EndHorizontal();
                }
            }
            GuiSpace(5);
            GUILayout.Label("Options", b2style);
            GuiLine(1);
            GuiSpace(5);

            profile.PhysicalObject = GUILayout.Toggle(profile.PhysicalObject, "Physical Object");
            profile.Targetable = GUILayout.Toggle(profile.Targetable, "Targetable");

            GUILayout.EndVertical();
            GuiSpace(5);
            GuiLine(1);

            //if (abicraftObject.InstantiateObjectToPool)
            // abicraftObject.InstantiateToPoolAmount = EditorGUILayout.IntField(abicraftObject.InstantiateToPoolAmount);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
