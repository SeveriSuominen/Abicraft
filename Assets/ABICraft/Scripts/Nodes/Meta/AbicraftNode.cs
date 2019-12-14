using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public abstract class AbicraftNode : Node
{
    public virtual void Initialize(AbicraftNodeExecution execution)
    {
        return;
    }

    public virtual void Evaluate(AbicraftNodeExecution execution)
    {
        return;
    }

    public virtual IEnumerator ExecuteNode(AbicraftNodeExecution e)
    {
        yield return null;
    }
}

