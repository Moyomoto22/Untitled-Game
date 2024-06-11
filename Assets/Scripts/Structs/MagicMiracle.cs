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
    // –‚–@/ŠïÕ
    [SerializeField]
    public bool isMagic;
    // Á”ïMP
    [SerializeField]
    public int MPCost;
    // ‰r¥ŽžŠÔ
    [SerializeField]
    public double castTime;

    public override bool CanUse(CharacterStatus user)
    {
        return user.mp >= MPCost && !user.isSilenced;
    }
}