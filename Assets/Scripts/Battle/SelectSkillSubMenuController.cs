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
/// 戦闘画面 - スキル選択サブメニューコントローラ
/// </summary>
public class SelectSkillSubMenuController : MonoBehaviour
{
    private List<string> skillCategoryNames = new List<string>() { "魔法", "奇跡", "アーツ", "アクティブ" };
    private int currentCategoryIndex = 0;

    private EventSystem eventSystems;

    private GameObject targetSelectMenuInstance;

    private BattleCommandManager battleCommandManager;

    public TextMeshProUGUI currentCategoryText;
    public TextMeshProUGUI nextCategoryText;
    public TextMeshProUGUI previousCategoryText;

    public GameObject content;
    public GameObject button;

    public Image detailImage;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailCost;
    public TextMeshProUGUI detailPoint;
    public TextMeshProUGUI description;
    public GameObject attributes;
    public GameObject classes;

    public GameObject targetSelectMenu;

    private CharacterStatus turnCharacter;
    private List<Skill> skills;

    void Awake()
    {
        eventSystems = FindObjectOfType<EventSystem>();
        battleCommandManager = FindObjectOfType<BattleCommandManager>();

        //選択中カテゴリを初期化
        UpdateCategoryTexts();
        
        // ターンキャラクターの使用可能スキルを一覧にセット
        turnCharacter = TurnCharacter.Instance.CurrentCharacter;
        skills = turnCharacter.learnedSkills;
        SetSkills();
    }

    /// <summary>
    /// 選択中カテゴリのスキルを一覧にセットする
    /// </summary>
    public void SetSkills()
    {
        switch (currentCategoryIndex)
        {
            case 0:
                // 魔法
                SetMagicsOrMiracles(true);
                break;
            case 1:
                // 奇跡
                SetMagicsOrMiracles(false);
                break;
            case 2:
                // アーツ
                SetArts();
                break;
            case 3:
                // アクティブスキル
                SetActives();
                break;
        }
        // 一覧の一番上を選択
        SelectButton(0);
    }

    /// <summary>
    /// 魔法/奇跡を一覧にセット
    /// </summary>
    /// <param name="isMagics">魔法か</param>
    private void SetMagicsOrMiracles(bool isMagics)
    {
        List<Skill> ms = new List<Skill>();

        if (isMagics)
        {
            ms = skills.Where(x => x.skillCategory == Constants.SkillCategory.Magic).ToList();
        }
        else
        {
            ms = skills.Where(x => x.skillCategory == Constants.SkillCategory.Miracle).ToList();
        }

        foreach (MagicMiracle m in ms)
        {
            GameObject obj = Instantiate(button, content.transform, false); // 一覧に表示するボタンのベースをインスタンス生成
            var comp = obj.GetComponent<SkillComponent>();                  // ボタンに紐づくスキル情報を格納するコンポーネント
            var newButton = obj.transform.GetChild(0).gameObject;           // ボタン本体

            comp.icon.sprite = m.icon;                                      // アイコン
            comp.skillName.text = m.skillName;                              // スキル名称
            comp.cost.text = "MP";                                          // コストの種別
            comp.point.text = m.MPCost.ToString();                          // 消費MP
            AddSelectOrDeselectActionToButtons(newButton, m);               // ボタンの選択・選択解除時のアクションを設定

            // スキルが使用可能か判定
            if (turnCharacter.CanUseSkill(m))
            {
                // ボタン押下時のアクションを追加
                AddOnClickActionToSkillButton(newButton, m);
            }
            else
            {
                // ボタンタイトルをグレーアウト
                comp.skillName.color = CommonController.GetColor(Constants.darkGray);
            }


        }
    }

    /// <summary>
    /// アーツを一覧にセット
    /// </summary>
    private void SetArts()
    {
        var actives = skills.Where(x => x.skillCategory == Constants.SkillCategory.Arts).ToList();

        foreach (Arts art in actives)
        {
            GameObject obj = Instantiate(button, content.transform, false);
            var comp = obj.GetComponent<SkillComponent>();
            var newButton = obj.transform.GetChild(0).gameObject;

            comp.icon.sprite = art.icon;
            comp.skillName.text = art.skillName;
            comp.cost.text = "TP";
            comp.point.text = art.TPCost.ToString();
            AddSelectOrDeselectActionToButtons(newButton, art);
            if (turnCharacter.CanUseSkill(art))
            {
                AddOnClickActionToSkillButton(newButton, art);
            }
            else
            {
                comp.skillName.color = Color.gray;
            }
        }
    }

