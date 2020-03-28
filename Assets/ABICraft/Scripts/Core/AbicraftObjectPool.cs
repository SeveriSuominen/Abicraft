using AbicraftMonos;
using AbicraftMonos.Action;
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

        public static AbicraftObject Spawn(AbicraftObject abj, Transform parent)
        {
            return Spawn(abj, abj.transform.position, abj.transform.rotation, parent);
        }

        public static AbicraftObject Spawn(AbicraftObject abj, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (!abj)
            {
                Debug.LogWarning("Abicraft: Trying to spawn NULL object");
                return null;
            }
               
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].name == abj.name && !pool[i].ActivePool)
                {
                    pool[i].transform.SetParent(parent);
                    pool[i].transform.position = position;
                    pool[i].transform.rotation = rotation;
                    pool[i].gameObject.SetActive(true);
                    pool[i].ActivePool = true;

                    AbicraftActionMono[] abjmonos = pool[i].GetComponents<AbicraftActionMono>();
                    for (int j = 0; j < abjmonos.Length; j++)
                    {
                        abjmonos[j].OnSpawn();
                    }

                    return pool[i];
                }
            }
            Debug.LogWarning("Abicraft: Pool didnt find unactive object to spawn, using Instantiate instead, using Instantiate method will affect performance, consider increase AbicraftObject start instantiate amount or if not pooled, pool object");

            AbicraftObject instantiated = GameObject.Instantiate(abj.gameObject, abj.transform.position, abj.transform.rotation, parent).GetComponent<AbicraftObject>();

            AbicraftActionMono[] monos = instantiated.GetComponents<AbicraftActionMono>();
            for (int j = 0; j < monos.Length; j++)
            {
                monos[j].OnSpawn();
            }

            instantiated.transform.rotation = rotation;
            instantiated.transform.position = position;

            return instantiated;
        }

        public static bool Despawn(GameObject gameObj)
        {
            AbicraftObject abj;

            if (gameObj && (abj = gameObj.GetComponent<AbicraftObject>()))
            {
                return Despawn(abj);
            }
            Debug.LogError("Abicraft: Trying to despawn object from pool with gameObject instance that is not AbicraftObject");
            return false;
        }

        public static bool Despawn(AbicraftObject abj)
        {
            if (!abj)
                return false;

            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i] == abj && pool[i].ActivePool)
                {
                    AbicraftActionMono[] monos = abj.GetComponents<AbicraftActionMono>();
                    for (int j = 0; j < monos.Length; j++)
                    {
                        monos[j].ResetWhenPooled();
                    }

                    pool[i].transform.SetParent(abicraftObjectPoolParent.transform);
                    pool[i].ActivePool = false;

                    pool[i].ResetObject();

                    pool[i].gameObject.SetActive(false);
                    return true;
                }
            }
            Debug.LogWarning("Abicraft: Couldn't find object to despawn. Destroing instead. " + abj);
            GameObject.Destroy(abj.gameObject);
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

                if (!objRef.gameObject.activeSelf)
                    continue;

                if (!alreadyInstantiated.Contains(objRef.name) && objRef.InstantiateObjectToPool)
                {
                    for (int j = 0; j < objRef.InstantiateToPoolAmount; j++)
                    {
                        string name = objRef.gameObject.name;
                        GameObject objInstantiated = GameObject.Instantiate(objRef.gameObject, abicraftObjectPoolParent.transform);

                        AbicraftObject instantiated_abj = objInstantiated.GetComponent<AbicraftObject>();
                        instantiated_abj.ActivePool = false;
                        instantiated_abj.Original = objRef;
                        instantiated_abj.IsPoolingClone = true;

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

