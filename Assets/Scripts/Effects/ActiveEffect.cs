using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    /// ���ʏ����Ăяo��
    /// </summary>
    /// <param name="type">���ʋ敪</param>
    /// <param name="user">�g�p��</param>
    /// <param name="objective">�Ώێ�</param>
    /// <returns></returns>
    public async UniTask<bool> CallEffect(Constants.ActiveEffectType type, Character user, Character objective)
    {
        bool result = true;

        damageStrings = new List<string>();

        if (animatedText != null)
        {
            animationManager = animatedText.GetComponent<TextAnimationManager>();          // �_���[�W�e�L�X�g�A�j���[�V�����Ǘ��N���X
        }
        if (objective.spriteObject != null)
        {
            spriteManipulator = objective.spriteObject.GetComponentInChildren<SpriteManipulator>();  // �X�v���C�g����N���X
        }

        switch (type)
        {
            case Constants.ActiveEffectType.AttackWithWeapon:           // �����_���[�W
                await AttackWithWeapon(user, objective);
                break;
            case Constants.ActiveEffectType.Guard:                    // �h��
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
            case Constants.ActiveEffectType.HealHPByStatus:                   // HP��
                await HealHPByStatus(user, objective);
                break;
            case Constants.ActiveEffectType.CurePoison:                // �Ŏ���
                await CurePoison(user, objective);
                break;
            case Constants.ActiveEffectType.AddStan:                  // �X�^���t�^
                await AddStan(user, objective);
                break;
            case Constants.ActiveEffectType.UpdateHate:
                await UpdateHate(user);
                break;
            case Constants.ActiveEffectType.ReduceHate:
                await UpdateHateByRatio(user);
                break;
            case Constants.ActiveEffectType.HealHpByItem:             // HP��(�A�C�e��)
                result = await HealHPByItem(user, objective);
                break;
            case Constants.ActiveEffectType.HealMpByItem:             // MP��(�A�C�e��)
                result = await HealMPByItem(user, objective);
                break;
            case Constants.ActiveEffectType.Steal:             // MP��(�A�C�e��)
                await Steal(user, objective);
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
    public async UniTask Guard(Character objective)
    {
        objective.IsGuarded = true;
    }

    /// <summary>
    /// ��������ōU��
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask AttackWithWeapon(Character user, Character objective)
    {
        var attack = user.PAttack;
        var defence = objective.PDefence;
        var attibutes = new List<Constants.Attribute>();
        var times = 1;
        var weaponCategory = Constants.WeaponCategory.Fist;

        if (user.RightArm != null)
        {
            times = user.RightArm.Times;
            attibutes = user.RightArm.Attributes.Select(a => a.attribute).ToList();
            weaponCategory = user.RightArm.WeaponCategory;
        }
        else
        {
            // �f��͑Ō�����
            attibutes.Add(Constants.Attribute.Blow);
        }

        for (int i = 1; i < times + 1; i++)
        {
            // ��𔻒�
            if (!DamageCalculator.Dodge(objective.EvationRate))
            {
                // ��b�_���[�W���v�Z
                var baseDamage = DamageCalculator.CalcurateBaseDamage(attack, defence);
                // �����ɂ��_���[�W����
                baseDamage = DamageCalculator.CalculateDamageWithAttributes(baseDamage, attibutes, objective);
                // ��b�_���[�W�փ����_���������Z
                var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);
                // �N���e�B�J���q�b�g����
                bool criticalHit = DamageCalculator.JudgeCritical(user.CriticalRate);

                if (criticalHit)
                {
                    // �N���e�B�J���{����Z
                    damage = (int)(damage * Constants.criticalDamageRatio);
                }
                // �h�䔻��
                damage = DamageCalculator.ReducingByGuard(objective, damage);

                if (damage < 0)
                {
                    damage = 0;
                }

                // �ŏI�I�ȃ_���[�W���A�\������_���[�W�e�L�X�g�̃��X�g�ɒǉ�
                damageStrings.Add(damage.ToString());
                if (criticalHit)
                {
                    // �N���e�B�J���̏ꍇ�̓e�L�X�g�̖�����"�I"��ǉ�
                    damageStrings[i - 1] += "!";
                }
                // �Ώۂ�HP�����Z
                _ = objective.ReduceHP(damage);

                // �w�C�g�v�Z�E����o���l�Z
                if (user is Ally)
                {
                    Ally ally = user as Ally;
                    int index = PartyMembers.Instance.GetIndex(ally);
                    HateManager.Instance.IncreaseHate(index, damage, skill.HateRatio);

                    ally.GetWeaponExp(weaponCategory, 3);
                }
            }
            else
            {
                damageStrings.Add("Dodge");
            }
        }
        await ShowDamageEffects(objective);
    }

    /// <summary>
    /// �����_���[�W
    /// </summary>
    /// </summary>
    /// <param name="user">�g�p��</param>
    /// <param name="objective">�Ώ�</param>
    /// <returns></returns>
    public async UniTask PhysicalDamage(Character user, Character objective)
    {
        var attack = user.PAttack;
        var defence = objective.PDefence;
        var attibutes = skill.Attributes.Select(s => s.attribute).ToList();
        var times = skill.Times;

        for (int i = 1; i < times + 1; i++)
        {
            // ��𔻒�
            if (!DamageCalculator.Dodge(objective.EvationRate))
            {
                // ��b�_���[�W���v�Z
                var baseDamage = DamageCalculator.CalcurateBaseDamage(attack, defence);
                // �����ɂ��_���[�W����
                baseDamage = DamageCalculator.CalculateDamageWithAttributes(baseDamage, attibutes, objective);
                // ��b�_���[�W�փ����_���������Z
                var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);
                // �N���e�B�J���q�b�g����
                bool criticalHit = DamageCalculator.JudgeCritical(user.CriticalRate);
                // �_���[�W�{��
                damage = (int)(damage * skill.DamageRatio);

                if (criticalHit)
                {
                    // �N���e�B�J���{����Z
                    damage = (int)(damage * Constants.criticalDamageRatio);
                }
                // �h�䔻��
                damage = DamageCalculator.ReducingByGuard(objective, damage);

                if (damage < 0)
                {
                    damage = 0;
                }

                // �ŏI�I�ȃ_���[�W���A�\������_���[�W�e�L�X�g�̃��X�g�ɒǉ�
                damageStrings.Add(damage.ToString());
                if (criticalHit)
                {
                    // �N���e�B�J���̏ꍇ�̓e�L�X�g�̖�����"�I"��ǉ�
                    damageStrings[i - 1] += "!";
                }
                // �Ώۂ�HP�����Z
                // �Q�[�W�����Ɠ����Ɏ��s���邽�ߓ�������
                _ = UniTask.WhenAll(
                objective.ReduceHP(damage),
                user.ReduceTP(skill.TpCost));

                // �w�C�g�v�Z
                if (user is Ally)
                {
                    Ally ally = user as Ally;
                    int index = PartyMembers.Instance.GetIndex(ally);
                    HateManager.Instance.IncreaseHate(index, damage, skill.HateRatio);
                }
            }
            else
            {
                damageStrings.Add("Dodge");
                await user.ReduceTP(skill.TpCost);
            }
        }
        await ShowDamageEffects(objective);
    }

    /// <summary>
    /// ���@�_���[�W
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask MagicalDamage(Character user, Character objective)
    {
        var magic = skill as MagicMiracle;

        var attack = (int)(user.MAttack * magic.StatusRatio);  // ���@�U�� * �X�e�[�^�X�␳
        var defence = objective.MDefence;                      // ���@�h��
        var times = magic.Times;                               // �U����
        var abstractDamage = magic.BaseValue;                  // ���_���[�W
        var damageRatio = magic.DamageRatio;                   // �_���[�W�{��

        bool criticalHit = DamageCalculator.JudgeCritical(user.CriticalRate);

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
            // �Q�[�W�����Ɠ����Ɏ��s���邽�ߓ�������
            _ = UniTask.WhenAll(
            objective.ReduceHP(damage),
            user.ReduceMP(magic.MpCost));

        }
        await ShowDamageEffects(objective);
    }

    /// <summary>
    /// �_���[�W
    /// </summary>
    /// <param name="user">�g�p��</param>
    /// <param name="objective">�Ώ�</param>
    /// <returns></returns>
    public string DealDamage(Character user, Character objective, int offenceValue, int defenceValue)
    {
        string damageString;

        // ��b�_���[�W���v�Z
        var baseDamage = DamageCalculator.CalcurateBaseDamage(offenceValue, defenceValue);
        // ��b�_���[�W�փ����_���������Z
        var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);
        // �N���e�B�J���q�b�g����
        bool criticalHit = DamageCalculator.JudgeCritical(user.CriticalRate);

        if (criticalHit)
        {
            // �N���e�B�J���{����Z
            damage = (int)(damage * Constants.criticalDamageRatio);
        }
        // �h�䔻��
        damage = DamageCalculator.ReducingByGuard(objective, damage);

        if (damage < 0)
        {
            damage = 0;
        }

        // �ŏI�I�ȃ_���[�W
        damageString = (damage.ToString());
        if (criticalHit)
        {
            // �N���e�B�J���̏ꍇ�̓e�L�X�g�̖�����"�I"��ǉ�
            damageString += "!";
        }
        // �Ώۂ�HP�����Z
        _ = objective.ReduceHP(damage);

        // �w�C�g�v�Z
        if (user is Ally)
        {
            Ally ally = user as Ally;
            int index = PartyMembers.Instance.GetIndex(ally);
            HateManager.Instance.IncreaseHate(index, damage, skill.HateRatio);
        }

        return damageString;

    }

    /// <summary>
    /// HP��(���@�U���͈ˑ�)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask HealHP(Character user, Character objective)
    {
        var baseAmount = skill.BaseValue;                   // ���񕜗�
        var status = user.MAttack - user.Int + user.Mnd;    // ���@�U���� - INT + MND (���햂�@�U���� + MND)
        var times = skill.Times;
        var ratio = skill.DamageRatio;

        for (int i = 1; i < times + 1; i++)
        {
            var baseDamage = baseAmount + (int)(status * ratio);
            var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);

            bool criticalHit = DamageCalculator.JudgeCritical(user.CriticalRate);

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
                _ = user.ReduceMP(magic.MpCost));
            }
            else
            {
                _ = objective.HealHP(damage);
            }

            await ShowHealEffects(objective, healHPTextColor);
        }
    }

    /// <summary>
    /// HP��(�X�e�[�^�X�ˑ�)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask HealHPByStatus(Character user, Character objective)
    {
        var baseAmount = skill.BaseValue;
        var status = GetValueDependStatus(user);
        var times = skill.Times;
        var ratio = skill.DamageRatio;

        for (int i = 1; i < times + 1; i++)
        {
            var baseDamage = baseAmount + (int)(status * ratio);
            var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);

            damageStrings.Add(damage.ToString());

            if (skill is MagicMiracle)
            {
                var magic = skill as MagicMiracle;
                UniTask.WhenAll(
                _ = objective.HealHP(damage),
                _ = user.ReduceMP(magic.MpCost));
            }
            else
            {
                _ = objective.HealHP(damage);
            }

            await ShowHealEffects(objective, healHPTextColor);
        }
    }

    /// <summary>
    /// �Ŏ���
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask CurePoison(Character user, Character objective)
    {
        if (objective.IsPoisoned)
        {
            objective.IsPoisoned = false;
        }
    }

    /// <summary>
    /// �X�^���t�^
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask AddStan(Character user, Character objective)
    {
        // (1 - �X�^���ϐ�)%��������
        double successRate = 1 - objective.ResistStan;
        System.Random random = new System.Random();
        int randomValue = random.Next(0, 1);
        bool isSuccess = randomValue < successRate;

        if (isSuccess)
        {
            var str = new List<string>() { "�X�^��" };

            objective.IsStunned = true;
            objective.StunRemainingTurn = 2;
            await animationManager.ShowDamageTextAnimation(objective, str, spriteManipulator);
        }
    }

    /// <summary>
    /// �g�p�҂̃w�C�g����
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask UpdateHate(Character user)
    {
        int baseValue = GetValueDependStatus(user);
        int value = (int)(baseValue * skill.DamageRatio);

        int index = PartyMembers.Instance.GetIndex(user as Ally);
        double reduceRate = skill.DamageRatio;

        HateManager.Instance.UpdateHateByConst(index, value);

        await UniTask.Delay(1);
    }

    /// <summary>
    /// �g�p�҂̃w�C�g����(����)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask UpdateHateByRatio(Character user)
    {
        int index = PartyMembers.Instance.GetIndex(user as Ally);
        double reduceRate = skill.DamageRatio;

        // �X�L�� - �_���[�W�{�������݃w�C�g�ɏ�Z
        HateManager.Instance.ReduceHate(index, reduceRate);

        await UniTask.Delay(1);
    }

    /// <summary>
    /// �X�e�B�[��
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask Steal(Character user, Character objective)
    {
        Enemy enemy = objective as Enemy;

        if (!enemy.IsStolen)
        {
            double successRate = 1 - objective.ResistStan;
            System.Random random = new System.Random();

            // DEX�{�[�i�X�𐬌����ɉ��Z
            float dexBonus = user.Dex / 1000f;
            float successRateOne = enemy.StealRateOne + dexBonus;
            float successRateTwo = enemy.StealRateTwo + dexBonus;

            float randomValue1 = (float)random.NextDouble();
            float randomValue2 = (float)random.NextDouble();

            // ���߂�A�C�e��1
            if (successRateOne > randomValue1)
            {
                enemy.IsStolen = true;
                ItemInventory2.Instance.AddCopyItem(enemy.StealItemOne);

                var message = enemy.StealItemOne.ItemName + " �𓐂񂾁I";
                var sprite = enemy.StealItemOne.IconImage;

                ToastMessageManager.Instance.ShowToastMessage(message, sprite);
            }
            // ���߂�A�C�e��2
            else if (successRateTwo > randomValue2)
            {
                enemy.IsStolen = true;
                ItemInventory2.Instance.AddCopyItem(enemy.StealItemTwo);

                var message = enemy.StealItemTwo.ItemName + " �𓐂񂾁I";
                var sprite = enemy.StealItemTwo.IconImage;

                ToastMessageManager.Instance.ShowToastMessage(message, sprite);
            }
            else
            {
                var message = "���߂Ȃ������I";
                ToastMessageManager.Instance.ShowToastMessage(message);
            }
        }
        else
        {
            var message = "���������Ă��Ȃ��I";
            ToastMessageManager.Instance.ShowToastMessage(message);
        }
    }


    /// <summary>
    /// HP��(�A�C�e��)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask<bool> HealHPByItem(Character user, Character objective)
    {
        // HP���^���̏ꍇ�͏������s
        if (objective.HP < objective.MaxHp)
        {
            var baseAmount = item.BaseValue;           // ���񕜗�
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
    public async UniTask<bool> HealMPByItem(Character user, Character objective)
    {
        // MP���^���̏ꍇ�͏������s
        if (objective.MP < objective.MaxMp)
        {
            var baseAmount = item.BaseValue;            // ���񕜗�
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
    private async UniTask ShowDamageEffects(Character objective)
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
    private async UniTask ShowHealEffects(Character objective, Color color)
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

    private int GetValueDependStatus(Character user)
    {
        int value = 0;
        switch (skill.DependentStatus)
        {
            case Skill.DependStatus.STR:
                value = user.Str;
                break;
            case Skill.DependStatus.VIT:
                value = user.Vit;
                break;
            case Skill.DependStatus.DEX:
                value = user.Dex;
                break;
            case Skill.DependStatus.AGI:
                value = user.Agi;
                break;
            case Skill.DependStatus.INT:
                value = user.Int;
                break;
            case Skill.DependStatus.MND:
                value = user.Mnd;
                break;
        }
        return value;
    }
}
