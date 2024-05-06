using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtilities : MonoBehaviour
{

    /// <summary>
    /// int型を除算後四捨五入して返す
    /// </summary>
    /// <param name="dividend"></param>
    /// <param name="divisor"></param>
    /// <returns></returns>
    public static int RoundDivide(int dividend, int divisor)
    {
        // 2で割り、四捨五入する
        return (int)Math.Round((double)dividend / divisor, MidpointRounding.AwayFromZero);
    }
}
