using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

using AbicraftCore;

namespace AbicraftNodes
{
    public class OnExecuteNode : AbicraftExecutionNode
    {
        public static uint id = 100;
        public Texture2D icon;

        public override void Initialize(AbicraftNodeExecution execution)
        {

        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            //TAKING INPUT SNAPSHOT WHEN STARTING EXECUTE 
            yield return null;
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return new AbicraftLifeline();
        }
    }
}
