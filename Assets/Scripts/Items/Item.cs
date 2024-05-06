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
    // ���
    [SerializeField]
    public Constants.ItemCategory itemCategory;
    //�@�A�C�e����
    [SerializeField]
    public string itemName;
    //�@�g�p��
    [SerializeField]
    public bool usable;
    //�@���p��
    [SerializeField]
    public bool sellable;
    //�@����
    [SerializeField, Multiline(6)]
    public string description;
    //�@���i
    [SerializeField]
    public int value;
    //�@���A���e�B
    [SerializeField]
    public Constants.Rarity rarity;
    //�@�A�C�R��
    [SerializeField]
    public Sprite iconImage;
    // �����������o�[ID
    [SerializeField]
    public int equippedAllyID;
    // �Ώ� 0:�Ȃ� 1:���� 2:���� 3:�G  
    public int target;
    // �Ώۂ��S�̂�
    public bool isTargetAll;
    // ����
    public List<Constants.EffectType> effects;
    // ���ʗ�
    public int baseValue;
    // �g�p��
    public CharacterStatus User;
    // �Ώ�
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