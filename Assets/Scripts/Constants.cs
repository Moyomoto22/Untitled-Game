using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static string[] characterNames = { "�A���b�N�X", "�j�R", "�^�o�T", "�A���V�A" };
    
    #region Addressable �p�X �����X�e�[�^�X
    public const string alexPath = "Assets/Scripts/Ally/Alex.asset";
    public const string nicoPath = "Assets/Scripts/Ally/Nico.asset";
    public const string tabathaPath = "Assets/Scripts/Ally/Tabatha.asset";
    public const string aliciaPath = "Assets/Scripts/Ally/Alicia.asset";
    #endregion

    #region Addressable �p�X InputAction
    public const string inputActionPath = "Assets/InputActions/InputActions.prefab";
    #endregion

    #region Addressable �p�X �A�C�e���C���x���g��
    public const string itemInventoryPath = "Assets/Data/Item/ItemInventory.prefab";
    #endregion

    #region ���j���[��� �O���f�[�V�����J���[�R�[�h
    public const string gradationBlue = "#3C52B799"; //  #192b5d
    public const string gradationRed = "#B32D4099";
    public const string gradationPurple = "#5D21A299";
    public const string gradationGreen = "#1E8C9899";
    #endregion

    #region ���j���[��� �����F�J���[�R�[�h
    public const string white = "#FFFFFF";
    public const string gray = "#808080";
    public const string darkGray = "#696969";
    public const string blue = "#4C82FF";
    public const string red = "#FF4C55";
    public const string purple = "#8439CD";
    public const string green = "#38C8A9";
    #endregion

    // ���j���[��� �{�^���A�j���[�V�������x
    public const double buttonAnimateSpeed = 0.3; 

    const int maxLevel = 10;

    #region ��{�����Ȑ�

    public static int[] hpGrow = new int[maxLevel] { 36, 42, 50, 58, 66, 74, 82, 92, 102, 120 };
    public static int[] mpGrow = new int[maxLevel] { 20, 26, 32, 38, 44, 50, 56, 62, 70, 84 };
    public static int[] prmGrow = new int[maxLevel] { 15, 18, 22, 25, 28, 32, 35, 40, 44, 50 };
    public static int[] spGrow = new int[maxLevel] { 10, 15, 20, 25, 30, 35, 40, 45, 50, 55 };

    #endregion

    // �K�v�o���l�Ȑ�
    public static int[] requiredExp = new int[maxLevel - 1] { 40, 80, 160, 320, 640, 1020, 1600, 2400, 4000};

    // �K�v����o���l�Ȑ�
    public static int[] requiredWeaponExp = new int[20] {0, 20, 40, 80, 100, 130, 160, 190, 220, 250, 290, 330, 370, 410, 450, 500, 550, 600, 650, 700};

    // �N���e�B�J���������_���[�W�{��
    public const double criticalDamageRatio = 1.5;

    // �w�U���x�X�L��ID
    public const string attackSkillID = "0000";

    // �w�s���ł��Ȃ��x�X�L��ID
    public const string stunnedSkillID = "9998";

    // �w�h��x�X�L��ID
    public const string guardSkillID = "9999";

    // �h�䎞�̃_���[�W�y����
    public const float reductionRateByGuard = 0.66f;

    // �X�e�[�^�X
    public enum Status
    {
        MaxHP,
        MaxMP,
        STR,
        VIT,
        DEX,
        AGI,
        INT,
        MND
    }

    // ���j���[��� �\�����
    public enum MenuState
    {
        Main = 0,
        Item = 1,
        Equip = 2,
        Skill = 3,
        Class = 4,
        Status = 5,
        Option = 6
    }

    // �A�C�e�����
    public enum ItemCategory
    {
        Consumable,
        Material,
        Weapon,
        Head,
        Body,
        Accessary,
        Misc,
        All
    }

    // �A�C�e�����
    public enum SkillCategory
    {
        Magic,
        Miracle,
        Arts,
        Active,
        Passive
    }

    // ������
    public enum WeaponCategory
    {
        Sword = 0,
        Blade = 1,
        Dagger = 2,
        Spear = 3,
        Ax = 4,
        Hammer = 5,
        Fist = 6,
        Bow = 7,
        Staff = 8,
        Shield= 9
    }

    // ����
    public enum Attribute
    {
        Physical,
        Slash,
        Thrust,
        Blow,
        Magic,
        Fire,
        Ice,
        Thunder,
        Wind,
    }

    /// <summary>
    /// ��Ԉُ�
    /// </summary>
    public enum State
    {
        Poisoned,
        Toxiced,
        Paralyzed,
        Sleeped,
        Silenced,
        Dazed,
        Tempted,
        Frosted,
        KnockedOut
    }

    // ���ʖ���
    public enum EffectName
    {
        HealHP,
        HealMP,
        GainTP
    }

    // ���ʎ��
    public enum ActiveEffectType
    {
        AttackWithWeapon,
        Guard,
        HealHP,
        HealHPByStatus,
        CurePoison,
        CureParalyze,
        CureSleep,
        CureSilence,
        CureDaze,
        CureTemp,
        CureFrost,
        HealHpByItem,
        HealMpByItem,
        PhysicalDamage,
        MagicalDamage,
        AddPoison,
        AddParalyze,
        AddSleep,
        AddSilence,
        AddDaze,
        AddTemp,
        AddFrost,
        AddStan,
        ManaConservation,
        UpdateHate,
        ReduceHate,
        Rampart,
        Steal,        
        SpellBoost,
        OverHeal,
        Concentrate,
        influencer,
        UpdatePA,
        UpdateMA,
        UpdatePD,
        UpdateMD,
        UpdateAGI,
        UpdateCRT,
        UpdateEVA,
        UpdateBLC,
        UpdateCNT
    }

    public enum PassiveEffectType
    {
        // ��b�X�e�[�^�X
        GainMAXHP,
        GainMAXMP,
        GainSTR,
        GainVIT,
        GainDEX,
        GainAGI,
        GainINT,
        GainMND,

        // �T�u�X�e�[�^�X
        GainCritical,
        GainEvation,
        Counter,
        Block,

        // Ex�X�L��
        Berserk1,
        Berserk2,
        Berserk3,
        Chivalry1,
        Chivalry2,
        Chivalry3,
        Renki1,
        Renki2,
        Renki3,
        TreasureHunt1,
        TreasureHunt2,
        TreasureHunt3,
        HawkEye1,
        HawkEye2,
        HawkEye3,
        ForceEater1,
        ForceEater2,
        ForceEater3,
        Devotion1,
        Devotion2,
        Devotion3,
        Enhancer1,
        Enhancer2,
        Enhancer3,

        // ���푕��
        CanEquipSword,
        CanEquipBlade,
        CanEquipDagger,
        CanEquipSpear,
        CanEquipAx,
        CanEquipHammer,
        CanEquipFist,
        CanEquipBow,
        CanEquipStaff,
        CanEquipShield,
        EinHander,
        Nitouryu,

        // ���@�E���
        CanUseMagic1,
        CanUSeMagic2,
        CanUseMagic3,
        CanUseMiracle1,
        CanUseMiracle2,
        CanUseMiracle3,

        // ������
        Regeneration,
        Refresh,
        Regain,

        // �t�B�[���h
        Stealth,
        Sneak,
        OwlEye,
        LockSmith,

        // ���̑�
        ReduceMPCost,
        GainHate,
        ReduceHate,
        Calm,
        Rage,
        FireStarter,
        StealUp,
        Datto,
        VultureEye,
        Farmacy,        
    }

    // ���A���e�B
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    public enum TargetType
    {
        None = 0,
        Self = 1,
        Ally = 2,
        AllAllies = 3,
        Enemy = 4,
        AllEnemies = 5
    }

    public enum StatusEffectIndex
    {
        Poison = 0,
        Paralyze = 1,
        Sleep = 2,
        Silence = 3,
        Daze = 4,
        Temp = 5,
        Frost = 6,
        Stun = 7,
        PAttackUp = 8,
        PAttackDown = 9,
        MAttackUp = 10,
        MAttackDown = 11,
        PDefenceUp = 12,
        PDefenceDown = 13,
        MDefenceUp = 14,
        MDefenceDown = 15,
        AGIUp = 16,
        AGIDown = 17,
        Misc = 18
    }
}
