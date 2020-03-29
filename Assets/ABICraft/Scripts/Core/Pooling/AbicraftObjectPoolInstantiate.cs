using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbicraftObjectPoolInstantiate
{
    public AbicraftObject abjRef;
    public int  amountForScene;
    public bool includeForScene;

    public static AbicraftObjectPoolInstantiate Create(AbicraftObject abj)
    {
        return new AbicraftObjectPoolInstantiate()
        {
            abjRef = abj,
            amountForScene = abj.InstantiateToPoolAmount,
            includeForScene = true
        };
    }
}
