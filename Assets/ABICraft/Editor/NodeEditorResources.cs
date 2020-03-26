using AbicraftNodes.Meta;
using UnityEditor;
using UnityEngine;

namespace AbicraftNodeEditor {
    public static class NodeEditorResources {
        // Textures
        public static Texture2D dot { get { return _dot != null ? _dot : _dot = Resources.Load<Texture2D>("xnode_dot_alt"); } }
        private static Texture2D _dot;
        public static Texture2D dotOuter { get { return _dotOuter != null ? _dotOuter : _dotOuter = Resources.Load<Texture2D>("xnode_dot_outer"); } }
        private static Texture2D _dotOuter;

        public static Texture2D nodeBody { get { return _nodeBody != null ? _nodeBody : _nodeBody = Resources.Load<Texture2D>("node_value_alt_png"); } }
        private static Texture2D _nodeBody;

        public static Texture2D nodeBodyExec { get { return _nodeBodyExec != null ? _nodeBodyExec : _nodeBodyExec = Resources.Load<Texture2D>("node_execution_alt_png"); } }
        private static Texture2D _nodeBodyExec;

        public static Texture2D nodeBodyLoop { get { return _nodeBodyLoop != null ? _nodeBodyLoop : _nodeBodyLoop = Resources.Load<Texture2D>("node_loop_alt_png"); } }
        private static Texture2D _nodeBodyLoop;

        public static Texture2D nodeHighlight { get { return _nodeHighlight != null ? _nodeHighlight : _nodeHighlight = Resources.Load<Texture2D>("xnode_node_highlight"); } }
        private static Texture2D _nodeHighlight;

        public static Texture2D nodeAreaBG { get { return _nodeAreaBG != null ? _nodeAreaBG : _nodeAreaBG = Resources.Load<Texture2D>("testarea"); } }
        private static Texture2D _nodeAreaBG;

        public static Texture2D nodeArea { get { return _nodeArea != null ? _nodeArea : _nodeArea = Resources.Load<Texture2D>("testareaHeaderpsd3"); } }
        private static Texture2D _nodeArea;

        public static Texture2D trashbin { get { return _trashbin != null ? _trashbin : _trashbin = Resources.Load<Texture2D>("baseline_cancel_black_36dp"); } }
        private static Texture2D _trashbin;
        
        public static Texture2D abicraftIcon { get { return _abicraftIcon != null ? _abicraftIcon : _abicraftIcon = Resources.Load<Texture2D>("Abicraft/big-logo"); } }
        private static Texture2D _abicraftIcon;

        // Stylestestarea
        public static Styles styles { get { return _styles != null ? _styles : _styles = new Styles(); } }
        public static Styles _styles = null;
        public static GUIStyle OutputPort { get { return new GUIStyle(EditorStyles.label) { alignment = TextAnchor.UpperRight }; } }
        public class Styles {
            public GUIStyle inputPort, nodeHeader, nodeBody, tooltip, nodeHighlight, abicraft_infobar, abicraft_infobar_center, abicraft_button;

            public GUIStyle GetAbicraftNodeStyle(AbicraftNode node)
            {
                GUIStyle nodeBodyStyle = new GUIStyle();

                if (node.GetType().IsSubclassOf(typeof(AbicraftExecutionNode)) || node.GetType().IsSubclassOf(typeof(AbicraftActionReceiverNode)) || node.GetType().IsSubclassOf(typeof(AbicraftActionSenderNode)))
                    nodeBodyStyle.normal.background = NodeEditorResources.nodeBodyExec;
                else if(node.GetType().IsSubclassOf(typeof(AbicraftExecutionLoopNode)))
                    nodeBodyStyle.normal.background = NodeEditorResources.nodeBodyLoop;
                else
                    nodeBodyStyle.normal.background = NodeEditorResources.nodeBody;

                nodeBodyStyle.border = new RectOffset(32, 32, 32, 32);
                nodeBodyStyle.padding = new RectOffset(16, 16, 4, 16);

                return nodeBodyStyle;
            }

            public Styles() {
                GUIStyle baseStyle = new GUIStyle("Label");
                baseStyle.fixedHeight = 18;

                inputPort = new GUIStyle(baseStyle);
                inputPort.alignment = TextAnchor.UpperLeft;
                inputPort.padding.left = 10;

                nodeHeader = new GUIStyle();
                nodeHeader.alignment = TextAnchor.MiddleCenter;
                nodeHeader.fontStyle = FontStyle.Bold;
                nodeHeader.normal.textColor = Color.white;

                abicraft_infobar = new GUIStyle();
                abicraft_infobar.alignment = TextAnchor.UpperLeft;
                abicraft_infobar.fontStyle = FontStyle.Bold;
                abicraft_infobar.normal.textColor = Color.white;
                abicraft_infobar.fontSize = 15;
                abicraft_infobar.margin = new RectOffset(55, 55, 20, 0);

                abicraft_infobar_center = new GUIStyle();
                abicraft_infobar_center.alignment = TextAnchor.UpperCenter;
                abicraft_infobar_center.fontStyle = FontStyle.Bold;
                abicraft_infobar_center.normal.textColor = Color.white;
                abicraft_infobar_center.fontSize = 15;
                //abicraft_infobar_center.margin = new RectOffset(25, 25, -90 , 0);

                abicraft_button = new GUIStyle();
                abicraft_button.alignment = TextAnchor.UpperCenter;
                abicraft_button.fontStyle = FontStyle.Bold;
                abicraft_button.normal.textColor = Color.white;
                abicraft_button.fontSize = 15;
                abicraft_button.normal.background = NodeEditorResources.nodeBody;
                //abicraft_button.border = new RectOffset(32, 32, 32, 32);
                abicraft_button.padding = new RectOffset(16, 16, 16, 16);
                //abicraft_infobar_center.margin = new RectOffset(25, 25, -90 , 0);

                nodeBody = new GUIStyle();
                nodeBody.normal.background = NodeEditorResources.nodeBody;
                nodeBody.border = new RectOffset(32, 32, 32, 32);
                nodeBody.padding = new RectOffset(16, 16, 4, 16);

                nodeHighlight = new GUIStyle();
                nodeHighlight.normal.background = NodeEditorResources.nodeHighlight;
                nodeHighlight.border = new RectOffset(32, 32, 32, 32);

                tooltip = new GUIStyle("helpBox");
                tooltip.alignment = TextAnchor.MiddleCenter;
            }
        }

        public static Texture2D GenerateGridTexture(Color line, Color bg) {
            Texture2D tex = new Texture2D(64, 64);
            Color[] cols = new Color[64 * 64];
            for (int y = 0; y < 64; y++) {
                for (int x = 0; x < 64; x++) {
                    Color col = bg;
                    if (y % 16 == 0 || x % 16 == 0) col = Color.Lerp(line, bg, 0.65f);
                    if (y == 63 || x == 63) col = Color.Lerp(line, bg, 0.35f);
                    cols[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Grid";
            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateCrossTexture(Color line) {
            Texture2D tex = new Texture2D(64, 64);
            Color[] cols = new Color[64 * 64];
            for (int y = 0; y < 64; y++) {
                for (int x = 0; x < 64; x++) {
                    Color col = line;
                    if (y != 31 && x != 31) col.a = 0;
                    cols[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Grid";
            tex.Apply();
            return tex;
        }
    }
}