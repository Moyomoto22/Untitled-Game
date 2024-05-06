using SpriteGlow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// 戦闘画面 - 対象選択サブメニューコントローラ
/// </summary>
public class TargetSelectSubMenuController : MonoBehaviour
{
    public GameObject frame;
    public GameObject header;
    public Image image;
    public TextMeshProUGUI skillName;
    public GameObject buttonPrefab;

    public int numberOfButtons;
    public float spacing;

    private EventSystem eventSystems;

    public List<GameObject> createdButtons;

    // Start is called before the first frame update
    void Start()
    {
        //CreateSubMenuSingleRow();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        eventSystems = FindObjectOfType<EventSystem>();
    }

    /// <summary>
    /// 
    /// </summary>
    public void CreateSubMenuSingleRow(int numberOfButtons)
    {
        RectTransform frameRect = frame.GetComponent<RectTransform>();
        RectTransform headerRect = header.GetComponent<RectTransform>();

        float offset = 20.0f;
        float buttonHeight = buttonPrefab.GetComponent<RectTransform>().sizeDelta.y;
        float frameHeight = buttonHeight * numberOfButtons + spacing * (numberOfButtons - 1) + offset * 2; // スペースを追加
        frameRect.sizeDelta = new Vector2(frame.GetComponent<RectTransform>().sizeDelta.x, frameHeight);

        // 最初のボタンのY座標を計算。フレームの中心からの相対位置として設定。
        float firstButtonY = frameHeight / 2.0f - buttonHeight / 2.0f - offset;

        createdButtons = new List<GameObject>();

        for (int i = 0; i < numberOfButtons; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, frame.transform, false);
            // 各ボタンのY座標を計算。最初のボタンから順にスペースを開けて配置。
            float buttonY = firstButtonY - i * (buttonHeight + spacing);
            Vector3 newPosition = new Vector3(0, buttonY, 0);
            newButton.GetComponent<RectTransform>().anchoredPosition = newPosition;

            createdButtons.Add(newButton);
        }

        float frameTop = frameRect.anchoredPosition.y + frameRect.rect.height / 2;
        float headerHeight = headerRect.rect.height;

        // objectAの底辺をobjectBの上辺にスペースを空けて配置
        headerRect.anchoredPosition = new Vector2(headerRect.anchoredPosition.x, frameTop + headerHeight / 2 + 1.0f);
    }

    public void SetHeader(Skill skill, Item item)
    {
        var icon = skill != null ? skill.icon : item.iconImage;
        var name = skill != null ? skill.skillName : item.itemName;
        
        image.sprite = icon;
        skillName.text = name;
    }

    public void SetButtonTitles(List<string> titles)
    {
        if (titles.Count == createdButtons.Count)
        {
            for (int i = 0; i < titles.Count; i++)
            {
                createdButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = titles[i];
            }
        }
    }

    public void SelectButton(int number = 0)
    {
        if (eventSystems != null && createdButtons.Count > 0)
        {
            var buttonToSelect = createdButtons[number].transform.GetChild(0).gameObject;
            Debug.Log(eventSystems.name);
            eventSystems.SetSelectedGameObject(buttonToSelect);
            Debug.Log("select " + createdButtons[number].GetComponentInChildren<TextMeshProUGUI>().text);
        }
    }

    public void AddSelectOrDeselectActionToButtonsSelectEnemy(bool isOnSelect)
    {
        BattleController battleController = FindObjectOfType<BattleController>();
        if (battleController != null)
        {
            for (int i = 0; i < createdButtons.Count; i++)
            {
                // 戦闘不能でないエネミーのリストを取得
                var aliveEnemies = EnemyManager.Instance.GetEnemiesInsExceptKnockedOut();
                // 戦闘不能でないエネミーのリストのバトル中検索用インデックスを取得
                var index = aliveEnemies[i].GetComponent<EnemyBehaviour>().indexInBattle;

                GameObject button = createdButtons[i].transform.GetChild(0).gameObject;
                EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();

                EventTrigger.Entry entry = new EventTrigger.Entry();

                if (isOnSelect)
                {
                    entry.eventID = EventTriggerType.Select; // Selectイベントをリッスン
                    entry.callback.AddListener((data) =>
                    {
                        // 上で取得したインデックスを引数に渡す
                        battleController.StartFlashingGlowEffect(index).Forget();
                    });
                }
                else
                {
                    entry.eventID = EventTriggerType.Deselect; // Deselectイベントをリッスン
                    entry.callback.AddListener((data) =>
                    {
                        // 上で取得したインデックスを引数に渡す
                        battleController.StopFlashingGlowEffect(index);
                    });
                }
                // エントリをトリガーリストに追加
                trigger.triggers.Add(entry);
            }
        }
    }
}
