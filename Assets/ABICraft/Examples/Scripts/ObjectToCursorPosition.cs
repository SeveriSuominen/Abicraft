using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectToCursorPosition : MonoBehaviour
{
    public Transform  pivot, toObj;
    public GameObject cursor;

    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam   = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Vector3.Distance (
           new Vector3(toObj.transform.position.x, 0, toObj.transform.position.z),
           new Vector3(pivot.transform.position.x, 0, pivot.transform.position.z))
           >= 0.33f
        )
        {
            pivot.transform.position = Vector3.Lerp(pivot.transform.position, toObj.transform.position, 0.05f);
        }
    }
}
