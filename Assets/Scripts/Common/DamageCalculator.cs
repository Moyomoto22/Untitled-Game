using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator
{
    private static readonly System.Random random = new System.Random();

    //// <summary>
    /// 基礎ダメージを計算する
    /// </summary>
    /// <returns></returns>
    public static int CalcurateBaseDamage(int offence, int deffence)
    {
        // 基礎ダメージ = 攻撃側パラメータ / 2 - 防御側パラメータ / 3
        int baseDamage = offence / 2 - deffence / 3;
        return baseDamage;
    }

    /// <summary>
    /// 基礎ダメージに乱数値を加算・減算する
    /// </summary>
    /// <returns></returns>
    public static int AddRandomValueToBaseDamage(int baseDamage)
    {
        int baseRandomValue = baseDamage / 16 + 1;
        int randomModifier = random.Next(-baseRandomValue, baseRandomValue + 1);
        int damage = baseDamage + randomModifier;

        return damage;
    }

    /// <summary>
    /// クリティカル判定
    /// </summary>
    /// <param name="criticalRate"></param>
    /// <returns></returns>
    public static bool JudgeCritical(int criticalRate)
    {       
        int result = random.Next(100);

        return criticalRate > result;

    }

    /// <summary>
    /// 防御によるダメージ軽減
    /// </summary>
    /// <returns></returns>
    public static int ReducingByGuard(CharacterStatus objective, int damage)
    {
        var finalDamage = damage;
        if (objective.isGuarded)
        {
            finalDamage = (int)(damage * Constants.reductionRateByGuard);
        }
        return finalDamage;
    }
}
