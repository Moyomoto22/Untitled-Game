using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "MagicMiracle", menuName = "Skills/CreateMagicMiracle")]
public class MagicMiracle : Skill
{
    // @/ïÕ
    [SerializeField]
    public bool isMagic;
    // ÁïMP
    [SerializeField]
    public int MPCost;
    // r¥Ô
    [SerializeField]
    public double castTime;
    // gpÂ\NX
    [SerializeField]
    public List<BaseClass> usableClasses;

    public override bool CanUse(CharacterStatus user)
    {
        return user.mp >= MPCost && !user.isSilenced;
    }
}