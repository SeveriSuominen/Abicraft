
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

using AbicraftCore;


public abstract class AbicraftExecutionNode : AbicraftNode
{
    [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
    public AbicraftLifeline In;
    [Output]
    public AbicraftLifeline Out;

    public override object GetValue(AbicraftNodeExecution e, NodePort port)
    {
        return GetInputValue<AbicraftLifeline>(e, "In");
    }
}

