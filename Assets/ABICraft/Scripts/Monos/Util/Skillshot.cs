using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skillshot : AbicraftMono
{
    public Vector3 towards;
    public float Speed;
    public float MaxRange;

    public Vector3 startpoint;

    private void Start()
    {
        destroyWholeGameobject = true;
    }

    void FixedUpdate()
    {
        transform.position += towards * Speed;

        CycleFrameStep();
    }

    public override bool DestroyWhen()
    {
        return Vector3.Distance(startpoint, transform.position) > MaxRange;
    }
}
