using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// 戦闘画面 - コマンド管理
/// </summary>
public class BattleCommandManager : MonoBehaviour
{
    public BattleController battleController;
    public InputActionManager inputAction;
    public EventSystem eventSystems;
    
    public GameObject targetSelectMenu;
    public GameObject skillSelectMenu;
    public GameObject itemSelectMenu;
    
    public List<GameObject> buttons;
    public GameObject animatedText;

    public Skill selectedSkill;
    public Item selectedItem;

    private int lastSelectedButtonIndex = 0;

    private GameObject targetSelectMenuInstance;
    private GameObject skillSelectMenuInstance;
    private GameObject itemSelectMenuInstance;

    private List<GameObject> displayedWindows;

    private void Start()
    {
    }

    private void Awake()
    {
        displayedWindows = new List<GameObject>();
    }

    /// <summary>
    /// 攻撃ボタン押下
    /// </summary>
    public void OnPressAttackButton()
    {
        // ID:"0000"のスキル(攻撃)を選択
        var attackSkillID = Constants.attackSkillID;
        selectedSkill = SkillManager.Instance.GetSkillByID(attackSkillID);
        lastSelectedButtonIndex = 0;

        // 敵選択サブメニューを表示
        DisplayEnemyTargetSubMenu(0);

        setButtonFillAmount(0);
        ToggleButtonsInteractable(false);
    }

    /// <summary>
    /// スキルボタン押下
    /// </summary>
    public void OnPressSkillButton()
    {
        lastSelectedButtonIndex = 1;

        skillSelectMenuInstance = Instantiate(skillSelectMenu, this.transform, false);
        displayedWindows.Add(skillSelectMenuInstance);

        setButtonFillAmount(1);
        ToggleButtonsInteractable(false);
    }

    /// <summary>
    /// アイテムボタン押下
    /// </summary>
    public void OnPressItemButton()
    {
        lastSelectedButtonIndex = 2;

        itemSelectMenuInstance = Instantiate(itemSelectMenu, this.transform, false);
        displayedWindows.Add(itemSelectMenuInstance);

        setButtonFillAmount(2);
        ToggleButtonsInteractable(false);
    }

    /// <summary>
    /// 防御ボタン押下
    /// </summary>
    public async void OnPressGuardButton()
    {
        // ID:"9999"のスキル(防御)を選択
        var skillID = Constants.guardSkillID;
        selectedSkill = SkillManager.Instance.GetSkillByID(skillID);
        lastSelectedButtonIndex = 3;

        ToggleButtonsInteractable(false);

        // ターンキャラクターのインデックスを取得
        var index = TurnCharacter.Instance.CurrentCharacterIndex;
        // 自分を対象にスキルを使用
        await SkillToSelf(index);

        ToggleButtonsInteractable(true);
    }

    /// <summary>
    /// 逃げるボタン押下
    /// </summary>
    public async void OnPressRunButton()
    {
        if (SceneController.Instance != null)
        {
            EnemyManager.Instance.Initialize();

            await SceneController.Instance.SwitchFieldAndBattleScene("AbandonedFortress1F");
            CommonVariableManager.playerCanMove = true;
        }
    }

