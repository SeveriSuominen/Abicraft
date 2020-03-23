using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalFade : MonoBehaviour
{
    public float FadeOverSeconds;
    float elapsed;

    MeshRenderer rnd;
    bool active;

    void Awake()
    {
        active = true;
        rnd = GetComponent<MeshRenderer>();
    }

    void FixedUpdate()
    {
        if (active)
        {
            elapsed += Time.deltaTime;
            Debug.Log(((elapsed + 0.0000001f) / FadeOverSeconds));
            Color col = rnd.material.GetColor("_Color");
            col.a = Mathf.Lerp(0, 1, 1 - ((elapsed + 0.0000001f) / FadeOverSeconds));

            rnd.material.SetColor("_Color", col);

            if (elapsed > FadeOverSeconds)
                active = false;
        }
  
    }
}
