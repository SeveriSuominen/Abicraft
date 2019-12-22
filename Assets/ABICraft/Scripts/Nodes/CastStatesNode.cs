using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using AbicraftNodeEditor;

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
            var state = AbicraftGlobalContext.abicraft.dataFile.GlobalStates[selectedIndex];
            // CHANGE PATH!!!!!!!!!!!!!!!!

            if (!obj.activeStates.Contains(state))
            {
                obj.activeStates.Add(state);
                Debug.Log("obj APPLIED state: " + state.name);
            }
            else
            {
                Debug.Log("obj already has state: " + state.name);
            }
          
            yield return null;
        }
    }
}


