using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Skillshot : AbicraftActionMono
{
    public Vector3 towards;
    public float Speed;
    public float MaxRange;

    public Vector3 startpoint;

    [HideInInspector]
    public AbicraftObject rayAbicraftObject;

    Rigidbody rigid;

    public override object ReturnData()
    {
        return rayAbicraftObject;
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();

        destroyWholeGameobject = true;
    }

    public void MoveToStartPoint()
    {
        transform.position = startpoint;
        transform.rotation = Quaternion.LookRotation(towards, Vector3.up);
    }

    void FixedUpdate()
    {
        if (Active)
        {
            transform.position += towards * Speed;
            transform.rotation = Quaternion.LookRotation(towards, Vector3.up);

            RaycastHit hit = GetRaycastHit(RaycastDirection.Forward);

            if (hit.transform)
            {
                if(rayAbicraftObject == null || rayAbicraftObject.transform != hit.transform)
                {
                    rayAbicraftObject = hit.transform.GetComponent<AbicraftObject>();
                }

                if (rayAbicraftObject)
                {
                    if (Vector3.Distance(transform.position, hit.point) < 1f)
                        CompleteActionAs(true);
                }
            }

            if (Vector3.Distance(startpoint, transform.position) > MaxRange)
            {
                CompleteActionAs(false);
            }
        }
    }
}
