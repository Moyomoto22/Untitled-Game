using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Items/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items = new List<Item>(); // アイテムのリスト

    private Dictionary<string, Item> itemDictionary; // アイテムIDとアイテムオブジェクトの対応関係を管理するディクショナリ

    private void OnEnable()
    {
        // アイテムリストをディクショナリに変換
        itemDictionary = new Dictionary<string, Item>();
        foreach (var item in items)
        {
            itemDictionary.Add(item.ID, item);
        }
    }

    // IDでアイテムを検索するメソッド
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