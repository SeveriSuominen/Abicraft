using AbicraftCore;
using AbicraftMonos;
using AbicraftMonos.Action;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastController : AbicraftActionMono
{
    public delegate void OnKeyActivator(AbicraftActionMono mono);

    public AbicraftObject  senderObject;

    public AbicraftObject  castcollider_abj;
    public Camera          cam;
    public Vector3         position;
    public KeyCode         keyCode;
}
