using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.VFX
{
    public class ShakeNode : AbicraftExecutionNode
    {
        public static uint id = 113;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject Obj;

        // How long the object should shake for.

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float shakeDuration = 10f;

        // Amplitude of the shake. A larger value shakes the camera harder.
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float shakeAmount = 0.7f;
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float decreaseFactor = 1.0f;

        public override void Initialize(AbicraftNodeExecution execution)
        {
            
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftObject obj = GetInputValue<AbicraftObject>(e, "Obj", Obj);
            if (obj)
            {
                Shake shakeMono = obj.gameObject.AddComponent<Shake>();

                shakeMono.target = GetInputValue<AbicraftObject>(e, "obj").gameObject;

                shakeMono.decreaseFactor = decreaseFactor;
                shakeMono.shakeAmount = shakeAmount;
                shakeMono.shakeDuration = shakeDuration;
            }
            yield return null;
        }
    }
}