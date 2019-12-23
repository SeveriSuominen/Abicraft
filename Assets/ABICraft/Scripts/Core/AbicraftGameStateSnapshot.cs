using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftCore
{
    [System.Serializable]
    public class AbicraftGameStateSnapshot
    {
        public static AbicraftGameStateSnapshot TakeSnapshot { get { return CreateSnapshot(); } }

        static AbicraftInputReferences inputReferences;

        public Camera camera { get; private set; }
        public AbicraftObject player { get; private set; }
        public Vector3 mousePosition2D { get; private set; }
        public Vector3 mousePosition3D { get; private set; }

        public static void InjectInputDataReferences(AbicraftInputReferences inputReferences)
        {
            AbicraftGameStateSnapshot.inputReferences = inputReferences;
        }

        static AbicraftGameStateSnapshot CreateSnapshot()
        {
            AbicraftGameStateSnapshot snapshot = new AbicraftGameStateSnapshot();

            snapshot.camera = Camera.main;
            snapshot.player = inputReferences.Player;
            snapshot.mousePosition2D = Input.mousePosition;

            //Using NULL_FORMAT to detect not initializes non nullable values, with Abicraft.
            snapshot.mousePosition3D = GlobalHelpers.NULL_FORMAT;

            RaycastHit raycastHit;

            if (Physics.Raycast(snapshot.camera.ScreenPointToRay(Input.mousePosition), out raycastHit))
            {
                //After correct value initilization, no longer NULL_FORMAT value :)
                snapshot.mousePosition3D = raycastHit.point;
            }

            return snapshot;
        }
    }

}

