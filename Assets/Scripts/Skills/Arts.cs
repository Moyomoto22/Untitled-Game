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
    // Á”ïTP
    [SerializeField]
    public int TPCost;
    //@•Ší
    [SerializeField]
    public Constants.WeaponCategory weapon;

    public override bool CanUse(CharacterStatus user)
    {
        return user.tp >= TPCost;
    }
}