using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using System.Linq;
using AbicraftMonos;
using AbicraftCore;

[System.Serializable]
public class AbicraftInspector : EditorWindow
{
    static string filter = "", activeTab;
    static readonly List<Object> selectedBuffer = new List<Object>();
    static readonly List<AbicraftState> selectedStatesBuffer = new List<AbicraftState>();

    List<InspectorCache.ScannedSceneObjCacheEntry> scannedSceneObjs = new List<InspectorCache.ScannedSceneObjCacheEntry>();
    List<AbicraftObject> scannedAssetAbjs = new List<AbicraftObject>();
    List<AbicraftState>  scannedStates    = new List<AbicraftState>();

    List<string> scannedAssetAbjsPaths  = new List<string>();
    List<string> scannedAssetStatePaths = new List<string>();

    //OBJECT VIEW
    float sceneObjectViewSplitWidth, sceneObjectViewSplitHeight;
    Rect sceneObjectViewSplitWidthCursorRect, sceneObjectViewSplitHeightCursorRect;
    static Vector2 sceneObjectScrollView = Vector2.zero, assetObjectScrollView = Vector2.zero;

    bool resizeHorizontal = false, resizeVertical = false;
    //-----------------

    //STATES VIEW
    float stateViewSplitWidth;
    Rect stateViewSplitWidthCursorRect;
    static Vector2 stateScrollView = Vector2.zero;

    bool stateResizeHorizontal;
    //-----------------

    Vector2 scrollPosition = Vector2.zero;

    int selectedTab = 0, lastTab = 0;

    [MenuItem("Tools/Abicraft/Inspector")]
    static void CreateAndShow()
    {
        EditorWindow window = EditorWindow.GetWindow<AbicraftInspector>("Abicraft Inspector");
        window.Show();
    }


    void OnEnable()
    {
        //SCENE OBJECT SPLIT WIDTH
        sceneObjectViewSplitWidth = this.position.width / 2;
        sceneObjectViewSplitWidthCursorRect = new Rect(sceneObjectViewSplitWidth, 25, 3f, this.position.width);

        sceneObjectViewSplitHeight = this.position.height / 2;
        sceneObjectViewSplitHeightCursorRect = new Rect(0, sceneObjectViewSplitHeight + 10, sceneObjectViewSplitWidth - 22, 3f);

        //States SPLIT WIDTH
        stateViewSplitWidth = this.position.width / 2;
        stateViewSplitWidthCursorRect = new Rect(stateViewSplitWidth, 25, 3f, this.position.width);
    }


