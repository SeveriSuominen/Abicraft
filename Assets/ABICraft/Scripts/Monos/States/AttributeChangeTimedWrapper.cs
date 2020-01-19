using AbicraftCore;
using AbicraftMonos;
using AbicraftMonos.Action;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeChangeTimedWrapper : AbicraftActionMono
{
    public AbicraftObject    obj;
    public AbicraftAttribute attribute;

    public float setToValue;
           float differenceToOriginal;


    AbicraftAttribute.AbicraftObjectAttribute attributeObject;

    public override void OnActive()
    {
        if(Active && obj != null)
        {
            this.attributeObject = obj.attributes[attribute];

            if (this.attributeObject != null)
            {
                completeAsAfterSeconds = true;

                differenceToOriginal = attributeObject.baseValue - setToValue;
                attributeObject.baseValue    = setToValue;
            }
        }
    }

    public override void OnComplete(bool success)
    {
        if(attributeObject != null)
        {
            attributeObject.baseValue += differenceToOriginal;
        }
    }

    void Update()
    {
        base.ActionMonoUpdate(Time.deltaTime);

        if (Active && obj == null)
            CompleteActionAs(false);
    }
}
