using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public List<Item> itemInventory = new List<Item>();

    /// <summary>
    /// �C���x���g���ɃA�C�e����ǉ�����
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        Item newItem = Instantiate(item); // �N���[�����쐬
        itemInventory.Add(newItem);
    }

    /// <summary>
    /// �C���x���g������A�C�e�����폜����
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(Item item)
    {
        Item itemToRemove = itemInventory.Find(i => i == item);
        if (itemToRemove != null)
        {
            itemInventory.Remove(itemToRemove);
            Destroy(itemToRemove); // �C���X�^���X��j��
        }
        else
        {
            Debug.LogWarning("Item not found in inventory: " + item.itemName);
        }
    }

    /// <summary>
    /// �w�肳�ꂽBaseItem�̃C���f�b�N�X���擾����
    /// </summary>
    /// <param name="item">��������A�C�e��</param>
    /// <returns>�A�C�e���̃C���f�b�N�X�B������Ȃ��ꍇ��-1�B</returns>
    public int GetItemIndex(Item item)
    {
        return itemInventory.IndexOf(item);
    }

    /// <summary>
    /// �w�肳�ꂽ�C���f�b�N�X�̃A�C�e���ɑ������t���O�𗧂Ă�
    /// </summary>
    /// <param name="index">�������t���O�𗧂Ă�A�C�e���̃C���f�b�N�X</param>
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