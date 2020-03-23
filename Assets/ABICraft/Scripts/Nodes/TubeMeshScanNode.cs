using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Object
{
    public class TubeMeshScanNode : AbicraftExecutionNode
    {
        [Output]
        public List<AbicraftObject> Objs;
        private Dictionary<string, List<AbicraftObject>> ObjsIterations = new Dictionary<string, List<AbicraftObject>>();

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Position;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 Direction;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        [Range(0f, 100f)]
        public float radiusInner = .5f, radiusOuter = .30f;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        [Range(1, 360f)]
        public float angle = 360;

        public override void Initialize(AbicraftNodeExecution e)
        {
            e.Block();
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            GameObject obj = new GameObject("CONE_COLLIDER_");

            MeshFilter   filter   = obj.AddComponent<MeshFilter>();
            MeshCollider collider = obj.AddComponent<MeshCollider>();

            Cone cone = obj.AddComponent<Cone>();

            cone.transform.localScale = new Vector3(5, 5, 5);
            collider.convex = true;
            collider.isTrigger = true;

            cone.position    = GetInputValue<Vector3>(e, "Position", Position);
            cone.direction   = GetInputValue<Vector3>(e, "Direction", Direction);
            cone.radiusInner = GetInputValue<float>(e, "radiusInner", radiusInner); 
            cone.radiusOuter = GetInputValue<float>(e, "radiusOuter", radiusOuter);
            cone.angle = GetInputValue<float>(e, "angle", angle); 

            cone.Create();

            yield return new WaitForFixedUpdate();

            AddObjectToIterationIndex<List<AbicraftObject>>(ref ObjsIterations, e, cone.collisions);
   
            e.ReleaseBlock();

            Destroy(obj);

            yield return null;
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            if (port.fieldName.Equals("Objs"))
            {
                return GetObjectByIterationIndex<List<AbicraftObject>>(ref ObjsIterations, e);
            }
            return GetInputValue<AbicraftLifeline>(e, "In");
        }
    }
}