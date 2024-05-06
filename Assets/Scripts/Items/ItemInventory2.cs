using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory2 : MonoBehaviour
{
    public static ItemInventory2 Instance { get; private set; }

    public List<Item> items = new List<Item>();

    void Awake()
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

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public bool RemoveItem(Item item)
    {
        return items.Remove(item);
    }
}