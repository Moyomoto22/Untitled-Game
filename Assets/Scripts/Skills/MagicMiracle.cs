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
    // 魔法/奇跡
    [SerializeField]
    public bool isMagic;
    // 消費MP
    [SerializeField]
    public int MPCost;
    // 詠唱時間
    [SerializeField]
    public double castTime;
    // 使用可能クラス
    [SerializeField]
    public List<BaseClass> usableClasses;

    public override bool CanUse(CharacterStatus user)
    {
        return user.mp >= MPCost && !user.isSilenced;
    }
}