using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;
using AbicraftMonos;
using AbicraftNodes.Meta;

namespace AbicraftNodes.Action
{
    public class SpawnNode : AbicraftExecutionNode
    {
        [Output]
        public AbicraftObject SpawnedObject;
        private Dictionary<string, AbicraftObject> spawnedObjectsByIteration = new Dictionary<string, AbicraftObject>();

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public AbicraftObject ObjectToSpawn;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        public Vector3 spawnPosition, spawnRotation;

        public bool RandomYRot;

        public override void Initialize(AbicraftNodeExecution execution)
        {
           
        }

        public override IEnumerator ExecuteNode(AbicraftNodeExecution e)
        {
            AddLoopKey(e);

            var rotation = GetInputValue<Vector3>(e, "spawnRotation", spawnRotation);

            if (RandomYRot)
                rotation.y = Random.Range(0, 180);

            AbicraftObject abj = AbicraftObjectPool.Spawn (
                GetInputValue<AbicraftObject>(e, "ObjectToSpawn", ObjectToSpawn),
                GetInputValue<Vector3>(e, "spawnPosition", spawnPosition),
                Quaternion.Euler(rotation),
                null
            );

            if (abj)
            {
                AddObjectToIterationIndex<AbicraftObject>(ref spawnedObjectsByIteration, e, abj);
            }
            else
            {
                Debug.LogError("Abicraft: trying spawn null object");
            }
            yield return null;
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            if (port.fieldName == "Out")
                return GetInputValue<AbicraftLifeline>(e, "In");
            if (port.fieldName == "SpawnedObject" && e != null)
                return GetObjectByIterationIndex<AbicraftObject>(ref spawnedObjectsByIteration, e);

            return default;
        }
    }
}

