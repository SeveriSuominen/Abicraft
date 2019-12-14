using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbicraftActionMono : MonoBehaviour
{
    public bool destroyWholeGameobject = false;

    public bool Active { get; private set; }

    public bool ActionIsComplete { get; private set; }
    public bool ActionWasSuccess { get; private set; }

    public enum RaycastDirection
    {
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down
    }

    public readonly Dictionary<RaycastDirection, Vector3> rayDirections = new Dictionary<RaycastDirection, Vector3>();


    public void StartActionMono()
    {
        rayDirections.Add(RaycastDirection.Forward,     transform.forward);
        rayDirections.Add(RaycastDirection.Backward,    transform.forward * -1);
        rayDirections.Add(RaycastDirection.Left,        transform.right   * -1);
        rayDirections.Add(RaycastDirection.Right,       transform.right);
        rayDirections.Add(RaycastDirection.Up,          transform.up);
        rayDirections.Add(RaycastDirection.Down,        transform.up * -1);

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
          
        }

        return hit;
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
            Destroy(this.gameObject);
        else
            Destroy(this);
    }
}
