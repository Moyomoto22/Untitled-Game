using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemMenuSubWindowController : MonoBehaviour
{
    private EventSystem eventSystem;

    public ItemMenuController itemMenuController;

    public List<GameObject> buttons;

    public List<GameObject> HPGauges;
    public List<GameObject> MPGauges;

    public Item item;

    private void Awake()
    {
        SetAllyStatuses();
        SelectButton();
    }

    private void OnDestroy()
    {
        itemMenuController.SelectButton(itemMenuController.lastSelectButtonIndex);
        itemMenuController.ToggleButtonsInteractable(true);
    }

    private void SetAllyStatuses()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        List<AllyStatus> allies = PartyMembers.Instance.GetAllies();

        for (int i = 0; i < allies.Count; i++)
        {
            var ally = allies[i];

            ally.HPGauge = HPGauges[i];                                           // HP�Q�[�W
            ally.MPGauge = MPGauges[i];                                           // MP�Q�[�W

            GaugeManager hpGaugeManager = HPGauges[i].GetComponent<GaugeManager>();     // HP�Q�[�W�Ǘ��N���X
            GaugeManager mpGaugeManager = MPGauges[i].GetComponent<GaugeManager>();     // MP�Q�[�W�Ǘ��N���X

            hpGaugeManager.maxValueText.text = ally.maxHp2.ToString();            // �ő�HP�e�L�X�g
            hpGaugeManager.currentValueText.text = ally.hp.ToString();            // ����HP�e�L�X�g
            mpGaugeManager.maxValueText.text = ally.maxMp2.ToString();            // �ő�MP�e�L�X�g
            mpGaugeManager.currentValueText.text = ally.mp.ToString();            // ����MP�e�L�X�g

            hpGaugeManager.updateGaugeByText();
            mpGaugeManager.updateGaugeByText();
        }
    }

    public void SelectButton(int number = 0)
    {
        if (eventSystem != null)
        {
            var buttonToSelect = buttons[number];
            eventSystem.SetSelectedGameObject(buttonToSelect);
        }
    }

    public async void OnPressButton(int index)
    {
        await ItemToAlly(index);
    }

    /// <summary>
    /// �����փA�C�e�����g�p����
    /// </summary>
    /// <param name="index">�g�p�҂̃p�[�e�B���C���f�b�N�X</param>
    /// <returns></returns>
    private async UniTask ItemToAlly(int index)
    {
        if (item != null)
        {
            // �Ώۂ̒��Ԃ̃C���X�^���X���擾
            var target = PartyMembers.Instance.GetAllyByIndex(index);

            if (target != null)
            {
                ToggleButtonsInteractable(false);
                // �A�C�e�����g�p
                bool result = await target.UseItem(item, target);
                
                if (result)
                {
                    await itemMenuController.DestroyButtons();
                    itemMenuController.SetItems();
                    itemMenuController.ToggleButtonsInteractable(false);
                    // �g�p���A�C�e���������Ȃ�����C���X�^���X�j��
                    if (GetItemAmount() <= 0)
                    {
                        Destroy(gameObject); 
                    }
                }
                ToggleButtonsInteractable(true);
                SelectButton(index);
            }
        }
    }

    private int GetItemAmount()
    {
        var items = ItemInventory2.Instance.items;
        var amount = items.Where(i => i.ID == item.ID).ToList().Count;

        return amount;
    }

    /// <summary>
    /// �{�^����Interactable��؂�ւ���
    /// </summary>
    /// <param name="interactable">�L��/����</param>
    public void ToggleButtonsInteractable(bool interactable)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i].GetComponent<Button>();
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }
}
