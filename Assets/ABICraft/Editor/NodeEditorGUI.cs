using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using AbicraftNodeEditor.Internal;
using AbicraftNodes.Meta;
using AbicraftCore;

namespace AbicraftNodeEditor {
    /// <summary> Contains GUI methods </summary>
    public partial class NodeEditorWindow {
        public NodeGraphEditor graphEditor;
        private List<UnityEngine.Object> selectionCache;
        private List<AbicraftNode> culledNodes;
        /// <summary> 19 if docked, 22 if not </summary>
        private int topPadding { get { return isDocked() ? 19 : 22; } }
        /// <summary> Executed after all other window GUI. Useful if Zoom is ruining your day. Automatically resets after being run.</summary>
        public event Action onLateGUI;
        private static readonly Vector3[] polyLineTempArray = new Vector3[2];

        private void OnGUI() {
            Event e = Event.current;
            Matrix4x4 m = GUI.matrix;
            if (graph == null) return;
            ValidateGraphEditor();
            Controls();

            graphEditor.UpdateVariableDefinitions(graph);
            EditorUtility.SetDirty(graph);

            DrawGrid(position, zoom, panOffset);
            
            DrawConnections();
            DrawDraggedConnection();
            DrawNodes();
            DrawSelectionBox();
            DrawAbilityInspector();
            DrawTooltip();
            graphEditor.OnGUI();

            GUI.DrawTexture(new Rect(15, 15, 33,33), Resources.Load("Abicraft/big-logo") as Texture2D);
            GUILayout.Label("Abicraft Node", NodeEditorResources.styles.abicraft_infobar, GUILayout.Height(30));
            //GUI.Label(new Rect(position.width *0.5f, 20, 33, 33), "Editing: \"" + graph.name + "\"",  NodeEditorResources.styles.abicraft_infobar_center);

            if (!NodeEditorPreferences.GetSettings().autoSave)
            {
                /*GUI.skin.button.normal.textColor = Color.white;
                GUI.skin.button.onNormal.background = NodeEditorResources.nodeBodyLoop;
                GUI.skin.button.onHover.background = NodeEditorResources.nodeBodyExec;*/

                GUIStyle style = new GUIStyle(GUI.skin.button);

                //style.normal.textColor = Color.white;
                //style.normal.background = NodeEditorResources.nodeBodyExec;

                GUI.skin.button.onNormal.textColor = Color.white;

                Color org = GUI.color;
                //GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

                if(GUI.Button(new Rect(position.width - 125, position.height - 50, 100, 33), "Save", style))
                    AssetDatabase.SaveAssets();

                //GUI.color = org;
            }
            else
            {        
                GUI.Label(new Rect(position.width - 125, position.height - 50, 100, 33), "Autosaving", NodeEditorResources.styles.abicraft_infobar_center);
            }

            // Run and reset onLateGUI
            if (onLateGUI != null) {
                onLateGUI();
                onLateGUI = null;
            }

            GUI.matrix = m;
        }

        public static void BeginZoomed(Rect rect, float zoom, float topPadding) {
            GUI.EndClip();

            GUIUtility.ScaleAroundPivot(Vector2.one / zoom, rect.size * 0.5f);
            Vector4 padding = new Vector4(0, topPadding, 0, 0);
            padding *= zoom;
            GUI.BeginClip(new Rect(-((rect.width * zoom) - rect.width) * 0.5f, -(((rect.height * zoom) - rect.height) * 0.5f) + (topPadding * zoom),
                rect.width * zoom,
                rect.height * zoom));
        }

        public static void EndZoomed(Rect rect, float zoom, float topPadding) {
            GUIUtility.ScaleAroundPivot(Vector2.one * zoom, rect.size * 0.5f);
            Vector3 offset = new Vector3(
                (((rect.width * zoom) - rect.width) * 0.5f),
                (((rect.height * zoom) - rect.height) * 0.5f) + (-topPadding * zoom) + topPadding,
                0);
            GUI.matrix = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one);
        }

