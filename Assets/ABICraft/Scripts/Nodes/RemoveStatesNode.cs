using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using AbicraftNodeEditor;
using AbicraftCore;

namespace AbicraftNodes.Action
{
    public class RemoveStatesNode : AbicraftExecutionNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;

        public RemoveStatesMode RemoveMode;

        [System.Serializable]
        public enum RemoveStatesMode
        {
            Selected,
            AllNegative,
            AllPositive,
            Everything,
        }


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

                    if (obj.activeStates.Contains(state))
                    {
                        obj.activeStates.Remove(state);
                    }
                }
            }

            yield return null;
        }
    }
}


