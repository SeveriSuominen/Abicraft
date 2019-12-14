using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbicraftObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        AbicraftGlobalContext.AllObjects.Add(this);
    }

    private void OnDestroy()
    {
        AbicraftGlobalContext.AllObjects.Remove(this);
    }
}
