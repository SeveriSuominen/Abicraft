
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

using AbicraftCore;

public abstract class AbicraftExecutionLoopNode : AbicraftNode
{
    [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
    public AbicraftLifeline In;
    [Output]
    public AbicraftLifeline Loop;
    [Output]
    public float Iteration;
    [Output]
    public AbicraftLifeline Continue;

    [HideInInspector]
    public float max_iterations;

    public Dictionary<string, float> iterations = new Dictionary<string, float>();

    [HideInInspector]
    public List<int> iteratedIndices = new List<int>();
    //[Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Unconnected)]

    [HideInInspector]
    public int Iterations;

    [HideInInspector]
    public bool Parallel;

    public virtual int IterationCount(AbicraftNodeExecution e)
    {
        return GetInputValue<int>(e, "Iterations", Iterations);
    }

    protected bool NodeBelongsToLoop(NodePort port)
    {
        return true;
    }

    public override object GetValue(AbicraftNodeExecution e, NodePort port)
    {
        if (port.fieldName == "Iteration")
        {
            if (e != null)
            {
                foreach (var item in loopKeys)
                {
                    string key = port.fieldName + item + e.GetIterationIndex(loopKeys[e.ae.guid]);

                    if (iterations.ContainsKey(key))
                    {
                        return iterations[key];
                    }
                    else
                    {
                        iterations.Add(key, e.GetIterationIndex(loopKeys[e.ae.guid]));
                        return iterations[key];
                    }
                }
            }
            return 0;
        }
        else if (port.fieldName == "In")
        {
            return GetInputValue<AbicraftLifeline>(e, "In");
        }

        return default;
    }
}

