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
    // ����TP
    [SerializeField]
    public int TPCost;
    //�@����
    [SerializeField]
    public Constants.WeaponCategory weapon;

    public override bool CanUse(CharacterStatus user)
    {
        return user.tp >= TPCost;
    }
}