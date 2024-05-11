using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
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
    public static int[] requiredExp = new int[maxLevel] { 40, 80, 160, 320, 640, 1280, 2560, 5120, 10240, 20480 };

    // クリティカル発生時ダメージ倍率
    public const double criticalDamageRatio = 1.5;

    // 『攻撃』スキルID
    public const string attackSkillID = "0000";

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

    // 属性
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

    // レアリティ
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
