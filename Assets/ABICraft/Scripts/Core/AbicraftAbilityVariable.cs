using AbicraftMonos;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace AbicraftCore.Variables
{
    //LIST
    [System.Serializable]
    public class AbicraftAbilityVariableMap
    {
        readonly Dictionary<string, AbicraftAbilityVariable> VARIABLES = new Dictionary<string, AbicraftAbilityVariable>();

        public int Count { get { return VARIABLES.Count; } }

        public object this[string name]
        {
            get
            {
                if (VARIABLES.ContainsKey(name))
                {
                    return VARIABLES[name].GetVariableValue();
                }
                else
                    return null;
            }
        }

        public void Set(AbicraftAbilityExecution setterAE, string name, object value, Type type)
        {
            if (VARIABLES.ContainsKey(name))
            {
                AbicraftAbilityVariable variable = VARIABLES[name];

                variable.setterObj      = setterAE.senderObject;
                variable.setterAEGuid   = setterAE.guid;
                variable.VARIABLE_VALUE = value;
                variable.VARIABLE_TYPE  = type;
            }
            else
                VARIABLES.Add(name, new AbicraftAbilityVariable(setterAE, value, type));
        }
    }

    //VARIABLE
    [System.Serializable]
    public class AbicraftAbilityVariable
    {
        public AbicraftObject setterObj;
        public string         setterAEGuid;

        public object VARIABLE_VALUE;
        public Type   VARIABLE_TYPE;

        public AbicraftAbilityVariable(AbicraftAbilityExecution setterAE, object VARIABLE_VALUE, Type VARIABLE_TYPE)
        {
            this.setterObj    = setterAE.senderObject;
            this.setterAEGuid = setterAE.guid;

            this.VARIABLE_VALUE = VARIABLE_VALUE;
            this.VARIABLE_TYPE  = VARIABLE_TYPE;
        }

        public void SetVariableValue(object Value)
        {
            this.VARIABLE_VALUE = Value;
        }

        public object GetVariableValue()
        {
            return Convert.ChangeType(VARIABLE_VALUE, VARIABLE_TYPE);
        }
    }

    [System.Serializable]
    public class AbicraftAbilityVariableDefinition
    {
        public string VARIABLE_NAME;
        public Type   VARIABLE_TYPE;

        public AbicraftAbilityVariableDefinition(string VARIABLE_NAME, Type VARIABLE_TYPE)
        {
            this.VARIABLE_NAME = VARIABLE_NAME;
            this.VARIABLE_TYPE = VARIABLE_TYPE;
        }
    }
}
