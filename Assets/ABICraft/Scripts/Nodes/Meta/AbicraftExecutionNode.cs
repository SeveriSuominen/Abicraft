
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

using AbicraftCore;


public abstract class AbicraftExecutionNode : AbicraftNode
{
    [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
    public AbicraftLifeline In;
    [Output]
    public AbicraftLifeline Out;

    public override object GetValue(NodePort port)
    {
        return GetInputValue<AbicraftLifeline>("In");
    }
}