    /// <summary>
    /// 敵を対象にするサブメニューを表示する
    /// </summary>
    /// <param name="callerButtonID">呼び出し元ボタンID 0:攻撃, 1:スキル, 2:アイテム</param>
    public void DisplayEnemyTargetSubMenu(int callerButtonID = 0)
    {
        // サブメニューのインスタンス生成
        targetSelectMenuInstance = Instantiate(targetSelectMenu, this.transform, false);
        displayedWindows.Add(targetSelectMenu);

        // インスタンスのローカルスケールを設定
        targetSelectMenuInstance.transform.localScale = new Vector3(1f, 1f, 1.0f);
        targetSelectMenuInstance.transform.localPosition = new Vector3(450f, 180.0f, 0.0f);

        // 戦闘不能でない敵を取得
        var enemies = EnemyManager.Instance.GetAllEnemiesStatus(true);
        var enemyNames = enemies.Select(enemies => enemies.CharacterName).ToList();


        if (EnemyManager.Instance.AliveEnemyIndexes.Count > 0)
        {
            var firstEnemyIndex = EnemyManager.Instance.AliveEnemyIndexes.First();

            // 対象選択サブメニューを作成
            var subMenuController = targetSelectMenuInstance.GetComponent<TargetSelectSubMenuController>();
            subMenuController.CreateSubMenuSingleRow(enemyNames.Count);
            subMenuController.SetButtonTitles(enemyNames);
            subMenuController.SelectButton();
            subMenuController.AddSelectOrDeselectActionToButtonsSelectEnemy(true);
            subMenuController.AddSelectOrDeselectActionToButtonsSelectEnemy(false);
            AddClickActionToButtonsForEnemy(subMenuController, callerButtonID);

            //SelectButton(callerButtonID);

            // スキル選択から呼び出された場合
            if (callerButtonID == 1)
            {
                Destroy(skillSelectMenuInstance);
            }
            else if (callerButtonID == 2)
            {
                Destroy(skillSelectMenuInstance);
            }
            subMenuController.SetHeader(selectedSkill, selectedItem);
            battleController.StartFlashingGlowEffect(firstEnemyIndex);
        }
    }

    /// <summary>
    /// 仲間を対象にするサブメニューを表示する
    /// </summary>
    public void DisplayAllyTargetSubMenu(int callerButtonID = 0)
    {
        // サブメニューのインスタンス生成
        targetSelectMenuInstance = Instantiate(targetSelectMenu, this.transform, false);
        displayedWindows.Add(targetSelectMenu);

        // インスタンスのローカルスケールを設定
        targetSelectMenuInstance.transform.localScale = new Vector3(1.6f, 1.6f, 1.0f);
        targetSelectMenuInstance.transform.localPosition = new Vector3(-50.0f, 140.0f, 0.0f);

        // 仲間を取得
        var allies = PartyMembers.Instance.GetAllies();
        var allyNames = allies.Select(allies => allies.CharacterName).ToList();

        // 対象選択サブメニューを作成
        var subMenuController = targetSelectMenuInstance.GetComponent<TargetSelectSubMenuController>();
        subMenuController.CreateSubMenuSingleRow(allyNames.Count);
        subMenuController.SetButtonTitles(allyNames);
        subMenuController.SelectButton();
        subMenuController.SetHeader(selectedSkill, selectedItem);
        AddClickActionToButtonsForAlly(subMenuController, callerButtonID);

        Destroy(skillSelectMenuInstance);
        Destroy(itemSelectMenuInstance);
    }

    /// <summary>
    /// キャンセル(〇)ボタン押下
    /// </summary>
    /// <param name="context"></param>
    public void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SoundManager.Instance.PlayCancel();

