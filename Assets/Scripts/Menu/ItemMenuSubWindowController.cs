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
    public SkillMenuController skillMenuController;

    public List<GameObject> buttons;

    public List<GameObject> HPGauges;
    public List<GameObject> MPGauges;

    public Item item;

    public int userIndex;
    public Skill skill;

    private void Awake()
    {
        SetAllyes();
        SelectButton();
    }

    private void OnDestroy()
    {
        itemMenuController.SelectButton(itemMenuController.lastSelectButtonIndex);
        itemMenuController.setButtonFillAmount(itemMenuController.lastSelectButtonIndex);
        itemMenuController.ToggleButtonsInteractable(true);
    }

    private void SetAllyes()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        List<Ally> allies = PartyMembers.Instance.GetAllies();

        for (int i = 0; i < allies.Count; i++)
        {
            var ally = allies[i];

            ally.HPGauge = HPGauges[i];                                           // HPゲージ
            ally.MPGauge = MPGauges[i];                                           // MPゲージ

            GaugeManager hpGaugeManager = HPGauges[i].GetComponent<GaugeManager>();     // HPゲージ管理クラス
            GaugeManager mpGaugeManager = MPGauges[i].GetComponent<GaugeManager>();     // MPゲージ管理クラス

            hpGaugeManager.maxValueText.text = ally.MaxHp.ToString();            // 最大HPテキスト
            hpGaugeManager.currentValueText.text = ally.HP.ToString();            // 現在HPテキスト
            mpGaugeManager.maxValueText.text = ally.MaxMp.ToString();            // 最大MPテキスト
            mpGaugeManager.currentValueText.text = ally.MP.ToString();            // 現在MPテキスト

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
        if (item != null)
        {
            await ItemToAlly(index);
        }
        else if (skill != null)
        {
            await SkillToAlly(index);
        }
    }

    /// <summary>
    /// 味方へアイテムを使用する
    /// </summary>
    /// <param name="index">使用者のパーティ内インデックス</param>
    /// <returns></returns>
    private async UniTask ItemToAlly(int index)
    {
        if (item != null)
        {
            // 対象の仲間のインスタンスを取得
            var target = PartyMembers.Instance.GetAllyByIndex(index);

            if (target != null)
            {
                ToggleButtonsInteractable(false);
                // アイテムを使用
                bool result = await target.UseItem(item, target);
                
                if (result)
                {
                    await itemMenuController.DestroyButtons();
                    await itemMenuController.SetItems();
                    itemMenuController.ToggleButtonsInteractable(false);
                    // 使用中アイテムが無くなったらインスタンス破棄
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

    /// <summary>
    /// 味方へアイテムを使用する
    /// </summary>
    /// <param name="index">使用者のパーティ内インデックス</param>
    /// <returns></returns>
    private async UniTask SkillToAlly(int index)
    {
        if (skill != null)
        {
            // 対象の仲間のインスタンスを取得
            var user = PartyMembers.Instance.GetAllyByIndex(userIndex);
            var target = PartyMembers.Instance.GetAllyByIndex(index);

            if (target != null)
            {
                ToggleButtonsInteractable(false);
                // スキルを使用
                bool result = await user.UseSkill(skill, target);

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
    /// ボタンのInteractableを切り替える
    /// </summary>
    /// <param name="interactable">有効/無効</param>
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
