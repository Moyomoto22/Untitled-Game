using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    /// <summary>
    /// ダメージ計算の乱数幅を返す
    /// </summary>
    /// <returns></returns>
    public static int GenerateRamdomValue(int value)
    {
        int ratio = 16;
        int fraction = 1;
        int random;

        System.Random r = new System.Random();

        random = (int)(value / ratio + fraction);

        if (random < 0)
        {
            random = 1;
        }

        random = r.Next(-random, random);

        return random;

    }

    /// <summary>
    /// HP回復(ステータス依存・倍率)
    /// </summary>
    /// <param name="targetStatus"></param>
    /// <param name="healAmount"></param>
    /// <returns></returns>
    public static int HealHPByStatus(AllyStatus targetStatus, int baseAmount, int status, double ratio)
    {
        int healAmount = (int)(baseAmount + status * ratio);
        int random = GenerateRamdomValue(healAmount);

        healAmount = healAmount + random;

        if(healAmount < 0)
        {
            healAmount = 0;
        }

        int currentHp = Math.Min(targetStatus.hp + healAmount, targetStatus.maxHp2);

        if (targetStatus.hp < targetStatus.maxHp)
        {
            targetStatus.hp = targetStatus.hp + healAmount;

            if (targetStatus.hp > targetStatus.maxHp)
            {
                targetStatus.hp = targetStatus.maxHp;
            }
            return 0;
        }
        else return 1;
    }
}
