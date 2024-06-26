using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "Arts", menuName = "Skills/Arts")]
public class Arts : Skill
{
    //�@����
    [SerializeField]
    private Constants.WeaponCategory requiredWeaponCategory;

    public override bool CanUse(Character user)
    {
        // �E��ɓK�؂ȕ���𑕔����Ă��邩��TP���\��
        bool isEquippedValidWeapon = user.RightArm != null && user.RightArm.WeaponCategory == requiredWeaponCategory;
        bool hasEnoughTP = user.TP >= TpCost;

        return isEquippedValidWeapon && hasEnoughTP;
    }
}