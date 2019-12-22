using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbicraftNodeEditor;

namespace AbicraftNodes.Object
{
    public class PlayerNode : AbicraftValueNode
    {
        [Output] public AbicraftObject player;

        public override void Evaluate(AbicraftNodeExecution execution)
        {
            AbicraftGameStateSnapshot snapshot = execution.ae.initial_snapshot;
            string name = "VelluaMagician_A_SKEL";
            AbicraftObject obj;

            if ((obj = snapshot.player.GetComponent<AbicraftObject>()) == null)
                obj = snapshot.player.gameObject.AddComponent<AbicraftObject>();

            Debug.Log("get Player");
            player = AbicraftGlobalContext.FindObject(name);


            Debug.Log("OBJ IS...");
            Debug.Log(player == null);
            Debug.Log("______");
        }

        public override object GetValue(AbicraftNodeExecution e, NodePort port)
        {
            return player;
        }
    }
}