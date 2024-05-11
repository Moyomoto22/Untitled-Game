using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory2 : MonoBehaviour
{
    public static ItemInventory2 Instance { get; private set; }

    public List<Item> originalItems = new List<Item>();
    public List<Item> items = new List<Item>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ReplicateItems();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ReplicateItems()
    {
        foreach (var oi in originalItems)
        {
            var copy = CopyItem(oi);
            items.Add(copy);
        }
    }

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public bool RemoveItem(Item item)
    {
        return items.Remove(item);
    }

    /// <summary>
    /// アイテムのディープコピーを作成する
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public Item CopyItem(Item original)
    {
        // Reflectionを使用して正確なランタイム型を取得し、新しいインスタンスを作成
        Type itemType = original.GetType();
        Item newInstance = (Item)ScriptableObject.CreateInstance(itemType);

        // JSONを使用してデータをコピー
        string json = JsonUtility.ToJson(original);
        JsonUtility.FromJsonOverwrite(json, newInstance);

        return newInstance;
    }
}