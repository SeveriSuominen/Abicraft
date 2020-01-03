using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Action
{
    public class DespawnNode : AbicraftExecutionNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject ObjectToDespawn;

        public override void Initialize(AbicraftNodeExecution execution)
        {
           
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftObjectPool.Despawn(GetInputValue<AbicraftObject>(e, "ObjectToDespawn", ObjectToDespawn));
            yield return null;
        }
    }
}

