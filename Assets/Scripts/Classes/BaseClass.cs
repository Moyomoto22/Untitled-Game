using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "Class", menuName = "CreateClass")]
public class BaseClass : ScriptableObject
{
    //　ID
    [SerializeField]
    public int ID;
    //　クラス名
    [SerializeField]
    public string className;
    //　クラス名略称
    [SerializeField]
    public string classAbbreviation;
    //  キャラクター画像 全身
    [SerializeField]
    public List<Sprite> imagesA;
    //  キャラクター画像 四角
    [SerializeField]
    public List<Sprite> imagesB;
    //  キャラクター画像 バストアップ
    [SerializeField]
    public List<Sprite> imagesC;
    //  キャラクター画像 目線
    [SerializeField]
    public List<Sprite> imagesD;
    //　HP倍率
    [SerializeField]
    public double hpRate;
    //　MP倍率
    [SerializeField]
    public double mpRate;
    //　STR倍率
    [SerializeField]
    public double strRate;
    //　VIT倍率
    [SerializeField]
    public double vitRate;
    //　DEX倍率
    [SerializeField]
    public double dexRate;
    //　AGI倍率
    [SerializeField]
    public double agiRate;
    //　INT倍率
    [SerializeField]
    public double intRate;
    //　MND倍率
    [SerializeField]
    public double mndRate;
    // ステータス評価 0:HP ~ 7:MND
    [SerializeField]
    public List<string> statusRank;
    // 装備可能武器種別
    [SerializeField]
    public List<Constants.WeaponCategory> equippableWeapons;
    //　説明
    [SerializeField, Multiline(4)]
    public string description;
    // Exスキル
    [SerializeField]
    public Skill exSkill;
    // 習得スキル
    [SerializeField]
    public List<Skill> LearnSkills;
}