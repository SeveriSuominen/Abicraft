using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public abstract class AbicraftNode : Node
{
    public virtual void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
    {
        return;
    }

    public virtual void Evaluate(AbicraftAbilityExecution.AbicraftNodeExecution execution)
    {
        return;
    }

    public virtual IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
    {
        yield return null;
    }
}

