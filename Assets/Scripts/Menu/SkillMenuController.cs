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

public class SkillMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public MainMenuController mainMenuController;

    private List<string> characterNames = new List<string>() { "アレックス", "ニコ", "タバサ", "アリシア" };
    public int currentCharacterIndex = 0;

    public EventSystem eventSystem;
    public PlayerInput playerInput;

    public Image gradation;
    public Image nameBackGradation;

    public Image characterImage;
    public TextMeshProUGUI characterClass;
    public TextMeshProUGUI characterLevel;
    public GameObject SPGauge;
    public TextMeshProUGUI currentSP;
    public TextMeshProUGUI maxSP;

    public TextMeshProUGUI currentCharacterName;
    public TextMeshProUGUI nextCharacterName;
    public TextMeshProUGUI previousCharacterName;

    public List<TextMeshProUGUI> skillCategories;
    public int currentSkillCategoryIndex = 0;
    public GameObject underBar;
    private List<float> underBarPositions = new List<float>() {-312.0f, -238.0f, -175.0f, -101.0f, 3.0f, 127.0f};

    public GameObject skillButton;
    private Skill selectedSkill;

    public Image detailIcon;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailSPCost;
    public TextMeshProUGUI detailCategory;
    public TextMeshProUGUI detailDescription;

    public TextMeshProUGUI detailMPCost;
    public TextMeshProUGUI detailTPCost;
    public TextMeshProUGUI detailRecast;
    public TextMeshProUGUI detailLearn;

    #region 詳細 - 属性・クラス欄
    public Image detailAttributeOne;
    public Image detailAttributeTwo;
    public Image detailAttributeThree;
    public Image detailAttributeFour;
    public Image detailAttributeFive;
    public Image detailAttributeSix;

    public TextMeshProUGUI war;
    public TextMeshProUGUI pld;
    public TextMeshProUGUI mnk;
    public TextMeshProUGUI thf;
    public TextMeshProUGUI rng;
    public TextMeshProUGUI src;
    public TextMeshProUGUI clc;
    public TextMeshProUGUI sps;
    #endregion

    // スキル一欄 スクロールビュー内
    public GameObject content;

    private int lastSelectButtonIndex = 0;

    // 使用するスキル
    private Skill useSkill;

    public GameObject subWindow;
    public GameObject subWindowInstance;

    private bool isClosing = false;

    public List<string> colors = new List<string>() { "#3C52B7ff", "#B32D40ff", "#5D21A2ff", "#1E8C98ff" };

    // Start is called before the first frame update

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
        eventSystem = FindObjectOfType<EventSystem>();
        playerInput = FindObjectOfType<PlayerInput>();
        currentSkillCategoryIndex = 0;
        lastSelectButtonIndex = 0;

        SetCharacterInfo();     // キャラクター情報設定
        UpdateCharacterName();  // 左上キャラクター名初期化
        UpdateSkillCategory();  // スキルカテゴリ設定
        await SetSkillList(); // スキル選択欄設定
        InitializeSkillDetail(); // クラス詳細初期化

        SetInputActions();      // InputAction設定

        setButtonFillAmount(content, 0);
        SelectButton(content, 0); // 装備部位選択欄を選択

        var rect = underBar.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(underBarPositions[0], rect.anchoredPosition.y);

        await FadeIn();          // 画面フェードイン 
    }

    public async UniTask Init()
    {
        selectedSkill = null;
        await SetSkillList(); // スキル選択欄設定

        setButtonFillAmount(content, lastSelectButtonIndex);
        SelectButton(content, lastSelectButtonIndex); // 装備部位選択欄を選択
    }

    /// <summary>
    /// キャラクター情報を設定する(画像除く)
    /// </summary>
    public void SetCharacterInfo()
    {
        var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
        var color = CommonController.GetCharacterColorByIndex(currentCharacterIndex);

        gradation.color = color;
        nameBackGradation.color = color;

        characterImage.sprite = ch.CharacterClass.imagesA[currentCharacterIndex];
        characterLevel.text = ch.Level.ToString();
        characterClass.text = ch.CharacterClass.classAbbreviation;

        currentSP.text = ch.SP.ToString();
        maxSP.text = ch.MaxSp.ToString();

        ch.SPGauge = SPGauge;
        GaugeManager gaugeManager = SPGauge.GetComponent<GaugeManager>();
        gaugeManager.updateGaugeByText();
    }

    /// <summary>
    /// スキル一覧初期化
    /// </summary>
    /// <summary>
    /// スキル一覧初期化
    /// </summary>
    public async UniTask SetSkillList(bool isSelectTarget = true)
    {
        DestroyButtons();

        Ally ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
        List<Skill> skills = ch.LearnedSkills;
        List<Skill> filteredSkills = new List<Skill>();

        switch (currentSkillCategoryIndex)
        {
            case 0:
                filteredSkills = ch.EquipedSkills;
                break;
            case 1:
                filteredSkills = skills.Where(x => x.SkillCategory == Constants.SkillCategory.Magic).ToList();
                break;
            case 2:
                filteredSkills = skills.Where(x => x.SkillCategory == Constants.SkillCategory.Miracle).ToList();
                break;
            case 3:
                filteredSkills = skills.Where(x => x.SkillCategory == Constants.SkillCategory.Arts).ToList();
                break;
            case 4:
                filteredSkills = skills.Where(x => x.SkillCategory == Constants.SkillCategory.Active).ToList();
                break;
            case 5:
                filteredSkills = skills.Where(x => x.SkillCategory == Constants.SkillCategory.Passive).ToList();
                break;
        }

        List<Skill> orderedSkills = filteredSkills.OrderBy(x => x.ID).ToList();

        // 習得スキルを一覧にボタンとしてセットしていく
        foreach (Skill s in orderedSkills)
        {
            GameObject obj = Instantiate(skillButton, content.transform, false);    // 一覧に表示するボタンのベースをインスタンス生成
            var comp = obj.GetComponent<SkillComponent>();                           // ボタンに紐づくスキル情報を格納するコンポーネント
            if (comp != null)
            {
                comp.skill = s;
                comp.Initialize();
            }

            var newButton = obj.transform.GetChild(0).gameObject;              // ボタン本体
            AddSelectActionToButtons(newButton, s);               // 選択・選択解除時アクション設定
            var equipedSkillIDs = ch.EquipedSkills.Select(skill => skill.ID).ToList();
            // 装備可能・使用可能か判定
            if (s.CanEquip(ch) || (s.CanUseInMenu && s.CanUse(ch)))
            {
                // ボタン押下時のアクションを追加
                AddOnClickActionToSkillButton(newButton, s, ch);
            }
            // 装備中
            else if (equipedSkillIDs.Contains(s.ID))
            {
                comp.skillName.color = CommonController.GetColor(colors[currentCharacterIndex]);
            }
            else
            {
                // ボタンタイトルをグレーアウト
                comp.skillName.color = CommonController.GetColor(Constants.darkGray);
            }
        }
        await UniTask.DelayFrame(1);
    }

    /// <summary>
    /// ボタン選択時の動作を設定
    /// </summary>
    /// <param name="button"></param>
    /// <param name="item"></param>
    public void AddSelectActionToButtons(GameObject button, Skill s)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Select; // Selectイベントをリッスン
        entry.callback.AddListener((data) =>
        {
            SetSkillDetail(button, s);
        });

        // エントリをトリガーリストに追加
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// スキル詳細欄を初期化する
    /// </summary>
    private void InitializeSkillDetail()
    {
        detailIcon.enabled = false;
        detailName.text = "";
        detailSPCost.text = "-";
        detailCategory.text = "";
        detailDescription.text = "";
        detailMPCost.text = "-";
        detailTPCost.text = "-";
        detailRecast.text = "-";
        detailLearn.text = "-";
        detailAttributeOne.enabled = false;
        detailAttributeTwo.enabled = false;
        detailAttributeThree.enabled = false;
        detailAttributeFour.enabled = false;
        detailAttributeFive.enabled = false;
        detailAttributeSix.enabled = false;
        war.color = CommonController.GetColor(Constants.darkGray);
        pld.color = CommonController.GetColor(Constants.darkGray);
        mnk.color = CommonController.GetColor(Constants.darkGray);
        thf.color = CommonController.GetColor(Constants.darkGray);
        rng.color = CommonController.GetColor(Constants.darkGray);
        src.color = CommonController.GetColor(Constants.darkGray);
        clc.color = CommonController.GetColor(Constants.darkGray);
        sps.color = CommonController.GetColor(Constants.darkGray);
    }

    /// <summary>
    /// スキル詳細欄を設定する
    /// </summary>
    /// <param name="skill"></param>
    private void SetSkillDetail(GameObject button, Skill skill)
    {
        selectedSkill = skill;
        lastSelectButtonIndex = button.transform.parent.transform.GetSiblingIndex();

        detailIcon.enabled = true;
        detailIcon.sprite = skill.Icon;
        detailName.text = skill.SkillName;
        detailSPCost.text = skill.SpCost.ToString();
        detailCategory.text = CommonController.GetSkillCategoryString(skill.SkillCategory);
        detailDescription.text = skill.Description;

        if (skill is MagicMiracle)
        {
            var m = skill as MagicMiracle;
            detailMPCost.text = m.MpCost.ToString();
        }
        else
        {
            detailMPCost.text = "-";
        }

        if (skill is Arts)
        {
            var a = skill as Arts;
            detailTPCost.text = a.TpCost.ToString();
        }
        else
        {
            detailTPCost.text = "-";
        }

        if (skill is ActiveSkill)
        {
            var a = skill as ActiveSkill;
            detailRecast.text = a.RecastTurn.ToString();
        }
        else
        {
            detailRecast.text = "-";
        }

        detailLearn.text = skill.Learn;

        #region 属性・クラス
        if (skill.Attributes.Count >= 1)
        {
            detailAttributeOne.enabled = true;
            detailAttributeOne.sprite = skill.Attributes[0].icon;
        }
        if (skill.Attributes.Count >= 2)
        {
            detailAttributeTwo.enabled = true;
            detailAttributeTwo.sprite = skill.Attributes[1].icon;
        }
        if (skill.Attributes.Count >= 3)
        {
            detailAttributeThree.enabled = true;
            detailAttributeThree.sprite = skill.Attributes[2].icon;
        }
        if (skill.Attributes.Count >= 4)
        {
            detailAttributeFour.enabled = true;
            detailAttributeFour.sprite = skill.Attributes[3].icon;
        }
        if (skill.Attributes.Count >= 5)
        {
            detailAttributeFive.enabled = true;
            detailAttributeFive.sprite = skill.Attributes[4].icon;
        }
        if (skill.Attributes.Count >= 6)
        {
            detailAttributeSix.enabled = true;
            detailAttributeSix.sprite = skill.Attributes[5].icon;
        }

        foreach (var C in skill.UsableClasses)
        {
            if (C.ID == "01")
            {
                war.color = Color.white;
            }
            if (C.ID == "02")
            {
                pld.color = Color.white;
            }
            if (C.ID == "03")
            {
                mnk.color = Color.white;
            }
            if (C.ID == "04")
            {
                thf.color = Color.white;
            }
            if (C.ID == "05")
            {
                rng.color = Color.white;
            }
            if (C.ID == "06")
            {
                src.color = Color.white;
            }
            if (C.ID == "07")
            {
                clc.color = Color.white;
            }
            if (C.ID == "08")
            {
                sps.color = Color.white;
            }

        }
        #endregion
    }

    /// </summary>
    /// 装備一覧ボタン押下時の動作を設定する
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="item"></param>
    private void AddOnClickActionToSkillButton(GameObject obj, Skill skill, Ally ch)
    {
        var button = obj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener( async () => await OnClickActionToSkillButton(skill, ch));
        }
    }

    /// <summary>
    /// ボタン押下時アクションをボタンに設定
    /// </summary>
    /// <param name="skill"></param>
    private async UniTask OnClickActionToSkillButton(Skill skill, Ally ch)
    {
        SoundManager.Instance.PlaySubmit();
        switch (skill.SkillCategory)
        {
            case Constants.SkillCategory.Miracle:
                DisplaySubMenu(skill);
                break;
            case Constants.SkillCategory.Active:
            case Constants.SkillCategory.Passive:
                ch.EquipSkill(skill);
                await Init();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// キャラクター選択サブメニューを表示する
    /// </summary>
    /// <param name="item"></param>
    private void DisplaySubMenu(Skill skill)
    {

        var selectedButton = EventSystem.current.currentSelectedGameObject;
        // アイテム選択時
        Vector3 pos = selectedButton.transform.position;

        // 一覧の下側のアイテムが選択されたらサブウインドウをカーソルの上側に表示
        float offset = pos.y < 320 ? 85 : -100;

        // カーソル位置を記憶するため、選択中のアイテムのインデックスを保存
        lastSelectButtonIndex = selectedButton.transform.parent.transform.GetSiblingIndex();

        var position = new Vector3(pos.x + 490, pos.y + offset, pos.z);

        subWindowInstance = Instantiate(subWindow, position, Quaternion.identity, transform.parent);
        var controller = subWindowInstance.GetComponent<ItemMenuSubWindowController>();
        if (controller != null)
        {
            controller.skill = skill;
            controller.userIndex = currentCharacterIndex;
            controller.skillMenuController = this;
        }
        ToggleButtonsInteractable(content, false);
        setButtonFillAmount(content, lastSelectButtonIndex);
    }

    /// <summary>
    /// キャラクター切り替え
    /// </summary>
    /// <returns></returns>
    private async UniTask ChangeCharacter()
    {
        float duration = 0.2f;
        
        await characterImage.gameObject.GetComponent<SpriteManipulator>().FadeOut(duration);
        SetCharacterInfo();

        Color color = CommonController.GetCharacterColorByIndex(currentCharacterIndex);

        ToggleButtonsInteractable(content, true);

        InitializeSkillDetail();

        lastSelectButtonIndex = 0;
        await Init();

        
        await UniTask.WhenAll(       
            characterImage.gameObject.GetComponent<SpriteManipulator>().FadeIn(duration),
            gradation.gameObject.GetComponent<SpriteManipulator>().AnimateColor(color, duration * 2),
            nameBackGradation.gameObject.GetComponent<SpriteManipulator>().AnimateColor(color, duration * 2)
            );
    }

    private void UpdateSkillCategory()
    {
        //var rect = underBar.GetComponent<RectTransform>();

        for (int i = 0; i < skillCategories.Count; i++)
        {
            skillCategories[i].color = CommonController.GetColor(Constants.darkGray);
        }
        skillCategories[currentSkillCategoryIndex].color = Color.white;
        //rect.anchoredPosition = new Vector2(underBarPositions[currentSkillCategoryIndex], rect.anchoredPosition.y);
    }

    private async UniTask SlideUnderBar(float duration = 0.2f)
    {
        var rect = underBar.GetComponent<RectTransform>();
        var pos = new Vector2(underBarPositions[currentSkillCategoryIndex], rect.anchoredPosition.y);
        await rect.DOAnchorPos(pos, duration).SetEase(Ease.InOutQuad).SetUpdate(true);
    }

    ///　--------------------------------------------------------------- ///
    ///　--------------------------- 汎用処理 --------------------------- ///
    ///　--------------------------------------------------------------- ///

    private void SetInputActions()
    {
        if (playerInput != null)
        {
            // InputActionAssetを取得
            var inputActionAsset = playerInput.actions;

            // "Main"アクションマップを取得
            var actionMap = inputActionAsset.FindActionMap("Menu");
            // アクションを取得
            var rs = actionMap.FindAction("RightShoulder");
            var ls = actionMap.FindAction("LeftShoulder");
            var rt = actionMap.FindAction("RightTrigger");
            var lt = actionMap.FindAction("LeftTrigger");
            var cancel = actionMap.FindAction("Cancel");
            var general = actionMap.FindAction("General");

            // イベントリスナーを設定
            cancel.performed += OnPressCancelButton;
            general.performed += OnPressGeneralButton;
            rs.performed += OnPressRSButton;
            ls.performed += OnPressLSButton;
            rt.performed += OnPressRTButton;
            lt.performed += OnPressLTButton;

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
            var actionMap = inputActionAsset.FindActionMap("Menu");
            // アクションを取得
            var rs = actionMap.FindAction("RightShoulder");
            var ls = actionMap.FindAction("LeftShoulder");
            var rt = actionMap.FindAction("RightTrigger");
            var lt = actionMap.FindAction("LeftTrigger");
            var cancel = actionMap.FindAction("Cancel");
            var general = actionMap.FindAction("General");

            cancel.performed -= OnPressCancelButton;
            general.performed -= OnPressGeneralButton;
            rs.performed -= OnPressRSButton;
            ls.performed -= OnPressLSButton;
            rt.performed -= OnPressRTButton;
            lt.performed -= OnPressLTButton;
        }
    }

    /// <summary>
    /// キャンセルボタン押下時処理
    /// </summary>
    public async void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed && !isClosing)
        {
            SoundManager.Instance.PlayCancel();

            isClosing = true;
            Debug.Log("cancel button is performing.");
            if (subWindowInstance != null)
            {
                Destroy(subWindowInstance);
                await Init();
            }
            else
            {
                // アイテムメニューのフェードアウト
                await FadeOut(gameObject, 0.3f);
                if (mainMenuController != null)
                {
                    // メインメニューの初期化
                    await mainMenuController.InitializeFromChildren("Skill");
                }
                // アイテムメニューインスタンスの破棄
                gameObject.SetActive(false);
            }
            isClosing = false;
        }
    }

    /// <summary>
    /// 汎用ボタン押下時 - 装備中スキルをはずす
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressGeneralButton(InputAction.CallbackContext context)
    {
        if (context.performed && subWindowInstance == null)
        {
            if (selectedSkill != null)
            {
                var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
                await ch.UnEquipSkill(selectedSkill);

                await Init();
            }
        }
    }

    /// <summary>
    /// Rボタン押下時処理 - スキルカテゴリ切り替え - 次ページ
    /// </summary>
    public async void OnPressRSButton(InputAction.CallbackContext context)
    {
        if (context.performed && subWindowInstance == null)
        {
            SoundManager.Instance.PlaySelect(0.5f);

            currentSkillCategoryIndex = (currentSkillCategoryIndex + 1) % skillCategories.Count;
            UpdateSkillCategory();

            await UniTask.WhenAll(
                 SlideUnderBar(),
                SetSkillList());

            ToggleButtonsInteractable(content, true);
            SelectButton(content, 0);
        }
    }

    /// <summary>
    /// Lボタン押下時処理 - スキルカテゴリ切り替え - 前ページ
    /// </summary>
    public async void OnPressLSButton(InputAction.CallbackContext context)
    {
        if (context.performed && subWindowInstance == null)
        {
            SoundManager.Instance.PlaySelect(0.5f);

            currentSkillCategoryIndex = (currentSkillCategoryIndex - 1 + skillCategories.Count) % skillCategories.Count;
            UpdateSkillCategory();

            await UniTask.WhenAll(
                 SlideUnderBar(),
                SetSkillList());

            ToggleButtonsInteractable(content, true);
            SelectButton(content, 0);
        }
    }

    /// <summary>
    /// RTボタン押下時処理 - キャラクター切り替え - 次ページ
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressRTButton(InputAction.CallbackContext context)
    {
        if (context.performed && subWindowInstance == null)
        {
            SoundManager.Instance.PlaySelect(0.5f);

            currentCharacterIndex = (currentCharacterIndex + 1) % characterNames.Count;
            UpdateCharacterName();

            await ChangeCharacter();
        }
    }

    /// <summary>
    /// LTボタン押下時処理 - キャラクター切り替え - 前ページ
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressLTButton(InputAction.CallbackContext context)
    {
        if (context.performed && subWindowInstance == null)
        {
            SoundManager.Instance.PlaySelect(0.5f);
            
            currentCharacterIndex = (currentCharacterIndex - 1 + characterNames.Count) % characterNames.Count;
            UpdateCharacterName();

            await ChangeCharacter();
        }
    }

    /// <summary>
    /// 左上キャラクター名称更新
    /// </summary>
    private void UpdateCharacterName()
    {
        currentCharacterName.text = characterNames[currentCharacterIndex];
        nextCharacterName.text = characterNames[(currentCharacterIndex + 1) % characterNames.Count];
        previousCharacterName.text = characterNames[(currentCharacterIndex - 1 + characterNames.Count) % characterNames.Count];

        //ClearSkillDetailInfo();
    }

    /// <summary>
    /// ボタンのInteractableを切り替える
    /// </summary>
    /// <param name="interactable">有効/無効</param>
    public void ToggleButtonsInteractable(GameObject obj, bool interactable)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Transform child = obj.transform.GetChild(i);
            Button button = child.GetComponentInChildren<Button>();

            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }

    /// <summary>
    /// ボタンを選択状態にする
    /// </summary>
    /// <param name="number"></param>
    public void SelectButton(GameObject obj, int number = 0)
    {
        if (eventSystem != null && obj.transform.childCount > 0)
        {
            var buttonToSelect = obj.transform.GetChild(number).GetChild(0).gameObject;

            // スクリプトから選択状態にする場合、効果音は鳴らさない
            var controller = buttonToSelect.GetComponent<MainMenuButtonManager>();
            if (controller != null)
            {
                controller.shouldPlaySound = false;
            }

            eventSystem.SetSelectedGameObject(buttonToSelect);
        }
    }

    /// <summary>
    /// ボタンのFillAmountを操作する
    /// </summary>
    /// <param name="number">対象ボタンのコマンド内でのインデックス</param>
    public void setButtonFillAmount(GameObject obj, int number)
    {
        int numberOfChildren = obj.transform.childCount;

        // 対象インデックスに該当するボタンのみFillAmountを1にし、それ以外は0にする
        for (int i = 0; i < numberOfChildren; i++)
        {
            int fillAmount = i == number ? 1 : 0;
            Transform child = obj.transform.GetChild(i);
            Image buttonImage = child.GetComponentInChildren<Image>();
            buttonImage.fillAmount = fillAmount;
        }
    }

    /// <summary>
    /// 一覧内のボタンをすべて破棄する
    /// </summary>
    /// <returns></returns>
    public async UniTask DestroyButtons()
    {
        //var content = GameObject.FindWithTag("ScrollViewContent");
        // アイテム一覧内のオブジェクトを削除
        int childCount = content.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = content.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        await UniTask.DelayFrame(1);
    }

    public async UniTask FadeIn(float duration = 0.3f)
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


}
