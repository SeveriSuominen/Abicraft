using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftMonos.Action;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Action
{
    public class PushNode : AbicraftExecutionNode
    {
        public static uint id = 113;

        [Output]
        public AbicraftSignal CompleteSignal;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;

        public bool forcePush;

        // How long the object should shake for.
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AnimationCurve ForceOverDistanceCurve;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Direction;
 
        // Amplitude of the shake. A larger value shakes the camera harder.
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Range = 0.7f;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float MaxRange = 3f;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Force = 1.0f;//, YForce = 0;
 
        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            CompleteSignal = new AbicraftSignal();

            AbicraftObject obj = GetInputValue<AbicraftObject>(e, "Obj", Obj);

            if (obj)
            {
                Push push = obj.gameObject.AddComponent<Push>();

                push.Direction = GetInputValue<Vector3>(e, "Direction", Direction);
                push.Force = GetInputValue<float>(e, "Force", Force);
               // push.YForce = GetInputValue<float>(e, "YForce", YForce);
                push.Range = GetInputValue<float>(e, "Range", Range);
                push.MaxRange = GetInputValue<float>(e, "MaxRange", MaxRange);
                push.curve = GetInputValue<AnimationCurve>(e, "Curve", ForceOverDistanceCurve);
                push.forcePush = this.forcePush;

                push.StartActionMono();

                while (!push.ActionIsComplete)
                {
                    yield return new WaitForFixedUpdate();
                }
                CompleteSignal.Active = true;
            }
            yield return null;
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            if (port.fieldName.Equals("CompleteSignal"))
                return CompleteSignal;

            return GetInputValue<AbicraftLifeline>(e, "In");
        }
    }
}