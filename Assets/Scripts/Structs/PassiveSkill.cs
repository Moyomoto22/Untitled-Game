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
    // �p�b�V�u�X�L������
    [SerializeField] private List<Constants.PassiveEffectType> passiveEffects;

    public override bool CanUse(Character user)
    {
        return false;
    }

    /// <summary>
    /// �X�L���̌��ʂ�K�p����
    /// </summary>
    /// <returns></returns>
    public void ApplyPassiveEffect(Ally user)
    {
        foreach (var effectType in passiveEffects)
        {
            PassiveEffect effect = PassiveEffect.Instance;
            effect.Initialize(this);
            effect.CallEffect(effectType, user, Objective);
        }
    }
}