using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Math
{
    public class RotateDirectionVector3Node : AbicraftValueNode
    {
        [Input ] public Vector3 directionIn;
        [Output] public Vector3 directionOut;

        [Input]  public Vector3 RotateAmountDegrees;

        public override void Evaluate(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            
        }

        public override object GetValue(NodePort port)
        {
            Vector3 original = GetInputValue<Vector3>("directionIn");
            //Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(original), Vector3.one);

            return RotateRadians(original, Mathf.PI * RotateAmountDegrees.y / 180.0f );

            /*return  new Vector3(
                original.x * Mathf.Sqrt(RotateAmount.x),
                original.y * Mathf.Sqrt(RotateAmount.y),
                original.z * Mathf.Sqrt(RotateAmount.z)
            ).normalized;*/
        }

        public Vector3 RotateRadians(Vector3 v, float radians)
        {
            var ca = Mathf.Cos(radians);
            var sa = Mathf.Sin(radians);
            return new Vector3(ca * v.x - sa * v.z, 0, sa * v.x + ca * v.z);
        }
    }
}