using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbicraftObject : MonoBehaviour
{
    [SerializeField]
    public bool InstantiateObjectToPool;
    [SerializeField]
    public int InstantiateToPoolAmount;
    [HideInInspector]
    public AbicraftObject Original;
    [HideInInspector]
    public bool ActivePool;
    [HideInInspector]
    public int PoolIndex;

    public readonly List<AbicraftState> activeStates = new List<AbicraftState>();

    void Awake()
    {
        AbicraftGlobalContext.AllObjects.Add(this);
    }

    private void OnDestroy()
    {
        AbicraftGlobalContext.AllObjects.Remove(this);
    }

    public void ResetObject()
    {
        transform.position   = Original.transform.position;
        transform.rotation   = Original.transform.rotation;
        transform.localScale = Original.transform.localScale;
    }
}
