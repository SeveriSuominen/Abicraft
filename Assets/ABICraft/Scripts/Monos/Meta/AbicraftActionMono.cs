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
        public bool destroyWholeGameobject = false;

        public bool Active { get; private set; }

        public bool ActionIsComplete { get; private set; }
        public bool ActionWasSuccess { get; private set; }

        public readonly Dictionary<RaycastDirection, Vector3> rayDirections = new Dictionary<RaycastDirection, Vector3>();
        public readonly Dictionary<RaycastDirection, RaycastHit> rayHits = new Dictionary<RaycastDirection, RaycastHit>();

        public void StartActionMono()
        {
            rayDirections.Add(RaycastDirection.Forward, transform.forward);
            rayDirections.Add(RaycastDirection.Backward, transform.forward * -1);
            rayDirections.Add(RaycastDirection.Left, transform.right * -1);
            rayDirections.Add(RaycastDirection.Right, transform.right);
            rayDirections.Add(RaycastDirection.Up, transform.up);
            rayDirections.Add(RaycastDirection.Down, transform.up * -1);

            RaycastHit dumbHit = new RaycastHit();

            rayHits.Add(RaycastDirection.Forward, dumbHit);
            rayHits.Add(RaycastDirection.Backward, dumbHit);
            rayHits.Add(RaycastDirection.Left, dumbHit);
            rayHits.Add(RaycastDirection.Right, dumbHit);
            rayHits.Add(RaycastDirection.Up, dumbHit);
            rayHits.Add(RaycastDirection.Down, dumbHit);

            Active = true;
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

        protected void CompleteActionAs(bool success)
        {
            if (ActionIsComplete)
                return;

            ActionIsComplete = true;
            ActionWasSuccess = success;

            StartCoroutine(End());
        }

        private IEnumerator End()
        {
            yield return new WaitForFixedUpdate();

            if (destroyWholeGameobject)
            {
                AbicraftObjectPool.Despawn(this.gameObject);
            }

            Destroy(this);
        }
    }
}

