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
    [SerializeField]
    public List<Constants.Attribute> attributes;
    //　習得クラス
    [SerializeField]
    public BaseClass learnClass;
    //　習得レベル
    [SerializeField]
    public int learnLevel;
    //　効果
    [SerializeField]
    public List<Constants.EffectType> effects;
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
    [SerializeField]
    public bool isEquipped;

    public CharacterStatus User;
    public CharacterStatus Objective;

    // スキル使用可能判定 基底メソッド
    public abstract bool CanUse(CharacterStatus user);

    /// <summary>
    /// スキルの効果を適用する
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> applyEffect()
    {
        var result = true;
        foreach(var effectType in effects)
        {
            Effect effect = Effect.Instance;
            effect.Initialize(this, null);
            result = await effect.CallEffect(effectType, User, Objective);
        }
        return result;
    }
}