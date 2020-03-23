using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftMonos.Action
{
    //[RequireComponent(typeof(Rigidbody))]
    public class Recast : AbicraftActionMono
    {
        public KeyCode keyCode;

        void Update()
        {
            if (Active)
            {
                base.ActionMonoUpdate(Time.deltaTime);

                if (Input.GetKeyDown(keyCode))
                    CompleteActionAs(true);
            }
        }
    }
}


