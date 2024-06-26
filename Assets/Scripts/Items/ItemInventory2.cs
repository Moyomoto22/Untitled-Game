using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory2 : MonoBehaviour
{
    public static ItemInventory2 Instance { get; private set; }

    [SerializeField] private List<Item> originalItems;
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
            copy.EquippedAllyID = 0;
            items.Add(copy);
        }
    }

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public void AddCopyItem(Item item)
    {
        Item copy = CopyItem(item);
        AddItem(copy);
    }

    public bool RemoveItem(Item item)
    {
        return items.Remove(item);
    }

    private void RemoveAllItems()
    {
        if (items != null)
        {
            items.Clear();
        }
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

    /// <summary>
    /// セーブデータからアイテム一覧を取得する
    /// </summary>
    public void GetItemsFromSavedData(List<Item> loadedData)
    {
        RemoveAllItems();

        foreach (Item item in loadedData)
        {
            //AddCopyItem(item);
            AddItem(item);
        }
    }

    // IDからアイテムを取得し、取得したアイテムのコピーを返す
    public Item GetItemByID(string id)
    {
        foreach (Item originalItem in originalItems)
        {
            if (originalItem.ID == id)
            {
                Item copy = CopyItem(originalItem);
                return copy;
            }
        }
        Debug.LogWarning("Item with ID: " + id + " not found.");
        return null;
    }
}