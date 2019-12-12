using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbicraftActionMono : MonoBehaviour
{
    protected bool destroyWholeGameobject = false;

    public bool ActionIsComplete { get; private set; }
    public bool ActionWasSuccess { get; private set; }

    public virtual object ReturnData()
    {
        return null;
    }

    protected void CompleteActionAs(bool success)
    {
        if (ActionIsComplete)
            return;

        ActionIsComplete = true;
        ActionWasSuccess = success;

        StartCoroutine(End());
    }

    private IEnumerator End()
    {
        yield return new WaitForFixedUpdate();

        if (destroyWholeGameobject)
            Destroy(this.gameObject);
        else
            Destroy(this);
    }
}
