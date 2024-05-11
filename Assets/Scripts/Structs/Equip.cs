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
    public List<Class> equipableClasses;

    public bool CanEquip(AllyStatus user, int equipAreaIndex)
    {
        // �����\�N���X��
        if (!equipableClasses.Contains(user.Class))
        {
            return false;
        }

        // �������łȂ���
        if (equippedAllyID > 0)
        {
            return false;
        }

        if (this is Weapon)
        {
            Weapon weapon = this as Weapon;

            // ���ȊO�͉E��ȊO�ɂ͑����s��
            if (weapon.weaponCategory != Constants.WeaponCategory.Shield && equipAreaIndex != 0)
            {
                return false;
            }

            // ��
            if (weapon.weaponCategory == Constants.WeaponCategory.Shield)
            {
                // ����ȊO�ɂ͑����s��
                if (equipAreaIndex != 1)
                {
                    return false;
                }
                // �E�肪���莝���̏ꍇ�͑����s��
                else if (user.rightArm != null)
                {
                    return !user.rightArm.isTwoHanded;
                }
            }

        }
        return true;
    }


}