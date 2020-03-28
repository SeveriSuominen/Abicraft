using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimLightsForSeconds : MonoBehaviour
{
    public float dimmingInDuration, dimmingOutDuration, dimForSeconds;

    float totalDur, elapsed;

    public Color dimTo;

    Color org;

    Light dirLight;

    void Start()
    {
        totalDur = dimmingInDuration + dimmingOutDuration + dimForSeconds;
        dirLight = GameObject.Find("Directional Light").GetComponent<Light>();

        org = dirLight.color;
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed < dimmingInDuration)
        {
            dirLight.color = Color.Lerp(org, dimTo, elapsed / dimmingInDuration);
        }

        else if (elapsed < dimmingInDuration + dimForSeconds)
        {
            dirLight.color = dimTo;
        }

        else if (elapsed >= dimmingInDuration + dimForSeconds)
        {
            dirLight.color = Color.Lerp(dimTo, org, elapsed / totalDur);
        }

        if (elapsed > totalDur)
            Destroy(this);
    }
}
