using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Equip : Item
{
    //�@�����U����
    [SerializeField]
    public int pAttack = 0;
    //�@���@�U����
    [SerializeField]
    public int mAttack = 0;
    //�@�����h���
    [SerializeField]
    public int pDefence = 0;
    //�@���@�h���
    [SerializeField]
    public int mDefence = 0;
    //�@�ő�HP
    [SerializeField]
    public int maxHp = 0;
    //�@�ő�MP
    [SerializeField]
    public int maxMp = 0;
    //�@STR
    [SerializeField]
    public int str = 0;
    //�@VIT
    [SerializeField]
    public int vit = 0;
    //�@DEX
    [SerializeField]
    public int dex = 0;
    //�@AGI
    [SerializeField]
    public int agi = 0;
    //�@INT
    [SerializeField]
    public int inte = 0;
    //�@MND
    [SerializeField]
    public int mnd = 0;
    // �����\�N���X
    [SerializeField]
    public List<BaseClass> equipableClasses;
    

}