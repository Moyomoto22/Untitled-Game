using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Equip : Item
{
    //　物理攻撃力
    [SerializeField]
    public int pAttack = 0;
    //　魔法攻撃力
    [SerializeField]
    public int mAttack = 0;
    //　物理防御力
    [SerializeField]
    public int pDefence = 0;
    //　魔法防御力
    [SerializeField]
    public int mDefence = 0;
    //　最大HP
    [SerializeField]
    public int maxHp = 0;
    //　最大MP
    [SerializeField]
    public int maxMp = 0;
    //　STR
    [SerializeField]
    public int str = 0;
    //　VIT
    [SerializeField]
    public int vit = 0;
    //　DEX
    [SerializeField]
    public int dex = 0;
    //　AGI
    [SerializeField]
    public int agi = 0;
    //　INT
    [SerializeField]
    public int inte = 0;
    //　MND
    [SerializeField]
    public int mnd = 0;
    // 装備可能クラス
    [SerializeField]
    public List<BaseClass> equipableClasses;
    

}