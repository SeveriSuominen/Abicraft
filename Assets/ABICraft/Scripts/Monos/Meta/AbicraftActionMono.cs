using AbicraftCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RaycastDirection
{
    Forward,
    Backward,
    Left,
    Right,
    Up,
    Down
}

namespace AbicraftMonos.Action
{
    public abstract class AbicraftActionMono : MonoBehaviour
    {
        public bool despawnWholeGameobject = false;
        public bool dontDestroyActionMonoOnComplete = false;

        public bool  Active { get; private set; }
        public bool  CompleteAfterSeconds { get; private set; }
        public float CompleteAfterSecondsAmount { get; private set; }

        float actionMonoLifetimeElapsed;
        protected bool  completeAsAfterSeconds;

        public bool ActionIsComplete { get; private set; }
        public bool ActionWasSuccess { get; private set; }

        public Dictionary<RaycastDirection, Vector3> rayDirections = new Dictionary<RaycastDirection, Vector3>();
        public Dictionary<RaycastDirection, RaycastHit> rayHits    = new Dictionary<RaycastDirection, RaycastHit>();

        /// <summary> Call this method in AbicraftActionMono derived class to update actionMono</summary>
        protected void ActionMonoUpdate(float deltaTime)
        {
            if(Active)
            {
                if (CompleteAfterSeconds && actionMonoLifetimeElapsed >= CompleteAfterSecondsAmount)
                {
                    CompleteActionAs(completeAsAfterSeconds);
                }
                actionMonoLifetimeElapsed += deltaTime;
            }
        }

        public virtual void OnComplete(bool status){}
        public virtual void OnActive(){}
        public virtual void OnSpawn(){}
        public virtual void ResetWhenPooled(){}

        public void StartActionMono(float completeAfterSeconds, bool completeAs)
        {
            this.CompleteAfterSecondsAmount = completeAfterSeconds;
            this.completeAsAfterSeconds = completeAs;

            this.CompleteAfterSeconds = true;

            StartActionMono();
       
        }

        void AddKeyIfNotContaining<K, V>(ref Dictionary<K,V> dic, K key, V value)
        {
            if(!dic.ContainsKey(key))
                dic.Add(key, value);
        }

        public void StartActionMono()
        {
            AddKeyIfNotContaining<RaycastDirection, Vector3>(ref rayDirections, RaycastDirection.Forward, transform.forward);
            AddKeyIfNotContaining<RaycastDirection, Vector3>(ref rayDirections, RaycastDirection.Backward, transform.forward * -1);
            AddKeyIfNotContaining<RaycastDirection, Vector3>(ref rayDirections, RaycastDirection.Left, transform.right * -1);
            AddKeyIfNotContaining<RaycastDirection, Vector3>(ref rayDirections, RaycastDirection.Right, transform.right);
            AddKeyIfNotContaining<RaycastDirection, Vector3>(ref rayDirections, RaycastDirection.Up, transform.up);
            AddKeyIfNotContaining<RaycastDirection, Vector3>(ref rayDirections, RaycastDirection.Down, transform.up * -1);

            /*
            rayDirections.Add(RaycastDirection.Forward, transform.forward);
            rayDirections.Add(RaycastDirection.Backward, transform.forward * -1);
            rayDirections.Add(RaycastDirection.Left, transform.right * -1);
            rayDirections.Add(RaycastDirection.Right, transform.right);
            rayDirections.Add(RaycastDirection.Up, transform.up);
            rayDirections.Add(RaycastDirection.Down, transform.up * -1);
            */

            RaycastHit dumbHit = new RaycastHit();

            AddKeyIfNotContaining<RaycastDirection, RaycastHit>(ref rayHits, RaycastDirection.Forward, dumbHit);
            AddKeyIfNotContaining<RaycastDirection, RaycastHit>(ref rayHits, RaycastDirection.Backward, dumbHit);
            AddKeyIfNotContaining<RaycastDirection, RaycastHit>(ref rayHits, RaycastDirection.Left, dumbHit);
            AddKeyIfNotContaining<RaycastDirection, RaycastHit>(ref rayHits, RaycastDirection.Right, dumbHit);
            AddKeyIfNotContaining<RaycastDirection, RaycastHit>(ref rayHits, RaycastDirection.Up, dumbHit);
            AddKeyIfNotContaining<RaycastDirection, RaycastHit>(ref rayHits, RaycastDirection.Down, dumbHit);

            /*
            rayHits.Add(RaycastDirection.Forward, dumbHit);
            rayHits.Add(RaycastDirection.Backward, dumbHit);
            rayHits.Add(RaycastDirection.Left, dumbHit);
            rayHits.Add(RaycastDirection.Right, dumbHit);
            rayHits.Add(RaycastDirection.Up, dumbHit);
            rayHits.Add(RaycastDirection.Down, dumbHit);
            */
            Active = true;
            OnActive();
        }

        public virtual object ReturnData()
        {
            return null;
        }

        public RaycastHit GetRaycastHit(RaycastDirection dir)
        {
            Ray ray = new Ray(transform.position, rayDirections[dir]);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                rayHits[dir] = hit;
            }

            return rayHits[dir];
        }

        public void CompleteActionAs(bool success)
        {
            if (ActionIsComplete)
                return;

            ActionIsComplete = true;
            Active = false;
            ActionWasSuccess = success;

            OnComplete(success);

            StartCoroutine(End());
        }

        private IEnumerator End()
        {
            yield return new WaitForFixedUpdate();

            if (despawnWholeGameobject)
            {
                AbicraftObjectPool.Despawn(this.gameObject);
            }

            if (dontDestroyActionMonoOnComplete)
            {
                actionMonoLifetimeElapsed = 0;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}

