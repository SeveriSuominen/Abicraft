using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticPositionRTCastController : CastController
{
    void Update()
    {
        if (Active)
        {
            ActionMonoUpdate(Time.deltaTime);

            Vector3 mousePosition = Vector3.zero;

            RaycastHit raycastHit;

            if (Input.GetKeyDown(keyCode))
            {
                CompleteActionAs(true);
            }

            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out raycastHit))
            {
                mousePosition = raycastHit.point;
            }

            this.position = this.transform.position = senderObject.transform.position;

            var lookPos = (mousePosition - this.transform.position).normalized;
            lookPos.y = 90;

            this.transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }
}
