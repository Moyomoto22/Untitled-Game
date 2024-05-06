using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    public List<Item> AllItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ID �ɂ���ăX�L�����������郁�\�b�h
    public Item GetItemByID(string id)
    {
        foreach (Item item in AllItems)
        {
            if (item.ID == id)
            {
                return item;
            }
        }
        Debug.LogWarning("Item with ID: " + id + " not found.");
        return null;
    }
}