using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbicraftMono : MonoBehaviour
{
    public delegate bool Destructor  ();
    public abstract bool DestroyWhen ();

    protected bool destroyWholeGameobject = false;

    protected virtual void CycleFrameStep()
    {
        if (DestroyWhen())
        {
            if (destroyWholeGameobject)
                Destroy(this.gameObject);
            else
                Destroy(this);
        }
    }
}
