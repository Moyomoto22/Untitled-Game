using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキル基底クラス
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "Skill", menuName = "CreateSkill")]
public abstract class Skill : ScriptableObject
{
    public enum DependStatus
    {
        STR = 1,
        VIT = 2,
        DEX = 3,
        AGI = 4,
        INT = 5,
        MND = 6
    }

    // 既存のプロパティ
    [SerializeField] private string id;
    [SerializeField] private Constants.SkillCategory skillCategory;
    [SerializeField] private string skillName;
    [SerializeField] private Sprite icon;
    [SerializeField] private bool usable;
    [SerializeField, Multiline(6)] private string description;
    [SerializeField] private int spCost;
    [SerializeField] private int mpCost;
    [SerializeField] private int tpCost;
    [SerializeField] private int recastTurn;
    [SerializeField] private int remainingTurn;
    [SerializeField] private List<AttributeType> attributes;
    [SerializeField] private string learn;
    [SerializeField] private List<Constants.ActiveEffectType> effects;
    [SerializeField] private int baseValue;
    [SerializeField] private int status;
    [SerializeField] private double statusRatio;
    [SerializeField] private double damageRatio;
    [SerializeField] private double hateRatio = 1.0;
    [SerializeField] private int times;
    [SerializeField] private Constants.TargetType target;
    [SerializeField] private bool isEquipped;
    [SerializeField] private List<Class> usableClasses;
    [SerializeField] private bool canUseInMenu = false;
    [SerializeField] private bool isExSkill = false;
    [SerializeField] private int getTP;
    [SerializeField] private int giveTP;
    [SerializeField] private Character user;
    [SerializeField] private Character objective;


    #region プロパティのゲッターとセッター
    public string ID
    {
        get { return id; }
        set { id = value; }
    }

    public Constants.SkillCategory SkillCategory
    {
        get { return skillCategory; }
        set { skillCategory = value; }
    }

    public string SkillName
    {
        get { return skillName; }
        set { skillName = value; }
    }

    public Sprite Icon
    {
        get { return icon; }
        set { icon = value; }
    }

    public bool Usable
    {
        get { return usable; }
        set { usable = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public int SpCost
    {
        get { return spCost; }
        set { spCost = value; }
    }

    public int MpCost
    {
        get { return mpCost; }
        set { mpCost = value; }
    }

    public int TpCost
    {
        get { return tpCost; }
        set { tpCost = value; }
    }

    public int RecastTurn
    {
        get { return recastTurn; }
        set { recastTurn = value; }
    }

    public int RemainingTurn
    {
        get { return remainingTurn; }
        set {
            if (value < 0)
            {
                remainingTurn = 0;
            }
            else
            {
                remainingTurn = value;
            }
        }
    }

    public List<AttributeType> Attributes
    {
        get { return attributes; }
        set { attributes = value; }
    }

    public string Learn
    {
        get { return learn; }
        set { learn = value; }
    }

    public List<Constants.ActiveEffectType> Effects
    {
        get { return effects; }
        set { effects = value; }
    }

    public int BaseValue
    {
        get { return baseValue; }
        set { baseValue = value; }
    }

    // 依存ステータス 
    [SerializeField] private DependStatus dependentStatus = DependStatus.STR;
    public DependStatus DependentStatus
    {
        get { return dependentStatus; }
        set { dependentStatus = value; }
    }

    public double StatusRatio
    {
        get { return statusRatio; }
        set { statusRatio = value; }
    }

    public double DamageRatio
    {
        get { return damageRatio; }
        set { damageRatio = value; }
    }

    public double HateRatio
    {
        get { return hateRatio; }
        set { hateRatio = value; }
    }

    public int Times
    {
        get { return times; }
        set { times = value; }
    }

    public Constants.TargetType Target
    {
        get { return target; }
        set { target = value; }
    }

    public bool IsEquipped
    {
        get { return isEquipped; }
        set { isEquipped = value; }
    }

    public List<Class> UsableClasses
    {
        get { return usableClasses; }
        set { usableClasses = value; }
    }

    public bool CanUseInMenu
    {
        get { return canUseInMenu; }
        set { canUseInMenu = value; }
    }

    public bool IsExSkill
    {
        get { return isExSkill; }
        set { isExSkill = value; }
    }

    public int GetTP
    {
        get { return getTP; }
        set { getTP = value; }
    }

    public int GiveTP
    {
        get { return giveTP; }
        set { giveTP = value; }
    }

    public Character User
    {
        get { return user; }
        set { user = value; }
    }

    public Character Objective
    {
        get { return objective; }
        set { objective = value; }
    }
    #endregion

    // スキル使用可能判定 基底メソッド
    public abstract bool CanUse(Character user);

    // スキル装備可能判定
    public bool CanEquip(Ally user)
    {
        var leftSp = user.MaxSp - user.SP;
        if (!(this is ActiveSkill || this is PassiveSkill))
        {
            return false;
        }
        if (leftSp < spCost)
        {
            return false;
        }
        else if (user.EquipedSkills.Any(skill => skill.ID == this.ID))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// スキルの効果を適用する
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> ApplyActiveEffect()
    {
        var result = true;
        foreach (var effectType in effects)
        {
            ActiveEffect effect = ActiveEffect.Instance;
            effect.Initialize(this, null);
            result = await effect.CallEffect(effectType, User, Objective);
        }
        return result;
    }

    /// <summary>
    /// スキル使用後のクールダウンを開始する
    /// </summary>
    public void StartCoolDown()
    {
        RemainingTurn = RecastTurn;
    }

    /// <summary>
    /// ターン終了時など、スキルの使用可能までの残りターンを進める
    /// </summary>
    public void CountCoolDown()
    {
        RemainingTurn--;
    }
}