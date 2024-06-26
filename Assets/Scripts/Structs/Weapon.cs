using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Weapon", menuName = "CreateWeapon")]
public class Weapon : Equip
{
    public enum DependStatus
    {
        STR = 1,
        DEX = 2,
        INT = 3,
        MND = 4
    }

    // 種別
    [SerializeField] private Constants.WeaponCategory weaponCategory;
    public Constants.WeaponCategory WeaponCategory
    {
        get { return weaponCategory; }
        set { weaponCategory = value; }
    }

    // 属性
    [SerializeField] private List<AttributeType> attributes;
    public List<AttributeType> Attributes
    {
        get { return attributes; }
        set { attributes = value; }
    }

    // 依存ステータス 
    // 0: STR 1: DEX 2: INT 3:MND
    [SerializeField] private DependStatus dependentStatus = DependStatus.STR;
    public DependStatus DependentStatus
    {
        get { return dependentStatus; }
        set { dependentStatus = value; }
    }

    // 両手持ち
    [SerializeField] private bool isTwoHanded = false;
    public bool IsTwoHanded
    {
        get { return isTwoHanded; }
        set { isTwoHanded = value; }
    }

    // 攻撃回数
    [SerializeField] private int times = 1;
    public int Times
    {
        get { return times; }
        set { times = value; }
    }

    // 攻撃時発動効果
    [SerializeField] private List<Constants.ActiveEffectType> attackEffects;
    public List<Constants.ActiveEffectType> AttackEffects
    {
        get { return attackEffects; }
        set { attackEffects = value; }
    }
}
