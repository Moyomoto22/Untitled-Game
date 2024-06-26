using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Item : ScriptableObject
{
    // ID
    [SerializeField] private string id;
    public string ID
    {
        get { return id; }
        set { id = value; }
    }

    // 種別
    [SerializeField] private Constants.ItemCategory itemCategory;
    public Constants.ItemCategory ItemCategory
    {
        get { return itemCategory; }
        set { itemCategory = value; }
    }

    // アイテム名
    [SerializeField] private string itemName;
    public string ItemName
    {
        get { return itemName; }
        set { itemName = value; }
    }

    // 使用可否
    [SerializeField] private bool usable;
    public bool Usable
    {
        get { return usable; }
        set { usable = value; }
    }

    // 売却可否
    [SerializeField] private bool sellable;
    public bool Sellable
    {
        get { return sellable; }
        set { sellable = value; }
    }

    // 説明
    [SerializeField, Multiline(6)] private string description;
    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    // 価格
    [SerializeField] private int value;
    public int Value
    {
        get { return value; }
        set { this.value = value; }
    }

    // レアリティ
    [SerializeField] private Constants.Rarity rarity;
    public Constants.Rarity Rarity
    {
        get { return rarity; }
        set { rarity = value; }
    }

    // アイコン
    [SerializeField] private Sprite iconImage;
    public Sprite IconImage
    {
        get { return iconImage; }
        set { iconImage = value; }
    }

    // 装備中メンバーID
    [SerializeField] private int equippedAllyID;
    public int EquippedAllyID
    {
        get { return equippedAllyID; }
        set { equippedAllyID = value; }
    }

    // 装備中部位インデックス 0:右手 1:左手 2:頭 3:胴 4:装飾品1 5:装飾品2
    [SerializeField] private int equippedPart;
    public int EquippedPart
    {
        get { return equippedPart; }
        set { equippedPart = value; }
    }

    // 対象 0:なし 1:自分 2:味方 3:敵  
    [SerializeField] private Constants.TargetType target;
    public Constants.TargetType Target
    {
        get { return target; }
        set { target = value; }
    }

    // 対象が全体か
    [SerializeField] private bool isTargetAll;
    public bool IsTargetAll
    {
        get { return isTargetAll; }
        set { isTargetAll = value; }
    }

    // 効果
    [SerializeField] private List<Constants.ActiveEffectType> effects;
    public List<Constants.ActiveEffectType> Effects
    {
        get { return effects; }
        set { effects = value; }
    }

    // 効果量
    [SerializeField] private int baseValue;
    public int BaseValue
    {
        get { return baseValue; }
        set { baseValue = value; }
    }

    // 使用者
    private Character user;
    public Character User
    {
        get { return user; }
        set { user = value; }
    }

    // 対象
    private Character objective;
    public Character Objective
    {
        get { return objective; }
        set { objective = value; }
    }

    public void SetEquippedAllyID(int characterID)
    {
        equippedAllyID = characterID;
    }

    public async UniTask<bool> ApplyEffect()
    {
        bool result = true;
        foreach (var effectType in effects)
        {
            ActiveEffect effect = ActiveEffect.Instance;
            effect.Initialize(null, this);
            result = await effect.CallEffect(effectType, User, Objective);
        }
        return result;
    }
}
