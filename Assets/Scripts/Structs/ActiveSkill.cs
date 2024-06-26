using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "ActiveSkill", menuName = "Skills/ActiveSkill")]
public class ActiveSkill : Skill
{
    public override bool CanUse(Character user)
    {
        return RemainingTurn <= 0;
    }
}