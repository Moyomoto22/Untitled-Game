using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public List<Item> itemInventory = new List<Item>();

    /// <summary>
    /// インベントリにアイテムを追加する
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        Item newItem = Instantiate(item); // クローンを作成
        itemInventory.Add(newItem);
    }

    /// <summary>
    /// インベントリからアイテムを削除する
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(Item item)
    {
        Item itemToRemove = itemInventory.Find(i => i == item);
        if (itemToRemove != null)
        {
            itemInventory.Remove(itemToRemove);
            Destroy(itemToRemove); // インスタンスを破棄
        }
        else
        {
            Debug.LogWarning("Item not found in inventory: " + item.itemName);
        }
    }

    /// <summary>
    /// 指定されたBaseItemのインデックスを取得する
    /// </summary>
    /// <param name="item">検索するアイテム</param>
    /// <returns>アイテムのインデックス。見つからない場合は-1。</returns>
    public int GetItemIndex(Item item)
    {
        return itemInventory.IndexOf(item);
    }

    /// <summary>
    /// 指定されたインデックスのアイテムに装備中フラグを立てる
    /// </summary>
    /// <param name="index">装備中フラグを立てるアイテムのインデックス</param>
    public void SetItemUnusable(int index, int characterID)
    {
        if (index >= 0 && index < itemInventory.Count)
        {
            itemInventory[index].equippedAllyID = characterID;
        }
        else
        {
            Debug.LogWarning("Invalid index: " + index);
        }
    }

}