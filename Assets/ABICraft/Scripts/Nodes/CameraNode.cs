using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftCore;

namespace AbicraftNodes.Object
{
    public class CameraNode : AbicraftValueNode
    {
        [Output] public AbicraftObject camera;

        public override void Evaluate(AbicraftNodeExecution execution)
        {
            AbicraftGameStateSnapshot snapshot = execution.ae.initial_snapshot;

            AbicraftObject obj;

            if ((obj = snapshot.camera.GetComponent<AbicraftObject>()) == null)
                obj = snapshot.camera.gameObject.AddComponent<AbicraftObject>();

            camera = obj;   
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return camera;
        }
    }
}