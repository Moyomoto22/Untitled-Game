using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ItemMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public MainMenuController mainMenuController;

    public List<Image> categoryImages;
    private int currentCategoryIndex;

    public GameObject button;

    public Image detailImage;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI category;
    public TextMeshProUGUI description;
    public TextMeshProUGUI rarety;

    public GameObject content;

    [SerializeField]
    private GameObject itemIcon;
    [SerializeField]
    private GameObject itemName;
    [SerializeField]
    private GameObject itemCategory;
    [SerializeField]
    private GameObject itemDescription;
    [SerializeField]
    private GameObject itemDetail;
    [SerializeField]
    private GameObject amount;
    [SerializeField]
    private GameObject rarity;

    public GameObject subWindow;
    public GameObject subWindowInstance;
    public int lastSelectButtonIndex = 0;

    public EventSystem eventSystem;
    public PlayerInput playerInput;

    private bool isClosing;

    // Start is called before the first frame update
    void OnEnable()
    {
        //Initialize();
    }

    private void OnDisable()
    {
        RemoveInputActions();
    }

    private void Update()
    {
        if (content == null)
        {
            Debug.LogWarning("content is null!");
        }
    }

    public async UniTask Initialize()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        eventSystem.enabled = true;
        playerInput = FindObjectOfType<PlayerInput>();

        SetInputActions();

        SetItemDetailInfo(null);

        /// フィルタするカテゴリを初期化
        currentCategoryIndex = 0;
        SetCategoryImage();

        ToggleButtonsInteractable(content);

        await SetItems();
        // 一番上のボタンを選択
        SelectButton(0);
        setButtonFillAmount(0);

        await FadeIn();
        await UniTask.DelayFrame(1);
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
            var rs = actionMap.FindAction("RightShoulder");
            var ls = actionMap.FindAction("LeftShoulder");
            var cancel = actionMap.FindAction("Cancel");
            if (rs == null || ls == null || cancel == null)
            {
                Debug.LogError("Actions not found!");
                return;
            }

            // イベントリスナーを設定
            cancel.performed += OnPressCancelButton;
            rs.performed += OnPressNextButton;
            ls.performed += OnPressPreviousButton;

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
            var cancel = actionMap.FindAction("Cancel");
            if (rs != null && ls != null && cancel != null)
            {
                cancel.performed -= OnPressCancelButton;
                rs.performed -= OnPressNextButton;
                ls.performed -= OnPressPreviousButton;
            }
        }
    }

    /// <summary>
    /// キャンセルボタン押下
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed && !isClosing)
        {
            isClosing = true;
            Debug.Log("cancel button is performing.");
            if (subWindowInstance != null)
            {
                Destroy(subWindowInstance);          
            }
            else
            {
                // アイテムメニューのフェードアウト
                await FadeOutChildren(gameObject, 0.3f);
                if (mainMenuController != null)
                {
                    // メインメニューの初期化
                    await mainMenuController.InitializeFromChildren("Item");
                }
                // アイテムメニューインスタンスの破棄
                gameObject.SetActive(false);
            }
            isClosing = false;
        }
    }

    /// <summary>
    /// 次へ(R1)ボタン押下
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressNextButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("RightShoulderButton has pressed.");
            await DestroyButtons();
            // カテゴリ切り替え
            await NextCategory();
            // 一番上のボタンを選択
            SelectButton(0);
        }
    }

    /// <summary>
    /// 前へ(L1)ボタン押下
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressPreviousButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("LeftShoulderButton has pressed.");
            await DestroyButtons();
            // カテゴリ切り替え
            await PreviousCategory();
            // 一番上のボタンを選択
            SelectButton(0);
        }
    }

    /// <summary>
    /// アイテムボタンを一覧にセット
    /// </summary>
    public async UniTask SetItems()
    {
        await DestroyButtons();

        // フィルタするアイテムカテゴリを取得
        Constants.ItemCategory category = Constants.ItemCategory.Consumable;

        switch (currentCategoryIndex)
        {
            case 0:
                category = Constants.ItemCategory.Consumable;
                break;
            case 1:
                category = Constants.ItemCategory.Material;
                break;
            case 2:
                category = Constants.ItemCategory.Weapon;
                break;
            case 3:
                category = Constants.ItemCategory.Head;
                break;
            case 4:
                category = Constants.ItemCategory.Body;
                break;
            case 5:
                category = Constants.ItemCategory.Accessary;
                break;
            case 6:
                category = Constants.ItemCategory.Misc;
                break;
            default:
                category = Constants.ItemCategory.All;
                break;
        }

        // シングルトンから所持アイテム一覧を取得し、カテゴリでフィルタ
        var items = ItemInventory2.Instance.items;
        List<Item> filteredItems = new List<Item>();
        if (category != Constants.ItemCategory.All)
        {
            filteredItems = items.Where(x => x.itemCategory == category).ToList();
        }
        else
        {
            filteredItems = items;
        }
        var sortedItems = filteredItems.OrderBy(x => x.ID).ToList();

        HashSet<string> processedItemIds = new HashSet<string>();  // 登録済みアイテムのIDを記録するためのHashSet

        // 所持アイテムを一覧にボタンとしてセットしていく
        foreach (Item item in sortedItems)
        {
            if (processedItemIds.Contains(item.ID))
            {
                continue;  // このアイテムIDがすでに処理されていれば、次のアイテムへスキップ
            }
            //var content = GameObject.FindWithTag("ScrollViewContent");
            GameObject obj = Instantiate(button, content.transform, false);    // 一覧に表示するボタンのベースをインスタンス生成
            var comp = obj.GetComponent<ItemComponent>();                      // ボタンに紐づくスキル情報を格納するコンポーネント
            var newButton = obj.transform.GetChild(0).gameObject;              // ボタン本体
            var amount = items.Where(i => i.ID == item.ID).ToList().Count;     // アイテム所持数

            comp.icon.sprite = item.iconImage;                                 // アイコン
            comp.itemName.text = item.itemName;                                // アイテム名称
            comp.amount.text = amount.ToString();                              // 所持数
            AddSelectOrDeselectActionToButtons(newButton, item);               // 選択・選択解除時アクション設定
            // アイテムが使用可能か判定
            if (item.usable)
            {
                // ボタン押下時のアクションを追加
                AddOnClickActionToItemButton(newButton, item);
            }
            else
            {
                // ボタンタイトルをグレーアウト
                comp.itemName.color = CommonController.GetColor(Constants.darkGray);
            }
            processedItemIds.Add(item.ID);                                     // このアイテムIDを処理済みとして記録
        }
        await UniTask.DelayFrame(1);
    }

    /// <summary>
    /// ボタンを選択状態にする
    /// </summary>
    /// <param name="number"></param>
    public void SelectButton(int number = 0)
    {
        if (eventSystem != null && content.transform.childCount > 0)
        {
            var buttonToSelect = content.transform.GetChild(number).GetChild(0).gameObject;
            var buttonTitle = content.transform.GetChild(number).GetChild(2).GetComponent<TextMeshProUGUI>().text;
            Debug.Log(buttonTitle);
            eventSystem.SetSelectedGameObject(buttonToSelect);
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
        // 対象が味方かつ単体の時
        if (item.target == 2 && !item.isTargetAll)
        {
            DisplaySubMenu(item);
        }
        else if (item.target == 2 && item.isTargetAll)
        {

        }

    }

    /// <summary>
    /// キャラクター選択サブメニューを表示する
    /// </summary>
    /// <param name="item"></param>
    private void DisplaySubMenu(Item item)
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
            controller.item = item;
            controller.itemMenuController = this;
        }
        ToggleButtonsInteractable(false);
        setButtonFillAmount(lastSelectButtonIndex);
    }

    /// <summary>
    /// ボタンのInteractableを切り替える
    /// </summary>
    /// <param name="interactable">有効/無効</param>
    public void ToggleButtonsInteractable(bool interactable)
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Transform child = content.transform.GetChild(i);
            Button button = child.GetComponentInChildren<Button>();

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
        int numberOfChildren = content.transform.childCount;

        // 対象インデックスに該当するボタンのみFillAmountを1にし、それ以外は0にする
        for (int i = 0; i < numberOfChildren - 1; i++)
        {
            int fillAmount = i == number ? 1 : 0;
            Transform child = content.transform.GetChild(i);
            Image buttonImage = child.GetComponentInChildren<Image>();
            buttonImage.fillAmount = fillAmount;
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
    /// 詳細欄にアイテムの詳細を表示する
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

    private void SetCategoryImage()
    {
        foreach (var image in categoryImages)
        {
            image.color = CommonController.GetColor(Constants.darkGray);
        }
        categoryImages[currentCategoryIndex].color = CommonController.GetColor(Constants.white);
    }

    /// <summary>
    /// スキルカテゴリ切り替え - 次ページ
    /// </summary>
    public async UniTask NextCategory()
    {
        currentCategoryIndex = (currentCategoryIndex + 1) % categoryImages.Count;

        SetCategoryImage();
        await SetItems();
    }

    /// <summary>
    /// スキルカテゴリ切り替え - 前ページ
    /// </summary>
    public async UniTask PreviousCategory()
    {
        currentCategoryIndex = (currentCategoryIndex - 1 + categoryImages.Count) % categoryImages.Count;

        SetCategoryImage();
        await SetItems();
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
        }
    }
}
