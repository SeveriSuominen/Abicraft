using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


[System.Serializable]
public class Abicraft : MonoBehaviour
{
    public AbicraftObject   Player;

    public List<AbicraftObject> instantiateToPool = new List<AbicraftObject>();

    private void Start()
    {
        AbiCraftStateSnapshot.InjectInputDataReferences(
            new AbicraftInputReferences
            {
                Player = this.Player
            }
        );

        AbicraftObjectPool.LoadPooledObjects(instantiateToPool);
        AbicraftObjectPool.LoadAllContextPooledObjects();
    }
}


