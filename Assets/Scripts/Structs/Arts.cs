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
    //　武器
    [SerializeField]
    private Constants.WeaponCategory requiredWeaponCategory;

    public override bool CanUse(Character user)
    {
        // 右手に適切な武器を装備しているかつTPが十分
        bool isEquippedValidWeapon = user.RightArm != null && user.RightArm.WeaponCategory == requiredWeaponCategory;
        bool hasEnoughTP = user.TP >= TpCost;

        return isEquippedValidWeapon && hasEnoughTP;
    }
}