using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AbicraftMonos;
using AbicraftCore;

public static class AbicraftObjectControl 
{

    static string filter = "", activeTab;
    static readonly List<Object> selectedBuffer = new List<Object>();

    public static Rect position;

    static readonly List<AbicraftObject> scannedSceneAbjs = new List<AbicraftObject>();

    static readonly List<AbicraftObject> scannedAssetAbjs = new List<AbicraftObject>();
    static readonly List<string> scannedAssetAbjsPaths = new List<string>();

    //SCENE OBJECT VIEW
    static float sceneObjectViewSplitWidth, sceneObjectViewSplitHeight;
    static Rect sceneObjectViewSplitWidthCursorRect, sceneObjectViewSplitHeightCursorRect;
    static Vector2 sceneObjectScrollView = Vector2.zero, assetObjectScrollView = Vector2.zero;

    static bool resizeHorizontal = false, resizeVertical = false;
    //-----------------

    static Vector2 scrollPosition = Vector2.zero;
    static int selectedTab = 0;

    [MenuItem("Tools/Abicraft/Inspector")]
    static void CreateAndShow()
    {
        EditorWindow window = EditorWindow.GetWindow<AbicraftInspector>("Abicraft Inspector");
        window.Show();
    }

    static void TryInjectAbicraftInstance()
    {
        Abicraft[] scene_abicraft = GameObject.FindObjectsOfType(typeof(Abicraft)) as Abicraft[];

        Debug.Log(scene_abicraft.Length);

        if (scene_abicraft.Length > 1)
        {
            Debug.LogError("There is more than one Abicraft component on Scene, only one allowed");
            return;
        }
        else if (scene_abicraft.Length == 1)
        {
            Abicraft abicraft = scene_abicraft[0];
            abicraft.enabled = true;

            abicraft.Inject();
        }
    }

    public static List<AbicraftObject> LoadPrefabsContaining()
    {
        string[] temp = AssetDatabase.GetAllAssetPaths();
        List<string> resultPaths = new List<string>();
        foreach (string s in temp)
        {
            if (s.Contains(".prefab"))
            {
                AbicraftObject abj = AssetDatabase.LoadAssetAtPath<AbicraftObject>(s);

                if (abj)
                {
                    scannedAssetAbjsPaths.Add(s);
                    scannedAssetAbjs.Add(abj);
                }
            }
        }
        return scannedAssetAbjs;
    }

    public static void Initialize()
    {
        //SCENE OBJECT SPLIT WIDTH
        sceneObjectViewSplitWidth = position.width / 2;
        sceneObjectViewSplitWidthCursorRect = new Rect(sceneObjectViewSplitWidth, 25, 3f, position.width);

        sceneObjectViewSplitHeight = position.height / 2;
        sceneObjectViewSplitHeightCursorRect = new Rect(0, sceneObjectViewSplitHeight + 10, sceneObjectViewSplitWidth - 22, 3f);
    }

