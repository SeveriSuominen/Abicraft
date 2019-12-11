using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using XNode;

namespace AbicraftNodes.Action
{
    public class AnimationNode : AbicraftExecutionNode
    {
        public static uint id = 111;

        [Input] public string AnimationTrigger;
        [Input] public float  BlockForSeconds;

        //Wanna pass arguments for editor? Use non-seriziable sturct to hold that data
        //and access it in custom editor via Target.
        public NodeData data;

        public struct NodeData
        {
            public Color statusColor;
        }
        //-----------------------------------------------------------------------------

        public override void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            data = new NodeData();
            execution.Block();
        }

        public override IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            Debug.Log("Animation Node");

            GameObject player = AbiCraftStateSnapshot.TakeSnapshot.player.gameObject;

            player.GetComponent<Animator>().SetTrigger(AnimationTrigger);
            player.GetComponent<NavMeshAgent>().ResetPath();
            data.statusColor = Color.yellow;

            yield return new WaitForSeconds(BlockForSeconds);

            data.statusColor = Color.green;
            execution.ReleaseBlock();
        }
    }

}


