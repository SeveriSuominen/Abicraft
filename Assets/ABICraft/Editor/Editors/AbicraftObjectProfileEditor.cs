using AbicraftCore;
using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AbicraftNodes.Editors
{
    [CustomEditor(typeof(AbicraftObjectProfile))]
    public class AbicraftObjectProfileEditor : AbicratEditor
    {
        int instantiatePoolAmount;

        public override void OnInspectorGUI()
        {
            var profile = target as AbicraftObjectProfile;

        
            GUIStyle gstyle = new GUIStyle(GUI.skin.GetStyle("HelpBox"));

            GUILayout.BeginVertical(gstyle);
            GUILayout.Label("Name", EditorStyles.boldLabel);
            GuiLine(1);
            GuiSpace(5);
            profile.TypeName = GUILayout.TextField(profile.TypeName);

            GuiSpace(5);
            GUILayout.Label("Passive abilites are looped until life time ends or passive ability is interupted", gstyle);

            GUILayout.EndVertical();
            GuiSpace(5);

            GUILayout.BeginVertical(gstyle);
            GUILayout.Label("Attributes", EditorStyles.boldLabel);
            GuiLine(1);
            GuiSpace(5);
            GUILayout.Label("Base", EditorStyles.boldLabel);
            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);

            foreach (var attr in AbicraftGlobalContext.abicraft.dataFile.GlobalAttributes)
            {
                if(attr.Category == AbicraftAttribute.AttributeCategory.Base)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(attr.AttributeName);

                    GUILayout.EndHorizontal();
                }
            }

            GuiSpace(5);
            GuiLine(1);
            GuiSpace(5);
            GUILayout.Label("Special", EditorStyles.boldLabel);
            GuiSpace(5);

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
            GUILayout.EndVertical();

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
