using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public abstract class AbicraftExecutionNode : AbicraftNode
{
    [Input ] public int In;
    [Output] public int Out;

    public override object GetValue(NodePort port)
    {
        return GetInputValue<int>("Out", In + 1);
    }
}

