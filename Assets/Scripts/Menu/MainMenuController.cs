using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public TextMeshProUGUI header;

    public GameObject MenuOutline;
    public GameObject mainCamera;

    private Constants.MenuState menuState;

    public GameObject BackGroundImage;
    public GameObject main;

    public GameObject characterSelectSubMenu;
    public GameObject characterSelectSubMenuInstance;

    public GameObject itemMenuObject;
    public GameObject itemMenuInstance;

    public GameObject equipMenuObject;
    public GameObject equipMenuInstance;

    public GameObject skillMenuObject;
    public GameObject skillMenuInstance;

    public GameObject classMenuObject;
    public GameObject classMenuInstance;

    public GameObject statusMenuObject;
    public GameObject statusMenuInstance;

    public GameObject systemMenuObject;
    public GameObject systemMenuInstance;

    public GameObject equip;
    public GameObject skill;
    public GameObject Class;
    public GameObject status;
    public GameObject help;
    public GameObject options;


    public List<GameObject> buttons;
    public List<Image> characterImages;
    public List<RectTransform> characterFrames;
    public List<TextMeshProUGUI> classNames;
    public List<TextMeshProUGUI> levels;
    public List<GameObject> HPGauges;
    public List<GameObject> MPGauges;
    public List<GameObject> TPGauges;
    public List<GameObject> EXPGauges;

    public TextMeshProUGUI info;

    [SerializeField]
    private List<GameObject> screens = new List<GameObject>();

    [SerializeField]
    public CinemachineVirtualCamera vCamera;

    [SerializeField]
    public EventSystem eventSystem;

    [SerializeField]
    public GameObject itemButton;
    [SerializeField]
    public GameObject equipButton;
    [SerializeField]
    public GameObject skillButton;
    [SerializeField]
    public GameObject statusButton;
    [SerializeField]
    public GameObject classButton;
    [SerializeField]
    public GameObject systemButton;

    private GameObject selectedButton;

    public GameObject inputActionParent;
    private InputAction inputAction;

    public PlayerInput playerInput;

    int showingCharacterID = 1;
    int lastSelectButtonIndex = 0;

    StatusMenuController statusMenuController;

    public TextMeshProUGUI playTime;

    //private VCamManager vcm = new VCamManager();

    // Start is called before the first frame update
    void Update()
    {
        playTime.text = PlayTimeManager.Instance.FormatPlayTime();
    }

    async void OnEnable()
    {
        EnableInputActionMap("Menu", "Main");
    }

    private async UniTaskVoid Awake()
    {
        DOTween.Init();
        await Initialize();
        lastSelectButtonIndex = 0;
    }

    void OnDisable()
    {
        RemoveInputActions();
        EnableInputActionMap("Main", "Menu");
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public async UniTask Initialize()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        SetInputActions();

        var duration = 0.5f;

        // 背景スプライトのフェードイン
        SpriteManipulator manipulator = BackGroundImage.GetComponent<SpriteManipulator>();
        await manipulator.FadeIn(0.5f);

        // ステータス設定
        SetAllyes();

        // ボタンとキャラクター枠のアニメーション
        await UniTask.WhenAll(
            SlideButtons(duration),
            SlideFrames(duration)
            );

        // 先頭のボタンを選択
        SelectButton();

        //var image = BackGroundImage.GetComponent<Image>();

        //image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        //await image.DOFade(1, 1.0f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
    }

    private void SetInputActions()
    {
        if (playerInput != null)
        {
            // InputActionAssetを取得
            var inputActionAsset = playerInput.actions;

            // "Main"アクションマップを取得
            var actionMap = inputActionAsset.FindActionMap("Menu");
            // アクションを取得
            var cancel = actionMap.FindAction("Cancel");
            var openMenu = actionMap.FindAction("OpenMenu");
            var general = actionMap.FindAction("General");

            cancel.performed += OnPressCancelButton;
            openMenu.performed += OnPressMenuButton;
            general.performed += OnPressGeneralButton;
            // アクションマップを有効にする
            actionMap.Enable();
        }
    }

    private void RemoveInputActions()
    {
        // イベントリスナーを解除
        if (playerInput != null)
        {
            // InputActionAssetを取得
            var inputActionAsset = playerInput.actions;
            // "Main"アクションマップを取得
            var actionMap = inputActionAsset.FindActionMap("Menu");
            // アクションを取得
            var cancel = actionMap.FindAction("Cancel");
            var openMenu = actionMap.FindAction("OpenMenu");
            var general = actionMap.FindAction("General");

            cancel.performed -= OnPressCancelButton;
            openMenu.performed -= OnPressMenuButton;
            general.performed -= OnPressGeneralButton;
        }
    }

    private void SetAllyes()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        List<Ally> allies = PartyMembers.Instance.GetAllies();

        for (int i = 0; i < allies.Count; i++)
        {
            var ally = allies[i];

            characterImages[i].sprite = ally.CharacterClass.imagesB[i];                    // キャラクター画像
            classNames[i].text = ally.CharacterClass.classAbbreviation;
            levels[i].text = ally.Level.ToString();
            ally.HPGauge = HPGauges[i];                                           // HPゲージ
            ally.MPGauge = MPGauges[i];                                           // MPゲージ
            ally.TPGauge = TPGauges[i];                                           // TPゲージ

            GaugeManager hpGaugeManager = HPGauges[i].GetComponent<GaugeManager>();     // HPゲージ管理クラス
            GaugeManager mpGaugeManager = MPGauges[i].GetComponent<GaugeManager>();     // MPゲージ管理クラス
            GaugeManager tpGaugeManager = TPGauges[i].GetComponent<GaugeManager>();     // TPゲージ管理クラス
            GaugeManager expGaugeManager = EXPGauges[i].GetComponent<GaugeManager>();     // EXPゲージ管理クラス

            hpGaugeManager.maxValueText.text = ally.MaxHp.ToString();            // 最大HPテキスト
            hpGaugeManager.currentValueText.text = ally.HP.ToString();            // 現在HPテキスト
            mpGaugeManager.maxValueText.text = ally.MaxMp.ToString();            // 最大MPテキスト
            mpGaugeManager.currentValueText.text = ally.MP.ToString();            // 現在MPテキスト
            tpGaugeManager.currentValueText.text = ally.TP.ToString();            // 現在TPテキスト
            expGaugeManager.maxValueText.text = ally.GetCurrentClassNextExp().ToString();            // 最大EXPテキスト
            expGaugeManager.currentValueText.text = ally.GetCurrentClassEarnedExp().ToString();            // 現在EXPテキスト

            hpGaugeManager.updateGaugeByText();
            mpGaugeManager.updateGaugeByText();
            tpGaugeManager.updateGaugeByText();
            expGaugeManager.updateGaugeByText();
        }
    }

    public async UniTask SlideButtons(float duration)
    {
        // x -290 y -536 650 1010 1370

        var unitDuration = duration / buttons.Count;

        foreach (GameObject button in buttons)
        {
            RectTransform transform = button.GetComponent<RectTransform>();
            //_ = button.DOAnchorPos(new Vector2(-130f, -90f), 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            await transform.DOAnchorPos(new Vector2(transform.anchoredPosition.x + 470.0f, transform.anchoredPosition.y), unitDuration).SetEase(Ease.InOutQuad).SetUpdate(true);
            //await UniTask.Delay(10);
        }

    }

    public async UniTask SlideFrames(float duration)
    {
        // x -290 y -536 650 1010 1370

        var unitDuration = duration / characterFrames.Count;

        foreach (RectTransform frame in characterFrames)
        {
            //_ = button.DOAnchorPos(new Vector2(-130f, -90f), 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            await frame.DOAnchorPos(new Vector2(frame.anchoredPosition.x - 1500.0f, frame.anchoredPosition.y), unitDuration).SetEase(Ease.InOutQuad).SetUpdate(true);
            //await UniTask.Delay(10);
        }
    }

    /// <summary>
    /// キャンセルボタン押下
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SoundManager.Instance.PlayCancel();

            if (characterSelectSubMenuInstance != null)
            {
                Destroy(characterSelectSubMenuInstance);
            }
            else
            {
                await CloseMenu();
            }
        }
    }

    /// <summary>
    /// メニューボタン押下
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressMenuButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SoundManager.Instance.PlayCancel();
            await CloseMenu();
        }
    }

    /// <summary>
    /// 汎用ボタン(△)押下
    /// </summary>
    /// <param name="context"></param>
    public void OnPressGeneralButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //SaveManager.Instance.SaveGame();
        }
    }

    /// <summary>
    /// メニュー画面を開く
    /// </summary>
    public void OpenMenu()
    {
        // プレイヤーの動きを止める
        CommonVariableManager.playerCanMove = false;

        CommonController.PauseGame();
        MenuOutline.SetActive(true);
    }

    /// <summary>
    /// メニュー画面を閉じる
    /// </summary>
    public async UniTask CloseMenu()
    {
        await FadeOutChildren(gameObject);
        Destroy(gameObject);
        CommonController.ResumeGame();
    }

    /// <summary>
    /// メインメニュー Itemボタン押下時
    /// アイテム画面遷移処理
    /// </summary>
    public async void OnPressItemButton()
    {
        SoundManager.Instance.PlaySubmit();
        
        eventSystem.enabled = false;
        await FadeOutChildren(main);
        RemoveInputActions();
        header.text = "アイテム";
        itemMenuObject.SetActive(true);
        var controller = itemMenuObject.GetComponent<ItemMenuController>();
        await controller.Initialize();
    }

    public void OnPressEquipButton()
    {
        SoundManager.Instance.PlaySubmit();

        DisplayCharacterSelectSubMenu(1);
    }

    public async UniTask GoToEquipMenu(int characterIndex)
    {
        eventSystem.enabled = false;
        Destroy(characterSelectSubMenuInstance);
        await FadeOutChildren(main);
        RemoveInputActions();
        header.text = "装備";
        var controller = equipMenuObject.GetComponent<EquipMenuController>();
        controller.currentCharacterIndex = characterIndex;
        equipMenuObject.SetActive(true);
        eventSystem.enabled = true;
    }

    public void OnPressSkillButton()
    {
        SoundManager.Instance.PlaySubmit();

        DisplayCharacterSelectSubMenu(2);
    }

    public async UniTask GoToSkillMenu(int characterIndex)
    {
        eventSystem.enabled = false;
        Destroy(characterSelectSubMenuInstance);
        await FadeOutChildren(main);
        RemoveInputActions();
        header.text = "スキル";
        var controller = skillMenuObject.GetComponent<SkillMenuController>();
        controller.currentCharacterIndex = characterIndex;
        skillMenuObject.SetActive(true);
        eventSystem.enabled = true;
    }

    public void OnPressClassButton()
    {
        SoundManager.Instance.PlaySubmit();

        DisplayCharacterSelectSubMenu(3);
    }

    public async UniTask GoToClassMenu(int characterIndex)
    {
        eventSystem.enabled = false;
        Destroy(characterSelectSubMenuInstance);
        await FadeOutChildren(main);
        RemoveInputActions();
        header.text = "クラス";
        var controller = classMenuObject.GetComponent<ClassMenuController>();
        controller.currentCharacterIndex = characterIndex;
        classMenuObject.SetActive(true);
        eventSystem.enabled = true;
    }

    /// <summary>
    /// メインメニュー Statusボタン押下時
    /// アイテム画面遷移処理
    /// </summary>
    public void OnPressStatusButton()
    {
        SoundManager.Instance.PlaySubmit();

        DisplayCharacterSelectSubMenu(4);
    }

    public async UniTask GoToStatusMenu(int characterIndex)
    {
        eventSystem.enabled = false;
        Destroy(characterSelectSubMenuInstance);
        await FadeOutChildren(main);
        RemoveInputActions();
        header.text = "ステータス";
        var controller = statusMenuObject.GetComponent<StatusMenuController>();
        controller.currentCharacterIndex = characterIndex;
        statusMenuObject.SetActive(true);
        eventSystem.enabled = true;
    }

    /// <summary>
    /// メインメニュー システムボタン押下時
    /// システム画面遷移処理
    /// </summary>
    public async void OnPressSystemButton()
    {
        SoundManager.Instance.PlaySubmit();

        eventSystem.enabled = false;
        await FadeOutChildren(main);
        RemoveInputActions();
        header.text = "システム";
        systemMenuObject.SetActive(true);
        //var controller = systemMenuObject.GetComponent<SystemMenuController>();
        eventSystem.enabled = true;
    }

    /// <summary>
    /// キャラクター選択サブメニューを表示する
    /// </summary>
    /// <param name="destinationMenuIndex"></param>
    public void DisplayCharacterSelectSubMenu(int destinationMenuIndex)
    {
        if (characterSelectSubMenuInstance == null)
        {
            var selectedButton = EventSystem.current.currentSelectedGameObject;
            var pos = selectedButton.transform.position;
            var newPos = new Vector3(pos.x + 360.0f, pos.y - 118.0f, pos.z);

            characterSelectSubMenuInstance = Instantiate(characterSelectSubMenu, newPos, Quaternion.identity, transform);
            var controller = characterSelectSubMenuInstance.GetComponent<MainMenuCharacterSelectSubWindowController>();
            controller.mainMenuController = this;
            controller.destinationMenuIndex = destinationMenuIndex;

            ToggleButtonsInteractable(false);
            setButtonFillAmount(destinationMenuIndex);
        }
    }

    public void ShowScreen(int number)
    {
        if (screens != null && screens.Count > 0)
        {
            screens[number].SetActive(true);
        }
    }

    public void CloseScreen()
    {
        if (screens != null && screens.Count > 0)
        {
            foreach (GameObject screen in screens)
            {
                screen.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ボタン選択
    /// </summary>
    /// <param name="number"></param>
    public void SelectButton(int number = 0)
    {
        if (eventSystem != null)
        {
            var button = buttons[number];

            // スクリプトから選択状態にする場合、効果音は鳴らさない
            var controller = button.GetComponent<MainMenuButtonManager>();
            if (controller != null)
            {
                controller.shouldPlaySound = false;
            }

            eventSystem.SetSelectedGameObject(button.transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// ボタン選択時Info欄の内容を切り替える
    /// </summary>
    /// <param name="index"></param>
    public void OnSelectButton(int index)
    {
        var infoMessage = "";
        lastSelectButtonIndex = index;

        switch (index)
        {
            case 0:
                infoMessage = Messages.ItemButton;
                break;
            case 1:
                infoMessage = Messages.EquipButton;
                break;
            case 2:
                infoMessage = Messages.SkillButton;
                break;
            case 3:
                infoMessage = Messages.ClassButton;
                break;
            case 4:
                infoMessage = Messages.StatusButton;
                break;
            case 5:
                infoMessage = Messages.HelpButton;
                break;
            case 6:
                infoMessage = Messages.OptionButton;
                break;
            case 7:
                infoMessage = Messages.SystemButton;
                break;
        }
        if (info != null)
        {
            info.text = infoMessage;
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

    public void EnableInputActionMap(string enableMapName, string disableMapName)
    {
        // 特定の名前のInputActionを探して有効化
        InputActionMap eMap = playerInput.actions.FindActionMap(enableMapName);
        InputActionMap dMap = playerInput.actions.FindActionMap(disableMapName);

        if (eMap != null && dMap != null)
        {
            eMap.Enable();
            dMap.Disable();
        }
    }

    public async UniTask InitializeFromChildren(string fromMenuName)
    {
        header.text = "メインメニュー";
        //EnableInputActionMap("Main", fromMenuName);
        // ステータス設定
        SetAllyes();
        SetInputActions();
        await FadeInChildren(main);
        SelectButton(lastSelectButtonIndex);
    }

    public async UniTask FadeInChildren(GameObject gameObject, float duration = 0.3f)
    {
        // 親GameObjectの子要素すべてに対してフェードインを適用
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            // 透明度を0にアニメーション
            await canvasGroup.DOFade(1, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();
            canvasGroup.interactable = true;
            //gameObject.SetActive(false);
        }
    }

    public async UniTask FadeOutChildren(GameObject gameObject, float duration = 0.5f)
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
