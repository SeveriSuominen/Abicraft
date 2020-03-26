using AbicraftMonos.Action;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalFade : AbicraftActionMono
{
    public float FromAlpha, ToAlpha, FadeOverSeconds;
    float elapsed;

    MeshRenderer rnd;

    public override void OnSpawn()
    {
        rnd = GetComponent<MeshRenderer>();

        //despawnWholeGameobject = true;
        dontDestroyActionMonoOnComplete = true;

        StartActionMono(FadeOverSeconds, true);
    }

    public override void ResetWhenPooled()
    {
        Color col = rnd.material.GetColor("_Color");
        col.a = 1;

        elapsed = 0;
        rnd.material.SetColor("_Color", col);
    }

    void FixedUpdate()
    {
        if (Active)
        {
            base.ActionMonoUpdate(Time.deltaTime);

            elapsed += Time.deltaTime;
            Color col = rnd.material.GetColor("_Color");
            col.a = Mathf.Lerp(ToAlpha, FromAlpha, 1 - ((elapsed + 0.0000001f) / FadeOverSeconds));

            rnd.material.SetColor("_Color", col);
        }
    }
}
