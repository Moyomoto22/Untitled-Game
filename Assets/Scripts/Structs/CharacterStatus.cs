using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �L�����N�^�[���N���X
/// </summary>
[Serializable]
public abstract class CharacterStatus : ScriptableObject
{

    //�@�L�����N�^�[�̖��O
    [SerializeField]
    public string characterName;

    #region ���C���X�e�[�^�X
    //�@�L�����N�^�[�̃��x��
    [SerializeField]
    public int level;
    //�@�ő�HP
    [SerializeField]
    public int maxHp;
    //�@HP
    [SerializeField]
    public int hp;
    //�@�ő�MP
    [SerializeField]
    public int maxMp;
    //�@MP
    [SerializeField]
    public int mp;
    //�@�ő�TP
    [SerializeField]
    public int maxTp;
    //�@TP
    [SerializeField]
    public int tp;
    //�@STR
    [SerializeField]
    public int str;
    //�@VIT
    [SerializeField]
    public int vit;
    //�@DEX
    [SerializeField]
    public int dex;
    //�@AGI
    [SerializeField]
    public int agi;
    //�@INT
    [SerializeField]
    public int inte;
    //�@MND
    [SerializeField]
    public int mnd;

    // �␳��X�e�[�^�X
    //�@�ő�HP
    public int maxHp2;
    //�@�ő�MP
    public int maxMp2;
    //�@STR
    public int str2;
    //�@VIT
    public int vit2;
    //�@DEX
    public int dex2;
    //�@AGI
    public int agi2;
    //�@INT
    public int inte2;
    //�@MND
    public int mnd2;
    #endregion

    #region �T�u�X�e�[�^�X
    // �����U����
    [SerializeField]
    public int pAttack;
    // ���@�U����
    [SerializeField]
    public int mAttack;
    // �����h���
    [SerializeField]
    public int pDefence;
    // ���@�h���
    [SerializeField]
    public int mDefence;
    // �����N���e�B�J����
    [SerializeField]
    public int pCrit;
    // ���@�N���e�B�J����
    [SerializeField]
    public int mCrit;
    // �������
    [SerializeField]
    public int pAvo;
    // ���@���
    [SerializeField]
    public int mAvo;
    // �����J�E���^�[��
    [SerializeField]
    public int pCnt;
    // ���@�J�E���^�[��
    [SerializeField]
    public int mCnt;
    // �����J�b�g��
    [SerializeField]
    public int pCut;
    // ���@�J�b�g��
    [SerializeField]
    public int mCut;
    #endregion

    #region �X�e�[�^�X�ω��E�c��^�[��
    // �h�� 1�^�[�������p�����Ȃ��̂Ŏc��^�[���͖���
    public bool isGuarded;
    // �X�^�� 1�^�[�������p�����Ȃ��̂Ŏc��^�[���͖���
    public bool isStaned;
    //�@��
    public bool isPoisoned;
    public int poisonRemainingTurn;
    //�@�ғ�
    public bool isToxiced;
    public int toxicedRemainingTurn;
    //�@���
    public bool isParalyzed;
    public int paralyzedRemainingTurn;
    //�@����
    public bool isSleeped;
    public int sleepedRemainingTurn;
    //�@����
    public bool isSilenced;
    public int silencedRemainingTurn;
    //�@���f
    public bool isDazed;
    public int dazedRemainingTurn;
    //�@����
    public bool isTempted;
    public int temptedRemainingTurn;
    //�@����
    public bool isFrosted;
    public int frostedRemainingTurn;
    // �퓬�s�\
    public bool knockedOut;
    #endregion

    #region �X�e�[�^�X�o�t�E�f�o�t
    public float pAttackEffect = 1.0f;      // �����U����
    public int pAttackEffectRemainingTurn;
    public float mAttackEffect = 1.0f;      // ���@�U����
    public int mAttackEffectRemainingTurn;
    public float pDefenceEffect = 1.0f;     // �����h���
    public int pDefenceEffectRemainingTurn;
    public float mDefenceEffect = 1.0f;     // ���@�h���
    public int mDefenceEffectRemainingTurn;
    public float agiEffect = 1.0f;          // AGI
    public int agiEffectRemainingTurn;
    public float pCritEffect = 1.0f;        // �����N���e�B�J��
    public int pCritEffectRemainingTurn;
    public float mCritEffect = 1.0f;        // ���@�N���e�B�J��
    public int mCritEffectRemainingTurn;
    public float AvoidEffect = 1.0f;        // ���
    public int AvoidEffectRemainingTurn;
    public float pCutEffect = 1.0f;         // �����J�b�g��
    public int pCutEffectRemainingTurn;
    public float mCutEffect = 1.0f;         // ���@�J�b�g��
    public int mCutEffectRemainingTurn;
    #endregion

    #region ����
    // �E��
    [SerializeField]
    public Weapon rightArm;
    // ����
    [SerializeField]
    public Weapon leftArm;
    // ��
    [SerializeField]
    public Head head;
    // ��
    [SerializeField]
    public Body body;
    // �����i1
    [SerializeField]
    public Accessary accessary1;
    // �����i2
    [SerializeField]
    public Accessary accessary2;
    #endregion

    // �K���ς݃X�L��
    public List<Skill> learnedSkills;

    #region �ϐ�
    // ����
    [SerializeField]
    public int resistPhysical;
    // ���@
    [SerializeField]
    public int resistMagic;
    // �a��
    [SerializeField]
    public int resistSlash;
    // �h��
    [SerializeField]
    public int resistThrast;
    // �Ō�
    [SerializeField]
    public int resistBlow;
    // ��
    [SerializeField]
    public int resistFire;
    // �X
    [SerializeField]
    public int resistIce;
    // ��
    [SerializeField]
    public int resistThunder;
    // ��
    [SerializeField]
    public int resistWind;
    // ��
    [SerializeField]
    public int resistPoison;
    // ���
    [SerializeField]
    public int resistParalyze;
    // ����
    [SerializeField]
    public int resistAsleep;
    // ����
    [SerializeField]
    public int resistSilence;
    // ���e
    [SerializeField]
    public int resistDaze;
    // ����
    [SerializeField]
    public int resistCharm;
    // ����
    [SerializeField]
    public int resistFrostBite;
    #endregion

    public Vector2 positionInScreen;

    public GameObject spriteObject;

    public GameObject HPGauge;
    public GameObject MPGauge;
    public GameObject TPGauge;
    public GameObject SPGauge;
    public GameObject EXPGauge;


    public CharacterStatus()
    {
        maxHp2 = maxHp + (rightArm?.maxHp ?? 0) + (leftArm?.maxHp ?? 0) + (head?.maxHp ?? 0) + (body?.maxHp ?? 0) + (accessary1?.maxHp ?? 0) + (accessary2?.maxHp ?? 0);
        maxMp2 = maxMp + (rightArm?.maxMp ?? 0) + (leftArm?.maxMp ?? 0) + (head?.maxMp ?? 0) + (body?.maxMp ?? 0) + (accessary1?.maxMp ?? 0) + (accessary2?.maxMp ?? 0);
        str2 = str + (rightArm?.str ?? 0) + (leftArm?.str ?? 0) + (head?.str ?? 0) + (body?.str ?? 0) + (accessary1?.str ?? 0) + (accessary2?.str ?? 0);
        vit2 = vit + (rightArm?.vit ?? 0) + (leftArm?.vit ?? 0) + (head?.vit ?? 0) + (body?.vit ?? 0) + (accessary1?.vit ?? 0) + (accessary2?.vit ?? 0);
        dex2 = dex + (rightArm?.dex ?? 0) + (leftArm?.dex ?? 0) + (head?.dex ?? 0) + (body?.dex ?? 0) + (accessary1?.dex ?? 0) + (accessary2?.dex ?? 0);
        agi2 = agi + (rightArm?.agi ?? 0) + (leftArm?.agi ?? 0) + (head?.agi ?? 0) + (body?.agi ?? 0) + (accessary1?.agi ?? 0) + (accessary2?.agi ?? 0);
        inte2 = inte + (rightArm?.inte ?? 0) + (leftArm?.inte ?? 0) + (head?.inte ?? 0) + (body?.inte ?? 0) + (accessary1?.inte ?? 0) + (accessary2?.inte ?? 0);
        mnd2 = mnd + (rightArm?.mnd ?? 0) + (leftArm?.mnd ?? 0) + (head?.mnd ?? 0) + (body?.mnd ?? 0) + (accessary1?.mnd ?? 0) + (accessary2?.mnd ?? 0);

        // �����U���͈ˑ��l ����ɂ����STR or DEX or INT or MND���U���͂ɉ��Z
        int pAttackCorect = str;
        int pAttackCorectLeft = 0;
        if (rightArm != null)
        {
            switch (rightArm.dependentStatus)
            {
                case 1:
                    pAttackCorect = dex;
                    break;
                case 2:
                    pAttackCorect = inte;
                    break;
                case 3:
                    pAttackCorect = mnd;
                    break;
                default:
                    pAttackCorect = str;
                    break;
            }
        }

        if (leftArm != null)
        {
            if (leftArm.weaponCategory != Constants.WeaponCategory.Shield)
            {
                switch (leftArm.dependentStatus)
                {
                    case 1:
                        pAttackCorectLeft = dex;
                        break;
                    case 2:
                        pAttackCorectLeft = inte;
                        break;
                    case 3:
                        pAttackCorectLeft = mnd;
                        break;
                    default:
                        pAttackCorectLeft = str;
                        break;
                }
            }
        }

        // �T�u�X�e�[�^�X
        int pAttackLeft = pAttackCorectLeft + (leftArm?.pAttack ?? 0);
        pAttack = pAttackCorect + (rightArm?.pAttack ?? 0) + pAttackLeft + (head?.pAttack ?? 0) + (body?.pAttack ?? 0) + (accessary1?.pAttack ?? 0) + (accessary2?.pAttack ?? 0);
        mAttack = inte + (rightArm?.mAttack ?? 0) + (leftArm?.mAttack ?? 0) + (head?.mAttack ?? 0) + (body?.mAttack ?? 0) + (accessary1?.mAttack ?? 0) + (accessary2?.mAttack ?? 0);
        pDefence = vit + (rightArm?.pDefence ?? 0) + (leftArm?.pDefence ?? 0) + (head?.pDefence ?? 0) + (body?.pDefence ?? 0) + (accessary1?.pDefence ?? 0) + (accessary2?.pDefence ?? 0);
        mDefence = mnd / 2 + (rightArm?.mDefence ?? 0) + (leftArm?.mDefence ?? 0) + (head?.mDefence ?? 0) + (body?.mDefence ?? 0) + (accessary1?.mDefence ?? 0) + (accessary2?.mDefence ?? 0);
    }

    /// <summary>
    /// 1�^�[���؂�̌��ʂ���������
    /// </summary>
    /// <returns></returns>
    public void RefreshEffectsRemainOneTurn()
    {
        this.isStaned = false;
        this.isGuarded = false;
    }

    public bool CanUseSkill(Skill skill)
    {
        return skill.CanUse(this);
    }

    public async UniTask<bool> UseSkill(Skill skill, CharacterStatus objective)
    {
        bool result = true;
        if (skill != null && skill.usable && CanUseSkill(skill))
        {
            // �X�L���̌��ʂ�K�p����
            skill.User = this;
            skill.Objective = objective;
            result = await skill.applyActiveEffect();
        }
        else
        {
            return false;
        }
        return result;
    }

    public async UniTask<bool> UseItem(Item item, CharacterStatus objective)
    {
        bool result = true;
        if (item != null && item.usable)
        {
            // �X�L���̌��ʂ�K�p����
            item.User = this;
            item.Objective = objective;
            result = await item.applyEffect();
        }
        return result;
    }

    public async UniTask<int> ReduceHP(int amount)
    {
        hp -= amount;
        if (hp < 0)
        {
            hp = 0;
            knockedOut = true;
        }
        // HP���Q�[�W�ƕR�Â��Ă���ꍇ�A�j���[�V�����J�n
        if (HPGauge != null && this is AllyStatus)
        {
            var manager = HPGauge.GetComponent<GaugeManager>();
            await manager.AnimateTextAndGauge(hp);
        }
        else if (HPGauge != null && this is EnemyStatus)
        {
            var manager = HPGauge.GetComponent<GaugeManager>();
            float targetFillAmount = (float)hp / maxHp2;
            await manager.AnimateGaugeRenderer(targetFillAmount);
        }
        return hp;
    }

    public async UniTask<int> ReduceMP(int amount)
    {
        mp -= amount;
        if (mp < 0)
        {
            mp = 0;
        }
        // MP���Q�[�W�ƕR�Â��Ă���ꍇ�A�j���[�V�����J�n
        if (MPGauge != null)
        {
            var manager = MPGauge.GetComponent<GaugeManager>();
            await manager.AnimateTextAndGauge(mp);
        }
        return mp;
    }

    public async UniTask<int> HealHP(int amount)
    {
        hp += amount;
        if (hp > maxHp2)
        {
            hp = maxHp2;
        }
        // HP���Q�[�W�ƕR�Â��Ă���ꍇ�A�j���[�V�����J�n
        if (HPGauge != null)
        {
            var manager = HPGauge.GetComponent<GaugeManager>();
            await manager.AnimateTextAndGauge(hp);
        }
        return hp;
    }

    public async UniTask<int> HealMP(int amount)
    {
        mp += amount;
        if (mp > maxMp2)
        {
            mp = maxMp2;
        }
        // MP���Q�[�W�ƕR�Â��Ă���ꍇ�A�j���[�V�����J�n
        if (MPGauge != null)
        {
            var manager = MPGauge.GetComponent<GaugeManager>();
            await manager.AnimateTextAndGauge(mp);
        }
        return mp;
    }
}