using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "PassiveSkill", menuName = "Skills/PassiveSkill")]
public class PassiveSkill : Skill
{
    // ExƒXƒLƒ‹
    [SerializeField]
    public bool isExSkill;

    public override bool CanUse(CharacterStatus user)
    {
        return false;
    }
}