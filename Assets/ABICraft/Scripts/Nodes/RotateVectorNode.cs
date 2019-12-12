using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Math
{
    public class RotateVectorNode : AbicraftValueNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 directionIn;
        [Output] public Vector3 directionOut;

        [Input(typeConstraint = TypeConstraint.Strict, backingValue = ShowBackingValue.Never)]
        public Vector3 RotateAmountDegrees;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float X, Y, Z;

        public override void Evaluate(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            
        }

        public override object GetValue(NodePort port)
        {
            Vector3 original = GetInputValue<Vector3>("directionIn");
            Vector3 vector   = GetInputValue<Vector3>("RotateAmountDegrees");
            
            if (GetPort("RotateAmountDegrees").ConnectionCount == 0)
                vector = new Vector3(
                    GetInputValue<float>("X", X),
                    GetInputValue<float>("Y", Y),
                    GetInputValue<float>("Z", Z)
                );

            return RotateRadians(original, Mathf.PI * vector.y / 180.0f );
        }

        public Vector3 RotateRadians(Vector3 v, float radians)
        {
            var ca = Mathf.Cos(radians);
            var sa = Mathf.Sin(radians);
            return new Vector3(ca * v.x - sa * v.z, 0, sa * v.x + ca * v.z);
        }
    }
}