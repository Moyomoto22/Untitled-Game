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

/// <summary>
/// 装備画面コントローラー
/// </summary>
public class EquipMenuController : MonoBehaviour
{
    private List<string> characterNames = new List<string>() { "アレックス", "ニコ", "タバサ", "アリシア" };
    public GameObject mainMenu;
    public MainMenuController mainMenuController;
    public int currentCharacterIndex = 0;

    private bool isSelectingPart = true;
    private int currentPartIndex = 0;

    private List<Equip> equips;

    public Image characterImage;
    public Image gradation;

    public Image nameBackGradation;
    public TextMeshProUGUI currentCharacterName;
    public TextMeshProUGUI nextCharacterName;
    public TextMeshProUGUI previousCharacterName;

    public TextMeshProUGUI characterClass;
    public TextMeshProUGUI characterLevel;

    public GameObject equipPartButtonParent;
    public GameObject rightArm;
    public GameObject leftArm;
    public GameObject head;
    public GameObject body;
    public GameObject accessaryOne;
    public GameObject accessaryTwo;

    #region ステータス欄
    public TextMeshProUGUI PA;
    public TextMeshProUGUI MA;
    public TextMeshProUGUI PD;
    public TextMeshProUGUI MD;
    public TextMeshProUGUI MH;
    public TextMeshProUGUI MM;
    public TextMeshProUGUI ST;
    public TextMeshProUGUI VI;
    public TextMeshProUGUI DE;
    public TextMeshProUGUI AG;
    public TextMeshProUGUI IT;
    public TextMeshProUGUI MN;

    public TextMeshProUGUI pa;
    public TextMeshProUGUI ma;
    public TextMeshProUGUI pd;
    public TextMeshProUGUI md;
    public TextMeshProUGUI mh;
    public TextMeshProUGUI mm;
    public TextMeshProUGUI st;
    public TextMeshProUGUI vi;
    public TextMeshProUGUI de;
    public TextMeshProUGUI ag;
    public TextMeshProUGUI it;
    public TextMeshProUGUI mn;
    #endregion

    public List<Image> weaponIcons;
    public int currentWeaponCategoryIndex = 0;
    public Image headIcon;
    public Image bodyIcon;
    public Image accessaryIcon;

    public GameObject content;
    public GameObject equipButton;

    public Image detailIcon;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailCategory;
    public TextMeshProUGUI detailDescription;

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


    public EventSystem eventSystem;
    public PlayerInput playerInput;

    private int lastSelectButtonIndex;

    // 選択カーソルが装備欄にあるか
    private bool isSelectEquipColumn = true;

    public GameObject scrollView;
    private ScrollViewManager2 scrollViewManager;

    // Start is called before the first frame update

    private async void OnEnable()
    {
        await Initialize();
    }

    private void OnDisable()
    {
        RemoveInputActions();
    }

    public async UniTask Initialize()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        playerInput = FindObjectOfType<PlayerInput>();
        var items = ItemInventory2.Instance.items;
        equips = items.OfType<Equip>().ToList();

        currentWeaponCategoryIndex = 0;
        lastSelectButtonIndex = 0;
        currentPartIndex = 0;

        SetCharacterInfo();     // キャラクター情報設定
        UpdateCharacterName();  // 左上キャラクター名初期化

        SetInputActions();      // InputAction設定

        UpdateWeaponCategory(); // 武器カテゴリ初期化

        // 装備部位選択欄をInteractableに
        ToggleButtonsInteractable(equipPartButtonParent, true);
        ToggleButtonsInteractable(content, false);

        InitializeEquipDetail(); // 装備詳細初期化

        await SetEquipToList();

        SelectButton(equipPartButtonParent, 0); // 装備部位選択欄を選択
        setButtonFillAmount(equipPartButtonParent, 0);

