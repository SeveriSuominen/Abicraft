using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


public class Abicraft : MonoBehaviour
{
    public AbicraftObject   Player;
    public AbicraftDataFile input;

    private void Start()
    {
        AbiCraftStateSnapshot.InjectInputDataReferences(
            new AbicraftInputReferences
            {
                Player = this.Player
            }
        );
    }
}


