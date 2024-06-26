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
    /// HP��(�萔)
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
