using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftMonos;
using AbicraftMonos.Action;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Action
{
    [DisallowMultipleComponent]
    public class ColliderCastNode : AbicraftExecutionNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject markerCollider;

        [Output]
        public AbicraftSignal Casted;

        [Output]
        public List<AbicraftObject> Collisions;

        [Output]
        public Vector3 Position;

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            Casted = new AbicraftSignal();

            AbicraftObject abj_marker = AbicraftObjectPool.Spawn(GetInputValue(e, "markerCollider", markerCollider), null);
            ObjectToMouseController controller = abj_marker.gameObject.AddComponent<ObjectToMouseController>();

            AbicraftGameStateSnapshot snapshot = e.ae.initial_snapshot;

            controller.despawnWholeGameobject = false;

            controller.abj = abj_marker;
            controller.cam = snapshot.camera;
            controller.keyCode = KeyCode.Mouse1;
            controller.StartActionMono(e.ae.GetCooldownLeft(), false);

            while ((controller && !controller.ActionIsComplete))
            {
                yield return new WaitForFixedUpdate();
            }

            Position = controller.position;
          
            if (controller && controller.ActionWasSuccess)
            {
                ColliderTriggerCast cast = abj_marker.gameObject.AddComponent<ColliderTriggerCast>();

                yield return new WaitForFixedUpdate();

                Collisions = cast.abj_collisions;

                Destroy(cast);
                Destroy(controller);

                AbicraftObjectPool.Despawn(abj_marker);

                Casted.Active = true;
            }

            if (controller && !controller.ActionWasSuccess)
            {
                Destroy(controller);
                AbicraftObjectPool.Despawn(abj_marker);
            }
            yield return null;
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            if (port.fieldName == "Out")
                return GetInputValue<AbicraftLifeline>(e, "In");
            if (port.fieldName == "Casted")
                return Casted;
            if (port.fieldName  == "Collisions")
                return Collisions;
            if (port.fieldName == "Position")
                return Position;

            return null;
        }
    }
}