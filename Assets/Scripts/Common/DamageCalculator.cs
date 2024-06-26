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
    /// 属性によるダメージ計算
    /// </summary>
    /// <param name="baseDamage"></param>
    /// <param name="attackAttributes"></param>
    /// <param name="defenseResistances"></param>
    /// <returns></returns>
    public static int CalculateDamageWithAttributes(int baseDamage, List<Constants.Attribute> attackAttributes, Character objective)
    {
        double attributeModifier = 1.0f;

        foreach (var attribute in attackAttributes)
        {
            double mod = 0;
            
            switch (attribute)
            {
                case Constants.Attribute.Physical:
                    mod = objective.ResistPhysical;
                    break;
                case Constants.Attribute.Slash:
                    mod = objective.ResistSlash;
                    break;
                case Constants.Attribute.Thrust:
                    mod = objective.ResistThrast;
                    break;
                case Constants.Attribute.Blow:
                    mod = objective.ResistBlow;
                    break;
                case Constants.Attribute.Magic:
                    mod = objective.ResistMagic;
                    break;
                case Constants.Attribute.Fire:
                    mod = objective.ResistFire;
                    break;
                case Constants.Attribute.Ice:
                    mod = objective.ResistIce;
                    break;
                case Constants.Attribute.Thunder:
                    mod = objective.ResistThunder;
                    break;
                case Constants.Attribute.Wind:
                    mod = objective.ResistWind;
                    break;
            }
            attributeModifier -= mod;
        }

        int finalDamage = (int)(baseDamage * attributeModifier);
        return finalDamage;
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
    public static int ReducingByGuard(Character objective, int damage)
    {
        var finalDamage = damage;
        if (objective.IsGuarded)
        {
            finalDamage = (int)(damage * Constants.reductionRateByGuard);
        }
        return finalDamage;
    }

    /// <summary>
    /// 回避判定
    /// </summary>
    /// <returns></returns>
    public static bool Dodge(int evationRate)
    {
        int result = random.Next(100);

        return evationRate > result;
    }
}
