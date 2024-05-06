using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Weapon", menuName = "CreateWeapon")]
public class Weapon : Equip
{
    // 種別
    [SerializeField]
    public Constants.WeaponCategory weaponCategory;
    // 属性
    [SerializeField]
    public List<Constants.Attribute> attributes;
    // 依存ステータス 
    // 0: STR 1: DEX 2: INT 3:MND
    [SerializeField]
    public int dependentStatus = 0;
    // 両手持ち
    [SerializeField]
    public bool isTwoHanded = false;
    // 攻撃回数
    [SerializeField]
    public int times = 1;

}