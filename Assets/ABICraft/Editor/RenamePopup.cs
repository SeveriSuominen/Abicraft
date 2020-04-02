using UnityEditor;
using UnityEngine;

namespace AbicraftNodeEditor {
    /// <summary> Utility for renaming assets </summary>
    public class AreaEditPopup : EditorWindow {
        public static AreaEditPopup current { get; private set; }
        public Area target;
        public string name_input;
        public Color  color_input;

        private bool firstFrame = true;

        /// <summary> Show a rename popup for an asset at mouse position. Will trigger reimport of the asset on apply.
        public static AreaEditPopup Show(Area target, float width = 200) {
            AreaEditPopup window = EditorWindow.GetWindow<AreaEditPopup>(true, "Edit area " + target.name, true);
            if (current != null) current.Close();
            current = window;
            window.target = target;
            window.name_input  = target.areaName;
            window.minSize = new Vector2(100, 100);
            window.position = new Rect(0, 0, width, 44);
            window.color_input = target.color;
            GUI.FocusControl("ClearAllFocus");
            window.UpdatePositionToMouse();
            return window;
        }

        private void UpdatePositionToMouse() {
            if (Event.current == null) return;
            Vector3 mousePoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            Rect pos = position;
            pos.x = mousePoint.x - position.width * 0.5f;
            pos.y = mousePoint.y - 10;
            position = pos;
        }

        private void OnLostFocus() {
            // Make the popup close on lose focus
            //Close();
        }

        private void OnGUI() {
            if (firstFrame) {
                UpdatePositionToMouse();
                firstFrame = false;
            }
            GUILayout.Label("Area name");
            name_input  = EditorGUILayout.TextField(name_input);
            GUILayout.Label("Area color");
            color_input = EditorGUILayout.ColorField(GUIContent.none, color_input, false, true, false);
            Event e = Event.current;
            GUILayout.Space(3);
            if (GUILayout.Button("Apply") || (e.isKey && e.keyCode == KeyCode.Return))
            {
                target.areaName = name_input;
                target.color    = color_input;
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
                Close();
                target.TriggerOnValidate();
            }

            // If input is empty, revert name to default instead
            /*if (input == null || input.Trim() == "") {
                if (GUILayout.Button("Revert to default") || (e.isKey && e.keyCode == KeyCode.Return)) {
                    target.name = NodeEditorUtilities.NodeDefaultName(target.GetType());
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
                    Close();
					target.TriggerOnValidate();
                }
            }
            // Rename asset to input text
            else {
               
            }*/
        }
    }
}