            DestroyAllWindowInstances();
            ToggleButtonsInteractable(true);
            SelectButton(lastSelectedButtonIndex);
        }
    }

    /// <summary>
    /// 次へ(R1)ボタン押下
    /// </summary>
    /// <param name="context"></param>
    public void OnPressNextButton(InputAction.CallbackContext context)
    {
        if (context.performed && skillSelectMenuInstance != null)
        {
            var controller = skillSelectMenuInstance.GetComponent<SelectSkillSubMenuController>();
            // スキルウインドウのカテゴリ切り替え
            controller.NextCategory();
        }
    }

    /// <summary>
    /// 前へ(L1)ボタン押下
    /// </summary>
    /// <param name="context"></param>
    public void OnPressPreviousButton(InputAction.CallbackContext context)
    {
        if (context.performed && skillSelectMenuInstance != null)
        {
            var controller = skillSelectMenuInstance.GetComponent<SelectSkillSubMenuController>();
            // スキルウインドウのカテゴリ切り替え
            controller.PreviousCategory();
        }
    }

    /// <summary>
    /// ボタンのInteractableを切り替える
    /// </summary>
    /// <param name="interactable">有効/無効</param>
    public void ToggleButtonsInteractable(bool interactable)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i].transform.GetChild(0).GetComponent<Button>();
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }

    /// <summary>
    /// ボタンのFillAmountを操作する
    /// </summary>
    /// <param name="number">対象ボタンのコマンド内でのインデックス</param>
    public void setButtonFillAmount(int number)
    {
        int numberOfChildren = buttons.Count;

        // 対象インデックスに該当するボタンのみFillAmountを1にし、それ以外は0にする
        for (int i = 0; i < numberOfChildren - 1; i++)
        {
            int fillAmount = i == number ? 1 : 0;
            Transform child = buttons[i].transform.GetChild(0);
            Image buttonImage = child.GetComponentInChildren<Image>();
            buttonImage.fillAmount = fillAmount;
        }
    }

    /// <summary>
    /// コマンドボタンを選択状態にする
    /// </summary>
    /// <param name="number">対象ボタンのコマンド内でのインデックス</param>
    public void SelectButton(int number = 0)
    {
        if (eventSystems != null)
        {
            var button = buttons[number];

            // スクリプトから選択状態にする場合、効果音は鳴らさない
            var controller = button.GetComponent<MainMenuButtonManager>();
            if (controller != null)
            {
                controller.shouldPlaySound = false;
            }

            eventSystems.SetSelectedGameObject(button.transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// 対象選択サブメニューの各ボタンへ選択時アクションを設定する
    /// </summary>
    /// <param name="subMenuController"></param>
    /// <param name="callerButtonID"></param>
    private void AddClickActionToButtonsForEnemy(TargetSelectSubMenuController subMenuController, int callerButtonID)
    {
        var buttons = subMenuController.createdButtons;

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = EnemyManager.Instance.AliveEnemyIndexes[i];

            var buttonToSelect = buttons[i].transform.GetComponentInChildren<Button>();
            if (callerButtonID == 2)
            {
                buttonToSelect.onClick.AddListener(() => ItemToEnemy(index));  // アイテム使用
            }
            else
            {
                buttonToSelect.onClick.AddListener(() => SkillToEnemy(index)); // スキル使用
            }
        }
    }

    private void AddClickActionToButtonsForAlly(TargetSelectSubMenuController subMenuController, int callerButtonID)
    {
        var buttons = subMenuController.createdButtons;

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;

            var buttonToSelect = buttons[i].transform.GetComponentInChildren<Button>();
            if (callerButtonID == 2)
            {
                
                buttonToSelect.onClick.AddListener(() => ItemToAlly(index));  // アイテム使用
            }
            else
            {
                buttonToSelect.onClick.AddListener(() => SkillToAlly(index)); // スキル使用
            }
        }
    }

    /// <summary>
    /// 味方へスキルを使用する
    /// </summary>
    /// <param name="index">使用者のパーティ内インデックス</param>
    /// <returns></returns>
    public async UniTask SkillToAlly(int index)
    {
        Destroy(targetSelectMenuInstance);
        ToggleButtonsInteractable(true);
        SelectButton(1);

        TextAnimationManager textAnimationManager = new TextAnimationManager();

        // 対象の仲間のインスタンスを取得
        var target = PartyMembers.Instance.GetAllyByIndex(index);
        GameObject faceImage = battleController.faceImages[index];

        if (target != null)
        {
            var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

            if (turnCharacter != null && selectedSkill != null)
            {
                // スキルを使用
                await turnCharacter.UseSkill(selectedSkill, target);
            }
        }
        // ターン終了
        battleController.ChangeNextTurn();
    }

    /// <summary>
    /// 自分へスキルを使用する
    /// </summary>
    /// <param name="index">使用者のパーティ内インデックス</param>
    /// <returns></returns>
    public async UniTask SkillToSelf(int index)
    {
        Destroy(targetSelectMenuInstance);
        Destroy(skillSelectMenuInstance);
        ToggleButtonsInteractable(false);

        TextAnimationManager textAnimationManager = new TextAnimationManager();

        // 対象の仲間のインスタンスを取得
        var target = PartyMembers.Instance.GetAllyByIndex(index);
        GameObject faceImage = battleController.faceImages[index];

        if (target != null)
        {
            var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

            if (turnCharacter != null && selectedSkill != null)
            {
                // スキルを使用
                await turnCharacter.UseSkill(selectedSkill, target);
            }
        }
        ToggleButtonsInteractable(true);
        // ターン終了
        battleController.ChangeNextTurn();
    }

    /// <summary>
    /// 敵へスキルを使用する
    /// </summary>
    /// <param name="index">使用者のパーティ内インデックス</param>
    /// <returns></returns>
    private async UniTask SkillToEnemy(int index)
    {
        Destroy(targetSelectMenuInstance);
        SelectButton(lastSelectedButtonIndex);

        // 対象の敵のインスタンスを取得
        var enemy = EnemyManager.Instance.GetEnemyIns(index);
        if (enemy != null)
        {
            var comp = enemy.GetComponent<EnemyComponent>();
            var e = comp.status;
            var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

            if (e != null && turnCharacter != null && selectedSkill != null)
            {
                // スキルを使用
                await turnCharacter.UseSkill(selectedSkill, e);
            }
        }
        ToggleButtonsInteractable(true);
        // ターン終了
        battleController.ChangeNextTurn();
    }

    /// <summary>
    /// 敵全体へスキルを使用する
    /// </summary>
    /// <returns></returns>
    public async UniTask SkillToAllEnemies()
    {
        Destroy(targetSelectMenuInstance);
        Destroy(skillSelectMenuInstance);
        ToggleButtonsInteractable(false);

        // 生存中の敵のインスタンスを取得
        var aliveEnemies = EnemyManager.Instance.GetEnemiesInsExceptKnockedOut();
        var skillTasks = new List<UniTask>();

        foreach (var enemy in aliveEnemies)
        {
            if (enemy != null)
            {
                var comp = enemy.GetComponent<EnemyComponent>();
                var e = comp.status;
                var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

                if (e != null && turnCharacter != null && selectedSkill != null)
                {
                    Debug.Log($"Using skill on {e.CharacterName}");

                    // スキルを使用するタスクをリストに追加
                    skillTasks.Add(turnCharacter.UseSkill(selectedSkill, e));
                }
            }
        }
        // 全てのスキル使用タスクが完了するまで待機
        await UniTask.WhenAll(skillTasks);

        ToggleButtonsInteractable(true);
        // ターン終了
        battleController.ChangeNextTurn();
    }

    /// <summary>
    /// 味方へアイテムを使用する
    /// </summary>
    /// <param name="index">使用者のパーティ内インデックス</param>
    /// <returns></returns>
    private async UniTask ItemToAlly(int index)
    {
        Destroy(targetSelectMenuInstance);
        ToggleButtonsInteractable(true);
        SelectButton(2);

        if (selectedItem != null)
        {
            TextAnimationManager textAnimationManager = new TextAnimationManager();

            // 対象の仲間のインスタンスを取得
            var target = PartyMembers.Instance.GetAllyByIndex(index);
            GameObject faceImage = battleController.faceImages[index];

            if (target != null)
            {
                var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

                if (turnCharacter != null)
                {
                    // スキルを使用
                    await turnCharacter.UseItem(selectedItem, target);
                }
            }
            battleController.ChangeNextTurn();
        }
    }

    /// <summary>
    /// 敵へアイテムを使用する
    /// </summary>
    /// <param name="index">使用者のパーティ内インデックス</param>
    /// <returns></returns>
    private async UniTask ItemToEnemy(int index)
    {
        Destroy(targetSelectMenuInstance);
        SelectButton(2);

        if (selectedItem != null)
        {
            // 対象の敵のインスタンスを取得
            var enemy = EnemyManager.Instance.GetEnemyIns(index);
            if (enemy != null)
            {
                var comp = enemy.GetComponent<EnemyComponent>();
                var e = comp.status;
                var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

                if (e != null && turnCharacter != null)
                {
                    // アイテムを使用
                    await turnCharacter.UseItem(selectedItem, e);
                }
            }
            ToggleButtonsInteractable(true);
            battleController.ChangeNextTurn();
        }
    }

    /// <summary>
    /// サブメニューのインスタンスをすべて破棄する
    /// </summary>
    private void DestroyAllWindowInstances()
    {
        Destroy(targetSelectMenuInstance);
        Destroy(skillSelectMenuInstance);
        Destroy(itemSelectMenuInstance);
    }
}
