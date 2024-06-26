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
/// 戦闘画面 - アイテム選択サブメニューコントローラ
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

    private Character turnCharacter;
    private List<Item> items;

    void Awake()
    {
        eventSystems = FindObjectOfType<EventSystem>();
        battleCommandManager = FindObjectOfType<BattleCommandManager>();

        SetItemDetailInfo(null);

        /// 使用可能アイテムを一覧にセット
        turnCharacter = TurnCharacter.Instance.CurrentCharacter;
        items = ItemInventory2.Instance.items.Where(item => item.ItemCategory == Constants.ItemCategory.Consumable).ToList();
        SetItems();
    }

    /// <summary>
    /// アイテムボタンを一覧にセット
    /// </summary>
    private void SetItems()
    {
        HashSet<string> processedItemIds = new HashSet<string>();  // 登録済みアイテムのIDを記録するためのHashSet

        var sortedItems = items.OrderBy(x => x.ID).ToList();
        foreach (Item item in sortedItems)
        {
            if (processedItemIds.Contains(item.ID))
            {
                continue;  // このアイテムIDがすでに処理されていれば、次のアイテムへスキップ
            }

            GameObject obj = Instantiate(button, content.transform, false);    // 一覧に表示するボタンのベースをインスタンス生成
            var comp = obj.GetComponent<ItemComponent>();                      // ボタンに紐づくスキル情報を格納するコンポーネント
            var newButton = obj.transform.GetChild(0).gameObject;              // ボタン本体
            var amount = items.Where(i => i.ID == item.ID).ToList().Count;     // アイテム所持数

            comp.icon.sprite = item.IconImage;                                 // アイコン
            comp.itemName.text = item.ItemName;                                // アイテム名称
            comp.amount.text = amount.ToString();                              // 所持数
            AddSelectOrDeselectActionToButtons(newButton, item);               // 選択・選択解除時アクション設定
            AddOnClickActionToItemButton(newButton, item);                     // 押下時アクション設定
            processedItemIds.Add(item.ID);                                     // このアイテムIDを処理済みとして記録
        }
        // 一覧の一番上を選択
        SelectButton(0);
    }

    /// <summary>
    /// ボタンを選択状態にする
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
    /// ボタン押下時の動作を設定する
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
    /// ボタン押下時アクションをボタンに設定
    /// </summary>
    /// <param name="item"></param>
    private void OnClickActionToItemButton(Item item)
    {
        battleCommandManager.selectedItem = item;
        // スキルの対象が敵単体の時
        if (item.Target == Constants.TargetType.Enemy)
        {
            battleCommandManager.DisplayEnemyTargetSubMenu(2);
        }
        // 対象が味方単体の時
        else if (item.Target == Constants.TargetType.Ally)
        {
            battleCommandManager.DisplayAllyTargetSubMenu(2);
        }

    }

    /// <summary>
    /// ボタン選択・選択解除時の動作を設定
    /// </summary>
    /// <param name="button"></param>
    /// <param name="item"></param>
    public void AddSelectOrDeselectActionToButtons(GameObject button, Item item)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Select; // Selectイベントをリッスン
        entry.callback.AddListener((data) =>
        {
            // アイテム詳細を詳細欄に表示
            SetItemDetailInfo(item);
        });

        // エントリをトリガーリストに追加
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 詳細欄にスキルの詳細を表示する
    /// </summary>
    /// <param name="item"></param>
    private void SetItemDetailInfo(Item item)
    {
        if (item != null)
        {
            detailImage.enabled = true;
            detailImage.sprite = item.IconImage;
            detailName.text = item.ItemName;
            description.text = item.Description;
        }
        else
        {
            detailImage.enabled = false;
            detailName.text = "";
            description.text = "";
        }
    }
}