    static void CreateAssetList()
    {
        if (scannedAssetAbjs.Count > 0)
        {
            for (int i = 0; i < scannedAssetAbjs.Count; i++)
            {
                AbicraftObject obj = scannedAssetAbjs[i];

                if (!string.IsNullOrEmpty(filter) && obj.name.IndexOf(filter, System.StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    // Skip if a filter is applied and we don't match
                    continue;
                }

                Color guibgdef = GUI.backgroundColor;

                if (selectedBuffer.Contains(obj.transform.gameObject))
                {
                    GUI.backgroundColor = new Color32(217, 217, 217, 255);
                }

                GUILayout.BeginHorizontal();
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.hover.textColor = Color.blue;
                style.normal.textColor = new Color32(219, 29, 29, 255);
                style.margin.right = 0;
                style.alignment = TextAnchor.MiddleLeft;

                GUIStyle stylePath = new GUIStyle(EditorStyles.label);
                stylePath.normal.textColor = new Color32(100, 100, 100, 255);
                stylePath.alignment = TextAnchor.MiddleLeft;

                EditorGUIUtility.SetIconSize(new Vector2(16, 16));

                if (GUILayout.Button(new GUIContent(scannedAssetAbjs[i].name, AbicraftNodeEditor.NodeEditorResources.abicraftIcon), style, GUILayout.Width(200)))
                {
                    if (keyDown)
                    {
                        selectedBuffer.Add(obj.transform.gameObject);
                    }
                    else
                    {
                        selectedBuffer.Clear();
                        selectedBuffer.Add(obj.transform.gameObject);
                    }
                }
                GUILayout.Label(scannedAssetAbjsPaths[i], stylePath);

                GUILayout.EndHorizontal();
                GUI.backgroundColor = guibgdef;
            }
        }
    }

    public static void ObjectViews(Rect position)
    {
        AbicraftObjectControl.position = position;

        Color coldef = GUI.backgroundColor;

        ResizeScrollView(ref sceneObjectViewSplitWidth, ref sceneObjectViewSplitWidthCursorRect, ref resizeHorizontal, ResizeDir.Horizontal);

        GUI.backgroundColor = new Color32(184, 182, 182, 255);
        GUILayout.BeginArea(new Rect(0, 30, sceneObjectViewSplitWidth + 20, position.height));
        GUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUIStyle searchStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
        GUIStyle cancelStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");
        GUIStyle noCancelStyle = GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty");

        GUILayout.Space(10);
        filter = EditorGUILayout.TextField(filter, searchStyle, GUILayout.Width(sceneObjectViewSplitWidth - 30));
        if (!string.IsNullOrEmpty(filter))
        {
            if (GUILayout.Button("", cancelStyle))
            {
                filter = "";
                GUIUtility.hotControl = 0;
                EditorGUIUtility.editingTextField = false;
            }
        }
        else
        {
            GUILayout.Button("", noCancelStyle);
        }
        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();


        Color32 defColor = GUI.backgroundColor;
        GUIStyle stylelabel = new GUIStyle();
        stylelabel.margin.left = 0;
        stylelabel.padding.top = 5;
        stylelabel.padding.bottom = 5;
        stylelabel.padding.left = 7;
        stylelabel.fontSize = 15;

        GUIStyle margin = new GUIStyle();
        margin.margin.bottom = 5;

        GUI.backgroundColor = new Color32(217, 217, 217, 255);
        GUILayout.Label("Abicraft Scene Objects", stylelabel, GUILayout.Width(sceneObjectViewSplitWidth), GUILayout.Height(30));
        GUI.backgroundColor = defColor;

        sceneObjectScrollView = GUILayout.BeginScrollView(sceneObjectScrollView, margin, GUILayout.Width(sceneObjectViewSplitWidth), GUILayout.Height(sceneObjectViewSplitHeight));

        CreateSceneObjectTree();

        GUILayout.EndScrollView();

        GUI.backgroundColor = new Color32(217, 217, 217, 255);
        GUILayout.Label("Abicraft Assets", stylelabel, GUILayout.Width(sceneObjectViewSplitWidth), GUILayout.Height(30));
        GUI.backgroundColor = defColor;

        assetObjectScrollView = GUILayout.BeginScrollView(assetObjectScrollView, GUILayout.Width(sceneObjectViewSplitWidth), GUILayout.Height(position.height - sceneObjectViewSplitHeight - 150));

        CreateAssetList();

        GUILayout.EndScrollView();
        ResizeScrollView(ref sceneObjectViewSplitHeight, ref sceneObjectViewSplitHeightCursorRect, ref resizeVertical, ResizeDir.Vertical);
        //GUILayout.FlexibleSpace();
        if (GUILayout.Button("Scan", GUILayout.Width(sceneObjectViewSplitWidth - 5), GUILayout.Height(32)))
        {
            scannedAssetAbjs.Clear();
            LoadPrefabsContaining();
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();

        GUI.backgroundColor = coldef;

        //Repaint();

        GUILayout.FlexibleSpace();
    }

    //KeyDown for multiselect "Left CTRL"
    static bool keyDown = false;

    static void DrawSceneObjectTree(Transform transform)
    {

        if (!string.IsNullOrEmpty(filter) && transform.name.IndexOf(filter, System.StringComparison.CurrentCultureIgnoreCase) == -1)
        {
            // Skip if a filter is applied and we don't match
            return;
        }
        GUIStyle style = new GUIStyle(EditorStyles.label);

        if (selectedBuffer.Contains(transform.gameObject))
        {
            style = new GUIStyle(style);
            style.normal.textColor = Color.blue;
        }

        Rect rect = EditorGUILayout.GetControlRect();
        rect = EditorGUI.IndentedRect(rect);

        float rectX = rect.x; //Using as a padding
        rect.width = sceneObjectViewSplitWidth;
        rect.x = 0;

        style.padding.left = Mathf.FloorToInt(rectX);

        Rect marker = new Rect(10, rect.y + 8, rectX - 20, 1);

        GUIContent content;

        if (transform.GetComponent<AbicraftObject>() || transform.GetComponent<Abicraft>())
        {
            Color bgcolordef = GUI.backgroundColor;

            style.hover.textColor = Color.blue;
            style.normal.textColor = new Color32(219, 29, 29, 255);

            if (selectedBuffer.Contains(transform.gameObject))
            {
                GUI.backgroundColor = new Color32(217, 217, 217, 255);
            }

            content = new GUIContent(transform.name);

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == (KeyCode.LeftControl))
            {
                keyDown = true;
            }

            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == (KeyCode.LeftControl))
            {
                keyDown = false;
            }

            if (GUI.Button(rect, content, style))
            {
                if (keyDown)
                {
                    selectedBuffer.Add(transform.gameObject);
                }
                else
                {
                    selectedBuffer.Clear();
                    selectedBuffer.Add(transform.gameObject);
                }
            }

            Rect iconRect = new Rect(marker);

            iconRect.width = 16;
            iconRect.height = 16;
            iconRect.x = sceneObjectViewSplitWidth - 35;
            iconRect.y = iconRect.y - 6;

            GUI.DrawTexture(marker, EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(iconRect, AbicraftNodeEditor.NodeEditorResources.abicraftIcon);

            GUI.backgroundColor = bgcolordef;
        }
        else
        {
            style.normal.textColor = new Color32(100, 100, 100, 255);
            content = new GUIContent(transform.name);

            GUI.Label(rect, content, style);

            Color defcol = GUI.color;
            GUI.color = new Color32(156, 154, 154, 255);
            GUI.DrawTexture(marker, EditorGUIUtility.whiteTexture);
            GUI.color = defcol;
        }
    }

    public enum ResizeDir
    {
        Horizontal,
        Vertical
    }

    private static void ResizeScrollView(ref float axel, ref Rect cursorRect, ref bool resize, ResizeDir dir)
    {
        Color def = GUI.color;
        GUI.color = new Color32(133, 133, 133, 255);
        GUI.DrawTexture(cursorRect, EditorGUIUtility.whiteTexture);
        GUI.color = def;
        EditorGUIUtility.AddCursorRect(cursorRect, dir == ResizeDir.Horizontal ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);

        if (Event.current.type == EventType.MouseDown && cursorRect.Contains(Event.current.mousePosition))
        {
            resize = true;
        }
        if (resize)
        {
            if (dir == ResizeDir.Horizontal)
            {
                axel = Event.current.mousePosition.x;

                axel = axel > position.width - 50 ? position.width - 50 : axel;
                axel = axel < 200 ? 200 : axel;
            }

            if (dir == ResizeDir.Vertical)
            {
                axel = Event.current.mousePosition.y;

                axel = axel > position.height - 200 ? position.height - 200 : axel;
                axel = axel < 50 ? 50 : axel;
            }
        }
        if (Event.current.type == EventType.MouseUp)
        {
            resize = false;
        }

        if (dir == ResizeDir.Horizontal)
        {
            cursorRect.Set(axel, cursorRect.y, cursorRect.width, position.height);
        }

        if (dir == ResizeDir.Vertical)
        {
            cursorRect.Set(cursorRect.x, axel + 50, sceneObjectViewSplitWidth, cursorRect.height);
        }
    }

    static void CreateSceneObjectTree()
    {
        Object[] scene_objs = GameObject.FindObjectsOfType(typeof(GameObject));

        List<Transform> objs_null_parent = new List<Transform>();

        for (int i = 0; i < scene_objs.Length; i++)
        {
            GameObject gobj = scene_objs[i] as GameObject;

            if (gobj.transform.parent == null)
                objs_null_parent.Add(gobj.transform);
        }

        for (int i = 0; i < objs_null_parent.Count; i++)
        {
            RecurseChildren(objs_null_parent[i]);
        }
    }

    static void RecurseChildren(Transform parent)
    {
        EditorGUI.indentLevel++;
        foreach (Transform childTransform in parent)
        {
            DrawSceneObjectTree(childTransform);

            if (childTransform.childCount > 0)
                RecurseChildren(childTransform);
        }
        EditorGUI.indentLevel--;
    }
}
