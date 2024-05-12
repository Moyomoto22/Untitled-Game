using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�C�e���E�X�L������(�V���O���g��)
/// </summary>
public class ActiveEffect : MonoBehaviour
{
    private Skill skill;
    private Item item;
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

    public static ActiveEffect Instance { get; private set; }

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
    /// <param name="item">�g�p�A�C�e��</param>
    public void Initialize(Skill skill, Item item)
    {
        this.skill = skill;
        this.item = item;

        healHPTextColor = CommonController.GetColor("#32C883"); // HP�񕜎��̃e�L�X�g�J���[
        healMPTextColor = CommonController.GetColor("#569FE5"); // MP�񕜎��̃e�L�X�g�J���[
    }

    /// <summary>
    /// ���ʏ����Ăяo�����\�b�h
    /// </summary>
    /// <param name="type">���ʋ敪</param>
    /// <param name="user">�g�p��</param>
    /// <param name="objective">�Ώێ�</param>
    /// <returns></returns>
    public async UniTask<bool> CallEffect(Constants.ActiveEffectType type, CharacterStatus user, CharacterStatus objective)
    {
        bool result = true;
        
        damageStrings = new List<string>();

        if (animatedText != null)
        {
            animationManager = animatedText.GetComponent<TextAnimationManager>();          // �_���[�W�e�L�X�g�A�j���[�V�����Ǘ��N���X
        }
        if (objective.spriteObject != null)
        {
            spriteManipulator = objective.spriteObject.GetComponent<SpriteManipulator>();  // �X�v���C�g����N���X
        }

        switch (type)
        {
            case Constants.ActiveEffectType.Guard:                    // �����_���[�W
                await Guard(objective);
                break;
            case Constants.ActiveEffectType.PhysicalDamage:           // �����_���[�W
                await PhysicalDamage(user, objective);
                break;
            case Constants.ActiveEffectType.MagicalDamage:            // ���@�_���[�W
                await MagicalDamage(user, objective);
                break;
            case Constants.ActiveEffectType.HealHP:                   // HP��
                await HealHP(user, objective);
                break;
            case Constants.ActiveEffectType.HealHpByItem:             // HP��(�A�C�e��)
                result = await HealHPByItem(user, objective);
                break;
            case Constants.ActiveEffectType.HealMpByItem:             // MP��(�A�C�e��)
                result = await HealMPByItem(user, objective);
                break;
            default:
                break;
        }
        return result;
    }

    /// <summary>
    /// �h��
    /// </summary>
    /// <param name="objective">�Ώێ�</param>
    public async UniTask Guard(CharacterStatus objective)
    {
        objective.isGuarded = true;
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
    /// ���@�_���[�W
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask MagicalDamage(CharacterStatus user, CharacterStatus objective)
    {
        var magic = skill as MagicMiracle;

        var attack = (int)(user.mAttack * magic.statusRatio);  // ���@�U�� * �X�e�[�^�X�␳
        var defence = objective.mDefence;                      // ���@�h��
        var times = magic.times;                               // �U����
        var abstractDamage = magic.baseValue;                  // ���_���[�W
        var damageRatio = magic.damageRatio;                   // �_���[�W�{��

        bool criticalHit = DamageCalculator.JudgeCritical(user.mCrit);

        for (int i = 1; i < times + 1; i++)
        {
            // ��{�_���[�W = (���_���[�W + ���@�U���� * �X�e�[�^�X�␳ / 2 - ���@�h�� / 3) * �_���[�W�{��
            var baseDamage = (int)((abstractDamage + DamageCalculator.CalcurateBaseDamage(attack, defence)) * damageRatio);
            // ����(��{�_���[�W / 16 + 1)
            var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);

            if (criticalHit)
            {
                damage = (int)(damage * Constants.criticalDamageRatio);
            }
            damage = DamageCalculator.ReducingByGuard(objective, damage);

            damageStrings.Add(damage.ToString());
            if (criticalHit)
            {
                damageStrings[i - 1] += "!";
            }
            UniTask.WhenAll(
            objective.ReduceHP(damage),
            user.ReduceMP(magic.MPCost));
            
        }
        await ShowDamageEffects(objective);

    }

    /// <summary>
    /// HP��
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask HealHP(CharacterStatus user, CharacterStatus objective)
    {
        

        var baseAmount = skill.baseValue;                   // ���񕜗�
        var status = user.mAttack - user.inte2 + user.mnd2; // ���@�U���� - INT + MND (���햂�@�U���� + MND)
        var times = skill.times;
        var ratio = skill.damageRatio;

        for (int i = 1; i < times + 1; i++)
        {
            var baseDamage = baseAmount + (int)(status * ratio);
            var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);

            bool criticalHit = DamageCalculator.JudgeCritical(user.mCrit);

            if (criticalHit)
            {
                damage = (int)(damage * Constants.criticalDamageRatio);
            }
            damageStrings.Add(damage.ToString());
            if (criticalHit)
            {
                damageStrings[i - 1] += "!";
            }

            if (skill is MagicMiracle)
            {
                var magic = skill as MagicMiracle;
                UniTask.WhenAll(
                _ = objective.HealHP(damage),
                _ = user.ReduceMP(magic.MPCost));
            }
            else
            {
                _ = objective.HealHP(damage);
            }
               
            await ShowHealEffects(objective, healHPTextColor);
        }
    }

    /// <summary>
    /// HP��(�A�C�e��)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask<bool> HealHPByItem(CharacterStatus user, CharacterStatus objective)
    {
        // HP���^���̏ꍇ�͏������s
        if (objective.hp < objective.maxHp2)
        {
            var baseAmount = item.baseValue;           // ���񕜗�
            damageStrings.Add(baseAmount.ToString());
            ItemInventory2.Instance.RemoveItem(item);
            await UniTask.WhenAll(
                objective.HealHP(baseAmount),
                ShowHealEffects(objective, healHPTextColor));
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// MP��(�A�C�e��)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask<bool> HealMPByItem(CharacterStatus user, CharacterStatus objective)
    {
        // MP���^���̏ꍇ�͏������s
        if (objective.mp < objective.maxMp2)
        {
            var baseAmount = item.baseValue;            // ���񕜗�
            damageStrings.Add(baseAmount.ToString());
            ItemInventory2.Instance.RemoveItem(item);
            await UniTask.WhenAll(
                objective.HealMP(baseAmount),
                ShowHealEffects(objective, healMPTextColor)
                );
            return true;
        }
        else
        {
            return false;            
        }
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
