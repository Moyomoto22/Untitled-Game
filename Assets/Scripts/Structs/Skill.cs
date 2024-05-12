using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �X�L�����N���X
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "Skill", menuName = "CreateSkill")]
public abstract class Skill : ScriptableObject
{
    // ID
    [SerializeField]
    public string ID;
    // ���
    [SerializeField]
    public Constants.SkillCategory skillCategory;
    //�@�X�L����
    [SerializeField]
    public string skillName;
    //�@�A�C�R��
    [SerializeField]
    public Sprite icon;
    //�@�g�p��
    [SerializeField]
    public bool usable;
    //�@����
    [SerializeField, Multiline(6)]
    public string description;
    // ����SP
    [SerializeField]
    public int spCost;
    //�@����
    public List<Attibute> attributes;
    // �K�����@s
    public string learn;
    // �A�N�e�B�u����
    public List<Constants.ActiveEffectType> effects;
    // ��b�l
    public int baseValue;
    // �␳�X�e�[�^�X
    private int status;
    // �␳�X�e�[�^�X�W��
    public double statusRatio;
    // �_���[�W�{��
    public double damageRatio;
    // ��
    public int times;
    // �Ώ� 0:�Ȃ� 1:���� 2:���� 3:�G  
    public int target;
    // �Ώۂ��S�̂�
    public bool isTargetAll;
    //�@��������
    public bool isEquipped;
    // �g�p�\�N���X
    public List<Class> usableClasses;
    // ���j���[��ʂŎg�p�ł��邩
    public bool canUseInMenu = false;
    // EX�X�L����
    public bool isExSkill = false;

    public CharacterStatus User;
    public CharacterStatus Objective;

    // �X�L���g�p�\���� ��ꃁ�\�b�h
    public abstract bool CanUse(CharacterStatus user);

    // �X�L�������\����
    public bool CanEquip(AllyStatus user)
    {
        var leftSp = user.maxSp - user.sp;
        if (!(this is ActiveSkill || this is PassiveSkill))
        {
            return false;
        }
        if (leftSp <= spCost)
        {
            return false;
        }
        else if (user.equipedSkills.Contains(this))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// �X�L���̌��ʂ�K�p����
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> applyActiveEffect()
    {
        var result = true;
        foreach(var effectType in effects)
        {
            ActiveEffect effect = ActiveEffect.Instance;
            effect.Initialize(this, null);
            result = await effect.CallEffect(effectType, User, Objective);
        }
        return result;
    }
}