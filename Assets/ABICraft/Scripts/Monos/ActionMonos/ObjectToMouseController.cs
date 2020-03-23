using AbicraftCore;
using AbicraftMonos;
using AbicraftMonos.Action;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToMouseController : AbicraftActionMono
{
    public delegate void OnKeyActivator(AbicraftActionMono mono);

    public AbicraftObject abj;
    public Camera cam;
    
    public Vector3 position { get; private set; }

    public KeyCode keyCode;

    void Update()
    {
        if (Active)
        {
            ActionMonoUpdate(Time.deltaTime);

            RaycastHit raycastHit;

            if (Input.GetKeyDown(keyCode))
            {
                CompleteActionAs(true);
            }

            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out raycastHit))
            {
                position = abj.transform.position = raycastHit.point;
            }
        }
    }
}
