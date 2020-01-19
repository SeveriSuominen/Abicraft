using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AbicraftMonos.Action
{
    //[RequireComponent(typeof(Rigidbody))]
    public class Push : AbicraftActionMono
    {
        AbicraftObject abj;

        public bool forcePush;

        public AnimationCurve curve;
        public Vector3 Direction;
        public float Range = 0.7f;
        public float Force = 1.0f;
        public float YForce = 0;
        private float currentRange;

        Vector3 startPoint;

        private void Awake()
        {
            abj = GetComponent<AbicraftObject>();

            startPoint = transform.position;
            despawnWholeGameobject = false;
        }
        Vector3 moveTo, moveToY;
        public override void OnActive()
        {
            if (forcePush)
            {
                abj.rigidBody.AddForce(Direction * ((Force * 2000) * abj.rigidBody.mass));
                // abj.rigidBody.AddForce(new Vector3(0, 1, 0) * (YForce * 2000));
                moveTo = transform.position + (Direction * Range);
                moveToY = new Vector3(moveTo.x, 5, moveTo.z);
            }
        }

        bool downshifted;

        void FixedUpdate()
        {
            if (Active && !forcePush)
            {
                currentRange = Vector3.Distance(startPoint, transform.position);
                transform.position += Direction * (Force * curve.Evaluate(currentRange / Range));
                
                if (Vector3.Distance(startPoint, transform.position) > Range)
                {
                    CompleteActionAs(true);
                }
            }

            if(Active && forcePush)
            {
                currentRange = Vector3.Distance(startPoint, transform.position);
                if (!downshifted && currentRange >= Range * 0.5f)
                {
                    //abj.rigidBody.AddForce(new Vector3(0, -1, 0) * (YForce * 2000));
                    downshifted = true;
                }
              
                abj.rigidBody.drag = Force * (2f * (15 / Range));

                if (abj.rigidBody.velocity.magnitude < 0.1f)
                {
                    CompleteActionAs(true);
                }
            }
        }
    }
}
