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
    // パッシブスキル効果
    public List<Constants.PassiveEffectType> passiveEffects;

    public override bool CanUse(CharacterStatus user)
    {
        return false;
    }

    /// <summary>
    /// スキルの効果を適用する
    /// </summary>
    /// <returns></returns>
    public void applyPassiveEffect(AllyStatus user)
    {
        foreach (var effectType in passiveEffects)
        {
            PassiveEffect effect = PassiveEffect.Instance;
            effect.Initialize(this);
            effect.CallEffect(effectType, user, Objective);
        }
    }
}