        public void DrawGrid(Rect rect, float zoom, Vector2 panOffset) {

            rect.position = Vector2.zero;

            Vector2 center = rect.size / 2f;
            Texture2D gridTex = graphEditor.GetGridTexture();
            Texture2D crossTex = graphEditor.GetSecondaryGridTexture();

            // Offset from origin in tile units
            float xOffset = -(center.x * zoom + panOffset.x) / gridTex.width;
            float yOffset = ((center.y - rect.size.y) * zoom + panOffset.y) / gridTex.height;

            Vector2 tileOffset = new Vector2(xOffset, yOffset);

            // Amount of tiles
            float tileAmountX = Mathf.Round(rect.size.x * zoom) / gridTex.width;
            float tileAmountY = Mathf.Round(rect.size.y * zoom) / gridTex.height;

            Vector2 tileAmount = new Vector2(tileAmountX, tileAmountY);

            // Draw tiled background
            GUI.DrawTextureWithTexCoords(rect, gridTex, new Rect(tileOffset, tileAmount));
            GUI.DrawTextureWithTexCoords(rect, crossTex, new Rect(tileOffset + new Vector2(0.5f, 0.5f), tileAmount));
        }

        public void DrawSelectionBox() {
            for (int n = 0; n < graph.areas.Count; n++)
            {
                if (graph.areas[n].dragginArea || graph.areas[n].resisingArea)
                    return;
            }

            if (currentActivity == NodeActivity.DragGrid) {
                Vector2 curPos = WindowToGridPosition(Event.current.mousePosition);
                Vector2 size = curPos - dragBoxStart;
                Rect r = new Rect(dragBoxStart, size);
                r.position = GridToWindowPosition(r.position);
                r.size /= zoom;
                Handles.DrawSolidRectangleWithOutline(r, new Color(0, 0, 0, 0.1f), new Color(1, 1, 1, 0.6f));
            }
        }

        public static bool DropdownButton(string name, float width) {
            return GUILayout.Button(name, EditorStyles.toolbarDropDown, GUILayout.Width(width));
        }

        /// <summary> Show right-click context menu for hovered reroute </summary>
        void ShowRerouteContextMenu(RerouteReference reroute) {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Remove"), false, () => reroute.RemovePoint());
            contextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
        }

        /// <summary> Show right-click context menu for hovered port </summary>
        void ShowPortContextMenu(NodePort hoveredPort) {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Clear Connections"), false, () => hoveredPort.ClearConnections());
            contextMenu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
            if (NodeEditorPreferences.GetSettings().autoSave) AssetDatabase.SaveAssets();
        }

