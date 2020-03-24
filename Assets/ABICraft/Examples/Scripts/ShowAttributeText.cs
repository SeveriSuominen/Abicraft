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

        abj.SetAttributeValue(abj, attr, 1000);
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = abj.GetAttributeAmount(abj, attr).ToString();
    }
}
