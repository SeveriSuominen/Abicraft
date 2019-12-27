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
    public class MissileNode : AbicraftActionSenderNode
    {

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject missile;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AnimationCurve speedOverDistanceCurve;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 direction;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 startPosition;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float startSpeed, endSpeed, maxRange;

        public override void Initialize(AbicraftNodeExecution execution)
        {
            
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AbicraftObject temp = AbicraftObjectPool.Spawn(GetInputValue(e, "missile", missile), null);
            Missile shot = temp.gameObject.AddComponent<Missile>();

            shot.startpoint = GetInputValue<Vector3>(e, "startPosition");
            shot.towards    = GetInputValue<Vector3>(e, "direction");
            shot.StartSpeed = GetInputValue<float>  (e, "startSpeed", startSpeed);
            shot.EndSpeed   = GetInputValue<float>  (e, "endSpeed",   endSpeed);
            shot.MaxRange   = GetInputValue<float>  (e, "maxRange", maxRange);
            shot.curve      = GetInputValue<AnimationCurve> (e, "speedOverDistanceCurve", speedOverDistanceCurve);

            shot.MoveToStartPoint();

            e.activeMono = shot;

            shot.StartActionMono();

            yield return null;
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return GetInputValue<AbicraftLifeline>(e, "In");
        }
    }
}