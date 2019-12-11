using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//AbiCRAFT - Globalhelpers
public static class GlobalHelpers
{
    //Using NULL_FORMAT to detect not initializes non nullable values, with Abicraft.
    public static readonly Vector3 NULL_FORMAT = new Vector3(int.MinValue, int.MinValue, int.MinValue);

    public static bool IsNull(this float val)
    {
        return val == NULL_FORMAT.x;
    }

    public static bool IsNull(this Vector2 val)
    {
        return val == new Vector2(NULL_FORMAT.x, NULL_FORMAT.y);
    }

    public static bool IsNull(this Vector3 val)
    {
        return val ==  NULL_FORMAT;
    }
        
}
