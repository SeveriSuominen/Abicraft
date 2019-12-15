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
    public AbicraftObject original;
    [HideInInspector]
    public bool activePool;
    [HideInInspector]
    public int poolIndex;

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
        transform.position = original.transform.position;
        transform.rotation = original.transform.rotation;
        transform.localScale = original.transform.localScale;
    }
}
