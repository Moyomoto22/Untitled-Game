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

    private GameObject selectedButton;

    public GameObject inputActionParent;
    private InputAction inputAction;

    public PlayerInput playerInput;

    int showingCharacterID = 1;

    StatusMenuController statusMenuController;

    //private VCamManager vcm = new VCamManager();

    // Start is called before the first frame update
    void Start()
    {
    }

    async void OnEnable()
    {
    }

    private async UniTaskVoid Awake()
    {
        DOTween.Init();
        await Initialize();
    }

    void OnDestroy()
    {
        // イベントリスナーを解除
        if (playerInput != null)
        {
            // InputActionAssetを取得
            var inputActionAsset = playerInput.actions;
            // "Main"アクションマップを取得
            var actionMap = inputActionAsset.FindActionMap("Main");
            // アクションを取得
            var cancel = actionMap.FindAction("Cancel");
            if (cancel != null)
            {
                cancel.performed -= OnPressCancelButton;
            }
        }
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
        SetAllyStatuses();

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
            var actionMap = inputActionAsset.FindActionMap("Main");
            // アクションを取得
            var cancel = actionMap.FindAction("Cancel");
            if (cancel == null)
            {
                Debug.LogError("Actions not found!");
                return;
            }

            // イベントリスナーを設定
            cancel.performed += context => OnPressCancelButton(context);

            // アクションマップを有効にする
            actionMap.Enable();
        }
    }

    private void SetAllyStatuses()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        List<AllyStatus> allies = PartyMembers.Instance.GetAllies();

        for (int i = 0; i < allies.Count; i++)
        {
            var ally = allies[i];

            characterImages[i].sprite = ally.Class.imagesB[i];                    // キャラクター画像
            classNames[i].text = ally.Class.classAbbreviation;
            levels[i].text = ally.level.ToString();
            ally.HPGauge = HPGauges[i];                                           // HPゲージ
            ally.MPGauge = MPGauges[i];                                           // MPゲージ
            ally.TPGauge = TPGauges[i];                                           // TPゲージ

            GaugeManager hpGaugeManager = HPGauges[i].GetComponent<GaugeManager>();     // HPゲージ管理クラス
            GaugeManager mpGaugeManager = MPGauges[i].GetComponent<GaugeManager>();     // MPゲージ管理クラス
            GaugeManager tpGaugeManager = TPGauges[i].GetComponent<GaugeManager>();     // TPゲージ管理クラス
            GaugeManager expGaugeManager = EXPGauges[i].GetComponent<GaugeManager>();     // EXPゲージ管理クラス

            hpGaugeManager.maxValueText.text = ally.maxHp2.ToString();            // 最大HPテキスト
            hpGaugeManager.currentValueText.text = ally.hp.ToString();            // 現在HPテキスト
            mpGaugeManager.maxValueText.text = ally.maxMp2.ToString();            // 最大MPテキスト
            mpGaugeManager.currentValueText.text = ally.mp.ToString();            // 現在MPテキスト
            tpGaugeManager.currentValueText.text = ally.tp.ToString();            // 現在TPテキスト
            expGaugeManager.maxValueText.text = ally.nextExperience.ToString();            // 最大EXPテキスト
            expGaugeManager.currentValueText.text = ally.nextExperience.ToString();            // 現在EXPテキスト

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

        foreach(GameObject button in buttons)
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
    public void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (characterSelectSubMenuInstance != null)
            {
                Destroy(characterSelectSubMenuInstance);
            }
            else
            {
            }
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
    public void CloseMenu()
    {
        // プレイヤーを動かせるようにする
        CommonVariableManager.playerCanMove = true;
        
        CommonController.ResumeGame();
        MenuOutline.SetActive(false);

        CommonVariableManager.ShowingMenuState = Constants.MenuState.Main;
        CloseScreen();
        ShowScreen(0);
    }

    /// <summary>
    /// メインメニュー Itemボタン押下時
    /// アイテム画面遷移処理
    /// </summary>
    public async void OnPressItemButton()
    {
        await FadeOutChildren(main);

        header.text = "アイテム";

        if (itemMenuInstance == null)
        {
            itemMenuInstance = Instantiate(itemMenuObject, transform);
            var itemMenuController = itemMenuInstance.GetComponent<ItemMenuController>();
            itemMenuController.mainMenu = main;
            itemMenuController.mainMenuController = this;
        }
    }

    public void OnPressEquipButton()
    {
        DisplayCharacterSelectSubMenu(1);

        
    }

    public async UniTask GoToEquipMenu(int characterIndex)
    {
        Destroy(characterSelectSubMenuInstance);
        await FadeOutChildren(main);
        
        header.text = "装備";

        if (equipMenuInstance == null)
        {
            equipMenuInstance = Instantiate(equipMenuObject, transform);
            var controller = equipMenuInstance.GetComponent<EquipMenuController>();
            controller.mainMenu = main;
            controller.mainMenuController = this;
            controller.currentCharacterID = characterIndex;
        }
    }

    public void OnPressSkillButton()
    {
        CommonVariableManager.ShowingMenuState = Constants.MenuState.Skill;
        selectedButton = skillButton;

        CommonController.DisableInputActionMap(inputActionParent, "Player");

        CloseScreen();
        header.text = "スキル";
        ShowScreen((int)CommonVariableManager.ShowingMenuState);
    }

    public void OnPressClassButton()
    {
        CommonVariableManager.ShowingMenuState = Constants.MenuState.Class;
        selectedButton = classButton;

        CommonController.DisableInputActionMap(inputActionParent, "Player");

        CloseScreen();
        header.text = "クラス";
        ShowScreen((int)CommonVariableManager.ShowingMenuState);
    }

    /// <summary>
    /// メインメニュー Statusボタン押下時
    /// アイテム画面遷移処理
    /// </summary>
    public void OnPressStatusButton()
    {
        CommonVariableManager.ShowingMenuState = Constants.MenuState.Status;
        selectedButton = statusButton;

        CommonController.DisableInputActionMap(inputActionParent, "Player");

        CloseScreen();
        header.text = "ステータス";
        ShowScreen((int)CommonVariableManager.ShowingMenuState);
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

    public async UniTask InitializeFromChildren()
    {
        header.text = "メインメニュー";
        // ステータス設定
        SetAllyStatuses();
        await FadeInChildren(main);
        SelectButton();
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
        // 親GameObjectの子要素すべてに対してフェードアウトを適用
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            // 透明度を0にアニメーション
            await canvasGroup.DOFade(0, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();
            canvasGroup.interactable = false;
            //gameObject.SetActive(false);
        }
    }
}
