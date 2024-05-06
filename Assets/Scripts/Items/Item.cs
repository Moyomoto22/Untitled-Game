using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
//[CreateAssetMenu(fileName = "Class", menuName = "CreateClass")]
public class Item : ScriptableObject
{
    // ID
    [SerializeField]
    public string ID;
    // 種別
    [SerializeField]
    public Constants.ItemCategory itemCategory;
    //　アイテム名
    [SerializeField]
    public string itemName;
    //　使用可否
    [SerializeField]
    public bool usable;
    //　売却可否
    [SerializeField]
    public bool sellable;
    //　説明
    [SerializeField, Multiline(6)]
    public string description;
    //　価格
    [SerializeField]
    public int value;
    //　レアリティ
    [SerializeField]
    public Constants.Rarity rarity;
    //　アイコン
    [SerializeField]
    public Sprite iconImage;
    // 装備中メンバーID
    [SerializeField]
    public int equippedAllyID;
    // 対象 0:なし 1:自分 2:味方 3:敵  
    public int target;
    // 対象が全体か
    public bool isTargetAll;
    // 効果
    public List<Constants.EffectType> effects;
    // 効果量
    public int baseValue;
    // 使用者
    public CharacterStatus User;
    // 対象
    public CharacterStatus Objective;

    public void SetEquippedAllyID(int characterID)
    {
        equippedAllyID = characterID;
    }

    public async UniTask<bool> applyEffect()
    {
        bool result = true;
        foreach (var effectType in effects)
        {
            Effect effect = Effect.Instance;
            effect.Initialize(null, this);
            result = await effect.CallEffect(effectType, User, Objective);
        }
        return result;
    }
}