using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffects : MonoBehaviour
{
    /// <summary>
    /// Serializeされた使用時効果の名称から、効果を取得⇒処理を行う
    /// 処理はリターンコードを返す
    /// 0：正常終了
    /// 1：エラー
    /// </summary>
    /// <param name="effectName"></param>
    /// <param name="inventry"></param>
    /// <param name="targetStatus"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void EffectManager(Constants.EffectName effectName, ItemInventory inventory, Ally targetStatus, int value, string ID)
    {

        int returnCode = 0;

        switch (effectName)
        {
            case Constants.EffectName.HealHP:
                returnCode = HealHPByConstant(targetStatus, value);
                break;
            case Constants.EffectName.HealMP:
                returnCode = HealMP(targetStatus, value);
                break;
            case Constants.EffectName.GainTP:
                //HealHP(targetStatus, value);
                break;
            default:
                break;
        }

        if (returnCode == 0)
        {
            RemoveItem(inventory, ID);
        }
    }
    
    /// <summary>
    /// HP回復(定数)
    /// </summary>
    /// <param name="targetStatus"></param>
    /// <param name="healAmount"></param>
    /// <returns></returns>
    public static int HealHPByConstant(Ally targetStatus, int healAmount)
    {
        if (targetStatus.HP < targetStatus.MaxHp)
        {
            targetStatus.HP += healAmount;

            if (targetStatus.HP > targetStatus.MaxHp)
            {
                targetStatus.HP = targetStatus.MaxHp;
            }
            return 0;
        }

        else return 1;
    }

    

    public static int HealMP(Ally targetStatus, int healAmount)
    {
        if (targetStatus.MP < targetStatus.MaxMp)
        {
            targetStatus.MP += healAmount;

            if (targetStatus.MP > targetStatus.MaxMp)
            {
                targetStatus.MP = targetStatus.MaxMp;
            }
            return 0;
        }

        else return 1;
    }

    private static void RemoveItem(ItemInventory inventory, string ID)
    {
        // 指定したIDを持つ要素を1つだけ削除
        Item itemToRemove = inventory.itemInventory.Find(a => a.ID == ID);
        if (itemToRemove != null)
        {
            if (itemToRemove.ID == ID)
            {
                inventory.itemInventory.Remove(itemToRemove);
            }
            else
            {
                Console.WriteLine("指定したIDを持つ要素が見つかりません。");
            }
        }
    }
}
