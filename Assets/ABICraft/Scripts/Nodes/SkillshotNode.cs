using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Action
{
    public class SkillshotNode : AbicraftExecutionNode
    {
        public static uint id = 113;

        [Input] public Vector3 direction; 

        [Input] public AbicraftObject missile;
        [Input] public float speed, maxRange;

        public override void Initialize(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            
        }

        public override IEnumerator ExecuteNode(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            Debug.Log("Skillshot Node: " + GetInputValue<Vector3>("direction"));

            AbiCraftStateSnapshot snapshot = execution.AbilityExecution.initial_snapshot;
            
            GameObject temp = GameObject.Instantiate(missile.gameObject);

            Skillshot shot = temp.GetComponent<Skillshot>();

            shot.startpoint = temp.transform.position = snapshot.player.transform.position;
            shot.towards    = GetInputValue<Vector3>("direction");
            shot.Speed      = speed;
            shot.MaxRange   = maxRange;

            yield return null;
        }
    }
}