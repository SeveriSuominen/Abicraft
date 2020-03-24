using AbicraftMonos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowAttributeText : MonoBehaviour
{
    public AbicraftObject abj;
    Text text;

    AbicraftAttribute attr;

    void Start()
    {
        attr = AbicraftAttribute.Attribute("Mana");
        text = GetComponent<Text>();
    }

    void Update()
    {
        string txt = "";

        var list = abj.attributes.GetList();

        for (int i = 0; i < list.Length; i++)
        {
            txt += "(" + list[i].attribute.name + " : " + list[i].baseValue + "), ";
        }
        text.text = txt;
    }
}
