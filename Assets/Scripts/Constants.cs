using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
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
    public static int[] requiredExp = new int[maxLevel] { 40, 80, 160, 320, 640, 1280, 2560, 5120, 10240, 20480 };

    // �N���e�B�J���������_���[�W�{��
    public const double criticalDamageRatio = 1.5;

    // �w�U���x�X�L��ID
    public const string attackSkillID = "0000";

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
        Sword,
        Blade,
        Dagger,
        Spear,
        Ax,
        Hammer,
        Fist,
        Bow,
        Staff,
        Shield
    }

    // ����
    public enum Attribute
    {
        Physical,
        Slash,
        Thrust,
        Blow,
        Magical,
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
    public enum EffectType
    {
        AttackWithWeapon,
        Guard,
        HealHP,
        HealHpByItem,
        HealMpByItem,
        PhysicalDamage,
        MagicalDamage        
    }

    // ���A���e�B
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    public enum effectTarget
    {
        Myself,
        OneAlly,
        AllAllies,
        OneEnemy,
        AllEnemies
    }
}
