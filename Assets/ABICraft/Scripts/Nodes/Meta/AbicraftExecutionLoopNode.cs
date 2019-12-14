
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

using AbicraftCore;


public abstract class AbicraftExecutionLoopNode : AbicraftNode
{
    [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
    public AbicraftLifeline In;
    [Output]
    public AbicraftLifeline Loop;
    [Output]
    public AbicraftLifeline Continue;

    //[Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
    public bool Parallel;

    public override object GetValue(NodePort port)
    {
        return GetInputValue<AbicraftLifeline>("In");
    }
}

