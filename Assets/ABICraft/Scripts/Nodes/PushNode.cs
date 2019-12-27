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

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;

        // How long the object should shake for.

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AnimationCurve ForceOverDistanceCurve;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Direction;
 
        // Amplitude of the shake. A larger value shakes the camera harder.
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Range = 0.7f;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Force = 1.0f;

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftObject obj = GetInputValue<AbicraftObject>(e, "Obj", Obj);

            Debug.Log("OBJSA IS : " + obj);

            if (obj)
            {
                Push push = obj.gameObject.AddComponent<Push>();

                push.Direction = GetInputValue<Vector3>(e, "Direction", Direction);
                push.Force = GetInputValue<float>(e, "Force", Force);
                push.Range = GetInputValue<float>(e, "Range", Range);
                push.curve = GetInputValue<AnimationCurve>(e, "Curve", ForceOverDistanceCurve);

                push.StartActionMono();
            }
            yield return null;
        }
    }
}