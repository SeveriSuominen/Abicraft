using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using AbicraftMonos;

public class AbicraftInspector : EditorWindow
{
    static string filter = "", activeTab;
    static readonly List<Object> selectedBuffer = new List<Object>();
   
    static readonly List<AbicraftObject> scannedSceneAbjs = new List<AbicraftObject>();


    //SCENE OBJECT VIEW
    float sceneObjectViewSplitWidth, sceneObjectViewSplitHeight;
    Rect sceneObjectViewSplitWidthCursorRect, sceneObjectViewSplitHeightCursorRect;
    static Vector2 sceneObjectScrollView = Vector2.zero, assetObjectScrollView = Vector2.zero;

    bool resizeHorizontal = false, resizeVertical = false;
    //-----------------

    Vector2 scrollPosition = Vector2.zero;



    static int selectedTab = 0;

    [MenuItem("Tools/Abicraft/Inspector")]
    static void CreateAndShow()
    {
        EditorWindow window = EditorWindow.GetWindow<AbicraftInspector>("Abicraft Inspector");

        window.Show();
    }

    private void OnSelectionChange()
    {
        Repaint();
    }

    public int Tabs(ref int selected, params string[] tabs)
    {
        const float DarkGray = 0.6f;
        const float LightGray = 0.9f;
        const float StartSpace = 2;

        GUILayout.Space(StartSpace);
        Color storeColor = GUI.backgroundColor;
        Color highlightCol = new Color(DarkGray, DarkGray, DarkGray);
        Color bgCol = new Color(LightGray, LightGray, LightGray);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.padding.bottom = 2;
        buttonStyle.margin = new RectOffset(0, 0, 0, 0);

        GUILayout.BeginHorizontal();
        {   //Create a row of buttons
            for (int i = 0; i < tabs.Length; ++i)
            {
                GUI.backgroundColor = i == selected ? highlightCol : bgCol;
                if (GUILayout.Button(tabs[i], buttonStyle))
                {
                    selected = i; //Tab click
                }
            }
        }
        GUILayout.EndHorizontal();
        //Restore color
        GUI.backgroundColor = storeColor;
      
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, bgCol);
        texture.Apply();
        GUI.DrawTexture(new Rect(0, buttonStyle.lineHeight + buttonStyle.border.top + buttonStyle.margin.top + StartSpace, Screen.width, 4), texture);

