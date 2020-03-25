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

        abj.SetAttributeValue(abj, attr, 50000);
        text = GetComponent<Text>();
    }

    void Update()
    {
        string txt = "";

        if(Input.GetKeyDown(KeyCode.X))
            Debug.Log(abj.Max(AbicraftAttribute.Attribute("Mana")));

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log(abj.CastAttributeOn(200, abj, AbicraftAttribute.Attribute("Damage")));
        }
         
        
        var list = abj.attributes.GetList();

        for (int i = 0; i < list.Length; i++)
        {
            txt += "(" + list[i].attribute.name + " : " + list[i].baseValue + "), ";
        }
        text.text = txt;
    }
}
