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
    public class ColliderCastNode : AbicraftExecutionNode
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject collider;

        [Output]
        public AbicraftSignal Casted;

        [Output]
        public List<AbicraftObject> Collisions;

        [Output]
        public Vector3 Position;

        public ColliderCastMode castMode;

        [System.Serializable]
        public enum ColliderCastMode
        {
            MousePosition,
            StaticPositionRT,
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            Casted = new AbicraftSignal();

            AbicraftObject abj_marker = AbicraftObjectPool.Spawn(GetInputValue(e, "markerCollider", collider), null);
            AbicraftGameStateSnapshot snapshot = e.ae.initial_snapshot;

            CastController controller = null;

            switch (castMode)
            {
                case ColliderCastMode.MousePosition:
                    controller = abj_marker.gameObject.AddComponent<ObjectToMouseCastController>(); 
                    break;
                case ColliderCastMode.StaticPositionRT:
                    controller = abj_marker.gameObject.AddComponent<StaticPositionRTCastController>();
                    break;
            }

            controller.despawnWholeGameobject = false;

            controller.senderObject = e.ae.senderObject;
            controller.castcollider_abj = abj_marker;
            controller.cam = snapshot.camera;
            controller.keyCode  = KeyCode.Mouse0;

            controller.StartActionMono(e.ae.GetCooldownLeft(), false);

            while ((controller && !controller.ActionIsComplete))
            {
                yield return new WaitForFixedUpdate();
            }

            Position = controller.position;
          
            if (controller && controller.ActionWasSuccess)
            {
                ColliderTriggerCast cast = abj_marker.gameObject.AddComponent<ColliderTriggerCast>();
                cast.senderObject = e.ae.senderObject;

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