        return selected;
    }

    void OnEnable()
    {
        //SCENE OBJECT SPLIT WIDTH
        sceneObjectViewSplitWidth = this.position.width / 2;
        sceneObjectViewSplitWidthCursorRect = new Rect(sceneObjectViewSplitWidth, 0, 3f, this.position.width);

        sceneObjectViewSplitHeight = this.position.height / 2;
        sceneObjectViewSplitHeightCursorRect= new Rect(0, sceneObjectViewSplitHeight, sceneObjectViewSplitWidth -22, 3f); 
    }


    void SceneObjectsView()
    {
        Color coldef = GUI.backgroundColor;

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

        sceneObjectScrollView = GUILayout.BeginScrollView(sceneObjectScrollView, GUILayout.Width(sceneObjectViewSplitWidth), GUILayout.Height(sceneObjectViewSplitHeight));

        CreateTree();

        GUILayout.EndScrollView();

        //GUILayout.EndVertical();
        //GUILayout.EndArea();
        
        ResizeScrollView(ref sceneObjectViewSplitWidth, ref sceneObjectViewSplitWidthCursorRect, ref resizeHorizontal, ResizeDir.Horizontal);

        //GUILayout.BeginArea(new Rect(0, sceneObjectViewSplitHeight, sceneObjectViewSplitWidth + 20, position.height));
        //GUILayout.BeginVertical();

        assetObjectScrollView = GUILayout.BeginScrollView(assetObjectScrollView, GUILayout.Width(sceneObjectViewSplitWidth), GUILayout.Height(position.height - sceneObjectViewSplitHeight));

        CreateTree();

        GUILayout.EndScrollView();

        ResizeScrollView(ref sceneObjectViewSplitHeight, ref sceneObjectViewSplitHeightCursorRect, ref resizeVertical, ResizeDir.Vertical);

        GUILayout.EndVertical();
        GUILayout.EndArea();

        GUILayout.FlexibleSpace();
        GUI.backgroundColor = coldef;
    
        /*if (GUILayout.Button("Scan", GUILayout.Width(currentScrollViewWidth - 5), GUILayout.Height(60)))
        {
            CreateTree();
            scannedSceneAbjs.Clear();

            Object[] objs = GameObject.FindObjectsOfType(typeof(AbicraftObject));

            for (int i = 0; i < objs.Length; i++)
            {
                if (!string.IsNullOrEmpty(filter) && objs[i].name.IndexOf(filter, System.StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    continue;
                }
                else
                {
                    scannedSceneAbjs.Add(objs[i] as AbicraftObject);
                }
            }
        }*/

        Repaint();

        GUILayout.FlexibleSpace();
    }

    void OnGUI()
    {
        selectedTab = Tabs(ref selectedTab, "Setup", "Scene Objects", "Abilities", "States", "Attributes", "Pooling");

        switch (selectedTab)
        {
            case 1:
                SceneObjectsView();
                    break;
        }
    }

    static bool keyDown = false;

    void DrawName(Transform transform)
    {

        if (!string.IsNullOrEmpty(filter) && transform.name.IndexOf(filter, System.StringComparison.CurrentCultureIgnoreCase) == -1)
        {
            // Skip if a filter is applied and we don't match
            return;
        }
        GUIStyle style = new GUIStyle(EditorStyles.label);

        if (Selection.objects.Contains(transform.gameObject))
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
            style.normal.textColor = new Color32(100,100,100, 255);
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

    private void ResizeScrollView(ref float axel, ref Rect cursorRect, ref bool resize, ResizeDir dir)
    {
        GUI.DrawTexture(cursorRect, EditorGUIUtility.whiteTexture);
        EditorGUIUtility.AddCursorRect(cursorRect, dir == ResizeDir.Horizontal ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);

        if (Event.current.type == EventType.MouseDown && cursorRect.Contains(Event.current.mousePosition))
        {
            resize = true;
        }
        if (resize)
        {
            if(dir == ResizeDir.Horizontal)
            {
                axel = Event.current.mousePosition.x;

                axel = axel > position.width - 50 ? position.width - 50 : axel;
                axel = axel < 50 ? 50 : axel;

                cursorRect.Set(axel, cursorRect.y, cursorRect.width, position.height);
            }

            if (dir == ResizeDir.Vertical)
            {
                Debug.Log(Event.current.mousePosition.y + " ::: " + Event.current.type);

                axel = Event.current.mousePosition.y;

              //  axel = axel > position.height - 50 ? position.height - 50 : axel;
               // axel = axel < 50 ? 50 : axel;

                cursorRect.Set(cursorRect.x, axel +15, sceneObjectViewSplitWidth - 32, cursorRect.height);
            }
        }
        if (Event.current.type == EventType.MouseUp)
        {
            resize = false;
        }   
    }

    void CreateTree()
    {
        Object[] scene_objs = GameObject.FindObjectsOfType(typeof(GameObject));

        List<Transform> objs_null_parent = new List<Transform>(); 

        for (int i = 0; i < scene_objs.Length; i++)
        {
            GameObject gobj = scene_objs[i] as GameObject;

            if(gobj.transform.parent == null)
                objs_null_parent.Add(gobj.transform);
        }

        for (int i = 0; i < objs_null_parent.Count; i++)
        {
            RecurseChildren(objs_null_parent[i]);
        }
    }

    void RecurseChildren(Transform parent)
    {
        EditorGUI.indentLevel++;
        foreach (Transform childTransform in parent)
        {
            DrawName(childTransform);

            if (childTransform.childCount > 0)
                RecurseChildren(childTransform);
        }
        EditorGUI.indentLevel--;
    }
}


