using SpriteGlow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// �퓬��� - �A�C�e���I���T�u���j���[�R���g���[��
/// </summary>
public class SelectItemSubMenuController : MonoBehaviour
{
    private EventSystem eventSystems;

    private BattleCommandManager battleCommandManager;

    public GameObject content;
    public GameObject button;

    public Image detailImage;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI description;

    public GameObject targetSelectMenu;
    private GameObject targetSelectMenuInstance;

    private CharacterStatus turnCharacter;
    private List<Item> items;

    void Awake()
    {
        eventSystems = FindObjectOfType<EventSystem>();
        battleCommandManager = FindObjectOfType<BattleCommandManager>();

        SetItemDetailInfo(null);

        /// �g�p�\�A�C�e�����ꗗ�ɃZ�b�g
        turnCharacter = TurnCharacter.Instance.CurrentCharacter;
        items = ItemInventory2.Instance.items.Where(item => item.itemCategory == Constants.ItemCategory.Consumable).ToList();
        SetItems();
    }

    /// <summary>
    /// �A�C�e���{�^�����ꗗ�ɃZ�b�g
    /// </summary>
    private void SetItems()
    {
        foreach (Item item in items)
        {
            GameObject obj = Instantiate(button, content.transform, false);    // �ꗗ�ɕ\������{�^���̃x�[�X���C���X�^���X����
            var comp = obj.GetComponent<ItemComponent>();                      // �{�^���ɕR�Â��X�L�������i�[����R���|�[�l���g
            var newButton = obj.transform.GetChild(0).gameObject;              // �{�^���{��
            var amount = items.Where(i => i.ID == item.ID).ToList().Count;     // �A�C�e��������

            comp.icon.sprite = item.iconImage;                                 // �A�C�R��
            comp.itemName.text = item.itemName;                                // �A�C�e������
            comp.amount.text = amount.ToString();                              // ������
            AddSelectOrDeselectActionToButtons(newButton, item);               // �I���E�I���������A�N�V�����ݒ�
            AddOnClickActionToItemButton(newButton, item);                     // �������A�N�V�����ݒ�
        }
        // �ꗗ�̈�ԏ��I��
        SelectButton(0);
    }

    /// <summary>
    /// �{�^����I����Ԃɂ���
    /// </summary>
    /// <param name="number"></param>
    public void SelectButton(int number = 0)
    {
        if (eventSystems != null && content.transform.childCount > 0)
        {
            var buttonToSelect = content.transform.GetChild(0).GetChild(0).gameObject;
            Debug.Log(buttonToSelect.name);
            eventSystems.SetSelectedGameObject(buttonToSelect);
        }
    }

    /// </summary>
    /// �{�^���������̓����ݒ肷��
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="item"></param>
    private void AddOnClickActionToItemButton(GameObject obj, Item item)
    {
        var button = obj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnClickActionToItemButton(item));
        }
    }

    /// <summary>
    /// �{�^���������A�N�V�������{�^���ɐݒ�
    /// </summary>
    /// <param name="item"></param>
    private void OnClickActionToItemButton(Item item)
    {
        battleCommandManager.selectedItem = item;
        // �X�L���̑Ώۂ��G���P�̂̎�
        if (item.target == 3 && !item.isTargetAll)
        {
            battleCommandManager.DisplayEnemyTargetSubMenu(2);
        }
        // �Ώۂ��������P�̂̎�
        else if (item.target == 2 && !item.isTargetAll)
        {
            battleCommandManager.DisplayAllyTargetSubMenu(2);
        }

    }

    /// <summary>
    /// �{�^���I���E�I���������̓����ݒ�
    /// </summary>
    /// <param name="button"></param>
    /// <param name="item"></param>
    public void AddSelectOrDeselectActionToButtons(GameObject button, Item item)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Select; // Select�C�x���g�����b�X��
        entry.callback.AddListener((data) =>
        {
            // �A�C�e���ڍׂ��ڍח��ɕ\��
            SetItemDetailInfo(item);
        });

        // �G���g�����g���K�[���X�g�ɒǉ�
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// �ڍח��ɃX�L���̏ڍׂ�\������
    /// </summary>
    /// <param name="item"></param>
    private void SetItemDetailInfo(Item item)
    {
        if (item != null)
        {
            detailImage.enabled = true;
            detailImage.sprite = item.iconImage;
            detailName.text = item.itemName;
            description.text = item.description;
        }
        else
        {
            detailImage.enabled = false;
            detailName.text = "";
            description.text = "";
        }
    }
}