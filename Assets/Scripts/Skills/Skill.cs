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
    [SerializeField]
    public List<Constants.Attribute> attributes;
    //�@�K���N���X
    [SerializeField]
    public BaseClass learnClass;
    //�@�K�����x��
    [SerializeField]
    public int learnLevel;
    //�@����
    [SerializeField]
    public List<Constants.EffectType> effects;
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
    [SerializeField]
    public bool isEquipped;

    public CharacterStatus User;
    public CharacterStatus Objective;

    // �X�L���g�p�\���� ��ꃁ�\�b�h
    public abstract bool CanUse(CharacterStatus user);

    /// <summary>
    /// �X�L���̌��ʂ�K�p����
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> applyEffect()
    {
        var result = true;
        foreach(var effectType in effects)
        {
            Effect effect = Effect.Instance;
            effect.Initialize(this, null);
            result = await effect.CallEffect(effectType, User, Objective);
        }
        return result;
    }
}