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

public class EquipMenuController : MonoBehaviour
{
    private List<string> skillCategoryNames = new List<string>() { "アレックス", "ニコ", "タバサ", "アリシア" };
    public GameObject mainMenu;
    public MainMenuController mainMenuController;
    public int currentCharacterID;


    private TextMeshProUGUI[] allTMP;

    string path = "Assets/Data/Item/ItemInventory.prefab";

    public GameObject MainScreen;
    public GameObject EquipScreen;

    // キャラクターステータス
    private AllyStatus allyStatus;

    // キャラクター画像
    [SerializeField]
    private GameObject characterImage;

    [SerializeField]
    private GameObject characterName;
    [SerializeField]
    private GameObject classAbbreviation;
    [SerializeField]
    private GameObject characterLevel;

    // 装備欄
    public GameObject equipColumn;
    // ステータス欄
    public GameObject statusColumn;

    public GameObject gradation;

    public GameObject weaponIcons;
    private List<GameObject> wIcons = new List<GameObject>();
    public GameObject armourIcons;
    private List<GameObject> aIcons = new List<GameObject>();

    public GameObject rblb;

    // 右上武器・防具欄 スクロールビュー内
    public GameObject content;

    public GameObject equipObject;

    [SerializeField]
    private GameObject itemIcon;
    [SerializeField]
    private GameObject itemName;
    [SerializeField]
    private GameObject itemCategory;
    [SerializeField]
    private GameObject itemDescription;

    // 属性アイコン
    [SerializeField]
    private GameObject attributes;

    public Sprite Slash;
    public Sprite Thrust;
    public Sprite Blow;
    public Sprite Magic;
    public Sprite Fire;
    public Sprite Ice;
    public Sprite Thunder;
    public Sprite Wind;

    // 装備可能クラス
    [SerializeField]
    private GameObject classes;

    [SerializeField]
    public GameObject subWindow;

    [SerializeField]
    public EventSystem eventSystem;

    [SerializeField]
    private InputAction inputActions;

    private GameObject lastSelectedButton;
    private int lastSelectButtonIndex;
    private int lastSelectButtonIndexParent;

    CommonController commonController = new CommonController();

    // 右上アイテム一覧 フィルタ状態
    private Constants.ItemCategory bigFilterState;
    private int smallFilterState = 0;

    public GameObject inputActionParent;

    public Sprite buttonImageOff;
    public Sprite buttonImageOn;

    public GameObject subWindowHPMP;

    private ItemInventory itemInventory;

    private string selectedItemID;
    private ItemDatabase itemDatabase;

    // 選択カーソルが装備欄にあるか
    private bool isSelectEquipColumn = true;

    public GameObject scrollView;
    private ScrollViewManager2 scrollViewManager;

    // Start is called before the first frame update
    void Start()
    {
        //InitializeItemList();
        // アイテムデータベースを取得
        itemDatabase = FindObjectsOfType<CommonController>()[0].itemDatabase;
        // 【要削除！】アイテムインベントリアイテム追加
        CommonController.GetAllItems();

        wIcons = CommonController.GetChildrenGameObjects(weaponIcons);
        aIcons = CommonController.GetChildrenGameObjects(armourIcons);

        scrollViewManager = scrollView.GetComponent<ScrollViewManager2>();
        scrollViewManager.enabled = false;
    }

    private async void OnEnable()
    {
        // アイテム一覧のフィルタ枠を片手剣フィルタに
        if (wIcons.Count > 0 && aIcons.Count > 0)
        {
            foreach (GameObject icon in wIcons)
            {
                icon.SetActive(true);
                icon.GetComponent<Image>().color = Color.gray;
            }
            foreach (GameObject icon in aIcons)
            {
                icon.SetActive(false);
            }
            wIcons[0].GetComponent<Image>().color = Color.white;
        }

        bigFilterState = Constants.ItemCategory.Weapon;
        smallFilterState = 0;

        //allyStatus = await CommonController.GetAllyStatus(showingCharacterID);

        // アイテム一覧をAdressableから取得
        GameObject obj = await Addressables.LoadAssetAsync<GameObject>(path).Task;
        itemInventory = obj.GetComponent<ItemInventory>();

        // サブウインドウを非表示
        //subWindow.SetActive(false);

        // 装備メニュー用のアクションマップを有効化
        CommonController.EnableInputActionMap(inputActionParent, "EquipMenu");

        // 装備欄を選択可に
        ToggleInteractableButtonsInChildren(equipColumn.transform, true);
        // 右上アイテム一覧を選択不可に
        ToggleInteractableButtonsInChildren(content.transform, false);

        isSelectEquipColumn = true;

        // 画面初期化
        await InitializeEquipMenu();

        // 右上武器欄初期化
        await InitializeEquipItemList();

        selectedItemID = null;
        lastSelectButtonIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // EventSystemの現在選択されているGameObjectを取得
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;

        // 選択されたオブジェクトが変更された場合
        if (selectedButton != lastSelectedButton)
        {
            // 選択されたオブジェクトがButtonであるか確認
            Button button = selectedButton?.GetComponent<Button>();
            if (button != null)
            {
                // 右下にアイテム詳細を表示
                SetItemInformation(button.transform.parent);
            }

            //!selectedButton.CompareTag("ButtoninSubWIndow")
            lastSelectedButton = selectedButton;

            // 装備欄選択中の場合
            if (isSelectEquipColumn)
            {
                lastSelectButtonIndex = lastSelectedButton.transform.parent.transform.GetSiblingIndex();
                // 上から選択された行に応じて右上アイテム一覧の表示内容を切り替える
                Transform transform = EventSystem.current.currentSelectedGameObject.transform;

                // 1・2(右手・左手) ⇒ 武器
                if (transform.parent.transform.GetSiblingIndex() <= 1)
                {
                    bigFilterState = Constants.ItemCategory.Weapon;
                    
                    weaponIcons.SetActive(true);
                    foreach (GameObject icon in aIcons)
                    {
                        icon.SetActive(false);
                    }
                }
                // 3(頭)
                if (transform.parent.transform.GetSiblingIndex() == 2)
                {
                    bigFilterState = Constants.ItemCategory.Head;
                    
                    weaponIcons.SetActive(false);
                    foreach (GameObject icon in aIcons)
                    {
                        icon.SetActive(false);
                    }
                    aIcons[0].SetActive(true);
                }
                // 4(胴)
                if (transform.parent.transform.GetSiblingIndex() == 3)
                {
                    bigFilterState = Constants.ItemCategory.Body;

                    weaponIcons.SetActive(false);
                    foreach (GameObject icon in aIcons)
                    {
                        icon.SetActive(false);
                    }
                    aIcons[1].SetActive(true);
                }
                // 5・6(装飾品)
                if (transform.parent.transform.GetSiblingIndex() >= 4)
                {
                    bigFilterState = Constants.ItemCategory.Accessary;

                    weaponIcons.SetActive(false);
                    foreach (GameObject icon in aIcons)
                    {
                        icon.SetActive(false);
                    }
                    aIcons[2].SetActive(true);
                }

                InitializeEquipItemList();

            }
            else if (button != null)
            {
                List<GameObject> statuses = CommonController.GetChildrenGameObjects(statusColumn);
                CompareStatus(statuses, allyStatus, button.transform.parent);
            }
        }

        if (CommonController.IsChildOf(content, selectedButton))
        {
            scrollViewManager.enabled = true;
        }
        else
        {
            scrollViewManager.enabled = false;
        }
    }

