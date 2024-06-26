using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemComponent : MonoBehaviour
{
    public Item item;
    public Image icon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI amount;
    public TextMeshProUGUI equipMark;

    private void Awake()
    {
        if (item != null)
        {
            UpdateEquipMark();
        }
    }

    public void Initialize()
    {
        if (item != null)
        {
            icon.sprite = item.IconImage;
            itemName.text = item.ItemName;
            SetAmount();
            UpdateEquipMark();
        }
    }

    private void SetAmount()
    {
        var items = ItemInventory2.Instance.items;
        var am = items.Where(i => i.ID == item.ID).ToList().Count;
        if (am > 0)
        {
            amount.text = am.ToString();
        }
    }

    // 装備状態の更新
    private void UpdateEquipMark()
    {
        // 装備中であればequipMarkを表示し、特定の色を設定
        if (item.EquippedAllyID > 0)
        {
            equipMark.gameObject.SetActive(true);
            equipMark.color = CommonController.GetCharacterColorByIndex(item.EquippedAllyID - 1);
        }
        else
        {
            equipMark.gameObject.SetActive(false); // 装備されていなければ非表示
        }
    }
}
