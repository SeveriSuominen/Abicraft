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
    private static Vector2 scrollPos = Vector2.zero;
    static readonly List<AbicraftObject> scannedSceneAbjs = new List<AbicraftObject>();
    float currentScrollViewWidth;
    Vector2 scrollPosition = Vector2.zero;
    bool resize = false;
    Rect cursorChangeRect;

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
        //Draw a line over the bottom part of the buttons (ugly haxx)
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, highlightCol);
        texture.Apply();
        GUI.DrawTexture(new Rect(0, buttonStyle.lineHeight + buttonStyle.border.top + buttonStyle.margin.top + StartSpace, Screen.width, 4), texture);

        return selected;
    }

    void OnEnable()
    {
        //this.position = new Rect(200, 200, 400, 300);
        currentScrollViewWidth = this.position.height / 2;
        cursorChangeRect = new Rect(currentScrollViewWidth, 0, 5f, this.position.width);
    }


    static Color coldef;

    void OnGUI()
    {
        Tabs(ref selectedTab, "Setup", "Abilities", "States", "Attributes", "Pooling");
        coldef = GUI.backgroundColor;

        GUI.backgroundColor = new Color32(184, 182, 182, 255);
        GUILayout.BeginArea(new Rect(0, 30, currentScrollViewWidth + 20, this.position.height - 30));
        GUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUIStyle searchStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
        GUIStyle cancelStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");
        GUIStyle noCancelStyle = GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty");

        GUILayout.Space(10);
        filter = EditorGUILayout.TextField(filter, searchStyle, GUILayout.Width(currentScrollViewWidth - 30));
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

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(currentScrollViewWidth));

        /*foreach(var abj in scannedSceneAbjs)
         {
             GUILayout.Label(abj.name);
         }*/
        CreateTree();
        GUILayout.EndScrollView();
        
        ResizeScrollView();
        GUILayout.FlexibleSpace();
        GUI.backgroundColor = coldef;
        if (GUILayout.Button("Scan", GUILayout.Width(currentScrollViewWidth -5), GUILayout.Height(60)))
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
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();

        Repaint();

        GUILayout.FlexibleSpace();
    }

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

        Rect marker = new Rect(10, rect.y + 8, rect.x - 20, 1);

      

        GUIContent content;

        if (transform.GetComponent<AbicraftObject>() || transform.GetComponent<Abicraft>())
        {
            GUI.DrawTexture(marker, EditorGUIUtility.whiteTexture);

            style.hover.textColor = Color.blue;

            style.normal.textColor = new Color32(219, 29, 29, 255);

            content = new GUIContent(transform.name);
            if (GUI.Button(rect, content, style))
            {

            }

            Rect iconRect = new Rect(marker);

            iconRect.width = 16;
            iconRect.height = 16;
            iconRect.x = currentScrollViewWidth - 35;
            iconRect.y = iconRect.y - 6;

            GUI.DrawTexture(iconRect, AbicraftNodeEditor.NodeEditorResources.abicraftIcon);
        }
        else
        {
           // float useIntend = (EditorGUI.indentLevel) * 20;
           // marker.width = useIntend -20;

            Color defcol = GUI.color;
            GUI.color = new Color32(156, 154, 154, 255);
            GUI.DrawTexture(marker, EditorGUIUtility.whiteTexture);
            GUI.color = defcol;

            style.normal.textColor = new Color32(100,100,100, 255);
            content = new GUIContent(transform.name);
           //rect.x = useIntend;

            GUI.Label(rect, content, style);
        }
    }

    private void ResizeScrollView()
    {
        GUI.DrawTexture(cursorChangeRect, EditorGUIUtility.whiteTexture);
        EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeHorizontal);

        if (Event.current.type == EventType.MouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
        {
            resize = true;
        }
        if (resize)
        {
            currentScrollViewWidth = Event.current.mousePosition.x;

            currentScrollViewWidth = currentScrollViewWidth > position.width - 50 ? position.width - 50 : currentScrollViewWidth;
            currentScrollViewWidth = currentScrollViewWidth < 50 ? 50 : currentScrollViewWidth;

            cursorChangeRect.Set(currentScrollViewWidth, cursorChangeRect.y, cursorChangeRect.width, position.height);
        }
        if (Event.current.type == EventType.MouseUp)
            resize = false;
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