    /// <summary>
    /// 装備画面初期化
    /// </summary>
    public async Task InitializeEquipMenu()
    {
        string color = Constants.gradationBlue;

        switch (1)
        {
            case 1:
                color = Constants.gradationBlue;
                break;
            case 2:
                color = Constants.gradationRed;
                break;
            case 3:
                color = Constants.gradationPurple;
                break;
            case 4:
                color = Constants.gradationGreen;
                break;
            default:
                color = Constants.gradationBlue;
                break;
        }

        gradation.GetComponent<Image>().color = CommonController.GetColor(color);
        //allyStatus = await CommonController.GetAllyStatus(showingCharacterID);

        //characterImage.GetComponent<Image>().sprite = allyStatus.Class.imagesA[showingCharacterID - 1];

        characterName.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.characterName;
        characterLevel.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.level.ToString();
        classAbbreviation.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.Class.classAbbreviation;

        rblb.SetActive(false);

        InitializeEquipColumn(allyStatus);

        InitializeStatusColumn(allyStatus);
    }

    /// <summary>
    /// 装備欄リスト初期化
    /// </summary>
    public void InitializeEquipColumn(AllyStatus status)
    {
        List<GameObject> equips = CommonController.GetChildrenGameObjects(equipColumn);

        if (equips != null)
        {
            SetInfoToEquipColumn(equips, status);
        }

        foreach (GameObject obj in equips)
        {
            GameObject button = obj.transform.Find("Button").gameObject;
            button.GetComponent<Image>().fillAmount = 0;
        }

        // 装備欄 - 右手を選択
        GameObject rightArmButton = equips[0].transform.Find("Button").gameObject;
        if (rightArmButton != null)
        {
            eventSystem.SetSelectedGameObject(rightArmButton);
        }
        rightArmButton.GetComponent<Image>().fillAmount = 1;


    }

    /// <summary>
    /// ステータス欄初期化
    /// </summary>
    public void InitializeStatusColumn(AllyStatus status)
    {
        List<GameObject> statuses = CommonController.GetChildrenGameObjects(statusColumn);

        for (int i = 12; i < statuses.Count; i++)
        {
            statuses[i].SetActive(false);
        }

        if (statuses != null)
        {
            SetInfoToStatusColumn(statuses, status);
        }
    }

    /// <summary>
    /// 右上武器・防具リスト初期化
    /// </summary>

