using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbicraftNodes.Object
{
    public class CameraNode : AbicraftValueNode
    {
        [Output] public AbicraftObject camera;

        public override void Evaluate(AbicraftAbilityExecution.AbicraftNodeExecution execution)
        {
            AbiCraftStateSnapshot snapshot = execution.AbilityExecution.initial_snapshot;

            AbicraftObject obj;

            if ((obj = snapshot.camera.GetComponent<AbicraftObject>()) == null)
                obj = snapshot.camera.gameObject.AddComponent<AbicraftObject>();

            camera = obj;   
        }

        public override object GetValue(NodePort port)
        {
            return camera;
        }
    }
}