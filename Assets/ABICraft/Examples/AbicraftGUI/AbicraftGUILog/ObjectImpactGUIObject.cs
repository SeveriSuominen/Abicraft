using AbicraftMonos;
using AbicraftMonos.Action;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ObjectImpactGUIObject : AbicraftActionMono
{
    public Text     textfield;
    public RawImage iconfield;

    

    public float FadeMinAlpha, FadeMaxAlpha;
    public float secondsUntilFullFaded;

    [HideInInspector]
    public AbicraftObject targetAbj;

    [HideInInspector]
    public Texture2D icon;

    [HideInInspector]
    public string impactString;

    [HideInInspector]
    public Color color;

    float elapsed;
    Vector3 orgPos;
    Vector2 moveUnits;

    public override void OnActive()
    {
        base.despawnWholeGameobject = true;
        base.dontDestroyActionMonoOnComplete = true;

        textfield.color = color;

        moveUnits = new Vector2(Random.Range(-150, 300), Random.Range(100, 300));
        orgPos = transform.position;
    }

    public override void ResetWhenPooled()
    {
        icon               = null;
        impactString       = null;
        textfield.text     = null;
        iconfield.texture  = null;

        elapsed = 0;

        Color colIcon = iconfield.color;
        colIcon.a = 1;
        iconfield.color = colIcon;

        Color colText = textfield.color;
        colText.a = 1;
        textfield.color = colText;
    }

    void Update()
    {
        if (Active)
        {
            if (!targetAbj)
                CompleteActionAs(false);

            elapsed += Time.deltaTime;

            base.ActionMonoUpdate(Time.deltaTime);

            Color colIcon = iconfield.color;
            Color colText = textfield.color;

            if (impactString != null)
            {
                textfield.text = impactString;
            }

            if (icon)
            {
                iconfield.texture = icon;
            }

            var fading = Mathf.Clamp(((elapsed) / secondsUntilFullFaded), 0, 1);
            var pos = Camera.main.WorldToScreenPoint(targetAbj.transform.position);

            transform.position = Vector2.Lerp(pos, pos + new Vector3(moveUnits.x, moveUnits.y), fading);

            colIcon.a = Mathf.Lerp(FadeMaxAlpha, FadeMinAlpha, fading);
            colText.a = Mathf.Lerp(FadeMaxAlpha, FadeMinAlpha, fading);

            iconfield.color = colIcon;
            textfield.color = colText;
        }
    }
}
