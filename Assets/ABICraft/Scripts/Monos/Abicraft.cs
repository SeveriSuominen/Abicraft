using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;


[System.Serializable]
[RequireComponent(typeof(AbilityDispatcher))]
public class Abicraft : MonoBehaviour
{
    public AbicraftGlobalDataFile dataFile;

    private void Start()
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


