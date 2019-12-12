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
    public AbicraftObject hittedObject;

    Rigidbody rigid;

    public override object ReturnData()
    {
        return hittedObject;
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();

        destroyWholeGameobject = true;
    }

    public void MoveToStartPoint()
    {
        transform.position = startpoint;
    }

    private void OnCollisionStay(Collision other)
    {
        if (hittedObject = other.gameObject.GetComponent<AbicraftObject>())
        {
            CompleteActionAs(true);
        }
    }
    void FixedUpdate()
    {
        transform.position += towards * Speed;
        //rigid.AddForce(towards * Speed);

        if (Vector3.Distance(startpoint, transform.position) > MaxRange)
        {
            CompleteActionAs(false);
        }
    }
}
