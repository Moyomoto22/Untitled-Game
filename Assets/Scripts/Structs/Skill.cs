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
    // ID
    [SerializeField]
    public string ID;
    // 種別
    [SerializeField]
    public Constants.SkillCategory skillCategory;
    //　スキル名
    [SerializeField]
    public string skillName;
    //　アイコン
    [SerializeField]
    public Sprite icon;
    //　使用可否
    [SerializeField]
    public bool usable;
    //　説明
    [SerializeField, Multiline(6)]
    public string description;
    // 消費SP
    [SerializeField]
    public int spCost;
    //　属性
    public List<Attibute> attributes;
    // 習得方法s
    public string learn;
    // アクティブ効果
    public List<Constants.ActiveEffectType> effects;
    // 基礎値
    public int baseValue;
    // 補正ステータス
    private int status;
    // 補正ステータス係数
    public double statusRatio;
    // ダメージ倍率
    public double damageRatio;
    // 回数
    public int times;
    // 対象 0:なし 1:自分 2:味方 3:敵  
    public int target;
    // 対象が全体か
    public bool isTargetAll;
    //　装備中か
    public bool isEquipped;
    // 使用可能クラス
    public List<Class> usableClasses;
    // メニュー画面で使用できるか
    public bool canUseInMenu = false;
    // EXスキルか
    public bool isExSkill = false;

    public CharacterStatus User;
    public CharacterStatus Objective;

    // スキル使用可能判定 基底メソッド
    public abstract bool CanUse(CharacterStatus user);

    // スキル装備可能判定
    public bool CanEquip(AllyStatus user)
    {
        var leftSp = user.maxSp - user.sp;
        if (!(this is ActiveSkill || this is PassiveSkill))
        {
            return false;
        }
        if (leftSp <= spCost)
        {
            return false;
        }
        else if (user.equipedSkills.Contains(this))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// スキルの効果を適用する
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> applyActiveEffect()
    {
        var result = true;
        foreach(var effectType in effects)
        {
            ActiveEffect effect = ActiveEffect.Instance;
            effect.Initialize(this, null);
            result = await effect.CallEffect(effectType, User, Objective);
        }
        return result;
    }
}