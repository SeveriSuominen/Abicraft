
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

using AbicraftCore;


public abstract class AbicraftExecutionNode : AbicraftNode
{
    [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
    public AbicraftLifeline In;
    [Output]
    public AbicraftLifeline Out;

    protected void AddObjectToIterationIndex<T>(ref Dictionary<int, T> map, int iterationIndex, T obj)
    {
        map[iterationIndex] = obj;
    }

    protected T GetObjectByIterationIndex<T>(ref Dictionary<int, T> map, int iterationIndex)
    {
        if (map.ContainsKey(iterationIndex))
            return map[iterationIndex];
        else
            return default(T);
    }

    public override object GetValue(AbicraftNodeExecution e, NodePort port)
    {
        return GetInputValue<AbicraftLifeline>(e, "In");
    }
}

