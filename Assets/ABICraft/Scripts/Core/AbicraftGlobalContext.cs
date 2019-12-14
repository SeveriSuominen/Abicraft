using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbicraftGlobalContext
{
    public static readonly List<AbicraftObject> AllObjects = new List<AbicraftObject>();

    public static AbicraftObject FindObject(string name)
    {
        foreach (var obj in AllObjects)
        {
            if (obj.name == name)
                return obj;
        }
        
        GameObject obj_find_editor = GameObject.Find(name);

        if (obj_find_editor)
            return obj_find_editor.GetComponent<AbicraftObject>();

        return null;
    }

    public static AbicraftObject FindObject(GameObject gameObj)
    {
        return FindObject(gameObj ? gameObj.name : "");
    }
}
