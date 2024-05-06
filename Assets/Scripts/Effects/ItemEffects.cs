using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffects : MonoBehaviour
{
    /// <summary>
    /// Serialize���ꂽ�g�p�����ʂ̖��̂���A���ʂ��擾�ˏ������s��
    /// �����̓��^�[���R�[�h��Ԃ�
    /// 0�F����I��
    /// 1�F�G���[
    /// </summary>
    /// <param name="effectName"></param>
    /// <param name="inventry"></param>
    /// <param name="targetStatus"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void EffectManager(Constants.EffectName effectName, ItemInventory inventory, AllyStatus targetStatus, int value, string ID)
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
    /// HP��(�萔)
    /// </summary>
    /// <param name="targetStatus"></param>
    /// <param name="healAmount"></param>
    /// <returns></returns>
    public static int HealHPByConstant(AllyStatus targetStatus, int healAmount)
    {
        if (targetStatus.hp < targetStatus.maxHp)
        {
            targetStatus.hp = targetStatus.hp + healAmount;

            if (targetStatus.hp > targetStatus.maxHp)
            {
                targetStatus.hp = targetStatus.maxHp;
            }
            return 0;
        }

        else return 1;
    }

    

    public static int HealMP(AllyStatus targetStatus, int healAmount)
    {
        if (targetStatus.mp < targetStatus.maxMp)
        {
            targetStatus.mp = targetStatus.mp + healAmount;

            if (targetStatus.mp > targetStatus.maxMp)
            {
                targetStatus.mp = targetStatus.maxMp;
            }
            return 0;
        }

        else return 1;
    }

    private static void RemoveItem(ItemInventory inventory, string ID)
    {
        // �w�肵��ID�����v�f��1�����폜
        Item itemToRemove = inventory.itemInventory.Find(a => a.ID == ID);
        if (itemToRemove != null)
        {
            if (itemToRemove.ID == ID)
            {
                inventory.itemInventory.Remove(itemToRemove);
            }
            else
            {
                Console.WriteLine("�w�肵��ID�����v�f��������܂���B");
            }
        }
    }
}
