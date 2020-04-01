using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AbicraftMonos.Action
{
    //[RequireComponent(typeof(Rigidbody))]
    public class Push : AbicraftActionMono
    {
        AbicraftObject abj;

        public bool forcePush;

        bool wasKinematic, wasGravity, agentWasUpdatingPosition;

        public AnimationCurve curve;
        public Vector3 Direction;
        public float Range = 0.7f, MaxRange = 3.0f;
        public float Force = 1.0f;
        public float YForce = 0;
        private float currentRange;

        NavMeshAgent agent;

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
                agent = abj.GetComponent<NavMeshAgent>();

                if (agent)
                {
                    agentWasUpdatingPosition = agent.updatePosition;
                    agent.updatePosition = false;
                }

                if (abj.rigidBody.isKinematic)
                {
                    wasKinematic = true;
                    wasGravity = abj.rigidBody.useGravity;

                    abj.rigidBody.useGravity  = true;
                    abj.rigidBody.isKinematic = false;
                }

                abj.rigidBody.AddForce(Direction * ((Force * 2000) * abj.rigidBody.mass));
                StartCoroutine(DoubleCheckCompletion());
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

                var force = Direction * (Force * curve.Evaluate(currentRange / Range));

                if (Range != 0)
                    transform.position += force;
                else
                    Debug.LogWarning("Abicraft: Push node Range is 0");

                var distance = Vector3.Distance(startPoint, transform.position);

                if (distance > Range || Vector3.Distance(startPoint, transform.position) > MaxRange)
                {
                    CompleteActionAs(true);
                }
            }

            if(Active && forcePush)
            {
                currentRange = Vector3.Distance(startPoint, transform.position);
                if (!downshifted && currentRange >= Range * 0.5f )
                {
                    //abj.rigidBody.AddForce(new Vector3(0, -1, 0) * (YForce * 2000));
                    downshifted = true;
                }
              
                abj.rigidBody.drag = Force * (2f * (15 / Range));

                /*if (abj.rigidBody.velocity.magnitude < 0.3f)
                {
                    StartCoroutine(DoubleCheckCompletion());
                }*/
            }
        }

        IEnumerator DoubleCheckCompletion()
        {
            yield return new WaitForSeconds(0.2f);
           
            while (true)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(abj.transform.position, out hit, 30f, NavMesh.AllAreas)){}

                var kinematicCondition = wasKinematic  && Vector3.Distance(abj.transform.position, hit.position) < 0.2f;
                var nokinematicCondition = !wasKinematic && abj.rigidBody.velocity.magnitude < 0.3f;

                if (kinematicCondition || nokinematicCondition)
                {
                    if (agent)
                    {
                        agent.updatePosition = agentWasUpdatingPosition;
                    }

                    abj.rigidBody.isKinematic = wasKinematic;

                    if (wasKinematic)
                        abj.rigidBody.useGravity = wasGravity;

                    abj.transform.position = hit.position;
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
            CompleteActionAs(true);
        }
    }
}
