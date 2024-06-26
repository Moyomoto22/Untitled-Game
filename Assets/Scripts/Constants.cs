using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static string[] characterNames = { "アレックス", "ニコ", "タバサ", "アリシア" };
    
    #region Addressable パス 味方ステータス
    public const string alexPath = "Assets/Scripts/Ally/Alex.asset";
    public const string nicoPath = "Assets/Scripts/Ally/Nico.asset";
    public const string tabathaPath = "Assets/Scripts/Ally/Tabatha.asset";
    public const string aliciaPath = "Assets/Scripts/Ally/Alicia.asset";
    #endregion

    #region Addressable パス InputAction
    public const string inputActionPath = "Assets/InputActions/InputActions.prefab";
    #endregion

    #region Addressable パス アイテムインベントリ
    public const string itemInventoryPath = "Assets/Data/Item/ItemInventory.prefab";
    #endregion

    #region メニュー画面 グラデーションカラーコード
    public const string gradationBlue = "#3C52B799"; //  #192b5d
    public const string gradationRed = "#B32D4099";
    public const string gradationPurple = "#5D21A299";
    public const string gradationGreen = "#1E8C9899";
    #endregion

    #region メニュー画面 文字色カラーコード
    public const string white = "#FFFFFF";
    public const string gray = "#808080";
    public const string darkGray = "#696969";
    public const string blue = "#4C82FF";
    public const string red = "#FF4C55";
    public const string purple = "#8439CD";
    public const string green = "#38C8A9";
    #endregion

    // メニュー画面 ボタンアニメーション速度
    public const double buttonAnimateSpeed = 0.3; 

    const int maxLevel = 10;

    #region 基本成長曲線

    public static int[] hpGrow = new int[maxLevel] { 36, 42, 50, 58, 66, 74, 82, 92, 102, 120 };
    public static int[] mpGrow = new int[maxLevel] { 20, 26, 32, 38, 44, 50, 56, 62, 70, 84 };
    public static int[] prmGrow = new int[maxLevel] { 15, 18, 22, 25, 28, 32, 35, 40, 44, 50 };
    public static int[] spGrow = new int[maxLevel] { 10, 15, 20, 25, 30, 35, 40, 45, 50, 55 };

    #endregion

    // 必要経験値曲線
    public static int[] requiredExp = new int[maxLevel - 1] { 40, 80, 160, 320, 640, 1020, 1600, 2400, 4000};

    // 必要武器経験値曲線
    public static int[] requiredWeaponExp = new int[20] {0, 20, 40, 80, 100, 130, 160, 190, 220, 250, 290, 330, 370, 410, 450, 500, 550, 600, 650, 700};

    // クリティカル発生時ダメージ倍率
    public const double criticalDamageRatio = 1.5;

    // 『攻撃』スキルID
    public const string attackSkillID = "0000";

    // 『行動できない』スキルID
    public const string stunnedSkillID = "9998";

    // 『防御』スキルID
    public const string guardSkillID = "9999";

    // 防御時のダメージ軽減率
    public const float reductionRateByGuard = 0.66f;

    // ステータス
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

    // メニュー画面 表示状態
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

    // アイテム種別
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

    // アイテム種別
    public enum SkillCategory
    {
        Magic,
        Miracle,
        Arts,
        Active,
        Passive
    }

    // 武器種別
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

    // 属性
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
    /// 状態異常
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

    // 効果名称
    public enum EffectName
    {
        HealHP,
        HealMP,
        GainTP
    }

    // 効果種別
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
        // 基礎ステータス
        GainMAXHP,
        GainMAXMP,
        GainSTR,
        GainVIT,
        GainDEX,
        GainAGI,
        GainINT,
        GainMND,

        // サブステータス
        GainCritical,
        GainEvation,
        Counter,
        Block,

        // Exスキル
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

        // 武器装備
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

        // 魔法・奇跡
        CanUseMagic1,
        CanUSeMagic2,
        CanUseMagic3,
        CanUseMiracle1,
        CanUseMiracle2,
        CanUseMiracle3,

        // 自動回復
        Regeneration,
        Refresh,
        Regain,

        // フィールド
        Stealth,
        Sneak,
        OwlEye,
        LockSmith,

        // その他
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

    // レアリティ
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