    /// <summary>
    /// キャンセルボタン押下時処理
    /// </summary>
    public async void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Equip)
            {

                // EventSystemの現在選択されているGameObjectを取得
                GameObject selectedButton = EventSystem.current.currentSelectedGameObject;

                // 装備欄選択中の場合 ⇒ メイン画面に戻る
                if (CommonController.IsChildOf(equipColumn, selectedButton))
                {
                    EquipScreen.SetActive(false);

                    CommonVariableManager.ShowingMenuState = Constants.MenuState.Main;
                    MainScreen.SetActive(true);
                }
                // アイテム一覧選択中の場合 ⇒ 装備欄に戻る
                else if (EquipScreen.activeSelf && CommonController.IsChildOf(content, selectedButton))
                {
                    isSelectEquipColumn = true;

                    ToggleInteractableButtonsInChildren(content.transform, false);
                    ToggleInteractableButtonsInChildren(equipColumn.transform, true);

                    await InitializeEquipMenu();

                    // 記憶した行を選択
                    SelectTargetIndexItem(equipColumn, lastSelectButtonIndex);
                }
            }
        }
    }

    /// <summary>
    /// □ボタン(Y・Xボタン)押下時処理
    /// </summary>
    public async void OnPressNorthButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // EventSystemの現在選択されているGameObjectを取得
            GameObject selectedButton = EventSystem.current.currentSelectedGameObject;

            // 装備欄選択中の場合動作
            if (CommonController.IsChildOf(equipColumn, selectedButton))
            {
                // 装備を外す
                allyStatus.Unequip(CommonController.GetSiblingIndexInParent(selectedButton.transform.parent));
                await InitializeEquipMenu();

                // 記憶した行を選択
                SelectTargetIndexItem(equipColumn, lastSelectButtonIndex);
            }
        }
    }

    public async Task InitializeEquipItemList(bool isSelectTarget = true)
    {
        // アイテム一覧内のオブジェクトを削除(0.01秒待機)
        await DestroyChildren(content, "EquipObject");

        // 一番上へスクロール
        scrollViewManager.ScrollScrollView(1.0f);

        // アイテム一覧をAdressableから取得
        //List<Item> itemList = itemInventory.itemInventory;

        if (itemInventory != null)
        {
            List<Item> filteredItemList;
            switch (bigFilterState)
            {
                case Constants.ItemCategory.Weapon:
                    filteredItemList = itemInventory.itemInventory.OrderBy(x => x.ID).Where(x => x.itemCategory == Constants.ItemCategory.Weapon).ToList();
                    break;
                case Constants.ItemCategory.Head:
                    filteredItemList = itemInventory.itemInventory.OrderBy(x => x.ID).Where(x => x.itemCategory == Constants.ItemCategory.Head).ToList();
                    break;
                case Constants.ItemCategory.Body:
                    filteredItemList = itemInventory.itemInventory.OrderBy(x => x.ID).Where(x => x.itemCategory == Constants.ItemCategory.Body).ToList();
                    break;
                case Constants.ItemCategory.Accessary:
                    filteredItemList = itemInventory.itemInventory.OrderBy(x => x.ID).Where(x => x.itemCategory == Constants.ItemCategory.Accessary).ToList();
                    break;
                default:
                    filteredItemList = itemInventory.itemInventory.OrderBy(x => x.ID).Where(x => x.itemCategory == Constants.ItemCategory.Weapon).ToList();
                    //filteredWeaponList = weaponList.OrderBy(x => x.ID).Where(x => x.weaponCategory == smallFilterState).ToList();
                    break;
            }

            if (bigFilterState == Constants.ItemCategory.Weapon)
            {
                List<Weapon> weapons = new List<Weapon>();
                foreach (var item in filteredItemList)
                {
                    Weapon weapon = item as Weapon;
                    if (weapon != null)
                    {
                        weapons.Add(weapon);
                    }
                }

                weapons = weapons.OrderBy(x => x.ID).Where(x => x.weaponCategory == GetWeaponCategoryValue(smallFilterState)).ToList();

                int count = 0;
                foreach (var item in weapons)
                {
                    // 各アイテムの情報を格納
                    GameObject temp = Instantiate(equipObject);

                    temp.transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = item;
                    temp.transform.Find("Icon").GetComponent<Image>().sprite = item.iconImage;

                    Transform button = temp.transform.Find("Button");
                    GameObject equipMark = button.Find("EquipMark").gameObject;
                    equipMark.SetActive(false);

                    // 装備可能かチェック
                    Task<int> task = CommonController.CheckWhoEquiped(item);
                    int a = await task;

                    if (CommonController.CheckEquippable(allyStatus, item, lastSelectButtonIndex) && (a == 0 && a != 1))//showingCharacterID))
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
                        button.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                        button.GetComponent<Button>().onClick.AddListener(() => OnPressEquipItemButton(item));
                    }
                    else
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
                        button.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.gray);
                        if (a != 0)
                        {
                            equipMark.SetActive(true);
                            switch (a)
                            {
                                case 1:
                                    equipMark.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.blue);
                                    break;
                                case 2:
                                    equipMark.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.red);
                                    break;
                                case 3:
                                    equipMark.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.purple);
                                    break;
                                default:
                                    equipMark.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.green);
                                    break;
                            }
                        }
                    }
                    count++;

                    temp.transform.parent = content.transform;
                }
                Debug.Log(count);
                // 枠の位置を切り替え
                //foreach (GameObject outline in InnerOutlines)
                //{
                //    outline.SetActive(false);
                //}
                //InnerOutlines[smallFilterState].SetActive(true);
            }
            else
            {
                foreach (var item in filteredItemList)
                {
                    // 各アイテムの情報を格納
                    GameObject temp = Instantiate(equipObject);

                    temp.transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = item;
                    temp.transform.Find("Icon").GetComponent<Image>().sprite = item.iconImage;

                    Transform button = temp.transform.Find("Button");
                    GameObject equipMark = button.Find("EquipMark").gameObject;
                    equipMark.SetActive(false);

                    // 装備可能かチェック
                    Task<int> task = CommonController.CheckWhoEquiped(item);
                    int a = await task;

                    if (CommonController.CheckEquippable(allyStatus, item, lastSelectButtonIndex) && (a == 0 && a != 1))//showingCharacterID))
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
                        button.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                        button.GetComponent<Button>().onClick.AddListener(() => OnPressEquipItemButton(item));
                    }
                    else
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
                        button.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.gray);
                        if (a != 0)
                        {
                            equipMark.SetActive(true);
                            switch (a)
                            {
                                case 1:
                                    equipMark.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.blue);
                                    break;
                                case 2:
                                    equipMark.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.red);
                                    break;
                                case 3:
                                    equipMark.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.purple);
                                    break;
                                default:
                                    equipMark.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.green);
                                    break;
                            }
                        }
                    }

                    temp.transform.parent = content.transform;
                }

                // 枠を一番左に
                //foreach (GameObject outline in InnerOutlines)
                //{
                //    outline.SetActive(false);
                //}
                //InnerOutlines[0].SetActive(true);
                //bigFilterState = Constants.ItemCategory.Weapon;
                //smallFilterState = 0;
            }
            if (isSelectTarget)
            {
                // SelectTargetIndexItem();
            }
            else if (lastSelectedButton != null)
            {
                //lastSelectedButton.GetComponent<Image>().sprite = buttonImageOn;
            }
        }
    }

    /// <summary>
    /// R1(R・RB)ボタン押下
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressRightShoulderButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // EventSystemの現在選択されているGameObjectを取得
            GameObject selectedButton = EventSystem.current.currentSelectedGameObject;

            // 武器一覧選択中の場合のみ動作
            if (CommonController.IsChildOf(content, selectedButton))
            {
                if (bigFilterState == Constants.ItemCategory.Weapon)
                {
                    smallFilterState = smallFilterState + 1;

                    if (smallFilterState >= wIcons.Count)
                    {
                        smallFilterState = 0;
                    }

                    foreach (GameObject icon in wIcons)
                    {
                        icon.GetComponent<Image>().color = Color.gray;
                    }
                    wIcons[smallFilterState].GetComponent<Image>().color = Color.white;

                    await InitializeEquipItemList();

                    SelectTargetIndexItem(content, 0);
                    ToggleInteractableButtonsInChildren(content.transform, true);
                }
            }
        }
    }

    /// <summary>
    /// L1(L・LB)ボタン押下
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressLeftShoulderButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // EventSystemの現在選択されているGameObjectを取得
            GameObject selectedButton = EventSystem.current.currentSelectedGameObject;

            // 武器一覧選択中の場合のみ動作
            if (CommonController.IsChildOf(content, selectedButton))
            {
                if (bigFilterState == Constants.ItemCategory.Weapon)
                {
                    smallFilterState = smallFilterState - 1;

                    if (smallFilterState < 0)
                    {
                        smallFilterState = wIcons.Count - 1;
                    }

                    foreach (GameObject icon in wIcons)
                    {
                        icon.GetComponent<Image>().color = Color.gray;
                    }
                    wIcons[smallFilterState].GetComponent<Image>().color = Color.white;

                    await InitializeEquipItemList();

                    SelectTargetIndexItem(content, 0);
                    ToggleInteractableButtonsInChildren(content.transform, true);
                }
            }
        }
    }

    /// <summary>
    /// R2(RZ・RT)ボタン押下
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressRightTriggerButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isSelectEquipColumn)
            {
                //showingCharacterID = showingCharacterID + 1;
                //if (showingCharacterID > 4)
                //{
                //    showingCharacterID = 1;
                //}
                await InitializeEquipMenu();
                await InitializeEquipItemList();
            }
        }
    }

    /// <summary>
    /// L2(LZ・LT)ボタン押下
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressLeftTriggerButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isSelectEquipColumn)
            {
                //showingCharacterID = showingCharacterID - 1;
                //if (showingCharacterID < 1)
                //{
                //    showingCharacterID = 4;
                //}
                await InitializeEquipMenu();
                await InitializeEquipItemList();
            }
        }
    }

    /// <summary>
    /// オブジェクト配下の指定の順序のアイテムを選択する
    /// </summary>
    public void SelectTargetIndexItem(GameObject target, int index = 0)
    {
        if (target != null && content.transform.childCount > 0)
        {
            if (index > target.transform.childCount)
            {
                index = 0;
            }

            Transform targetItemTF = target.transform.GetChild(index);

            if (targetItemTF != null)
            {
                Transform firstItemButton = targetItemTF.Find("Button");

                if (firstItemButton != null)
                {
                    GameObject gameObject = firstItemButton.gameObject;
                    Button button = gameObject.GetComponent<Button>();
                    eventSystem.SetSelectedGameObject(gameObject);
                }
                //SetItemInformation(targetItemTF);
            }
        }
    }

    /// <summary>
    /// 選択されたアイテムの情報を表示
    /// </summary>
    /// <param name="transform"></param>
    private void SetItemInformation(Transform transform)
    {
        List<GameObject> icons = CommonController.GetChildrenGameObjects(attributes);
        List<GameObject> classList = CommonController.GetChildrenGameObjects(classes);
        Color white = CommonController.GetColor("#FFFFFF");
        Color gray = CommonController.GetColor("#808080");

        foreach (var icon in icons)
        {
            icon.SetActive(false);
        }

        foreach (var Class in classList)
        {
            Class.GetComponent<TextMeshProUGUI>().color = gray;
        }

        itemIcon.SetActive(false);
        itemName.SetActive(false);
        itemCategory.SetActive(false);
        itemDescription.SetActive(false);

        if (transform != null)
        {
            if (transform.Find("ItemInfo") != null)
            {
                if (transform.Find("ItemInfo").gameObject.GetComponent<ItemInfo>().itemInfo != null)
                {
                    Item item = transform.Find("ItemInfo").gameObject.GetComponent<ItemInfo>().itemInfo;
                    if (item != null)
                    {
                        itemIcon.SetActive(true);
                        itemName.SetActive(true);
                        itemCategory.SetActive(true);
                        itemDescription.SetActive(true);

                        itemIcon.GetComponent<Image>().sprite = item.iconImage;
                        itemName.GetComponent<TextMeshProUGUI>().text = item.itemName;

                        if (item is Weapon)
                        {
                            Weapon weapon = item as Weapon;
                            itemCategory.GetComponent<TextMeshProUGUI>().text = CommonController.GetWeaponCategoryString(weapon.weaponCategory);


                            for (int i = 0; i < weapon.attributes.Count; i++)
                            {
                                Sprite image = Slash;
                                switch (weapon.attributes[i])
                                {
                                    case Constants.Attribute.Slash:
                                        image = Slash;
                                        break;
                                    case Constants.Attribute.Thrust:
                                        image = Thrust;
                                        break;
                                    case Constants.Attribute.Blow:
                                        image = Blow;
                                        break;
                                    case Constants.Attribute.Magical:
                                        image = Magic;
                                        break;
                                    case Constants.Attribute.Fire:
                                        image = Fire;
                                        break;
                                    case Constants.Attribute.Ice:
                                        image = Ice;
                                        break;
                                    case Constants.Attribute.Thunder:
                                        image = Thunder;
                                        break;
                                    case Constants.Attribute.Wind:
                                        image = Wind;
                                        break;
                                }
                                icons[i].SetActive(true);
                                icons[i].GetComponent<Image>().sprite = image;
                            }

                            if (weapon.equipableClasses.Exists(x => x.className == "ウォリアー"))
                            {
                                classList[0].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (weapon.equipableClasses.Exists(x => x.className == "パラディン"))
                            {
                                classList[1].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (weapon.equipableClasses.Exists(x => x.className == "モンク"))
                            {
                                classList[2].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (weapon.equipableClasses.Exists(x => x.className == "シーフ"))
                            {
                                classList[3].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (weapon.equipableClasses.Exists(x => x.className == "レンジャー"))
                            {
                                classList[4].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (weapon.equipableClasses.Exists(x => x.className == "ソーサラー"))
                            {
                                classList[5].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (weapon.equipableClasses.Exists(x => x.className == "クレリック"))
                            {
                                classList[6].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (weapon.equipableClasses.Exists(x => x.className == "スペルソード"))
                            {
                                classList[7].GetComponent<TextMeshProUGUI>().color = white;
                            }
                        }
                        else if (item is Head)
                        {
                            Head head = item as Head;

                            if (head.equipableClasses.Exists(x => x.className == "ウォリアー"))
                            {
                                classList[0].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (head.equipableClasses.Exists(x => x.className == "パラディン"))
                            {
                                classList[1].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (head.equipableClasses.Exists(x => x.className == "モンク"))
                            {
                                classList[2].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (head.equipableClasses.Exists(x => x.className == "シーフ"))
                            {
                                classList[3].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (head.equipableClasses.Exists(x => x.className == "レンジャー"))
                            {
                                classList[4].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (head.equipableClasses.Exists(x => x.className == "ソーサラー"))
                            {
                                classList[5].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (head.equipableClasses.Exists(x => x.className == "クレリック"))
                            {
                                classList[6].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (head.equipableClasses.Exists(x => x.className == "スペルソード"))
                            {
                                classList[7].GetComponent<TextMeshProUGUI>().color = white;
                            }

                            itemCategory.GetComponent<TextMeshProUGUI>().text = CommonController.GetItemCategoryString(item.itemCategory);
                        }
                        else if (item is Body)
                        {
                            Body body = item as Body;

                            if (body.equipableClasses.Exists(x => x.className == "ウォリアー"))
                            {
                                classList[0].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (body.equipableClasses.Exists(x => x.className == "パラディン"))
                            {
                                classList[1].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (body.equipableClasses.Exists(x => x.className == "モンク"))
                            {
                                classList[2].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (body.equipableClasses.Exists(x => x.className == "シーフ"))
                            {
                                classList[3].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (body.equipableClasses.Exists(x => x.className == "レンジャー"))
                            {
                                classList[4].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (body.equipableClasses.Exists(x => x.className == "ソーサラー"))
                            {
                                classList[5].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (body.equipableClasses.Exists(x => x.className == "クレリック"))
                            {
                                classList[6].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            if (body.equipableClasses.Exists(x => x.className == "スペルソード"))
                            {
                                classList[7].GetComponent<TextMeshProUGUI>().color = white;
                            }
                            itemCategory.GetComponent<TextMeshProUGUI>().text = CommonController.GetItemCategoryString(item.itemCategory);
                        }
                        else
                        {
                            foreach (var Class in classList)
                            {
                                Class.GetComponent<TextMeshProUGUI>().color = white;
                            }
                            itemCategory.GetComponent<TextMeshProUGUI>().text = CommonController.GetItemCategoryString(item.itemCategory);
                        }
                        itemDescription.GetComponent<TextMeshProUGUI>().text = item.description;


                    }
                }
                else
                {

                }
            }
        }
    }

    /// <summary>
    /// 装備欄押下時
    /// </summary>
    public void OnPressEquipColumnButton()
    {
        // カーソル位置を記憶するため、選択中の行のインデックスを保存
        Transform transform = EventSystem.current.currentSelectedGameObject.transform;
        lastSelectButtonIndex = transform.parent.transform.GetSiblingIndex();

        ToggleInteractableButtonsInChildren(equipColumn.transform, false);
        ToggleInteractableButtonsInChildren(content.transform, true);

        isSelectEquipColumn = false;

        if (bigFilterState == Constants.ItemCategory.Weapon)
        {
            rblb.SetActive(true);
        }

        // 右上アイテム一覧の先頭を選択
        SelectTargetIndexItem(content);
    }

    /// <summary>
    /// 右上アイテム一覧押下時
    /// </summary>
    /// <param name="item"></param>
    public void OnPressEquipItemButton(Item item)
    {

        // アイテム選択時
        if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Equip && !isSelectEquipColumn)
        {

            if (item != null)
            {
                // 装備中アイテムを解除
                Equip bEquip = new Equip();
                switch (lastSelectButtonIndex)
                {
                    case 0:
                        bEquip = allyStatus.rightArm;
                        break;
                    case 1:
                        bEquip = allyStatus.leftArm;
                        break;
                    case 2:
                        bEquip = allyStatus.head;
                        break;
                    case 3:
                        bEquip = allyStatus.body;
                        break;
                    case 4:
                        bEquip = allyStatus.accessary1;
                        break;
                    case 5:
                        bEquip = allyStatus.accessary2;
                        break;
                }
                //SetEquippedAllyID(itemInventory, bEquip, 0);

                // 装備
                allyStatus.Equip(item, lastSelectButtonIndex);
                //item.SetEquippedAllyID(showingCharacterID);

                isSelectEquipColumn = true;

                ToggleInteractableButtonsInChildren(content.transform, false);
                ToggleInteractableButtonsInChildren(equipColumn.transform, true);

                InitializeEquipMenu();

                // 記憶した行を選択
                SelectTargetIndexItem(equipColumn, lastSelectButtonIndex);
            }
        }

    }

    /// <summary>
    /// アイテム一覧内の子オブジェクトを削除(0.01秒待機)
    /// </summary>
    private async Task DestroyChildren(GameObject content, string childName)
    {
        // アイテム一覧内のオブジェクトを削除
        int childCount = content.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = content.transform.GetChild(i);
            //if (child.name == childName)
            //{
            Destroy(child.gameObject);
            //}
        }
        await Task.Delay(10);
    }

    /// <summary>
    /// オブジェクト配下の全てのボタンのInteractableを切り替える
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="toggle"></param>
    private void ToggleInteractableButtonsInChildren(Transform parent, bool toggle)
    {
        // 子オブジェクトの数を取得
        int childCount = parent.childCount;

        // 子オブジェクトを順番にチェックしてButtonコンポーネントを持つか確認
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // Buttonコンポーネントを持つ場合、Interactableを変更する
            Button buttonComponent = child.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = toggle;
            }

            // 再帰的に子オブジェクトの子オブジェクトを検索する
            ToggleInteractableButtonsInChildren(child, toggle);
        }
    }

    /// <summary>
    /// アイテム種別の列挙型からフィルタ番号に変換する
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    private Constants.WeaponCategory GetWeaponCategoryValue(int val)
    {
        Constants.WeaponCategory category = Constants.WeaponCategory.Sword;

        switch (val)
        {
            case 0:
                category = Constants.WeaponCategory.Sword;
                break;
            case 1:
                category = Constants.WeaponCategory.Blade;
                break;
            case 2:
                category = Constants.WeaponCategory.Dagger;
                break;
            case 3:
                category = Constants.WeaponCategory.Spear;
                break;
            case 4:
                category = Constants.WeaponCategory.Ax;
                break;
            case 5:
                category = Constants.WeaponCategory.Hammer;
                break;
            case 6:
                category = Constants.WeaponCategory.Fist;
                break;
            case 7:
                category = Constants.WeaponCategory.Bow;
                break;
            case 8:
                category = Constants.WeaponCategory.Staff;
                break;
            case 9:
                category = Constants.WeaponCategory.Shield;
                break;
            default:
                break;
        }
        return category;
    }

    private void SetTextByVariableName(string str)
    {
        Debug.Log(str);
    }

    /// <summary>
    /// 装備欄の各アイテム情報を設定する
    /// </summary>
    /// <param name=""></param>
    private void SetInfoToEquipColumn(List<GameObject> equips, AllyStatus status)
    {

        // 右手・左手
        if (status.rightArm != null)
        {
            equips[0].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = status.rightArm;
            equips[0].transform.Find("Icon").GetComponent<Image>().enabled = true;
            equips[0].transform.Find("Icon").GetComponent<Image>().sprite = status.rightArm.iconImage;
            equips[0].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = status.rightArm.itemName;

            // 右手武器が両手持ちの場合、左手にも右手武器を表示（文字色はグレー）
            if (status.rightArm.isTwoHanded)
            {
                equips[1].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = status.rightArm;
                equips[1].transform.Find("Icon").GetComponent<Image>().enabled = true;
                equips[1].transform.Find("Icon").GetComponent<Image>().sprite = status.rightArm.iconImage;
                equips[1].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = status.rightArm.itemName;
                equips[1].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.gray);
            }
            else if (status.leftArm != null)
            {
                equips[1].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = status.leftArm;
                equips[1].transform.Find("Icon").GetComponent<Image>().enabled = true;
                equips[1].transform.Find("Icon").GetComponent<Image>().sprite = status.leftArm.iconImage;
                equips[1].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = status.leftArm.itemName;
                equips[1].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
            }
            else
            {
                equips[1].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = null;
                equips[1].transform.Find("Icon").GetComponent<Image>().enabled = false;
                equips[1].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = "なし";
                equips[1].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
            }
        }
        else
        {
            equips[0].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = null;
            equips[0].transform.Find("Icon").GetComponent<Image>().enabled = false;
            equips[0].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = "なし";

            if (status.leftArm != null)
            {
                equips[1].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = status.leftArm;
                equips[1].transform.Find("Icon").GetComponent<Image>().enabled = true;
                equips[1].transform.Find("Icon").GetComponent<Image>().sprite = status.leftArm.iconImage;
                equips[1].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = status.leftArm.itemName;
                equips[1].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
            }
            else
            {
                equips[1].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = null;
                equips[1].transform.Find("Icon").GetComponent<Image>().enabled = false;
                equips[1].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = "なし";
                equips[1].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
            }
        }

        // 頭
        if (status.head != null)
        {
            equips[2].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = status.head;
            equips[2].transform.Find("Icon").GetComponent<Image>().enabled = true;
            equips[2].transform.Find("Icon").GetComponent<Image>().sprite = status.head.iconImage;
            equips[2].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = status.head.itemName;
        }
        else
        {
            equips[2].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = null;
            equips[2].transform.Find("Icon").GetComponent<Image>().enabled = false;
            equips[2].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = "なし";
        }

        // 胴
        if (status.body != null)
        {
            equips[3].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = status.body;
            equips[3].transform.Find("Icon").GetComponent<Image>().enabled = true;
            equips[3].transform.Find("Icon").GetComponent<Image>().sprite = status.body.iconImage;
            equips[3].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = status.body.itemName;
        }
        else
        {
            equips[3].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = null;
            equips[3].transform.Find("Icon").GetComponent<Image>().enabled = false;
            equips[3].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = "なし";
        }

        // 装飾品1
        if (status.accessary1 != null)
        {
            equips[4].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = status.accessary1;
            equips[4].transform.Find("Icon").GetComponent<Image>().enabled = true;
            equips[4].transform.Find("Icon").GetComponent<Image>().sprite = status.accessary1.iconImage;
            equips[4].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = status.accessary1.itemName;
        }
        else
        {
            equips[4].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = null;
            equips[4].transform.Find("Icon").GetComponent<Image>().enabled = false;
            equips[4].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = "なし";
        }

        // 装飾品2
        if (status.accessary2 != null)
        {
            equips[5].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = status.accessary2;
            equips[5].transform.Find("Icon").GetComponent<Image>().enabled = true;
            equips[5].transform.Find("Icon").GetComponent<Image>().sprite = status.accessary2.iconImage;
            equips[5].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = status.accessary2.itemName;
        }
        else
        {
            equips[5].transform.Find("ItemInfo").GetComponent<ItemInfo>().itemInfo = null;
            equips[5].transform.Find("Icon").GetComponent<Image>().enabled = false;
            equips[5].transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().text = "なし";
        }
    }

    /// <summary>
    /// ステータス情報を設定する
    /// </summary>
    /// <param name=""></param>
    private void SetInfoToStatusColumn(List<GameObject> statuses, AllyStatus status)
    {
        //status.CalcStatus();


        // 物理攻撃力
        statuses[0].GetComponent<TextMeshProUGUI>().text = (status.pAttack).ToString();
        // 魔法攻撃力
        statuses[1].GetComponent<TextMeshProUGUI>().text = (status.mAttack).ToString();
        // 物理防御力
        statuses[2].GetComponent<TextMeshProUGUI>().text = (status.pDefence).ToString();
        // 魔法防御力
        statuses[3].GetComponent<TextMeshProUGUI>().text = (status.mDefence).ToString();
        // 最大HP
        statuses[4].GetComponent<TextMeshProUGUI>().text = (status.maxHp2).ToString();
        // 最大MP
        statuses[5].GetComponent<TextMeshProUGUI>().text = (status.maxMp2).ToString();
        // STR
        statuses[6].GetComponent<TextMeshProUGUI>().text = (status.str2).ToString();
        // VIT
        statuses[7].GetComponent<TextMeshProUGUI>().text = (status.vit2).ToString();
        // DEX
        statuses[8].GetComponent<TextMeshProUGUI>().text = (status.dex2).ToString();
        // AGI
        statuses[9].GetComponent<TextMeshProUGUI>().text = (status.agi2).ToString();
        // INT
        statuses[10].GetComponent<TextMeshProUGUI>().text = (status.inte2).ToString();
        // MND
        statuses[11].GetComponent<TextMeshProUGUI>().text = (status.mnd2).ToString();

        for (int i = 0; i < 12; i++)
        {
            statuses[i].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
        }
    }

    /// <summary>
    /// 装備前後のステータスを比較してステータス欄に表示する
    /// </summary>
    /// <param name=""></param>
    private void CompareStatus(List<GameObject> statuses, AllyStatus status, Transform transform)
    {

        InitializeStatusColumn(status);
        //AllyStatus newStatus = status;

        if (transform != null)
        {
            if (transform.Find("ItemInfo") != null)
            {
                if (transform.Find("ItemInfo").gameObject.GetComponent<ItemInfo>().itemInfo != null)
                {
                    Item item = transform.Find("ItemInfo").gameObject.GetComponent<ItemInfo>().itemInfo;

                    if (item != null)
                    {
                        // 物理攻撃力
                        int bPAttack = int.Parse(statuses[0].GetComponent<TextMeshProUGUI>().text);
                        // 魔法攻撃力
                        int bMAttack = int.Parse(statuses[1].GetComponent<TextMeshProUGUI>().text);
                        // 物理防御力
                        int bPDefence = int.Parse(statuses[2].GetComponent<TextMeshProUGUI>().text);
                        // 魔法防御力
                        int bMDefence = int.Parse(statuses[3].GetComponent<TextMeshProUGUI>().text);
                        // 最大HP
                        int bMaxHp = int.Parse(statuses[4].GetComponent<TextMeshProUGUI>().text);
                        // 最大MP
                        int bMaxMp = int.Parse(statuses[5].GetComponent<TextMeshProUGUI>().text);
                        // STR
                        int bStr = int.Parse(statuses[6].GetComponent<TextMeshProUGUI>().text);
                        // VIT
                        int bVit = int.Parse(statuses[7].GetComponent<TextMeshProUGUI>().text);
                        // DEX
                        int bDex = int.Parse(statuses[8].GetComponent<TextMeshProUGUI>().text);
                        // AGI
                        int bAgi = int.Parse(statuses[9].GetComponent<TextMeshProUGUI>().text);
                        // INT
                        int bInt = int.Parse(statuses[10].GetComponent<TextMeshProUGUI>().text);
                        // MND
                        int bMnd = int.Parse(statuses[11].GetComponent<TextMeshProUGUI>().text);

                        AllyStatus newAllyStatus = new AllyStatus();

                        newAllyStatus.classLevels = status.classLevels;
                        newAllyStatus.Class = status.Class;
                        newAllyStatus.maxHp = status.maxHp2;
                        newAllyStatus.maxMp = status.maxMp2;
                        newAllyStatus.str = status.str2;
                        newAllyStatus.vit = status.vit2;
                        newAllyStatus.dex = status.dex2;
                        newAllyStatus.agi = status.agi2;
                        newAllyStatus.inte = status.inte2;
                        newAllyStatus.mnd = status.mnd2;
                        newAllyStatus.rightArm = status.rightArm;
                        newAllyStatus.leftArm = status.leftArm;
                        newAllyStatus.head = status.head;
                        newAllyStatus.body = status.body;
                        newAllyStatus.accessary1 = status.accessary1;
                        newAllyStatus.accessary2 = status.accessary2;

                        EquipToStatus(newAllyStatus, item, lastSelectButtonIndex);

                        //AllyStatus n = CommonController.CalcStatus(newAllyStatus);
                        List<int> newList = new List<int> { newAllyStatus.pAttack, newAllyStatus.mAttack, newAllyStatus.pDefence, newAllyStatus.mDefence, newAllyStatus.maxHp2, newAllyStatus.maxMp2, newAllyStatus.str2, newAllyStatus.vit2, newAllyStatus.dex2, newAllyStatus.agi2, newAllyStatus.inte2, newAllyStatus.mnd2 };
                        //List<int> newList = new List<int> { n.pAttack, n.mAttack, newAllyStatus.pDefence, n.mDefence, n.maxHp, n.maxMp, n.str, n.vit, n.dex, n.agi, n.inte, n.mnd };

                        int diffPAttack = newAllyStatus.pAttack - bPAttack;
                        int diffMAttack = newAllyStatus.mAttack - bMAttack;
                        int diffPDefence = newAllyStatus.pDefence - bPDefence;
                        int diffMDefence = newAllyStatus.mDefence - bMDefence;
                        int diffMaxHp = newAllyStatus.maxHp2 - bMaxHp;
                        int diffMaxMp = newAllyStatus.maxMp2 - bMaxMp;
                        int diffStr = newAllyStatus.str2 - status.str2;
                        int diffVit = newAllyStatus.vit2 - status.vit2;
                        int diffDex = newAllyStatus.dex2 - status.dex2;
                        int diffAgi = newAllyStatus.agi2 - status.agi2;
                        int diffInt = newAllyStatus.inte2 - status.inte2;
                        int diffMnd = newAllyStatus.mnd2 - status.mnd2;

                        List<int> diffList = new List<int> { diffPAttack, diffMAttack, diffPDefence, diffMDefence, diffMaxHp, diffMaxMp, diffStr, diffVit, diffDex, diffAgi, diffInt, diffMnd };

                        for (int i = 0; i < diffList.Count; i++)
                        {
                            if (diffList[i] < 0)
                            {
                                statuses[i].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.red);
                                statuses[i].GetComponent<TextMeshProUGUI>().text = newList[i].ToString();

                                statuses[i + 12].SetActive(true);
                                statuses[i + 12].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.red);
                                statuses[i + 12].GetComponent<TextMeshProUGUI>().text = diffList[i].ToString();
                            }
                            else if (diffList[i] > 0)
                            {
                                statuses[i].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.blue);
                                statuses[i].GetComponent<TextMeshProUGUI>().text = newList[i].ToString();

                                statuses[i + 12].SetActive(true);
                                statuses[i + 12].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.blue);
                                statuses[i + 12].GetComponent<TextMeshProUGUI>().text = "+" + diffList[i].ToString();
                            }
                            else if (diffList[i] == 0)
                            {
                                statuses[i].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                                statuses[i].GetComponent<TextMeshProUGUI>().text = newList[i].ToString();

                                statuses[i + 12].SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// キャラクターステータスに指定部位のアイテムを装備させる
    /// </summary>
    /// <param name="status"></param>
    /// <param name="item"></param>
    /// <param name="index"></param>
    private static void EquipToStatus(AllyStatus status, Item item, int index)
    {
        if (item != null)
        {

            switch (index)
            {
                // 右手
                case 0:
                    Weapon weapon = item as Weapon;
                    if (weapon != null)
                    {
                        status.rightArm = weapon;
                    }
                    break;
                // 左手
                case 1:
                    Weapon weapon2 = item as Weapon;
                    if (weapon2 != null)
                    {
                        status.leftArm = weapon2;
                    }
                    break;
                // 頭
                case 2:
                    Head head = item as Head;
                    if (head != null)
                    {
                        status.head = head;
                    }
                    break;
                // 胴
                case 3:
                    Body body = item as Body;
                    if (body != null)
                    {
                        status.body = body;
                    }
                    break;
                // 装飾品1
                case 4:
                    Accessary accessary1 = item as Accessary;
                    if (accessary1 != null)
                    {
                        status.accessary1 = accessary1;
                    }
                    break;
                // 装飾品2
                case 5:
                    Accessary accessary2 = item as Accessary;
                    if (accessary2 != null)
                    {
                        status.accessary2 = accessary2;
                    }
                    break;
                default:
                    break;
            }

            // ステータス再計算
            status.CalcStatus();

        }


    }
}
