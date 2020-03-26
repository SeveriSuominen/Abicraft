using AbicraftCore;
using AbicraftMonos;
using AbicraftMonos.Action;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToMouseCastController : CastController
{
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
                position = castcollider_abj.transform.position = raycastHit.point;
            }
        }
    }
}
