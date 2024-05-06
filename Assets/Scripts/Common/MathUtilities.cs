using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtilities : MonoBehaviour
{

    /// <summary>
    /// int�^�����Z��l�̌ܓ����ĕԂ�
    /// </summary>
    /// <param name="dividend"></param>
    /// <param name="divisor"></param>
    /// <returns></returns>
    public static int RoundDivide(int dividend, int divisor)
    {
        // 2�Ŋ���A�l�̌ܓ�����
        return (int)Math.Round((double)dividend / divisor, MidpointRounding.AwayFromZero);
    }
}
