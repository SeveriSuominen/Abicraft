using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Action
{
    public class OnHitNode : AbicraftExecutionNode
    {
        public static uint id = 112;
        public Texture2D icon;

        public override void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {

        }

        public override IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            Debug.Log("LEL3");
            //TAKING INPUT SNAPSHOT WHEN STARTING EXECUTE 
            yield return null;
        }
    }
}