    /// <summary>
    /// アクティブスキルを一覧にセット
    /// </summary>
    private void SetActives()
    {
        var actives = skills.Where(x => x.skillCategory == Constants.SkillCategory.Active).ToList();

        foreach (ActiveSkill act in actives)
        {
            GameObject obj = Instantiate(button, content.transform, false);
            var comp = obj.GetComponent<SkillComponent>();
            var newButton = obj.transform.GetChild(0).gameObject;

            comp.icon.sprite = act.icon;
            comp.skillName.text = act.skillName;
            comp.cost.text = "リキャスト";
            comp.point.text = act.recastTurn.ToString();
            AddOnClickActionToSkillButton(newButton, act);
            AddSelectOrDeselectActionToButtons(newButton, act);
        }
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

    /// <summary>
    /// ボタン押下時の動作を設定する
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="skill"></param>
    private void AddOnClickActionToSkillButton(GameObject obj, Skill skill)
    {
        var button = obj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnClickActionToSkillButton(skill));
        }
    }

    /// <summary>
    /// ボタン押下時アクションをボタンに設定
    /// </summary>
    /// <param name="skill"></param>
    private void OnClickActionToSkillButton(Skill skill)
    {
        battleCommandManager.selectedSkill = skill;
        // スキルの対象が敵かつ単体の時
        if (skill.target == 3 && !skill.isTargetAll)
        {  
            battleCommandManager.DisplayEnemyTargetSubMenu(1);
        }
        // 対象が味方かつ単体の時
        else if (skill.target == 2 && !skill.isTargetAll)
        {
            battleCommandManager.DisplayAllyTargetSubMenu(1);
        }

    }

    /// <summary>
    /// ボタン選択・選択解除時の動作を設定
    /// </summary>
    /// <param name="button"></param>
    /// <param name="skill"></param>
    public void AddSelectOrDeselectActionToButtons(GameObject button, Skill skill)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Select; // Selectイベントをリッスン
        entry.callback.AddListener((data) =>
        {
            // スキル詳細を表示
            SetSkillDetailInfo(skill);
        });
        // エントリをトリガーリストに追加
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 詳細欄にスキルの詳細を表示する
    /// </summary>
    /// <param name="skill"></param>
    private void SetSkillDetailInfo(Skill skill)
    {
        if (skill != null)
        {
            detailImage.enabled = true;
            detailImage.sprite = skill.icon;
            detailName.text = skill.skillName;
            description.text = skill.description;
            if (skill is MagicMiracle)
            {
                var magic = skill as MagicMiracle;
                detailCost.text = "消費MP";
                detailPoint.text = magic.MPCost.ToString();
            }
            else if (skill is Arts)
            {
                var arts = skill as Arts;
                detailCost.text = "消費TP";
                detailPoint.text = arts.TPCost.ToString();
            }
            else if (skill is ActiveSkill)
            {
                var act = skill as ActiveSkill;
                detailCost.text = "リキャスト";
                detailPoint.text = act.recastTurn.ToString();
            }
        }
    }

    /// <summary>
    /// スキル詳細をクリアする
    /// </summary>
    private void ClearSkillDetailInfo()
    {
        detailImage.enabled = false;
        detailName.text = "";
        description.text = "";
        detailCost.text = "";
        detailPoint.text = "";
    }

    /// <summary>
    /// 一覧内のボタンをすべて破棄する
    /// </summary>
    /// <returns></returns>
    private async UniTask DestroyButtons()
    {
        // アイテム一覧内のオブジェクトを削除
        int childCount = content.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = content.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        await UniTask.Delay(10);
    }

    /// <summary>
    /// スキルカテゴリ切り替え - 次ページ
    /// </summary>
    public async void NextCategory()
    {
        currentCategoryIndex = (currentCategoryIndex + 1) % skillCategoryNames.Count;
        UpdateCategoryTexts();

        await DestroyButtons();
        SetSkills();
    }

    /// <summary>
    /// スキルカテゴリ切り替え - 前ページ
    /// </summary>
    public async void PreviousCategory()
    {
        currentCategoryIndex = (currentCategoryIndex - 1 + skillCategoryNames.Count) % skillCategoryNames.Count;
        UpdateCategoryTexts();

        await DestroyButtons();
        SetSkills();
    }

    /// <summary>
    /// スキルカテゴリ切り替え
    /// </summary>
    private void UpdateCategoryTexts()
    {
        currentCategoryText.text = skillCategoryNames[currentCategoryIndex];
        nextCategoryText.text = skillCategoryNames[(currentCategoryIndex + 1) % skillCategoryNames.Count];
        previousCategoryText.text = skillCategoryNames[(currentCategoryIndex - 1 + skillCategoryNames.Count) % skillCategoryNames.Count];

        ClearSkillDetailInfo();
    }
}