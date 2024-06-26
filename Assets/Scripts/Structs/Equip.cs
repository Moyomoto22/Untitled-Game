using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Equip : Item
{
    // �����U����
    [SerializeField] private int pAttack = 0;
    public int PAttack
    {
        get { return pAttack; }
        set { pAttack = value; }
    }

    // ���@�U����
    [SerializeField] private int mAttack = 0;
    public int MAttack
    {
        get { return mAttack; }
        set { mAttack = value; }
    }

    // �����h���
    [SerializeField] private int pDefence = 0;
    public int PDefence
    {
        get { return pDefence; }
        set { pDefence = value; }
    }

    // ���@�h���
    [SerializeField] private int mDefence = 0;
    public int MDefence
    {
        get { return mDefence; }
        set { mDefence = value; }
    }

    // �ő�HP
    [SerializeField] private int maxHp = 0;
    public int MaxHp
    {
        get { return maxHp; }
        set { maxHp = value; }
    }

    // �ő�MP
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

    // �N���e�B�J����
    [SerializeField] private int criticalRate = 0;
    public int CriticalRate
    {
        get { return criticalRate; }
        set { criticalRate = value; }
    }

    // ���
    [SerializeField] private int evationRate;
    public int EvationRate
    {
        get { return evationRate; }
        set { evationRate = value; }
    }

    // �J�E���^�[������
    [SerializeField] private int counterRate;
    public int CounterRate
    {
        get { return counterRate; }
        set { counterRate = value; }
    }

    // �u���b�N������
    [SerializeField] private int blockRate;
    public int BlockRate
    {
        get { return counterRate; }
        set { counterRate = value; }
    }

    // �u���b�N�������_���[�W�y����
    [SerializeField] private int blockReductionRate;
    public int BlockReductionRate
    {
        get { return blockReductionRate; }
        set { blockReductionRate = value; }
    }

    // �����\�N���X
    [SerializeField] private List<Class> equipableClasses;
    public List<Class> EquipableClasses
    {
        get { return equipableClasses; }
        set { equipableClasses = value; }
    }

    public bool CanEquip(Ally user, int equipAreaIndex)
    {
        // �����\�N���X��
        if (!equipableClasses.Contains(user.CharacterClass))
        {
            return false;
        }

        // �������łȂ���
        if (EquippedAllyID > 0)
        {
            return false;
        }

        if (this is Weapon weapon)
        {
            // ���ȊO�͉E��ȊO�ɂ͑����s��
            if (weapon.WeaponCategory != Constants.WeaponCategory.Shield && equipAreaIndex != 0)
            {
                return false;
            }

            // ��
            if (weapon.WeaponCategory == Constants.WeaponCategory.Shield)
            {
                // ����ȊO�ɂ͑����s��
                if (equipAreaIndex != 1)
                {
                    return false;
                }
                // �E�肪���莝���̏ꍇ�͑����s��
                else if (user.RightArm != null)
                {
                    return !user.RightArm.IsTwoHanded;
                }
            }
        }
        return true;
    }
}