        await FadeIn();          // 画面フェードイン 
    }

    public async UniTask Init(int index = 0)
    {
        isSelectingPart = true;

        SetCharacterInfo();
        await DestroyButtons();
        await SetEquipToList();

        InitializeCompareText();

        ToggleButtonsInteractable(equipPartButtonParent, true);
        ToggleButtonsInteractable(content, false);

        SelectButton(equipPartButtonParent, index);
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

    private void SetCharacterInfo()
    {
        var i = currentCharacterIndex;
        var ch = PartyMembers.Instance.GetAllyByIndex(i);
        var color = CommonController.GetCharacterColorByIndex(i);

        characterImage.sprite = ch.Class.imagesA[i];
        gradation.color = color;
        nameBackGradation.color = color;
        characterClass.text = ch.Class.classAbbreviation;
        characterLevel.text = ch.level.ToString();


        #region 装備欄
        leftArm.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        rightArm.GetComponentInChildren<TextMeshProUGUI>().text = ch.rightArm != null ? ch.rightArm.itemName : "なし";
        leftArm.GetComponentInChildren<TextMeshProUGUI>().text = ch.leftArm != null ? ch.leftArm.itemName : "なし";
        head.GetComponentInChildren<TextMeshProUGUI>().text = ch.head != null ? ch.head.itemName : "なし";
        body.GetComponentInChildren<TextMeshProUGUI>().text = ch.body != null ? ch.body.itemName : "なし";
        accessaryOne.GetComponentInChildren<TextMeshProUGUI>().text = ch.accessary1 != null ? ch.accessary1.itemName : "なし";
        accessaryTwo.GetComponentInChildren<TextMeshProUGUI>().text = ch.accessary2 != null ? ch.accessary2.itemName : "なし";

        if (ch.rightArm != null)
        {
            rightArm.transform.GetChild(1).GetComponent<Image>().enabled = true;
            rightArm.transform.GetChild(1).GetComponent<Image>().sprite = ch.rightArm.iconImage;
        }
        else
        {
            rightArm.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }

        if (ch.leftArm != null)
        {
            leftArm.transform.GetChild(1).GetComponent<Image>().enabled = true;
            leftArm.transform.GetChild(1).GetComponent<Image>().sprite = ch.leftArm.iconImage;
        }
        else
        {
            leftArm.transform.GetChild(1).GetComponent<Image>().enabled = false;
            if (ch.rightArm != null)
            {
                if (ch.rightArm.isTwoHanded)
                {
                    leftArm.GetComponentInChildren<TextMeshProUGUI>().text = ch.rightArm.itemName;
                    leftArm.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.darkGray);
                }
            }
            leftArm.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }

        if (ch.head != null)
        {
            head.transform.GetChild(1).GetComponent<Image>().enabled = true;
            head.transform.GetChild(1).GetComponent<Image>().sprite = ch.head.iconImage;
        }
        else
        {
            head.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }

        if (ch.body != null)
        {
            body.transform.GetChild(1).GetComponent<Image>().enabled = true;
            body.transform.GetChild(1).GetComponent<Image>().sprite = ch.body.iconImage;
        }
        else
        {
            body.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }

        if (ch.accessary1 != null)
        {
            accessaryOne.transform.GetChild(1).GetComponent<Image>().enabled = true;
            accessaryOne.transform.GetChild(1).GetComponent<Image>().sprite = ch.accessary1.iconImage;
        }
        else
        {
            accessaryOne.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }

        if (ch.accessary2 != null)
        {
            accessaryTwo.transform.GetChild(1).GetComponent<Image>().enabled = true;
            accessaryTwo.transform.GetChild(1).GetComponent<Image>().sprite = ch.accessary2.iconImage;
        }
        else
        {
            accessaryTwo.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }
        #endregion

        #region ステータス欄
        PA.color = Color.white;
        PA.text = ch.pAttack.ToString();
        MA.color = Color.white;
        MA.text = ch.mAttack.ToString();
        PD.color = Color.white;
        PD.text = ch.pDefence.ToString();
        MD.color = Color.white;
        MD.text = ch.mDefence.ToString();
        MH.color = Color.white;
        MH.text = ch.maxHp2.ToString();
        MM.color = Color.white;
        MM.text = ch.maxMp2.ToString();
        ST.color = Color.white;
        ST.text = ch.str2.ToString();
        VI.color = Color.white;
        VI.text = ch.vit2.ToString();
        DE.color = Color.white;
        DE.text = ch.dex2.ToString();
        AG.color = Color.white;
        AG.text = ch.agi2.ToString();
        IT.color = Color.white;
        IT.text = ch.inte2.ToString();
        MN.color = Color.white;
        MN.text = ch.mnd2.ToString();
        #endregion
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
    /// 装備欄押下時
    /// </summary>
    public void OnPressEquipColumnButton()
    {
        SoundManager.Instance.PlaySubmit();

        if (content.transform.childCount > 0)
        {
            // カーソル位置を記憶するため、選択中の行のインデックスを保存
            Transform transform = EventSystem.current.currentSelectedGameObject.transform;
            lastSelectButtonIndex = transform.parent.transform.GetSiblingIndex();

            ToggleButtonsInteractable(equipPartButtonParent, false);
            ToggleButtonsInteractable(content, true);
            isSelectingPart = false;

            isSelectEquipColumn = false;

            // 右上アイテム一覧の先頭を選択
            setButtonFillAmount(equipPartButtonParent, lastSelectButtonIndex);
            SelectButton(content, 0);
        }
    }

    /// <summary>
    /// 装備欄ボタン選択時
    /// </summary>
    public async void OnSelectEquipColumnButton(int index)
    {
        currentPartIndex = index;
        await SetEquipToList();
    }

    private async UniTask SetEquipToList()
    {
        InitializeCategoryIcons(currentPartIndex);
        if (currentPartIndex <= 1)
        {
            var weapons = GetFilteredWeapon(currentPartIndex);
            await SetWeapons(weapons);
        }
        else
        {
            var equips = GetFilteredEquip(currentPartIndex);
            await SetEquips(equips);
        }
    }

    private List<Weapon> GetFilteredWeapon(int index)
    {
        // フィルタするアイテムカテゴリを取得
        Constants.ItemCategory category = Constants.ItemCategory.Weapon;
        Constants.WeaponCategory weaponCategory = Constants.WeaponCategory.Sword;

        switch (currentWeaponCategoryIndex)
        {
            case 0:
                weaponCategory = Constants.WeaponCategory.Sword;
                break;
            case 1:
                weaponCategory = Constants.WeaponCategory.Blade;
                break;
            case 2:
                weaponCategory = Constants.WeaponCategory.Dagger;
                break;
            case 3:
                weaponCategory = Constants.WeaponCategory.Spear;
                break;
            case 4:
                weaponCategory = Constants.WeaponCategory.Ax;
                break;
            case 5:
                weaponCategory = Constants.WeaponCategory.Hammer;
                break;
            case 6:
                weaponCategory = Constants.WeaponCategory.Fist;
                break;
            case 7:
                weaponCategory = Constants.WeaponCategory.Bow;
                break;
            case 8:
                weaponCategory = Constants.WeaponCategory.Staff;
                break;
            case 9:
                weaponCategory = Constants.WeaponCategory.Shield;
                break;
        }

        List<Weapon> weapons = new List<Weapon>();
        List<Weapon> filteredWeapons = new List<Weapon>();

        weapons = equips.OfType<Weapon>().ToList();
        filteredWeapons = weapons.Where(x => x.weaponCategory == weaponCategory).ToList();

        return filteredWeapons.OrderBy(x => x.ID).ToList();
    }

    private List<Equip> GetFilteredEquip(int index)
    {
        Constants.ItemCategory category = Constants.ItemCategory.Head;
        // フィルタするアイテムカテゴリを取得
        switch (index)
        {
            case 2:
                category = Constants.ItemCategory.Head;
                break;
            case 3:
                category = Constants.ItemCategory.Body;
                break;
            case 4:
            case 5:
                category = Constants.ItemCategory.Accessary;
                break;
        }

        var filteredEquips = equips.Where(x => x.itemCategory == category).ToList();

        return filteredEquips.OrderBy(x => x.ID).ToList();
    }

    /// <summary>
    /// 武器を一覧にセット
    /// </summary>
    public async UniTask SetWeapons(List<Weapon> items)
    {
        await DestroyButtons();

        var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);

        //HashSet<string> processedItemIds = new HashSet<string>();  // 登録済みアイテムのIDを記録するためのHashSet

        // 所持アイテムを一覧にボタンとしてセットしていく
        foreach (Equip e in items)
        {
            GameObject obj = Instantiate(equipButton, content.transform, false);    // 一覧に表示するボタンのベースをインスタンス生成
            var comp = obj.GetComponent<ItemComponent>();                      // ボタンに紐づくスキル情報を格納するコンポーネント
            if (comp != null)
            {
                comp.item = e;
                comp.Initialize();
            }

            var newButton = obj.transform.GetChild(0).gameObject;              // ボタン本体
            AddSelectOrDeselectActionToButtons(newButton, e);               // 選択・選択解除時アクション設定
            // 装備可能か判定
            if (e.CanEquip(ch, currentPartIndex))
            {
                // ボタン押下時のアクションを追加
                AddOnClickActionToItemButton(newButton, e, ch);
            }
            else
            {
                // ボタンタイトルをグレーアウト
                comp.itemName.color = CommonController.GetColor(Constants.darkGray);
            }
        }
        await UniTask.DelayFrame(1);
    }

    /// <summary>
    /// 防具を一覧にセット
    /// </summary>
    public async UniTask SetEquips(List<Equip> items)
    {
        await DestroyButtons();

        var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);

        //HashSet<string> processedItemIds = new HashSet<string>();  // 登録済みアイテムのIDを記録するためのHashSet

        // 所持アイテムを一覧にボタンとしてセットしていく
        foreach (Equip e in items)
        {
            GameObject obj = Instantiate(equipButton, content.transform, false);    // 一覧に表示するボタンのベースをインスタンス生成
            var comp = obj.GetComponent<ItemComponent>();                      // ボタンに紐づくスキル情報を格納するコンポーネント
            if (comp != null)
            {
                comp.item = e;
                comp.Initialize();
            }

            var newButton = obj.transform.GetChild(0).gameObject;              // ボタン本体
            AddSelectOrDeselectActionToButtons(newButton, e);               // 選択・選択解除時アクション設定
            // 装備可能か判定
            if (e.CanEquip(ch, 0))
            {
                // ボタン押下時のアクションを追加
                AddOnClickActionToItemButton(newButton, e, ch);
            }
            else
            {
                // ボタンタイトルをグレーアウト
                comp.itemName.color = CommonController.GetColor(Constants.darkGray);
            }
        }
        await UniTask.DelayFrame(1);
    }

    /// </summary>
    /// 装備一覧ボタン押下時の動作を設定する
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="item"></param>
    private void AddOnClickActionToItemButton(GameObject obj, Equip item, AllyStatus ch)
    {
        var button = obj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnClickActionToEquipButton(item, ch));
        }
    }

    /// <summary>
    /// ボタン押下時アクションをボタンに設定
    /// </summary>
    /// <param name="item"></param>
    private async void OnClickActionToEquipButton(Equip item, AllyStatus ch)
    {
        SoundManager.Instance.PlaySubmit();

        ch.Equip(item, currentPartIndex);

        await Init(currentPartIndex);
    }

    /// <summary>
    /// ボタン選択・選択解除時の動作を設定
    /// </summary>
    /// <param name="button"></param>
    /// <param name="item"></param>
    public void AddSelectOrDeselectActionToButtons(GameObject button, Equip e)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Select; // Selectイベントをリッスン
        entry.callback.AddListener((data) =>
        {
            // アイテム詳細を詳細欄に表示
            if (e is Weapon)
            {
                var weapon = e as Weapon;
                SetWeaponDetailInfo(weapon);
            }
            else
            {
                SetEquipDetailInfo(e);
            }
            CompareStatus(e);

        });

        // エントリをトリガーリストに追加
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 装備カテゴリアイコンを初期化する
    /// </summary>
    /// <param name="index"></param>
    private void InitializeCategoryIcons(int index)
    {
        foreach (var icon in weaponIcons)
        {
            icon.enabled = false;
        }
        headIcon.enabled = false;
        bodyIcon.enabled = false;
        accessaryIcon.enabled = false;

        switch (index)
        {
            case 0:
            case 1:
                foreach (var icon in weaponIcons)
                {
                    icon.enabled = true;
                }
                break;
            case 2:
                headIcon.enabled = true;
                break;
            case 3:
                bodyIcon.enabled = true;
                break;
            case 4:
            case 5:
                accessaryIcon.enabled = true;
                break;
        }

    }

    /// <summary>
    /// 詳細欄を初期化する
    /// </summary>
    private void InitializeEquipDetail()
    {
        detailIcon.enabled = false;
        detailName.text = "";
        detailCategory.text = "";
        detailDescription.text = "";
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
    /// 詳細欄に武器の詳細を表示する
    /// </summary>
    /// <param name="item"></param>
    private void SetWeaponDetailInfo(Weapon weapon)
    {
        InitializeEquipDetail();

        if (weapon != null)
        {
            detailIcon.enabled = true;
            detailIcon.sprite = weapon.iconImage;
            detailName.text = weapon.itemName;
            detailCategory.text = CommonController.GetItemCategoryString(weapon.itemCategory);
            detailDescription.text = weapon.description;

            #region 属性・クラス
            if (weapon.attributes.Count >= 1)
            {
                detailAttributeOne.enabled = true;
                detailAttributeOne.sprite = weapon.attributes[0].icon;
            }
            if (weapon.attributes.Count >= 2)
            {
                detailAttributeTwo.enabled = true;
                detailAttributeTwo.sprite = weapon.attributes[1].icon;
            }
            if (weapon.attributes.Count >= 3)
            {
                detailAttributeThree.enabled = true;
                detailAttributeThree.sprite = weapon.attributes[2].icon;
            }
            if (weapon.attributes.Count >= 4)
            {
                detailAttributeFour.enabled = true;
                detailAttributeFour.sprite = weapon.attributes[3].icon;
            }
            if (weapon.attributes.Count >= 5)
            {
                detailAttributeFive.enabled = true;
                detailAttributeFive.sprite = weapon.attributes[4].icon;
            }
            if (weapon.attributes.Count >= 6)
            {
                detailAttributeSix.enabled = true;
                detailAttributeSix.sprite = weapon.attributes[5].icon;
            }

            foreach (var C in weapon.equipableClasses)
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
    }

    /// <summary>
    /// 詳細欄に装備の詳細を表示する
    /// </summary>
    /// <param name="item"></param>
    private void SetEquipDetailInfo(Equip item)
    {
        InitializeEquipDetail();

        if (item != null)
        {
            detailIcon.enabled = true;
            detailIcon.sprite = item.iconImage;
            detailName.text = item.itemName;
            detailCategory.text = CommonController.GetItemCategoryString(item.itemCategory);
            detailDescription.text = item.description;

            foreach (var C in item.equipableClasses)
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
        }
    }

    private void UpdateWeaponCategory()
    {
        for (int i = 0; i < weaponIcons.Count; i++)
        {
            weaponIcons[i].color = CommonController.GetColor(Constants.darkGray);
        }
        weaponIcons[currentWeaponCategoryIndex].color = Color.white;
    }

    /// <summary>
    /// キャラクター切り替え
    /// </summary>
    /// <returns></returns>
    private async UniTask ChangeCharacter()
    {
        SoundManager.Instance.PlaySelect(0.5f);

        SetCharacterInfo();

        Color color = CommonController.GetCharacterColorByIndex(currentCharacterIndex);

        ToggleButtonsInteractable(equipPartButtonParent, true);
        ToggleButtonsInteractable(content, false);

        isSelectingPart = true;
        InitializeEquipDetail();

        //// 一番上のボタンを選択
        SelectButton(equipPartButtonParent, 0);
        setButtonFillAmount(equipPartButtonParent, 0);

        Init();

        float duration = 0.2f;
        await UniTask.WhenAll(
            characterImage.gameObject.GetComponent<SpriteManipulator>().FadeOut(duration),
            characterImage.gameObject.GetComponent<SpriteManipulator>().FadeIn(duration),
            gradation.gameObject.GetComponent<SpriteManipulator>().AnimateColor(color, duration * 2),
            nameBackGradation.gameObject.GetComponent<SpriteManipulator>().AnimateColor(color, duration * 2)
            );
    }

    /// <summary>
    /// キャラクター切り替え
    /// </summary>
    private void UpdateCharacterName()
    {
        currentCharacterName.text = characterNames[currentCharacterIndex];
        nextCharacterName.text = characterNames[(currentCharacterIndex + 1) % characterNames.Count];
        previousCharacterName.text = characterNames[(currentCharacterIndex - 1 + characterNames.Count) % characterNames.Count];

        //ClearSkillDetailInfo();
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
            SoundManager.Instance.PlayCancel();

            // 装備欄選択中の場合 ⇒ メイン画面に戻る
            if (isSelectingPart)
            {
                // アイテムメニューのフェードアウト
                await FadeOutChildren(gameObject, 0.3f);
                gameObject.SetActive(false);
                // メインメニューの初期化
                await mainMenuController.InitializeFromChildren("Equip");
            }
            // 装備一覧選択中の場合 ⇒ 装備欄に戻る
            else
            {
                await Init(currentPartIndex);
            }
        }
    }

    /// <summary>
    /// 汎用ボタン押下時
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressGeneralButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isSelectingPart)
            {
                SoundManager.Instance.PlayCancel();
                var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
                ch.Unequip(currentPartIndex);

                await Init(currentPartIndex);
            }
        }
    }

    /// <summary>
    /// Rボタン押下時処理 - 武器カテゴリ切り替え - 次ページ
    /// </summary>
    public async void OnPressRSButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SoundManager.Instance.PlaySelect(0.5f);
            if (currentPartIndex <= 1)
            {
                currentWeaponCategoryIndex = (currentWeaponCategoryIndex + 1) % weaponIcons.Count;
                UpdateWeaponCategory();

                var weapons = GetFilteredWeapon(currentWeaponCategoryIndex);
                await SetWeapons(weapons);


                if (!isSelectingPart)
                {
                    ToggleButtonsInteractable(content, true);
                    SelectButton(content, 0);
                }
            }
        }
    }

    /// <summary>
    /// Lボタン押下時処理 - 武器カテゴリ切り替え - 前ページ
    /// </summary>
    public async void OnPressLSButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SoundManager.Instance.PlaySelect(0.5f);
            if (currentPartIndex <= 1)
            {
                currentWeaponCategoryIndex = (currentWeaponCategoryIndex - 1 + weaponIcons.Count) % weaponIcons.Count;
                UpdateWeaponCategory();

                var weapons = GetFilteredWeapon(currentWeaponCategoryIndex);
                await SetWeapons(weapons);

                if (!isSelectingPart)
                {
                    ToggleButtonsInteractable(content, true);
                    SelectButton(content, 0);
                }

            }
        }
    }

    /// <summary>
    /// RTボタン押下時処理
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressRTButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentCharacterIndex = (currentCharacterIndex + 1) % characterNames.Count;
            UpdateCharacterName();

            await ChangeCharacter();
        }
    }

    /// <summary>
    /// LTボタン押下時処理
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressLTButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentCharacterIndex = (currentCharacterIndex - 1 + characterNames.Count) % characterNames.Count;
            UpdateCharacterName();

            await ChangeCharacter();
        }
    }

    /// <summary>
    /// 装備前後のステータスを比較してステータス欄に表示する
    /// </summary>
    /// <param name=""></param>
    private void CompareStatus(Equip e)
    {
        SetCharacterInfo();

        AllyStatus ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
        AllyStatus dummy = PartyMembers.Instance.CopyAllyStatus(ch);

        dummy.Unequip(currentPartIndex, true);
        dummy.Equip(e, currentPartIndex, true);

        var pA = dummy.pAttack - ch.pAttack;
        var mA = dummy.mAttack - ch.mAttack;
        var pD = dummy.pDefence - ch.pDefence;
        var mD = dummy.mDefence - ch.mDefence;
        var mH = dummy.maxHp2 - ch.maxHp2;
        var mM = dummy.maxMp2 - ch.maxMp2;
        var sT = dummy.str2 - ch.str2;
        var vI = dummy.vit2 - ch.vit2;
        var dE = dummy.dex2 - ch.dex2;
        var aG = dummy.agi2 - ch.agi2;
        var iT = dummy.inte2 - ch.inte2;
        var mN = dummy.mnd2 - ch.mnd2;

        SetCompareTextStyle(PA, dummy.pAttack, pa, pA);
        SetCompareTextStyle(MA, dummy.mAttack, ma, mA);
        SetCompareTextStyle(PD, dummy.pDefence, pd, pD);
        SetCompareTextStyle(MD, dummy.mDefence, md, mD);
        SetCompareTextStyle(MH, dummy.maxHp2, mh, mH);
        SetCompareTextStyle(MM, dummy.maxMp2, mm, mM);
        SetCompareTextStyle(ST, dummy.str2, st, sT);
        SetCompareTextStyle(VI, dummy.vit2, vi, vI);
        SetCompareTextStyle(DE, dummy.dex2, de, dE);
        SetCompareTextStyle(AG, dummy.agi2, ag, aG);
        SetCompareTextStyle(IT, dummy.inte2, it, iT);
        SetCompareTextStyle(MN, dummy.mnd2, mn, mN);
    }

    /// <summary>
    /// ステータス欄の比較テキストのスタイルを設定する
    /// </summary>
    /// <param name="t">比較後テキスト</param>
    /// <param name="n">比較後数値</param>
    /// <param name="text">補正値テキスト</param>
    /// <param name="number">補正値</param>
    private void SetCompareTextStyle(TextMeshProUGUI t, int n, TextMeshProUGUI text, int number)
    {
        t.text = n.ToString();

        if (number > 0)
        {
            t.color = CommonController.GetColor(Constants.blue);
            text.text = "+ " + number.ToString();
            text.color = CommonController.GetColor(Constants.blue);
        }
        else if (number == 0)
        {
            t.color = Color.white;
            text.text = "";
        }
        else if (number < 0)
        {
            t.color = CommonController.GetColor(Constants.red);
            text.color = CommonController.GetColor(Constants.red);
            text.text = number.ToString();
        }
    }

    private void InitializeCompareText()
    {
        PA.color = Color.white;
        MA.color = Color.white;
        PD.color = Color.white;
        MD.color = Color.white;
        MH.color = Color.white;
        MM.color = Color.white;
        ST.color = Color.white;
        VI.color = Color.white;
        DE.color = Color.white;
        AG.color = Color.white;
        IT.color = Color.white;
        MN.color = Color.white;
        pa.text = "";
        ma.text = "";
        pd.text = "";
        md.text = "";
        mh.text = "";
        mm.text = "";
        st.text = "";
        vi.text = "";
        de.text = "";
        ag.text = "";
        it.text = "";
        mn.text = "";
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

    public async UniTask FadeOutChildren(GameObject gameObject, float duration = 0.3f)
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
