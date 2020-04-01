using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AbicraftCore;
using AbicraftMonos.Action;
using AbicraftMonos;

public class ColliderTriggerCast : MonoBehaviour
{
    public List<AbicraftObject> abj_collisions = new List<AbicraftObject>();
    public AbicraftObject senderObject;

    public bool ignoreSenderObject = true;

    private void OnTriggerStay(Collider collider)
    {
        var abj = collider.GetComponent<AbicraftObject>();

        if (abj && !abj_collisions.Contains(abj))
        {
            if(ignoreSenderObject && abj != senderObject)
                abj_collisions.Add(abj);
        }
    }
}
