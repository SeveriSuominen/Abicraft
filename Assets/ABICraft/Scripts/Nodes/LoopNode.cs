using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using XNode;

namespace AbicraftNodes.Action
{
    public class LoopNode : AbicraftExecutionLoopNode
    {
        public override void Initialize(AbicraftNodeExecution execution)
        {
            
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            yield return null;
        }
    }
}


