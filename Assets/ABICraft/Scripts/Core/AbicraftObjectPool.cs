using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftCore
{
    public static class AbicraftObjectPool
    {
        public static GameObject abicraftObjectPoolParent { get; private set; }

        static readonly List<AbicraftObject> pool = new List<AbicraftObject>();
        static readonly List<string> alreadyInstantiated = new List<string>();

        public static AbicraftObject Spawn(GameObject gameObj, Transform parent)
        {
            AbicraftObject obj;

            if (gameObj && (obj = gameObj.GetComponent<AbicraftObject>()))
            {
                return Spawn(obj, parent);
            }
            Debug.LogError("Abicraft: Trying to spawn object from pool with gameObject instance that is not AbicraftObject");
            return null;
        }

        public static AbicraftObject Spawn(AbicraftObject obj, Transform parent)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].name == obj.name && !pool[i].ActivePool)
                {
                    pool[i].transform.SetParent(parent);
                    pool[i].gameObject.SetActive(true);
                    pool[i].ActivePool = true;

                    return pool[i];
                }
            }
            Debug.LogWarning("Abicraft: Pool didnt find unactive object to spawn, using Instantiate instead, using Instantiate method will affect performance, consider increase AbicraftObject instantiate amount");
            return GameObject.Instantiate(obj.gameObject, parent).GetComponent<AbicraftObject>();
        }

        public static bool Despawn(GameObject gameObj)
        {
            AbicraftObject obj;

            if (gameObj && (obj = gameObj.GetComponent<AbicraftObject>()))
            {
                return Despawn(obj);
            }
            Debug.LogError("Abicraft: Trying to despawn object from pool with gameObject instance that is not AbicraftObject");
            return false;
        }

        public static bool Despawn(AbicraftObject obj)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i] == obj && pool[i].ActivePool)
                {
                    pool[i].transform.SetParent(abicraftObjectPoolParent.transform);
                    pool[i].ActivePool = false;

                    pool[i].ResetObject();

                    pool[i].gameObject.SetActive(false);
                    return true;
                }
            }
            Debug.LogWarning("Abicraft: Couldn't find object to despawn.");
            return false;
        }

        public static void LoadAllContextPooledObjects(bool reinitialize = false)
        {
            LoadPooledObjects(AbicraftGlobalContext.AllObjects, reinitialize);
        }

        static void Initialize()
        {
            string poolParentName = "AbicraftObjectPool";

            pool.Clear();
            alreadyInstantiated.Clear();
            abicraftObjectPoolParent = new GameObject(poolParentName);
        }

        // Update is called once per frame
        public static void LoadPooledObjects(List<AbicraftObject> objects, bool reinitialize = false)
        {
            if (reinitialize)
            {
                if (abicraftObjectPoolParent)
                {
                    GameObject.Destroy(abicraftObjectPoolParent);
                    abicraftObjectPoolParent = null;
                }
            }

            if (!abicraftObjectPoolParent)
            {
                Initialize();
            }

            for (int i = 0; i < objects.Count; i++)
            {
                AbicraftObject objRef = objects[i];

                if (!alreadyInstantiated.Contains(objRef.name) && objRef.InstantiateObjectToPool)
                {
                    for (int j = 0; j < objRef.InstantiateToPoolAmount; j++)
                    {
                        string name = objRef.gameObject.name;
                        GameObject objInstantiated = GameObject.Instantiate(objRef.gameObject, abicraftObjectPoolParent.transform);

                        AbicraftObject instantiated_abj = objInstantiated.GetComponent<AbicraftObject>();
                        instantiated_abj.ActivePool = false;
                        instantiated_abj.Original = objRef;

                        pool.Add(instantiated_abj);

                        objInstantiated.name = name;
                        objInstantiated.SetActive(false);
                    }

                    alreadyInstantiated.Add(objRef.gameObject.name);
                }
            }
        }
    }
}

