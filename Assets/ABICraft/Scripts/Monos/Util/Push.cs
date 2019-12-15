using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Push : AbicraftActionMono
{
    public AnimationCurve curve;
    public Vector3 Direction;
    public float Range = 0.7f;
    public float Force = 1.0f;

    private float currentRange;

    Vector3 startPoint;

    private void Awake()
    {
        startPoint = transform.position;
        destroyWholeGameobject = false;
    }

    void FixedUpdate()
    {
        if (Active)
        {
            currentRange = Vector3.Distance(startPoint, transform.position);


            transform.position += Direction * (Force * curve.Evaluate(currentRange / Range));

            if (Vector3.Distance(startPoint, transform.position) > Range)
            {
                CompleteActionAs(true);
            }
        }
    }
}
