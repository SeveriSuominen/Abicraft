using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

namespace AbicraftNodes.Action
{
    [DisallowMultipleComponent]
    public class MissileNode : AbicraftActionSenderNode
    {

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject missile;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AnimationCurve animationCurve;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 direction;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 startPosition;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public float StartSpeed, EndSpeed, maxRange;

        public int lul;

        public override void Initialize(AbicraftNodeExecution execution)
        {
            
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            if (lul == 33)
            {
                Debug.Log(e.iterationIndices.Count);

                foreach (KeyValuePair<string, int> item in e.iterationIndices)
                    Debug.Log(item.Key + " : " + item.Value);
            }

            AbicraftObject temp = AbicraftObjectPool.Spawn(missile, null);
            Missile shot = temp.gameObject.AddComponent<Missile>();

            shot.startpoint = GetInputValue<Vector3>(e, "startPosition");
            shot.towards    = GetInputValue<Vector3>(e, "direction");
            shot.StartSpeed = GetInputValue<float>  (e, "StartSpeed", StartSpeed);
            shot.EndSpeed   = GetInputValue<float>  (e, "EndSpeed",   EndSpeed);
            shot.MaxRange   = GetInputValue<float>  (e, "maxRange", maxRange);
            shot.curve      = GetInputValue<AnimationCurve> (e, "maxRange", animationCurve);

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