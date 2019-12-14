using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Action
{
    public class SkillshotNode : AbicraftExecutionNode
    {
        public static uint id = 113;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 direction;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 startPosition;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject missile;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float speed, maxRange;

        private AbicraftLifeline lifeline;

        public override void Initialize(AbicraftNodeExecution execution)
        {
            
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            GameObject temp = GameObject.Instantiate(missile.gameObject);

            lifeline = GetInputValue<AbicraftLifeline>(e, "In");
            Skillshot shot = temp.AddComponent<Skillshot>();

            shot.startpoint = GetInputValue<Vector3>(e, "startPosition");
            shot.towards    = GetInputValue<Vector3>(e, "direction");
            shot.Speed      = GetInputValue<float>(e, "speed", speed);
            shot.MaxRange   = GetInputValue<float>(e, "maxRange", maxRange);

            shot.MoveToStartPoint();

            lifeline.mono = shot;

            shot.StartActionMono();

            yield return null;
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return lifeline ?? GetInputValue<AbicraftLifeline>(e, "In");
        }
    }
}