    void OnGUI()
    {
        if (AbicraftGlobalContext.abicraft)
        {
            if (!Application.isPlaying)
            {
                if (AbicraftGlobalContext.abicraft.dataFile.cache == null)
                {
                    AbicraftGlobalContext.abicraft.dataFile.cache = new InspectorCache();
                    EditorUtility.SetDirty(AbicraftGlobalContext.abicraft.dataFile);
                }
                else
                {
                    AbicraftGlobalContext.abicraft.dataFile.cache.scannedSceneObjs = scannedSceneObjs.Count > 0 ? scannedSceneObjs : AbicraftGlobalContext.abicraft.dataFile.cache.scannedSceneObjs;
                    AbicraftGlobalContext.abicraft.dataFile.cache.scannedAssetAbjs = scannedAssetAbjs.Count > 0 ? scannedAssetAbjs : AbicraftGlobalContext.abicraft.dataFile.cache.scannedAssetAbjs;
                    AbicraftGlobalContext.abicraft.dataFile.cache.scannedStates = scannedStates.Count > 0 ? scannedStates : AbicraftGlobalContext.abicraft.dataFile.cache.scannedStates;

                    AbicraftGlobalContext.abicraft.dataFile.cache.scannedAssetAbjsPaths = scannedAssetAbjsPaths.Count > 0 ? scannedAssetAbjsPaths : AbicraftGlobalContext.abicraft.dataFile.cache.scannedAssetAbjsPaths;
                    AbicraftGlobalContext.abicraft.dataFile.cache.scannedAssetStatePaths = scannedAssetStatePaths.Count > 0 ? scannedAssetStatePaths : AbicraftGlobalContext.abicraft.dataFile.cache.scannedAssetStatePaths;

                    EditorUtility.SetDirty(AbicraftGlobalContext.abicraft.dataFile);
                }
            }
            else
            {
                if (AbicraftGlobalContext.abicraft.dataFile.cache != null)
                {
                    scannedSceneObjs = AbicraftGlobalContext.abicraft.dataFile.cache.scannedSceneObjs;
                    scannedAssetAbjs = AbicraftGlobalContext.abicraft.dataFile.cache.scannedAssetAbjs;
                    scannedStates = AbicraftGlobalContext.abicraft.dataFile.cache.scannedStates;

                    scannedAssetAbjsPaths = AbicraftGlobalContext.abicraft.dataFile.cache.scannedAssetAbjsPaths;
                    scannedAssetStatePaths = AbicraftGlobalContext.abicraft.dataFile.cache.scannedAssetStatePaths;
                }
            }

            selectedTab = Tabs(ref selectedTab, "Setup", "Objects", "Abilities", "States", "Types", "Attributes", "Pooling");

            if(selectedTab != lastTab)
            {
                if (selectedTab == 1)
                {
                    if (!Application.isPlaying)
                    {
                        scannedAssetAbjs.Clear();
                        LoadPrefabAbjsContaining();
                    }
                }

                if (selectedTab == 3)
                {
                    if (!Application.isPlaying)
                    {
                        scannedStates.Clear();
                        LoadPrefabStatesContaining();
                    }
                }

                lastTab = selectedTab;
            }

            switch (selectedTab)
            {
                case 1:
                    ObjectViews();
                    break;
                case 3:
                    StateViews();
                    break;
            }
        }
        else
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 15;
            style.alignment = TextAnchor.MiddleCenter;

            Rect rect = EditorGUILayout.GetControlRect();

            rect.width = 500;
            rect.x = (position.width * 0.5f) - (rect.width * 0.5f);
            rect.y =  position.height * 0.5f - 100;

            GUI.Label(rect, "Looks like there is no Abicraft component instance injected, click button to scan or create new instance", style);

            EditorGUIUtility.SetIconSize(new Vector2(32, 32));

            GUIStyle btnStyle = new GUIStyle();
            Rect btnRect = new Rect(rect);
            btnRect.y += 50;
            btnRect.width = 150;
            btnRect.height = 60;
            btnRect.x = (position.width * 0.5f) - (btnRect.width * 0.5f);

            if(GUI.Button(btnRect, AbicraftNodeEditor.NodeEditorResources.abicraftIcon))
                TryInjectAbicraftInstance();
        }
    }

    void TryInjectAbicraftInstance()
    {
        Abicraft[] scene_abicraft = GameObject.FindObjectsOfType(typeof(Abicraft)) as Abicraft[];

        if (scene_abicraft.Length > 1)
        {
            Debug.LogError("There is more than one Abicraft component on Scene, only one allowed");
            return;

        }
        else if(scene_abicraft.Length == 1)
        {
            Abicraft abicraft = scene_abicraft[0];
            abicraft.enabled = true;
            abicraft.Inject();
        }
    }

    private void OnSelectionChange()
    {
        Repaint();
    }
    
    public List<AbicraftObject> LoadPrefabAbjsContaining()
    {
        if (Application.isPlaying)
            return new List<AbicraftObject>();

        string[] temp = AssetDatabase.GetAllAssetPaths();
        List<string> resultPaths = new List<string>();
        foreach (string s in temp)
        {
            if (s.Contains(".prefab"))
            {
                AbicraftObject abj = AssetDatabase.LoadAssetAtPath<AbicraftObject>(s);

                if (abj)
                {
                    abj.IsSceneObject = false;

                    scannedAssetAbjsPaths.Add(s);
                    scannedAssetAbjs.Add(abj);
                }
            }
        }
        return scannedAssetAbjs;
    }

    public List<AbicraftState> LoadPrefabStatesContaining()
    {
        //if (Application.isPlaying)
            //return new List<AbicraftState>();

        string[] temp = AssetDatabase.GetAllAssetPaths();
        List<string> resultPaths = new List<string>();
        foreach (string s in temp)
        {
            if (s.Contains(".asset"))
            {
                AbicraftState state = AssetDatabase.LoadAssetAtPath<AbicraftState>(s);

                if (state)
                {
                    scannedAssetStatePaths.Add(s);
                    scannedStates.Add(state);
                }
            }
        }
        return scannedStates;
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

    void CreateAssetList()
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

                if (obj && selectedBuffer.Contains(obj.transform.gameObject))
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

    void CreateStateList()
    {
        if (scannedStates.Count > 0)
        {
            for (int i = 0; i < scannedStates.Count; i++)
            {
                AbicraftState state = scannedStates[i];

                if (!string.IsNullOrEmpty(filter) && state.name.IndexOf(filter, System.StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    // Skip if a filter is applied and we don't match
                    continue;
                }

                Color guibgdef = GUI.backgroundColor;

                if (selectedStatesBuffer.Contains(state))
                {
                    GUI.backgroundColor = new Color32(217, 217, 217, 255);
                }
                
                GUILayout.BeginHorizontal();
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.hover.textColor = Color.blue;

                style.margin.right = 0;
                style.alignment = TextAnchor.MiddleLeft;

                GUIStyle stylePath = new GUIStyle(EditorStyles.label);
                stylePath.normal.textColor = new Color32(100, 100, 100, 255);
                stylePath.alignment = TextAnchor.MiddleLeft;

                EditorGUIUtility.SetIconSize(new Vector2(16, 16));

                if (GUILayout.Toggle(AbicraftGlobalContext.abicraft.dataFile.GlobalStates.Contains(state), "", GUILayout.Width(15)))
                {
                    if (!AbicraftGlobalContext.abicraft.dataFile.GlobalStates.Contains(state))
                    {
                        AbicraftGlobalContext.abicraft.dataFile.GlobalStates.Add(state);
                    }
                }
                else
                {
                    if (AbicraftGlobalContext.abicraft.dataFile.GlobalStates.Contains(state))
                    {
                        AbicraftGlobalContext.abicraft.dataFile.GlobalStates.Remove(state);
                    }
                }

                if (GUILayout.Button(new GUIContent(scannedStates[i].name, scannedStates[i].icon), style, GUILayout.Width(200)))
                {
                    if (keyDown)
                    {
                        selectedStatesBuffer.Add(state);
                    }
                    else
                    {
                        selectedStatesBuffer.Clear();
                        selectedStatesBuffer.Add(state);
                    }
                }
           
                GUILayout.Label(scannedAssetStatePaths[i], stylePath);

                GUILayout.EndHorizontal();
                GUI.backgroundColor = guibgdef;
            }
        }
    }

    public void AbicraftObjectInspector()
    {
        if (selectedBuffer.Count() == 0)
            return;

        Color32 defColor = GUI.backgroundColor;
        GUIStyle styleHeader = new GUIStyle();
        styleHeader.margin = new RectOffset(0, 7, 25, 15);
        styleHeader.fontSize = 15;

        GUIStyle areamargin = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
        areamargin.margin = new RectOffset(0, 0, 10, 10);
        areamargin.padding = new RectOffset(10, 10, 10, 10);
        
        GUIStyle fieldmargin = new GUIStyle();
        fieldmargin.margin = new RectOffset(3, 0, 5, 5);

        GUILayout.Space(27);
        //GUI.backgroundColor = new Color32(217, 217, 217, 255);
        GUILayout.Label("Abicraft Object Inspector", styleHeader, GUILayout.Width(sceneObjectViewSplitWidth), GUILayout.Height(30));
        // GUI.backgroundColor = defColor;

        GameObject exmp = selectedBuffer.Last() as GameObject;

        AbicraftObject abj = exmp.GetComponent<AbicraftObject>();
        Abicraft abicraft = exmp.GetComponent<Abicraft>();

        if (abj)
        {
            GUILayout.BeginVertical(areamargin);
            GUILayout.Label("Name", fieldmargin);
            abj.name = EditorGUILayout.TextField(abj.name);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(areamargin);
            GUILayout.BeginHorizontal();

            if (abj.InstantiateObjectToPool = GUILayout.Toggle(abj.InstantiateObjectToPool, "Instantiate Object To Pool"))
            {
                GUILayout.FlexibleSpace();

                GUILayout.Label("Amount");
                abj.InstantiateToPoolAmount = EditorGUILayout.IntField(abj.InstantiateToPoolAmount, GUILayout.MaxWidth(60));

                if (!abj.IsSceneObject && !AbicraftGlobalContext.abicraft.dataFile.InstantiateToPool.Contains(abj))
                {
                    AbicraftGlobalContext.abicraft.dataFile.InstantiateToPool.Add(abj as AbicraftObject);
                }
            }
            else
            {
                if (!abj.IsSceneObject && AbicraftGlobalContext.abicraft.dataFile.InstantiateToPool.Contains(abj))
                {
                    AbicraftGlobalContext.abicraft.dataFile.InstantiateToPool.Remove(abj);
                }
            }

            if (GUI.changed && abj)
            {
                EditorUtility.SetDirty(abj);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        else if(abicraft)
        {

            GUILayout.BeginVertical(areamargin);
            GUILayout.Label("Abicraft global data file");
            abicraft.dataFile = EditorGUILayout.ObjectField(abicraft.dataFile, typeof(AbicraftGlobalDataFile), false) as AbicraftGlobalDataFile;
            GUILayout.EndVertical();
        }
    }

    public void AbicraftStateInspector()
    {
        if (selectedStatesBuffer.Count() == 0)
            return;

        Color32 defColor = GUI.backgroundColor;
        GUIStyle styleHeader = new GUIStyle();
        styleHeader.margin = new RectOffset(0, 7, 25, 15);
        styleHeader.fontSize = 15;

        GUIStyle areamargin = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
        areamargin.margin = new RectOffset(0, 0, 10, 10);
        areamargin.padding = new RectOffset(10, 10, 10, 10);

        GUIStyle fieldmargin = new GUIStyle();
        fieldmargin.margin = new RectOffset(3, 0, 5, 5);

        GUILayout.Space(27);
        //GUI.backgroundColor = new Color32(217, 217, 217, 255);
        GUILayout.Label("State Inspector", styleHeader, GUILayout.Width(stateViewSplitWidth), GUILayout.Height(30));
        // GUI.backgroundColor = defColor;

        AbicraftState state = selectedStatesBuffer.Last();
        
        if (state)
        {
            SerializedObject stateSerialized = new UnityEditor.SerializedObject(state);

            GUILayout.BeginVertical(areamargin);
            GUILayout.BeginHorizontal();

            state.icon = EditorGUILayout.ObjectField(state.icon, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64)) as Texture2D;
            GUILayout.Space(10);


            GUILayout.BeginVertical();
            GUILayout.Space(15);
            GUILayout.Label("Name", fieldmargin);
            state.name = EditorGUILayout.TextField(state.name);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical(areamargin);

            EditorGUILayout.PropertyField(stateSerialized.FindProperty("type"));

            GUILayout.EndVertical();

            stateSerialized.ApplyModifiedProperties();

            if (GUI.changed && state)
            {
                EditorUtility.SetDirty(state);
            }
        }
    }

    void StateViews()
    {
        Color coldef = GUI.backgroundColor;

        ResizeScrollView(ref stateViewSplitWidth, ref stateViewSplitWidthCursorRect, ref stateResizeHorizontal, ResizeDir.Horizontal, 3);

        GUI.backgroundColor = new Color32(184, 182, 182, 255);
        GUILayout.BeginArea(new Rect(0, 30, stateViewSplitWidth + 20, position.height));
        GUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUIStyle searchStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
        GUIStyle cancelStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");
        GUIStyle noCancelStyle = GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty");

        GUILayout.Space(10);
        filter = EditorGUILayout.TextField(filter, searchStyle, GUILayout.Width(stateViewSplitWidth -30));
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
        GUILayout.Space(5);

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
        GUILayout.Label("Abicraft States", stylelabel, GUILayout.Width(stateViewSplitWidth), GUILayout.Height(30));
        GUI.backgroundColor = defColor;

        stateScrollView = GUILayout.BeginScrollView(stateScrollView, margin, GUILayout.Width(stateViewSplitWidth), GUILayout.Height(position.height -125));

        CreateStateList();

        GUILayout.EndScrollView();

        //GUILayout.FlexibleSpace();
        if (GUILayout.Button("Scan", GUILayout.Width(stateViewSplitWidth - 5), GUILayout.Height(32)) && !Application.isPlaying)
        {
            scannedStates.Clear();
            LoadPrefabStatesContaining();
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();

        GUILayout.FlexibleSpace();

        Repaint();

        GUILayout.BeginArea(new Rect(stateViewSplitWidth + 20, 30, position.width - stateViewSplitWidth - 40, position.height));
        GUILayout.BeginVertical();

        GUI.backgroundColor = coldef;

        AbicraftStateInspector();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void ObjectViews()
    {
        Color coldef = GUI.backgroundColor;

        ResizeScrollView(ref sceneObjectViewSplitWidth, ref sceneObjectViewSplitWidthCursorRect, ref resizeHorizontal, ResizeDir.Horizontal, 1);

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
        GUILayout.Space(5);

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
        GUILayout.Label("Abicraft Asset Objects", stylelabel, GUILayout.Width(sceneObjectViewSplitWidth), GUILayout.Height(30));
        GUI.backgroundColor = defColor;

        assetObjectScrollView = GUILayout.BeginScrollView(assetObjectScrollView, GUILayout.Width(sceneObjectViewSplitWidth), GUILayout.Height(position.height - sceneObjectViewSplitHeight - 155));

        CreateAssetList();

        GUILayout.EndScrollView();
        ResizeScrollView(ref sceneObjectViewSplitHeight, ref sceneObjectViewSplitHeightCursorRect, ref resizeVertical, ResizeDir.Vertical, 1);
        //GUILayout.FlexibleSpace();
        if (GUILayout.Button("Scan", GUILayout.Width(sceneObjectViewSplitWidth - 5), GUILayout.Height(32)) && !Application.isPlaying)
        {
            scannedAssetAbjs.Clear();
            LoadPrefabAbjsContaining();
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();

        GUI.backgroundColor = coldef;

        Repaint();

        GUILayout.FlexibleSpace();

        GUILayout.BeginArea(new Rect(sceneObjectViewSplitWidth + 20, 30, position.width - sceneObjectViewSplitWidth - 40, position.height));
        GUILayout.BeginVertical();

        AbicraftObjectInspector();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    //KeyDown for multiselect "Left CTRL"
    static bool keyDown = false;

    void DrawSceneObjectTreeEntry(Transform transform, int indirectIndex = -1)
    {
        if (!string.IsNullOrEmpty(filter) && transform.name.IndexOf(filter, System.StringComparison.CurrentCultureIgnoreCase) == -1)
        {
            // Skip if a filter is applied and we don't match
            return;
        }

        if (!transform.GetComponent<AbicraftObject>() && !transform.GetComponent<Abicraft>())
        {
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

        float rectX = indirectIndex == -1 ? rect.x : indirectIndex * 15.5f; //Using as a padding
        rect.width = sceneObjectViewSplitWidth;
        rect.x = 0;

        style.padding.left = Mathf.FloorToInt(rectX);

        Rect marker = new Rect(10, rect.y + 8, rectX - 15, 1);

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

            if(transform.parent != null)
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

            if (transform.parent != null)
                GUI.DrawTexture(marker, EditorGUIUtility.whiteTexture);

            GUI.color = defcol;
        }
    }

    public enum ResizeDir
    {
        Horizontal,
        Vertical
    }

    private void ResizeScrollView(ref float axel, ref Rect cursorRect, ref bool resize, ResizeDir dir, int tab)
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
            if(dir == ResizeDir.Horizontal)
            {
                axel = Event.current.mousePosition.x;

                axel = axel > position.width - 200 ? position.width - 200 : axel;
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

        if(tab == 1)
        {
            if (dir == ResizeDir.Horizontal)
            {
                cursorRect.Set(axel, cursorRect.y, cursorRect.width, position.height);
            }

            if (dir == ResizeDir.Vertical)
            {
                cursorRect.Set(cursorRect.x, axel + 55, sceneObjectViewSplitWidth, cursorRect.height);
            }
        }

        if (tab == 3)
        {
            if (dir == ResizeDir.Horizontal)
            {
                cursorRect.Set(axel, cursorRect.y, cursorRect.width, position.height);
            }
        }
    }

    void CreateSceneObjectTree()
    {
        if (Application.isPlaying /* CREATE TREE FROM CACHE INSTEAD*/)
        {
            for (int i = 0; i < scannedSceneObjs.Count; i++)
            {
                DrawSceneObjectTreeEntry(scannedSceneObjs[i].obj, scannedSceneObjs[i].indirectIndex);
            }

            return;
        }

        //CLEAR CACHE
        scannedSceneObjs.Clear();

        Object[] scene_objs = GameObject.FindObjectsOfType(typeof(GameObject));

        List<Transform> objs_root_parent = new List<Transform>(); 

        for (int i = 0; i < scene_objs.Length; i++)
        {
            GameObject gobj = scene_objs[i] as GameObject;

            if(gobj.transform.parent == null)
            {
                objs_root_parent.Add(gobj.transform);
            }
              
        }

        for (int i = 0; i < objs_root_parent.Count; i++)
        {
            if (objs_root_parent[i])
            {
                if (AbicraftObjectPool.abicraftObjectPoolParent && objs_root_parent[i].Equals(AbicraftObjectPool.abicraftObjectPoolParent.transform))
                    continue;

                RecurseChildren(objs_root_parent[i], true);
            }  
        }
    }

    void RecurseChildren(Transform parent, bool firstPass = false)
    {
        //INCLUDE ROOT PARENT
        if (firstPass)
        {
            AbicraftObject abj;

            if ((abj = parent.GetComponent<AbicraftObject>()))
            {
                abj.IsSceneObject = true;
            }

            if ((abj = parent.GetComponent<AbicraftObject>()))
            {
                abj.IsSceneObject = true;
            }

            //CACHE OBJS FOR PLAY MODE
            scannedSceneObjs.Add(new InspectorCache.ScannedSceneObjCacheEntry(parent, EditorGUI.indentLevel));
            DrawSceneObjectTreeEntry(parent);
        }

        EditorGUI.indentLevel++;
        foreach (Transform childTransform in parent)
        {
            AbicraftObject abj;

            if((abj = childTransform.GetComponent<AbicraftObject>()))
            {
                abj.IsSceneObject = true;
            }

            //CACHE OBJS FOR PLAY MODE
            scannedSceneObjs.Add(new InspectorCache.ScannedSceneObjCacheEntry(childTransform, EditorGUI.indentLevel));

            DrawSceneObjectTreeEntry(childTransform);

            if (childTransform.childCount > 0)
                RecurseChildren(childTransform);
        }
        EditorGUI.indentLevel--;
    }
}


