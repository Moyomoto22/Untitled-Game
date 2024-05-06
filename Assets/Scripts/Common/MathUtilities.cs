using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtilities : MonoBehaviour
{

    /// <summary>
    /// intŒ^‚ğœZŒãlÌŒÜ“ü‚µ‚Ä•Ô‚·
    /// </summary>
    /// <param name="dividend"></param>
    /// <param name="divisor"></param>
    /// <returns></returns>
    public static int RoundDivide(int dividend, int divisor)
    {
        // 2‚ÅŠ„‚èAlÌŒÜ“ü‚·‚é
        return (int)Math.Round((double)dividend / divisor, MidpointRounding.AwayFromZero);
    }
}
