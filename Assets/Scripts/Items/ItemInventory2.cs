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
    /// �A�C�e���̃f�B�[�v�R�s�[���쐬����
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public Item CopyItem(Item original)
    {
        // Reflection���g�p���Đ��m�ȃ����^�C���^���擾���A�V�����C���X�^���X���쐬
        Type itemType = original.GetType();
        Item newInstance = (Item)ScriptableObject.CreateInstance(itemType);

        // JSON���g�p���ăf�[�^���R�s�[
        string json = JsonUtility.ToJson(original);
        JsonUtility.FromJsonOverwrite(json, newInstance);

        return newInstance;
    }
}