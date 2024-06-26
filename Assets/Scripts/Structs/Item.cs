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

    // ���
    [SerializeField] private Constants.ItemCategory itemCategory;
    public Constants.ItemCategory ItemCategory
    {
        get { return itemCategory; }
        set { itemCategory = value; }
    }

    // �A�C�e����
    [SerializeField] private string itemName;
    public string ItemName
    {
        get { return itemName; }
        set { itemName = value; }
    }

    // �g�p��
    [SerializeField] private bool usable;
    public bool Usable
    {
        get { return usable; }
        set { usable = value; }
    }

    // ���p��
    [SerializeField] private bool sellable;
    public bool Sellable
    {
        get { return sellable; }
        set { sellable = value; }
    }

    // ����
    [SerializeField, Multiline(6)] private string description;
    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    // ���i
    [SerializeField] private int value;
    public int Value
    {
        get { return value; }
        set { this.value = value; }
    }

    // ���A���e�B
    [SerializeField] private Constants.Rarity rarity;
    public Constants.Rarity Rarity
    {
        get { return rarity; }
        set { rarity = value; }
    }

    // �A�C�R��
    [SerializeField] private Sprite iconImage;
    public Sprite IconImage
    {
        get { return iconImage; }
        set { iconImage = value; }
    }

    // �����������o�[ID
    [SerializeField] private int equippedAllyID;
    public int EquippedAllyID
    {
        get { return equippedAllyID; }
        set { equippedAllyID = value; }
    }

    // ���������ʃC���f�b�N�X 0:�E�� 1:���� 2:�� 3:�� 4:�����i1 5:�����i2
    [SerializeField] private int equippedPart;
    public int EquippedPart
    {
        get { return equippedPart; }
        set { equippedPart = value; }
    }

    // �Ώ� 0:�Ȃ� 1:���� 2:���� 3:�G  
    [SerializeField] private Constants.TargetType target;
    public Constants.TargetType Target
    {
        get { return target; }
        set { target = value; }
    }

    // �Ώۂ��S�̂�
    [SerializeField] private bool isTargetAll;
    public bool IsTargetAll
    {
        get { return isTargetAll; }
        set { isTargetAll = value; }
    }

    // ����
    [SerializeField] private List<Constants.ActiveEffectType> effects;
    public List<Constants.ActiveEffectType> Effects
    {
        get { return effects; }
        set { effects = value; }
    }

    // ���ʗ�
    [SerializeField] private int baseValue;
    public int BaseValue
    {
        get { return baseValue; }
        set { baseValue = value; }
    }

    // �g�p��
    private Character user;
    public Character User
    {
        get { return user; }
        set { user = value; }
    }

    // �Ώ�
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
