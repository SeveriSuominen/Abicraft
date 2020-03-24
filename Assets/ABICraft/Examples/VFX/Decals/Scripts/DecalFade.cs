using AbicraftMonos.Action;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalFade : AbicraftActionMono
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

    public override void ResetWhenPooled()
    {
        active = true;
        Color col = rnd.material.GetColor("_Color");
        col.a = 1;

        elapsed = 0;

        rnd.material.SetColor("_Color", col);
    }

    void FixedUpdate()
    {
        if (active)
        {
            elapsed += Time.deltaTime;
            Color col = rnd.material.GetColor("_Color");
            col.a = Mathf.Lerp(0, 1, 1 - ((elapsed + 0.0000001f) / FadeOverSeconds));

            rnd.material.SetColor("_Color", col);

            if (elapsed > FadeOverSeconds)
            {
                active = false;
            }
        }
    }
}
