using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Items/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items = new List<Item>(); // �A�C�e���̃��X�g

    private Dictionary<string, Item> itemDictionary; // �A�C�e��ID�ƃA�C�e���I�u�W�F�N�g�̑Ή��֌W���Ǘ�����f�B�N�V���i��

    private void OnEnable()
    {
        // �A�C�e�����X�g���f�B�N�V���i���ɕϊ�
        itemDictionary = new Dictionary<string, Item>();
        foreach (var item in items)
        {
            itemDictionary.Add(item.ID, item);
        }
    }

    // ID�ŃA�C�e�����������郁�\�b�h
    public Item GetItemByID(string id)
    {
        if (itemDictionary.ContainsKey(id))
        {
            return itemDictionary[id];
        }
        else
        {
            Debug.LogWarning("Item with ID " + id + " not found.");
            return null;
        }
    }
}