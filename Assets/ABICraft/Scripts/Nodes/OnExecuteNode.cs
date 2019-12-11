using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes
{
    public class OnExecuteNode : AbicraftExecutionNode
    {
        public static uint id = 100;
        public Texture2D icon;

        public override void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {

        }

        public override IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            Debug.Log("On Execution Node");
            //TAKING INPUT SNAPSHOT WHEN STARTING EXECUTE 
            yield return null;
        }

        public override object GetValue(NodePort port)
        {
            return GetInputValue<int>("Out", 0);
        }
    }
}
