using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BattleResultControler : MonoBehaviour
{
    public BattleController battleController;
    public GameObject canvas;

    private List<AllyStatus> allies;

    public TextMeshProUGUI earnedEXP;
    public TextMeshProUGUI gold;
    public List<GameObject> droppedItems;
    public GameObject itemsOne;
    public GameObject itemsTwo;
    public GameObject itemsThree;
    public GameObject itemsFour;
    public GameObject itemsFive;
    public GameObject itemsSix;

    public List<Image> faces;
    public List<TextMeshProUGUI> classes;
    public List<TextMeshProUGUI> levels;
    public List<GameObject> expGauges;
    public List<GameObject> levelUpTexts;
    public List<GameObject> learnedSkills;

    public int earnedExp;
    public int earnedGold;
    public List<Item> earnedItems;

    private int maxLevelUpTimes;

    private PlayerInput playerInput;

    private bool isComplete = false;

    async void OnEnable()
    {
        await Initialize();
    }

    private void OnDisable()
    {
        RemoveInputActions();
    }

    /// <summary>
    /// スキル画面初期化
    /// </summary>
    public async UniTask Initialize()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        SetInputActions();

        allies = PartyMembers.Instance.GetAllies();
        SetCharactersInfo();
        SetDroppedItems();

        await UniTask.WhenAll(
            FadeIn(gameObject, 0.5f) // 画面フェードイン
          //SlideCanvas(1.0f)
            );

        await CountText(earnedEXP, earnedExp, 0.5f); // 獲得経験値カウントアップ
        await UniTask.DelayFrame(5);                 // 5フレーム待機
        await CountText(gold, earnedGold, 0.5f);     // 獲得ゴールドカウントアップ
        await UniTask.DelayFrame(20);                // 20フレーム待機

        int surplusExpOne = earnedExp;
        int surplusExpTwo = earnedExp;
        int surplusExpThree = earnedExp;
        int surplusExpFour = earnedExp;

        while (surplusExpOne >= 0 || surplusExpTwo >= 0 || surplusExpThree >= 0 || surplusExpFour >= 0)
        {
            SetCharactersInfo();

            // 各キャラクターに経験値を加算し、余剰経験値を取得
            var surplusExpTasks = new UniTask<int>[] {
                allies[0].GetExp(surplusExpOne),
                allies[1].GetExp(surplusExpTwo),
                allies[2].GetExp(surplusExpThree),
                allies[3].GetExp(surplusExpFour)
                };

            // 非同期処理を待機
            var results = await UniTask.WhenAll(surplusExpTasks);

            // 結果を変数に代入
            surplusExpOne = results[0];
            surplusExpTwo = results[1];
            surplusExpThree = results[2];
            surplusExpFour = results[3];

            await UniTask.DelayFrame(30);

            // レベルアップと習得スキル表示のタスクを作成
            var levelUpTasks = new List<UniTask>();

            if (surplusExpOne >= 0)
            {
                levelUpTasks.Add(LevelUpAndDisplaySkill(0));
            }
            if (surplusExpTwo >= 0)
            {
                levelUpTasks.Add(LevelUpAndDisplaySkill(1));
            }
            if (surplusExpThree >= 0)
            {
                levelUpTasks.Add(LevelUpAndDisplaySkill(2));
            }
            if (surplusExpFour >= 0)
            {
                levelUpTasks.Add(LevelUpAndDisplaySkill(3));
            }

            await UniTask.WhenAll(levelUpTasks);
            await UniTask.DelayFrame(20);
        }
        await UniTask.DelayFrame(30);
        isComplete = true;
    }

    /// <summary>
    /// 指定されたキャラクターのレベルアップとスキル表示を行う
    /// </summary>
    private async UniTask LevelUpAndDisplaySkill(int index)
    {
        await LevelUp(index);
        await DisplayLearnedSkill(index);
    }

    private async UniTask LevelUp(int index)
    {
        var m = levelUpTexts[index].GetComponent<TextAnimationManager>();
        var ma = levels[index].GetComponent<TextAnimationManager>();

        levels[index].text = (int.Parse(levels[index].text) + 1).ToString();

        await UniTask.WhenAll(
            m.LevelUpAnimation(),
            ma.TextScaleFlash());
    }

    private async UniTask DisplayLearnedSkill(int index)
    {
        AllyStatus ch = PartyMembers.Instance.GetAllyByIndex(index);
        int levelIndex = ch.level - 1;

        if (ch.Class.LearnSkills.Count >= ch.level)
        {
            Skill learnedSkill = ch.Class.LearnSkills[levelIndex];

            var image = learnedSkills[index].GetComponentInChildren<Image>();
            var skillName = learnedSkills[index].GetComponentInChildren<TextMeshProUGUI>();

            if (learnedSkill != null && image != null && skillName != null)
            {
                image.sprite = learnedSkill.icon;
                skillName.text = learnedSkill.skillName;
            }

            await FadeIn(learnedSkills[index]);
        }
    }

    private void SetCharactersInfo()
    {

        for (int i = 0; i < allies.Count; i++)
        {
            AllyStatus ch = allies[i];

            faces[i].sprite = ch.Class.imagesC[i];
            classes[i].text = ch.Class.classAbbreviation;
            levels[i].text = ch.level.ToString();

            ch.EXPGauge = expGauges[i];

            var manager = ch.EXPGauge.GetComponent<GaugeManager>();
            if (manager != null)
            {
                manager.maxValueText.text = ch.GetCurrentClassNextExp().ToString();
                manager.currentValueText.text = ch.GetCurrentClassEarnedExp().ToString();
                manager.updateGaugeByText();
            }
        }
    }

    private void SetDroppedItems()
    {
        foreach (var droppedItem in droppedItems)
        {
            droppedItem.SetActive(false);
        }

        if (earnedItems.Count > 0)
        {
            for (int i = 0; i < earnedItems.Count; i++)
            {
                droppedItems[i].SetActive(true);
                droppedItems[i].GetComponentInChildren<Image>().sprite = earnedItems[i].iconImage;
                droppedItems[i].GetComponentInChildren<TextMeshProUGUI>().text = earnedItems[i].itemName;
            }
        }
    }

    /// <summary>
    /// テキストを指定の値までカウントアップ・ダウンさせる
    /// </summary>
    /// <param name="targetValue">カウントさせる目標値</param>
    /// <param name="duration">カウントさせる時間間隔</param>
    /// <returns></returns>
    public async UniTask CountText(TextMeshProUGUI tm, int targetValue, float duration = 1.0f)
    {
        if (!int.TryParse(tm.text, out var currentValue))
        {
            return;
        }
        await DOTween.To(
            () => currentValue,                        // 開始値
            x => tm.text = x.ToString(), // テキスト更新のアクション
            targetValue,                            　 // 目標値
            duration                                　 // 継続時間
        ).SetEase(Ease.OutQuad).SetUpdate(true);                       // アニメーションのイージング設定
    }

    public async UniTask SlideCanvas(float duration)
    {
        var rect = canvas.GetComponent<RectTransform>();

        await rect.DOAnchorPos(new Vector2(rect.anchoredPosition.x + 1000.0f, rect.anchoredPosition.y), duration).SetEase(Ease.InOutQuad).SetUpdate(true);
    }

    private void SetInputActions()
    {
        if (playerInput != null)
        {
            // InputActionAssetを取得
            var inputActionAsset = playerInput.actions;

            // "Main"アクションマップを取得
            var actionMap = inputActionAsset.FindActionMap("Main");
            // アクションを取得
            var submit = actionMap.FindAction("SubMit");

            // イベントリスナーを設定
            submit.performed += OnPressSubmitButton;

            // アクションマップを有効にする
            actionMap.Enable();
        }
    }

    void RemoveInputActions()
    {
        // イベントリスナーを解除
        if (playerInput != null)
        {
            // InputActionAssetを取得
            var inputActionAsset = playerInput.actions;
            // "Main"アクションマップを取得
            var actionMap = inputActionAsset.FindActionMap("Main");
            // アクションを取得
            var submit = actionMap.FindAction("SubMit");

            submit.performed -= OnPressSubmitButton;
        }
    }

    private void OnPressSubmitButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isComplete)
            {
                EndScene();
            }
        }
    }

    public async UniTask FadeIn(GameObject gameObject, float duration = 0.3f)
    {
        // ゲームオブジェクトと CanvasGroup の存在を確認
        if (gameObject != null && gameObject.activeInHierarchy)
        {
            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // 透明度を1にアニメーション
                await canvasGroup.DOFade(1, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();
                canvasGroup.interactable = true;
            }
        }
    }

    public async UniTask FadeOut(GameObject gameObject, float duration = 0.3f)
    {
        // ゲームオブジェクトと CanvasGroup の存在を確認
        if (gameObject != null && gameObject.activeInHierarchy)
        {
            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // 透明度を0にアニメーション
                await canvasGroup.DOFade(0, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();
                canvasGroup.interactable = false;
            }
            // アニメーション完了後にゲームオブジェクトを破棄
            if (gameObject != null)
            {
                //Destroy(gameObject);
            }
        }
    }

    private async void EndScene()
    {
        if (SceneController.Instance != null)
        {
            //RemoveInputActions();
            EnemyManager.Instance.Initialize();

            await SceneController.Instance.SwitchFieldAndBattleScene("AbandonedFortress1F");
            CommonVariableManager.playerCanMove = true;
        }
    }
}
