using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbicraftMonos.Action
{
    public class Shake : AbicraftActionMono
    {
        // Transform of the camera to shake. Grabs the gameObject's transform
        // if null.
        public GameObject target;

        // How long the object should shake for.
        public float shakeDuration = 10f;

        // Amplitude of the shake. A larger value shakes the camera harder.
        public float shakeAmount = 0.7f;
        public float decreaseFactor = 1.0f;

        public Vector3 originalPos;

        void Start()
        {
            destroyWholeGameobject = false;
            originalPos = target.transform.localPosition;
        }

        void FixedUpdate()
        {
            if (shakeDuration > 0)
            {
                target.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakeDuration = 0f;
                target.transform.localPosition = originalPos;

                CompleteActionAs(true);
            }

        }
    }
}
