using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�L������(�V���O���g��)
/// </summary>
public class PassiveEffect : MonoBehaviour
{
    private PassiveSkill skill;

    private CharacterController user;
    private CharacterController objective;

    private List<string> damageStrings;
    public GameObject animatedText;

    private TextAnimationManager animationManager;
    private SpriteManipulator spriteManipulator;

    private GaugeManager hpGaugeManager;
    private GaugeManager mpGaugeManager;
    private GaugeManager tpGaugeManager;

    private Color healHPTextColor;
    private Color healMPTextColor;

    public static PassiveEffect Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <param name="skill">�g�p�X�L��</param>
    public void Initialize(PassiveSkill skill)
    {
        this.skill = skill;
    }

    /// <summary>
    /// ���ʏ����Ăяo�����\�b�h
    /// </summary>
    /// <param name="type">���ʋ敪</param>
    /// <param name="user">�g�p��</param>
    /// <param name="objective">�Ώێ�</param>
    /// <returns></returns>
    public void CallEffect(Constants.PassiveEffectType type, CharacterStatus user, CharacterStatus objective)
    {
        damageStrings = new List<string>();

        switch (type)
        {
            case Constants.PassiveEffectType.GainMAXHP:                    // �ő�HP�㏸
                GainMaxHP(user);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// HP�㏸
    /// </summary>
    /// <param name="user"></param>
    public void GainMaxHP(CharacterStatus user)
    {
        user.maxHp2 += skill.baseValue;
    }

    /// <summary>
    /// �����_���[�W
    /// </summary>
    /// <param name="user">�g�p��</param>
    /// <param name="objective">�Ώ�</param>
    /// <returns></returns>
    public async UniTask PhysicalDamage(CharacterStatus user, CharacterStatus objective)
    {
        var attack = user.pAttack;
        var defence = objective.pDefence;
        var times = 1;

        if (user.rightArm != null)
        {
            times = user.rightArm.times;
        }

        for (int i = 1; i < times + 1; i++)
        {
            // ��b�_���[�W���v�Z
            var baseDamage = DamageCalculator.CalcurateBaseDamage(attack, defence);
            // ��b�_���[�W�փ����_���������Z
            var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);
            // �N���e�B�J���q�b�g����
            bool criticalHit = DamageCalculator.JudgeCritical(user.pCrit);

            if (criticalHit)
            {
                // �N���e�B�J���{����Z
                damage = (int)(damage * Constants.criticalDamageRatio);
            }
            // �h�䔻��
            damage = DamageCalculator.ReducingByGuard(objective, damage);

            // �ŏI�I�ȃ_���[�W���A�\������_���[�W�e�L�X�g�̃��X�g�ɒǉ�
            damageStrings.Add(damage.ToString());
            if (criticalHit)
            {
                // �N���e�B�J���̏ꍇ�̓e�L�X�g�̖�����"�I"��ǉ�
                damageStrings[i - 1] += "!";
            }
            // �Ώۂ�HP�����Z
            _ = objective.ReduceHP(damage);
        }
        await ShowDamageEffects(objective);
    }

    /// <summary>
    /// �_���[�W�����o(�e�L�X�g�\���E�X�v���C�g�̃V�F�C�N�E�t���b�V��)
    /// </summary>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask ShowDamageEffects(CharacterStatus objective)
    {
        if (animationManager != null && spriteManipulator != null)
        {
            await animationManager.ShowDamageTextAnimation(objective, damageStrings, spriteManipulator);
        }
    }

    /// <summary>
    /// �񕜎����o(�e�L�X�g�\���E�X�v���C�g�̃V�F�C�N�E�t���b�V��)
    /// </summary>
    /// <param name="objective"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private async UniTask ShowHealEffects(CharacterStatus objective, Color color)
    {
        if (animationManager != null && spriteManipulator != null)
        {
            await UniTask.WhenAll(
            animationManager.ShowHealTextAnimation(objective, damageStrings, color),
            spriteManipulator.Flash(0.5f, color)
            );
        }
        else if (animationManager != null)
        {
            await animationManager.ShowHealTextAnimation(objective, damageStrings, color);
        }
    }
}
