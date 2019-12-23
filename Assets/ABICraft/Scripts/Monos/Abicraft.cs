using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;

[System.Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(AbilityDispatcher))]
public class Abicraft : MonoBehaviour
{
    public AbicraftGlobalDataFile dataFile;

    private void Update()
    {
        if (!AbicraftGlobalContext.abicraft)
            AbicraftGlobalContext.AddAbicraftInstance(this);
    }

    private void Start()
    {    
        if (Application.isPlaying)
        {
            if (!dataFile)
            {
                Debug.LogError("Abicraft: Missing data file reference");
                return;
            }

            AbicraftGlobalContext.AddAbicraftInstance(this);

            AbicraftGameStateSnapshot.InjectInputDataReferences(
                new AbicraftInputReferences
                {
                    Player = null
                }
            );

            AbicraftObjectPool.LoadPooledObjects(dataFile.InstantiateToPool);
            AbicraftObjectPool.LoadAllContextPooledObjects();
        }
    }
}


