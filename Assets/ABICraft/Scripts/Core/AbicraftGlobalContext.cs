using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftCore
{
    public static class AbicraftGlobalContext
    {
        public static Abicraft abicraft { get { return GetAbicraftInstance(); } private set { } }
        static Abicraft abicraftInstance;

        public static readonly List<AbicraftObject> AllObjects = new List<AbicraftObject>();

        static Abicraft GetAbicraftInstance()
        {
            return abicraftInstance;
        }

        public static void AddAbicraftInstance(Abicraft abicraft)
        {
            abicraftInstance = abicraft;
        }

        public static AbicraftObject FindObject(string name)
        {
            foreach (var obj in AllObjects)
            {
                if (obj.InstantiateObjectToPool)
                {
                    if (obj.ActivePool && obj.name == name)
                        return obj;
                }
                else
                {
                    if (obj.gameObject.activeSelf && obj.name == name)
                        return obj;
                }
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
}

