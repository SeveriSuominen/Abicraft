using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftMonos.Action
{
    //[RequireComponent(typeof(Rigidbody))]
    public class Missile : AbicraftActionMono
    {
        public Vector3 towards;
        public float StartSpeed, EndSpeed;
        public float MaxRange;

        private float currentSpeed = 0, currentDistance = 0;

        public AnimationCurve curve;

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

            despawnWholeGameobject = true;
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
                base.ActionMonoUpdate(Time.deltaTime);

                if (StartSpeed <= 0.025f)
                {
                    StartSpeed = 0.025f;
                }

                if (EndSpeed <= 0.025f)
                {
                    EndSpeed = 0.025f;
                }

                currentDistance = Vector3.Distance(startpoint, transform.position);

                if (curve == null)
                {
                    currentSpeed = Mathf.Lerp(StartSpeed, EndSpeed, currentDistance / MaxRange);
                }
                else
                {
                    currentSpeed = Mathf.Lerp(StartSpeed, EndSpeed, currentDistance / MaxRange);
                    currentSpeed = currentSpeed * curve.Evaluate(currentDistance / MaxRange) + 0.05f;
                }

                transform.position += towards * currentSpeed;
                transform.rotation = Quaternion.LookRotation(towards, Vector3.up);

                RaycastHit hit = GetRaycastHit(RaycastDirection.Forward);

                if (hit.transform)
                {
                    if (rayAbicraftObject == null || rayAbicraftObject.transform != hit.transform)
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
}


