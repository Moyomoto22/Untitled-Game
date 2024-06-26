using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Equip : Item
{
    // 物理攻撃力
    [SerializeField] private int pAttack = 0;
    public int PAttack
    {
        get { return pAttack; }
        set { pAttack = value; }
    }

    // 魔法攻撃力
    [SerializeField] private int mAttack = 0;
    public int MAttack
    {
        get { return mAttack; }
        set { mAttack = value; }
    }

    // 物理防御力
    [SerializeField] private int pDefence = 0;
    public int PDefence
    {
        get { return pDefence; }
        set { pDefence = value; }
    }

    // 魔法防御力
    [SerializeField] private int mDefence = 0;
    public int MDefence
    {
        get { return mDefence; }
        set { mDefence = value; }
    }

    // 最大HP
    [SerializeField] private int maxHp = 0;
    public int MaxHp
    {
        get { return maxHp; }
        set { maxHp = value; }
    }

    // 最大MP
    [SerializeField] private int maxMp = 0;
    public int MaxMp
    {
        get { return maxMp; }
        set { maxMp = value; }
    }

    // STR
    [SerializeField] private int str = 0;
    public int Str
    {
        get { return str; }
        set { str = value; }
    }

    // VIT
    [SerializeField] private int vit = 0;
    public int Vit
    {
        get { return vit; }
        set { vit = value; }
    }

    // DEX
    [SerializeField] private int dex = 0;
    public int Dex
    {
        get { return dex; }
        set { dex = value; }
    }

    // AGI
    [SerializeField] private int agi = 0;
    public int Agi
    {
        get { return agi; }
        set { agi = value; }
    }

    // INT
    [SerializeField] private int inte = 0;
    public int Int
    {
        get { return inte; }
        set { inte = value; }
    }

    // MND
    [SerializeField] private int mnd = 0;
    public int Mnd
    {
        get { return mnd; }
        set { mnd = value; }
    }

    // クリティカル率
    [SerializeField] private int criticalRate = 0;
    public int CriticalRate
    {
        get { return criticalRate; }
        set { criticalRate = value; }
    }

    // 回避率
    [SerializeField] private int evationRate;
    public int EvationRate
    {
        get { return evationRate; }
        set { evationRate = value; }
    }

    // カウンター発生率
    [SerializeField] private int counterRate;
    public int CounterRate
    {
        get { return counterRate; }
        set { counterRate = value; }
    }

    // ブロック発生率
    [SerializeField] private int blockRate;
    public int BlockRate
    {
        get { return counterRate; }
        set { counterRate = value; }
    }

    // ブロック発生時ダメージ軽減率
    [SerializeField] private int blockReductionRate;
    public int BlockReductionRate
    {
        get { return blockReductionRate; }
        set { blockReductionRate = value; }
    }

    // 装備可能クラス
    [SerializeField] private List<Class> equipableClasses;
    public List<Class> EquipableClasses
    {
        get { return equipableClasses; }
        set { equipableClasses = value; }
    }

    public bool CanEquip(Ally user, int equipAreaIndex)
    {
        // 装備可能クラスか
        if (!equipableClasses.Contains(user.CharacterClass))
        {
            return false;
        }

        // 装備中でないか
        if (EquippedAllyID > 0)
        {
            return false;
        }

        if (this is Weapon weapon)
        {
            // 盾以外は右手以外には装備不可
            if (weapon.WeaponCategory != Constants.WeaponCategory.Shield && equipAreaIndex != 0)
            {
                return false;
            }

            // 盾
            if (weapon.WeaponCategory == Constants.WeaponCategory.Shield)
            {
                // 左手以外には装備不可
                if (equipAreaIndex != 1)
                {
                    return false;
                }
                // 右手が両手持ちの場合は装備不可
                else if (user.RightArm != null)
                {
                    return !user.RightArm.IsTwoHanded;
                }
            }
        }
        return true;
    }
}
