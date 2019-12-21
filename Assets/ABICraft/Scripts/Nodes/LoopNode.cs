using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using AbicraftNodeEditor;

namespace AbicraftNodes.Action
{
    public class LoopNode : AbicraftExecutionLoopNode
    {

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Unconnected)]
        public int maxIterations;
        public bool parallel;

        public override void Initialize(AbicraftNodeExecution e)
        {
            Iterations = GetInputValue<int>(e, "maxIterations", maxIterations);
            Parallel = parallel;
        }
    }
}


