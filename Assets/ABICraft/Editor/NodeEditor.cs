using AbicraftNodes.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
#endif

namespace AbicraftNodeEditor {
    /// <summary> Base class to derive custom Node editors from. Use this to create your own custom inspectors and editors for your nodes. </summary>
    [CustomNodeEditor(typeof(AbicraftNode))]
    public class NodeEditor : AbicraftNodeEditor.Internal.NodeEditorBase<NodeEditor, NodeEditor.CustomNodeEditorAttribute, AbicraftNode> {

        private   readonly Color DEFAULTCOLOR = new Color32(255, 255,255, 255);//new Color32(90, 97, 105, 255);
        protected readonly Color ERRORCOLOR = new Color(1f, 0.55f, 0.55f, 1f);
        public readonly static float width = 208;
        /// <summary> Fires every whenever a node was modified through the editor </summary>
        public static Action<AbicraftNode> onUpdateNode;
        public readonly static Dictionary<NodePort, Vector2> portPositions = new Dictionary<NodePort, Vector2>();

#if ODIN_INSPECTOR
        internal static bool inNodeEditor = false;
#endif

        /*public virtual void OnHeaderGUI() {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }*/

        public virtual void OnHeaderGUI(GUIStyle style)
        {
            GUIStyle useStyle = style;

            if (style == null)
                useStyle = NodeEditorResources.styles.nodeHeader;

            GUILayout.Label(target.use_alt_name && target.alt_name != null ? target.alt_name: target.name, useStyle, GUILayout.Height(30));
        }
        
        /// <summary> Draws standard field editors for all public fields </summary>
        public virtual void OnBodyGUI() {
#if ODIN_INSPECTOR
            inNodeEditor = true;
#endif

            // Unity specifically requires this to save/update any serial object.
            // serializedObject.Update(); must go at the start of an inspector gui, and
            // serializedObject.ApplyModifiedProperties(); goes at the end.
            serializedObject.Update();
            string[] excludes = { "m_Script", "graph", "position", "ports" };

#if ODIN_INSPECTOR
            InspectorUtilities.BeginDrawPropertyTree(objectTree, true);
            GUIHelper.PushLabelWidth(84);
            objectTree.Draw(true);
            InspectorUtilities.EndDrawPropertyTree(objectTree);
            GUIHelper.PopLabelWidth();
#else

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren)) {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                NodeEditorGUILayout.PropertyField(iterator, true);
            }
#endif

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (NodePort dynamicPort in target.DynamicPorts) {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();

#if ODIN_INSPECTOR
            // Call repaint so that the graph window elements respond properly to layout changes coming from Odin    
            if (GUIHelper.RepaintRequested) {
                GUIHelper.ClearRepaintRequest();
                window.Repaint();
            }
#endif

#if ODIN_INSPECTOR
            inNodeEditor = false;
#endif
        }

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
            catch (ArgumentException e) {}
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
            catch{}
               
            rect.height = i_height;

            EditorGUI.DrawRect(rect, new Color(0, 0, 0, 0));
        }

        public virtual int GetWidth() {
            Type type = target.GetType();
            int width;
            if (type.TryGetAttributeWidth(out width)) return width;
            else return 208;
        }

        /// <summary> Returns color for target node </summary>
        public virtual Color GetTint() {
            // Try get color from [NodeTint] attribute
            Type type = target.GetType();
            Color color;
            if (type.TryGetAttributeTint(out color)) return color;
            // Return default color (grey)
            else return DEFAULTCOLOR;
        }

        public virtual GUIStyle GetBodyStyle(AbicraftNode node) {
            return NodeEditorResources.styles.GetAbicraftNodeStyle(node);
        }

        public virtual GUIStyle GetBodyHighlightStyle() {
            return NodeEditorResources.styles.nodeHighlight;
        }

        /// <summary> Add items for the context menu when right-clicking this node. Override to add custom menu items. </summary>
        public virtual void AddContextMenuItems(GenericMenu menu) {
            // Actions if only one node is selected
            if (Selection.objects.Length == 1 && Selection.activeObject is AbicraftNode) {
                AbicraftNode node = Selection.activeObject as AbicraftNode;
                menu.AddItem(new GUIContent("Move To Top"), false, () => NodeEditorWindow.current.MoveNodeToTop(node));
                menu.AddItem(new GUIContent("Rename"), false, NodeEditorWindow.current.RenameSelectedNode);
            }

            // Add actions to any number of selected nodes
            menu.AddItem(new GUIContent("Copy"), false, NodeEditorWindow.current.CopySelectedNodes);
            menu.AddItem(new GUIContent("Duplicate"), false, NodeEditorWindow.current.DuplicateSelectedNodes);
            menu.AddItem(new GUIContent("Remove"), false, NodeEditorWindow.current.RemoveSelectedNodes);

            // Custom sctions if only one node is selected
            if (Selection.objects.Length == 1 && Selection.activeObject is AbicraftNode) {
                AbicraftNode node = Selection.activeObject as AbicraftNode;
                menu.AddCustomContextMenuItems(node);
            }
        }

        /// <summary> Rename the node asset. This will trigger a reimport of the node. </summary>
        public void Rename(string newName) {
            if (newName == null || newName.Trim() == "") newName = NodeEditorUtilities.NodeDefaultName(target.GetType());
            target.name = newName;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
        }

        [AttributeUsage(AttributeTargets.Class)]
        public class CustomNodeEditorAttribute : Attribute,
        AbicraftNodeEditor.Internal.NodeEditorBase<NodeEditor, NodeEditor.CustomNodeEditorAttribute, AbicraftNode>.INodeEditorAttrib {
            private Type inspectedType;
            /// <summary> Tells a NodeEditor which Node type it is an editor for </summary>
            /// <param name="inspectedType">Type that this editor can edit</param>
            public CustomNodeEditorAttribute(Type inspectedType) {
                this.inspectedType = inspectedType;
            }

            public Type GetInspectedType() {
                return inspectedType;
            }
        }
    }
}
