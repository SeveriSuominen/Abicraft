using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Action
{
    public class CastStatesNode : AbicraftExecutionNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;

        [HideInInspector]
        public int    selectedIndex, lastIndex;

        [HideInInspector]
        public List<int> allSelectedIndices = new List<int>();

        public override void Initialize(AbicraftNodeExecution execution)
        {
           
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftObject obj = GetInputValue<AbicraftObject>(e, "Obj");

            if (obj)
            {
                foreach (int index in allSelectedIndices)
                {
                    var state = AbicraftGlobalContext.abicraft.dataFile.GlobalStates[index];

                    if (!obj.activeStates.Contains(state))
                    {
                        obj.ApplyState(state);
                    }
                }
            }

            yield return null;
        }
    }
}


