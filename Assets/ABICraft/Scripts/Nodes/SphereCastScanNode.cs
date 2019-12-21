using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

namespace AbicraftNodes.Object
{
    public class SphereCastScanNode : AbicraftValueNode
    {
        [Output]
        public List<AbicraftObject> Objs;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Position;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        //[Tooltip("You can leave direction to zero point, to scan nearby objects radius as distance")]
       
        public Vector3 Direction;

        
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float Radius;

        public override void Evaluate(AbicraftNodeExecution execution)
        {
            base.Evaluate(execution);
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            var position  = GetInputValue<Vector3>(e, "Position",  Position);
            var direction = GetInputValue<Vector3>(e, "Direction", Direction);
            var radius    = GetInputValue<float>  (e, "Radius",    Radius);

            if (Objs != null)
                Objs.Clear();

            RaycastHit[] hits = Physics.SphereCastAll(position, radius, direction);

            for (int i = 0; i < hits.Length; i++)
            {
                AbicraftObject obj;

                if (obj = hits[i].transform.GetComponent<AbicraftObject>())
                    Objs.Add(obj);
            }

            //Debug.Log("RAYHITS IN SPEHERE : " + hits.Length);

            return Objs;
        }
    }
}