        static Vector2 CalculateBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
            float u = 1 - t;
            float tt = t * t, uu = u * u;
            float uuu = uu * u, ttt = tt * t;
            return new Vector2(
                (uuu * p0.x) + (3 * uu * t * p1.x) + (3 * u * tt * p2.x) + (ttt * p3.x),
                (uuu * p0.y) + (3 * uu * t * p1.y) + (3 * u * tt * p2.y) + (ttt * p3.y)
            );
        }

        /// <summary> Draws a line segment without allocating temporary arrays </summary>
        static void DrawAAPolyLineNonAlloc(float thickness, Vector2 p0, Vector2 p1) {
            polyLineTempArray[0].x = p0.x;
            polyLineTempArray[0].y = p0.y;
            polyLineTempArray[1].x = p1.x;
            polyLineTempArray[1].y = p1.y;
            Handles.DrawAAPolyLine(thickness, polyLineTempArray);
        }

        /// <summary> Draw a bezier from output to input in grid coordinates </summary>
        public void DrawNoodle(Gradient gradient, NoodlePath path, NoodleStroke stroke, float thickness, List<Vector2> gridPoints) {
            // convert grid points to window points
            for (int i = 0; i < gridPoints.Count; ++i)
                gridPoints[i] = GridToWindowPosition(gridPoints[i]);

            Handles.color = gradient.Evaluate(0f);
            int length = gridPoints.Count;
            switch (path) {
                case NoodlePath.Curvy:
                    Vector2 outputTangent = Vector2.right;
                    for (int i = 0; i < length - 1; i++) {
                        Vector2 inputTangent;
                        // Cached most variables that repeat themselves here to avoid so many indexer calls :p
                        Vector2 point_a = gridPoints[i];
                        Vector2 point_b = gridPoints[i + 1];
                        float dist_ab = Vector2.Distance(point_a, point_b);
                        if (i == 0) outputTangent = zoom * dist_ab * 0.01f * Vector2.right;
                        if (i < length - 2) {
                            Vector2 point_c = gridPoints[i + 2];
                            Vector2 ab = (point_b - point_a).normalized;
                            Vector2 cb = (point_b - point_c).normalized;
                            Vector2 ac = (point_c - point_a).normalized;
                            Vector2 p = (ab + cb) * 0.5f;
                            float tangentLength = (dist_ab + Vector2.Distance(point_b, point_c)) * 0.005f * zoom;
                            float side = ((ac.x * (point_b.y - point_a.y)) - (ac.y * (point_b.x - point_a.x)));

                            p = tangentLength * Mathf.Sign(side) * new Vector2(-p.y, p.x);
                            inputTangent = p;
                        } else {
                            inputTangent = zoom * dist_ab * 0.01f * Vector2.left;
                        }

                        // Calculates the tangents for the bezier's curves.
                        float zoomCoef = 50 / zoom;
                        Vector2 tangent_a = point_a + outputTangent * zoomCoef;
                        Vector2 tangent_b = point_b + inputTangent * zoomCoef;
                        // Hover effect.
                        int division = Mathf.RoundToInt(.2f * dist_ab) + 3;
                        // Coloring and bezier drawing.
                        int draw = 0;
                        Vector2 bezierPrevious = point_a;
                        for (int j = 1; j <= division; ++j) {
                            if (stroke == NoodleStroke.Dashed) {
                                draw++;
                                if (draw >= 2) draw = -2;
                                if (draw < 0) continue;
                                if (draw == 0) bezierPrevious = CalculateBezierPoint(point_a, tangent_a, tangent_b, point_b, (j - 1f) / (float) division);
                            }
                            if (i == length - 2)
                                Handles.color = gradient.Evaluate((j + 1f) / division);
                            Vector2 bezierNext = CalculateBezierPoint(point_a, tangent_a, tangent_b, point_b, j / (float) division);
                            DrawAAPolyLineNonAlloc(thickness, bezierPrevious, bezierNext);
                            bezierPrevious = bezierNext;
                        }
                        outputTangent = -inputTangent;
                    }
                    break;
                case NoodlePath.Straight:
                    for (int i = 0; i < length - 1; i++) {
                        Vector2 point_a = gridPoints[i];
                        Vector2 point_b = gridPoints[i + 1];
                        // Draws the line with the coloring.
                        Vector2 prev_point = point_a;
                        // Approximately one segment per 5 pixels
                        int segments = (int) Vector2.Distance(point_a, point_b) / 5;

                        int draw = 0;
                        for (int j = 0; j <= segments; j++) {
                            draw++;
                            float t = j / (float) segments;
                            Vector2 lerp = Vector2.Lerp(point_a, point_b, t);
                            if (draw > 0) {
                                if (i == length - 2) Handles.color = gradient.Evaluate(t);
                                DrawAAPolyLineNonAlloc(thickness, prev_point, lerp);
                            }
                            prev_point = lerp;
                            if (stroke == NoodleStroke.Dashed && draw >= 2) draw = -2;
                        }
                    }
                    break;
                case NoodlePath.Angled:
                    for (int i = 0; i < length - 1; i++) {
                        if (i == length - 1) continue; // Skip last index
                        if (gridPoints[i].x <= gridPoints[i + 1].x - (50 / zoom)) {
                            float midpoint = (gridPoints[i].x + gridPoints[i + 1].x) * 0.5f;
                            Vector2 start_1 = gridPoints[i];
                            Vector2 end_1 = gridPoints[i + 1];
                            start_1.x = midpoint;
                            end_1.x = midpoint;
                            if (i == length - 2) {
                                DrawAAPolyLineNonAlloc(thickness, gridPoints[i], start_1);
                                Handles.color = gradient.Evaluate(0.5f);
                                DrawAAPolyLineNonAlloc(thickness, start_1, end_1);
                                Handles.color = gradient.Evaluate(1f);
                                DrawAAPolyLineNonAlloc(thickness, end_1, gridPoints[i + 1]);
                            } else {
                                DrawAAPolyLineNonAlloc(thickness, gridPoints[i], start_1);
                                DrawAAPolyLineNonAlloc(thickness, start_1, end_1);
                                DrawAAPolyLineNonAlloc(thickness, end_1, gridPoints[i + 1]);
                            }
                        } else {
                            float midpoint = (gridPoints[i].y + gridPoints[i + 1].y) * 0.5f;
                            Vector2 start_1 = gridPoints[i];
                            Vector2 end_1 = gridPoints[i + 1];
                            start_1.x += 25 / zoom;
                            end_1.x -= 25 / zoom;
                            Vector2 start_2 = start_1;
                            Vector2 end_2 = end_1;
                            start_2.y = midpoint;
                            end_2.y = midpoint;
                            if (i == length - 2) {
                                DrawAAPolyLineNonAlloc(thickness, gridPoints[i], start_1);
                                Handles.color = gradient.Evaluate(0.25f);
                                DrawAAPolyLineNonAlloc(thickness, start_1, start_2);
                                Handles.color = gradient.Evaluate(0.5f);
                                DrawAAPolyLineNonAlloc(thickness, start_2, end_2);
                                Handles.color = gradient.Evaluate(0.75f);
                                DrawAAPolyLineNonAlloc(thickness, end_2, end_1);
                                Handles.color = gradient.Evaluate(1f);
                                DrawAAPolyLineNonAlloc(thickness, end_1, gridPoints[i + 1]);
                            } else {
                                DrawAAPolyLineNonAlloc(thickness, gridPoints[i], start_1);
                                DrawAAPolyLineNonAlloc(thickness, start_1, start_2);
                                DrawAAPolyLineNonAlloc(thickness, start_2, end_2);
                                DrawAAPolyLineNonAlloc(thickness, end_2, end_1);
                                DrawAAPolyLineNonAlloc(thickness, end_1, gridPoints[i + 1]);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary> Draws all connections </summary>
        public void DrawConnections() {
            Vector2 mousePos = Event.current.mousePosition;
            List<RerouteReference> selection = preBoxSelectionReroute != null ? new List<RerouteReference>(preBoxSelectionReroute) : new List<RerouteReference>();
            hoveredReroute = new RerouteReference();

            List<Vector2> gridPoints = new List<Vector2>(2);

            Color col = GUI.color;
            foreach (AbicraftNode node in graph.nodes) {
                //If a null node is found, return. This can happen if the nodes associated script is deleted. It is currently not possible in Unity to delete a null asset.
                if (node == null) continue;

                // Draw full connections and output > reroute
                foreach (NodePort output in node.Outputs) {
                    //Needs cleanup. Null checks are ugly
                    Rect fromRect;
                    if (!_portConnectionPoints.TryGetValue(output, out fromRect)) continue;

                    Color portColor = graphEditor.GetPortColor(output);
                    for (int k = 0; k < output.ConnectionCount; k++) {
                        NodePort input = output.GetConnection(k);

                        Gradient noodleGradient = graphEditor.GetNoodleGradient(output, input);
                        float noodleThickness = graphEditor.GetNoodleThickness(output, input);
                        NoodlePath noodlePath = graphEditor.GetNoodlePath(output, input);
                        NoodleStroke noodleStroke = graphEditor.GetNoodleStroke(output, input);

                        // Error handling
                        if (input == null) continue; //If a script has been updated and the port doesn't exist, it is removed and null is returned. If this happens, return.
                        if (!input.IsConnectedTo(output)) input.Connect(output);
                        Rect toRect;
                        if (!_portConnectionPoints.TryGetValue(input, out toRect)) continue;

                        List<Vector2> reroutePoints = output.GetReroutePoints(k);

                        gridPoints.Clear();
                        gridPoints.Add(fromRect.center);
                        gridPoints.AddRange(reroutePoints);
                        gridPoints.Add(toRect.center);
                        DrawNoodle(noodleGradient, noodlePath, noodleStroke, noodleThickness, gridPoints);

                        // Loop through reroute points again and draw the points
                        for (int i = 0; i < reroutePoints.Count; i++) {
                            RerouteReference rerouteRef = new RerouteReference(output, k, i);
                            // Draw reroute point at position
                            Rect rect = new Rect(reroutePoints[i], new Vector2(12, 12));
                            rect.position = new Vector2(rect.position.x - 6, rect.position.y - 6);
                            rect = GridToWindowRect(rect);

                            // Draw selected reroute points with an outline
                            if (selectedReroutes.Contains(rerouteRef)) {
                                GUI.color = NodeEditorPreferences.GetSettings().highlightColor;
                                //GUI.DrawTexture(rect, NodeEditorResources.dotOuter);
                            }

                            GUI.color = portColor;
                            GUI.DrawTexture(rect, NodeEditorResources.dot);
                            
                            if (rect.Overlaps(selectionBox)) selection.Add(rerouteRef);
                            if (rect.Contains(mousePos)) hoveredReroute = rerouteRef;

                        }
                    }
                }
            }
            GUI.color = col;
            if (Event.current.type != EventType.Layout && currentActivity == NodeActivity.DragGrid) selectedReroutes = selection;
        }

        private void DrawAbilityInspector()
        {
            /*Color defColor = GUI.color;
            Rect rect = new Rect(15, 50, 250, 500); 

            GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            GUI.Box(rect, "");
            GUI.color = defColor;

            GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();

            graph.icon = EditorGUILayout.ObjectField(graph.icon, typeof(Texture2D), false) as Texture2D;

            GUILayout.EndVertical();
            GUILayout.EndArea();*/
        }

        private void DrawAreas()
        {
            Color defaultColor = GUI.color;

            for (int n = 0; n < graph.areas.Count; n++)
            {
                Area area = graph.areas[n];

                Vector2 vecpos = GridToWindowPositionNoClipped(new Vector2(area.areaRect.x, area.areaRect.y));
                Vector2 vecposAreaWidth = GridToWindowPositionNoClipped(new Vector2(area.areaRect.x + area.areaRect.width, area.areaRect.y + area.areaRect.height));

                Rect yrect = new Rect(vecpos.x, vecpos.y, area.areaRect.width, area.areaRect.height);
                Rect headerRect = new Rect(vecpos.x, vecpos.y, area.areaRect.width, 30);

                GUIStyle style = new GUIStyle(NodeEditorResources.styles.nodeBody);
                style.normal.background = NodeEditorResources.nodeArea;
                style.padding = new RectOffset();

                GUIStyle styleBG = new GUIStyle(NodeEditorResources.styles.nodeBody);
                styleBG.normal.background = NodeEditorResources.nodeAreaBG;
                styleBG.padding = new RectOffset();

                GUIStyle styleVisibleBtn = new GUIStyle(EditorStyles.miniButton);
                styleVisibleBtn.normal.textColor = Color.blue;
                styleVisibleBtn.padding = new RectOffset();

                Rect labelRect  = new Rect(yrect.x, yrect.y + (int)(10 * (zoom + 0.4f)), yrect.width, 20);
                Rect colorRect  = new Rect(yrect.x + 10, yrect.y + 10, 25, 20);

                Rect RemovebuttonRect  = new Rect(vecposAreaWidth.x - 32, yrect.y + 8, 22, 18);
                Rect VisiblebuttonRect = new Rect(vecposAreaWidth.x - 60, yrect.y + 8, 22, 17);

                area.color = EditorGUI.ColorField(colorRect, GUIContent.none, area.color, false, true, false);

                GUIStyle labelstyle = new GUIStyle(NodeEditorResources.styles.nodeHeader);

                float labelZoom = (zoom + 0.4f);

                labelstyle.fontSize = (int)(12 * Mathf.Clamp(labelZoom, 1, 3));
                //GUI.Label(labelRect, area.areaName, labelstyle);
                //GUI.Label(labelRect, area.areaName, labelstyle);

                GUI.color = area.color;
                GUI.Box(yrect, "", style);
                GUI.color = defaultColor;

                if (area.Visible)
                {
                    GUI.color = new Color(1, 1, 1, 0.1f);
                    GUI.Box(yrect, "", styleBG);
                    GUI.color = defaultColor;
                }

                EditorGUIUtility.SetIconSize(new Vector2(15, 15));

                if ( GUI.Button(RemovebuttonRect, new GUIContent(NodeEditorResources.trashbinNormalRed)))
                    area.graph.RemoveArea(area);

                Color bgcol = GUI.backgroundColor;
        
                GUI.backgroundColor = area.Visible ? Color.white : new Color(0,15f, 0.15f, 0.15f);

                if (GUI.Button(VisiblebuttonRect, new GUIContent(NodeEditorResources.eye), styleVisibleBtn))
                {
                    area.Visible = !area.Visible;
                }

                GUI.backgroundColor = bgcol;

                HorizResizer(area, headerRect, ref yrect);

                if (headerRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        for (int i = 0; i < graph.nodes.Count; i++)
                        {
                            // Skip null nodes. The user could be in the process renaming scripts, so removing them at this point is not advisable.
                            if (graph.nodes[i] == null) continue;
                            if (i >= graph.nodes.Count) return;

                            AbicraftNode node = graph.nodes[i];

                            if (yrect.Contains(GridToWindowPositionNoClipped(node.position)) && !area.movingNodes.Contains(node))
                                area.movingNodes.Add(node);
                        }

                        area.dragginArea = true;
                    }
                    if (Event.current.type == EventType.MouseUp)
                    {
                        area.movingNodes.Clear();
                        area.dragginArea = false;
                    }
                }

                if (area.dragginArea && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    for (int i = 0; i < area.movingNodes.Count; i++)
                    {
                        area.movingNodes[i].position += new Vector2(Event.current.delta.x, Event.current.delta.y);
                    }
                    
                    area.areaRect.x += Event.current.delta.x;
                    area.areaRect.y += Event.current.delta.y;
                }
                area.areaRect.width  = yrect.width;
                area.areaRect.height = yrect.height;
            }      
        }

        private void DrawNodes() {
            Event e = Event.current;
            if (e.type == EventType.Layout) {
                selectionCache = new List<UnityEngine.Object>(Selection.objects);
            }

            System.Reflection.MethodInfo onValidate = null;
            if (Selection.activeObject != null && Selection.activeObject is AbicraftNode) {
                onValidate = Selection.activeObject.GetType().GetMethod("OnValidate");
                if (onValidate != null) EditorGUI.BeginChangeCheck();
            }

            BeginZoomed(position, zoom, topPadding);

            Vector2 mousePos = Event.current.mousePosition;

            if (e.type != EventType.Layout) {
                hoveredNode = null;
                hoveredPort = null;
            }

            List<UnityEngine.Object> preSelection = preBoxSelection != null ? new List<UnityEngine.Object>(preBoxSelection) : new List<UnityEngine.Object>();

            // Selection box stuff
            Vector2 boxStartPos = GridToWindowPositionNoClipped(dragBoxStart);
            Vector2 boxSize = mousePos - boxStartPos;
            if (boxSize.x < 0) { boxStartPos.x += boxSize.x; boxSize.x = Mathf.Abs(boxSize.x); }
            if (boxSize.y < 0) { boxStartPos.y += boxSize.y; boxSize.y = Mathf.Abs(boxSize.y); }
            Rect selectionBox = new Rect(boxStartPos, boxSize);

            //Save guiColor so we can revert it
            Color guiColor = GUI.color;

            List<NodePort> removeEntries = new List<NodePort>();

            //DrawAreas
            DrawAreas();

            if (e.type == EventType.Layout) culledNodes = new List<AbicraftNode>();
            for (int n = 0; n < graph.nodes.Count; n++) {

                bool notVisible = false;

                for (int i = 0; i < graph.areas.Count; i++)
                {
                    Area area = graph.areas[i];

                    Vector2 vecpos = GridToWindowPositionNoClipped(new Vector2(area.areaRect.x, area.areaRect.y));
                    Vector2 vecposAreaWidth = GridToWindowPositionNoClipped(new Vector2(area.areaRect.x + area.areaRect.width, area.areaRect.y + area.areaRect.height));

                    Rect yrect = new Rect(vecpos.x, vecpos.y, area.areaRect.width, area.areaRect.height);

                    // Skip null nodes. The user could be in the process renaming scripts, so removing them at this point is not advisable.
                    if (graph.nodes[n] == null) continue;

                    AbicraftNode anode = graph.nodes[n];

                    if (yrect.Contains(GridToWindowPositionNoClipped(anode.position)))
                    {
                        notVisible = !area.Visible;
                    }
                }

                if (notVisible)
                {
                    continue;
                }
                
                // Skip null nodes. The user could be in the process of renaming scripts, so removing them at this point is not advisable.
                if (graph.nodes[n] == null) continue;
                if (n >= graph.nodes.Count) return;
                AbicraftNode node = graph.nodes[n];

                // Culling
                if (e.type == EventType.Layout) {
                    // Cull unselected nodes outside view
                    if (!Selection.Contains(node) && ShouldBeCulled(node)) {
                        culledNodes.Add(node);
                        continue;
                    }
                } else if (culledNodes != null && culledNodes.Contains(node)) continue;

                if (e.type == EventType.Repaint) {
                    removeEntries.Clear();
                    foreach (var kvp in _portConnectionPoints)
                        if (kvp.Key.node == node) removeEntries.Add(kvp.Key);
                    foreach (var k in removeEntries) _portConnectionPoints.Remove(k);
                }

                NodeEditor nodeEditor = NodeEditor.GetEditor(node, this);

                NodeEditor.portPositions.Clear();

                float labelZoom = (zoom);

                // Set default label width. This is potentially overridden in OnBodyGUI
                EditorGUIUtility.labelWidth = 84;

                //Get node position
                Vector2 nodePos = GridToWindowPositionNoClipped(node.position);
                Rect xrect = new Rect(nodePos, new Vector2(nodeEditor.GetWidth(), 4000));
                GUILayout.BeginArea(xrect);

                bool selected = false;

                if(selectionCache != null && graph != null)
                    selected = selectionCache.Contains(graph.nodes[n]);

                if (selected) {
                    GUIStyle style = new GUIStyle(nodeEditor.GetBodyStyle(node));
                    GUIStyle highlightStyle = new GUIStyle(nodeEditor.GetBodyHighlightStyle());
                    highlightStyle.padding = style.padding;
                    style.padding = new RectOffset();
                    GUI.color = nodeEditor.GetTint();
                    GUILayout.BeginVertical(style);
                    GUI.color = NodeEditorPreferences.GetSettings().highlightColor;
                    GUILayout.BeginVertical(new GUIStyle(highlightStyle));
                } else {
                    GUIStyle style = new GUIStyle(nodeEditor.GetBodyStyle(node));
                    GUI.color = nodeEditor.GetTint();
                    GUILayout.BeginVertical(style);
                }

                GUI.color = guiColor;
                EditorGUI.BeginChangeCheck();

                GUIStyle labelstyle = new GUIStyle(NodeEditorResources.styles.nodeHeader);
                labelstyle.fontSize = (int)(10 * Mathf.Clamp(labelZoom + 0.3f, 1f, 1.5f));

                var hold = GUI.backgroundColor;
                //Draw node contents
                GUI.backgroundColor = new Color(0.75f, 0.75f, 0.75f);
                nodeEditor.OnHeaderGUI(labelstyle);
                nodeEditor.OnBodyGUI();
                GUI.backgroundColor = hold;

                //If user changed a value, notify other scripts through onUpdateNode
                if (EditorGUI.EndChangeCheck()) {
                    if (NodeEditor.onUpdateNode != null) NodeEditor.onUpdateNode(node);
                    EditorUtility.SetDirty(node);
                    nodeEditor.serializedObject.ApplyModifiedProperties();
                }

                GUILayout.EndVertical();

                //Cache data about the node for next frame
                if (e.type == EventType.Repaint) {
                    Vector2 size = GUILayoutUtility.GetLastRect().size;
                    if (nodeSizes.ContainsKey(node)) nodeSizes[node] = size;
                    else nodeSizes.Add(node, size);

                    foreach (var kvp in NodeEditor.portPositions) {
                        Vector2 portHandlePos = kvp.Value;
                        portHandlePos += node.position;
                        Rect rect = new Rect(portHandlePos.x - 8, portHandlePos.y - 8, 16, 16);
                        portConnectionPoints[kvp.Key] = rect;
                    }
                }

                if (selected) GUILayout.EndVertical();

                if (e.type != EventType.Layout) {
                    //Check if we are hovering this node
                    Vector2 nodeSize = GUILayoutUtility.GetLastRect().size;
                    Rect windowRect = new Rect(nodePos, nodeSize);
                    if (windowRect.Contains(mousePos)) hoveredNode = node;

                    //If dragging a selection box, add nodes inside to selection
                    if (currentActivity == NodeActivity.DragGrid) {
                        if (windowRect.Overlaps(selectionBox)) preSelection.Add(node);
                    }

                    //Check if we are hovering any of this nodes ports
                    //Check input ports
                    foreach (NodePort input in node.Inputs) {
                        //Check if port rect is available
                        if (!portConnectionPoints.ContainsKey(input)) continue;
                        Rect r = GridToWindowRectNoClipped(portConnectionPoints[input]);
                        if (r.Contains(mousePos)) hoveredPort = input;
                    }
                    //Check all output ports
                    foreach (NodePort output in node.Outputs) {
                        //Check if port rect is available
                        if (!portConnectionPoints.ContainsKey(output)) continue;
                        Rect r = GridToWindowRectNoClipped(portConnectionPoints[output]);
                        if (r.Contains(mousePos)) hoveredPort = output;
                    }
                }

                GUILayout.EndArea();
            }

            if (e.type != EventType.Layout && currentActivity == NodeActivity.DragGrid) Selection.objects = preSelection.ToArray();
            EndZoomed(position, zoom, topPadding);

            //If a change in is detected in the selected node, call OnValidate method. 
            //This is done through reflection because OnValidate is only relevant in editor, 
            //and thus, the code should not be included in build.
            if (onValidate != null && EditorGUI.EndChangeCheck()) onValidate.Invoke(Selection.activeObject, null);
        }

        void HorizResizer(Area areaObj, Rect headerRect, ref Rect r, float detectionRange = 8f)
        {
            detectionRange *= 0.5f;
            //Rect resizer = r;

            Rect area = new Rect(new Vector2(r.position.x, r.position.y + headerRect.height), new Vector2(r.width, r.height - headerRect.height));
            Rect right = area, left = area, down = area;

            //Debug.Log("HEI");
            r.width = r.width > 5000 ? 5000 : r.width;
            r.width = r.width < 100 ? 100 : r.width;

            r.height = r.height > 5000 ? 5000 : r.height;
            r.height = r.height < 100 ? 100 : r.height;

            right.xMin = right.xMax - 30; //4 pixels wide
            right.xMax += 30;

            left.xMax = left.xMin + 30;
            left.xMin -= 30;

            down.yMax += 15;
            down.yMin += r.height - 70;
            down.xMax -= 30;
            down.xMin += 30;

            //GUI.Box(down, "tester");
            //down.yMin = down.height + 30;

            Rect[] rects = new Rect[] { right, left, down };
            string[] dir = new string[] { "right", "left", "down" };

            if (!areaObj.resisingArea)
            {
                for (int i = 0; i < rects.Length; i++)
                {
                    if (rects[i].Contains(Event.current.mousePosition))
                    {
                        areaObj.activeResizer = rects[i];
                        areaObj.dir = dir[i];
                        break;
                    }
                }
            }

            if(Event.current.type == EventType.MouseDown && Event.current.button == 0 && areaObj.activeResizer.Contains(Event.current.mousePosition))
            {
                areaObj.resisingArea = true;
            }

            Event current = Event.current;
            //EditorGUIUtility.AddCursorRect(areaObj.activeResizer, MouseCursor.ResizeHorizontal);
            //Need a way to remove this Contain check since we already know where the mouse is
            if (areaObj.resisingArea)
            {
                if (areaObj.dir.Equals("right"))
                { r.xMax = Event.current.mousePosition.x + current.delta.x; }
                  
                if(areaObj.dir.Equals("down"))
                { r.yMax = Event.current.mousePosition.y + current.delta.y; }     
            }

            if(Event.current.type == EventType.MouseUp)
                areaObj.resisingArea = false;
        }

        private bool ShouldBeCulled(AbicraftNode node) {

            Vector2 nodePos = GridToWindowPositionNoClipped(node.position);
            if (nodePos.x / _zoom > position.width) return true; // Right
            else if (nodePos.y / _zoom > position.height) return true; // Bottom
            else if (nodeSizes.ContainsKey(node)) {
                Vector2 size = nodeSizes[node];
                if (nodePos.x + size.x < 0) return true; // Left
                else if (nodePos.y + size.y < 0) return true; // Top
            }
            return false;
        }

        private void DrawTooltip() {
            if (hoveredPort != null && NodeEditorPreferences.GetSettings().portTooltips && graphEditor != null) {
                string tooltip = graphEditor.GetPortTooltip(hoveredPort);
                if (string.IsNullOrEmpty(tooltip)) return;
                GUIContent content = new GUIContent(tooltip);
                Vector2 size = NodeEditorResources.styles.tooltip.CalcSize(content);
                size.x += 8;
                Rect rect = new Rect(Event.current.mousePosition - (size), size);
                EditorGUI.LabelField(rect, content, NodeEditorResources.styles.tooltip);
                Repaint();
            }
        }
    }
}
