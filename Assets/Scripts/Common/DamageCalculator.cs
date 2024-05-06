using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator
{
    private static readonly System.Random random = new System.Random();

    //// <summary>
    /// ��b�_���[�W���v�Z����
    /// </summary>
    /// <returns></returns>
    public static int CalcurateBaseDamage(int offence, int deffence)
    {
        // ��b�_���[�W = �U�����p�����[�^ / 2 - �h�䑤�p�����[�^ / 3
        int baseDamage = offence / 2 - deffence / 3;
        return baseDamage;
    }

    /// <summary>
    /// ��b�_���[�W�ɗ����l�����Z�E���Z����
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
    /// �N���e�B�J������
    /// </summary>
    /// <param name="criticalRate"></param>
    /// <returns></returns>
    public static bool JudgeCritical(int criticalRate)
    {       
        int result = random.Next(100);

        return criticalRate > result;

    }

    /// <summary>
    /// �h��ɂ��_���[�W�y��
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
