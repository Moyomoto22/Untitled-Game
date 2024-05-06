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

public class SkillMenuController : MonoBehaviour
{

    private int showingCharacterID = 1;

    string path = "Assets/Data/Item/ItemInventory.prefab";

    public GameObject MainScreen;
    public GameObject SkillScreen;

    public GameObject gradation;

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
    [SerializeField]
    private GameObject maxHP;
    [SerializeField]
    private GameObject hp;
    [SerializeField]
    private GameObject maxMP;
    [SerializeField]
    private GameObject mp;
    [SerializeField]
    private GameObject maxSP;
    [SerializeField]
    private GameObject sp;

    // スキル一覧 フィルタ状態
    private int filterState;

    // スキル一覧 フィルタ文字列 親オブジェクト
    [SerializeField]
    private List<GameObject> filters;
    [SerializeField]
    private GameObject underBar;

    // スキル一欄 スクロールビュー内
    public GameObject content;

    public GameObject skillObject;

    [SerializeField]
    private GameObject skillIcon;
    [SerializeField]
    private GameObject skillName;
    [SerializeField]
    private GameObject skillCategory;
    [SerializeField]
    private GameObject spCost;
    [SerializeField]
    private GameObject skillDescription;
    [SerializeField]
    private GameObject mpCost;
    [SerializeField]
    private GameObject tpCost;
    [SerializeField]
    private GameObject recast;
    [SerializeField]
    private GameObject learn;

    // 属性アイコン
    [SerializeField]
    private GameObject attributes;

    // 使用可能クラス
    [SerializeField]
    private GameObject classes;

    public Sprite Slash;
    public Sprite Thrust;
    public Sprite Blow;
    public Sprite Magic;
    public Sprite Fire;
    public Sprite Ice;
    public Sprite Thunder;
    public Sprite Wind;



    [SerializeField]
    public GameObject subWindow;

    [SerializeField]
    private GameObject WeaponIcons;
    [SerializeField]
    private GameObject HeadIcon;
    [SerializeField]
    private GameObject BodyIcon;
    [SerializeField]
    private GameObject AccessaryIcon;

    [SerializeField]
    private List<GameObject> InnerOutlines;

    [SerializeField]
    public EventSystem eventSystem;

    [SerializeField]
    private InputAction inputActions;

    private GameObject lastSelectedButton;
    private int lastSelectButtonIndex = 0;

    public GameObject inputActionParent;

    public Sprite buttonImageOff;
    public Sprite buttonImageOn;

    public GameObject subWindowHPMP;

    private ItemInventory itemInventory;

    private string selectedItemID;
    private ItemDatabase itemDatabase;

    // 使用するスキル
    private Skill useSkill;

    public GameObject scrollView;
    private ScrollViewManager2 scrollViewManager;

    // Start is called before the first frame update
    void Start()
    {
        scrollViewManager = scrollView.GetComponent<ScrollViewManager2>();
    }

    // Update is called once per frame
    void Update()
    {
        // EventSystemの現在選択されているGameObjectを取得
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;

        // 選択されたオブジェクトが変更された場合
        if (selectedButton != lastSelectedButton && !subWindow.activeSelf)
        {
            // 選択されたオブジェクトがButtonであるか確認
            Button button = selectedButton?.GetComponent<Button>();
            if (button != null)
            {
                // 右下にアイテム詳細を表示
                SetSkillInformation(button.transform.parent);
            }
            lastSelectedButton = selectedButton;
        }
    }

    async void OnEnable()
    {
        filterState = 0;

        allyStatus = await CommonController.GetAllyStatus(showingCharacterID);

        // サブウインドウを非表示
        subWindow.SetActive(false);

        // スキルメニュー用のアクションマップを有効化
        CommonController.EnableInputActionMap(inputActionParent, "SkillMenu");

        // スキル一欄を選択可に
        ToggleInteractableButtonsInChildren(content.transform, true);
        // サブウインドウを選択不可に
        //ToggleInteractableButtonsInChildren(subWindow.transform, false);

        // 画面初期化
        await InitializeSkillMenu();

        // 右上武器欄初期化
        //await InitializeEquipItemList();

        selectedItemID = null;
        lastSelectButtonIndex = 0;
    }

    /// <summary>
    /// スキル画面初期化
    /// </summary>
    public async Task InitializeSkillMenu()
    {
        if (filters != null)
        {


            //showingCharacterID = 2;

            string color = Constants.gradationBlue;

            switch (showingCharacterID)
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
            allyStatus = await CommonController.GetAllyStatus(showingCharacterID);

            InithializeCharacterStatus();

            await InitializeSkillList(allyStatus);
        }
    }

    public void InithializeCharacterStatus()
    {
        characterImage.GetComponent<Image>().sprite = allyStatus.characterImage;

        characterName.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.characterName;
        characterLevel.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.level.ToString();
        classAbbreviation.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.Class.classAbbreviation;
        maxHP.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.maxHp.ToString();
        hp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.hp.ToString();
        maxMP.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.maxMp.ToString();
        mp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.mp.ToString();
        maxSP.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.maxSp.ToString();
        sp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.sp.ToString();
    }

    /// <summary>
    /// スキル一覧初期化
    /// </summary>
    public async Task InitializeSkillList(bool isSelectTarget = true)
    {

        // スキル一覧のフィルタ設定
        foreach (GameObject filter in filters)
        {
            filter.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.gray);
        }
        filters[filterState].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);

        Vector3 pos1 = underBar.transform.position;
        Vector3 pos2 = new Vector3(filters[filterState].transform.position.x, underBar.transform.position.y, underBar.transform.position.z);

        underBar.transform.position = Vector3.Lerp(pos1, pos2, 1.0f);

        // スキル一覧内のオブジェクトを削除(0.01秒待機)
        await DestroyChildren(content, "SkillObject");

        List<Skill> learnedSkills = allyStatus.learnedSkills;

        if (learnedSkills != null)
        {
            List<Skill> skills;
            switch (filterState)
            {
                case 0:
                    skills = learnedSkills.OrderBy(x => x.ID).Where(x => x.isEquipped).ToList();
                    break;
                case 1:
                    skills = learnedSkills.OrderBy(x => x.ID).Where(x => x.skillCategory == Constants.SkillCategory.Magic).ToList();
                    break;
                case 2:
                    skills = learnedSkills.OrderBy(x => x.ID).Where(x => x.skillCategory == Constants.SkillCategory.Miracle).ToList();
                    break;
                case 3:
                    skills = learnedSkills.OrderBy(x => x.ID).Where(x => x.skillCategory == Constants.SkillCategory.Arts).ToList();
                    break;
                case 4:
                    skills = learnedSkills.OrderBy(x => x.ID).Where(x => x.skillCategory == Constants.SkillCategory.Active).ToList();
                    break;
                case 5:
                    skills = learnedSkills.OrderBy(x => x.ID).Where(x => x.skillCategory == Constants.SkillCategory.Passive).ToList();
                    break;
                default:
                    skills = learnedSkills.OrderBy(x => x.ID).Where(x => x.isEquipped).ToList();
                    break;
            }

            foreach (var skill in skills)
            {
                // 各スキルの情報を格納
                GameObject temp = Instantiate(skillObject);

                temp.transform.Find("SkillInfo").GetComponent<SkillInfo>().skillInfo = skill;
                temp.transform.Find("Icon").GetComponent<Image>().sprite = skill.icon;

                if (skill.skillCategory == Constants.SkillCategory.Magic || skill.skillCategory == Constants.SkillCategory.Miracle)
                {
                    MagicMiracle magicMiracle = skill as MagicMiracle;
                    if (magicMiracle != null)
                    {
                        temp.transform.Find("SP").GetComponent<TextMeshProUGUI>().text = magicMiracle.MPCost.ToString();
                    }
                }
                else
                {
                    temp.transform.Find("SP").GetComponent<TextMeshProUGUI>().text = skill.spCost.ToString();
                }

                Transform button = temp.transform.Find("Button");
                GameObject equipMark = button.Find("EquipMark").gameObject;
                equipMark.SetActive(false);

                button.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName;

                // 装備中
                if (skill.isEquipped)
                {
                    button.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.red);
                    button.GetComponent<Button>().onClick.AddListener(() => OnPressSkillButton(skill, button));
                }
                else
                {
                    // 装備可能かチェック
                    string msg = allyStatus.CanEquipSkill(skill);
                    if (msg == "")
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);

                        // 魔法は使用・装備不可
                        if (skill.skillCategory == Constants.SkillCategory.Magic)
                        {
                            button.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.gray);
                        }
                        // 奇跡
                        else if (skill.skillCategory == Constants.SkillCategory.Miracle)
                        {
                            // 使用可能かチェック
                            if (allyStatus.CanUseSKill(skill))
                            {
                                button.GetComponent<Button>().onClick.AddListener(() => OnPressSkillButton(skill, button));
                            }
                            else
                            {
                                button.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.gray);
                            }
                        }
                        else
                        {
                            button.GetComponent<Button>().onClick.AddListener(() => OnPressSkillButton(skill, button));
                        }

                    }
                    else
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.gray);
                        //button.GetComponent<Button>().onClick.AddListener(() => OnPressSkillButton(skill));
                    }

                }
                temp.transform.parent = content.transform;
            }
        }

        if (content.transform.childCount > 0)
        {
            SelectTargetIndexItem(content, lastSelectButtonIndex);
        }

        // 一番上へスクロール
        //scrollViewManager.ScrollScrollView(1.0f);
    }

    private void SetSkillInformation(Transform transform)
    {
        List<GameObject> icons = CommonController.GetChildrenGameObjects(attributes);
        List<GameObject> classList = CommonController.GetChildrenGameObjects(classes);

        foreach (var icon in icons)
        {
            icon.SetActive(false);
        }

        foreach (var Class in classList)
        {
            Class.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
        }

        skillIcon.SetActive(false);
        skillName.SetActive(false);
        spCost.SetActive(false);
        skillCategory.SetActive(false);
        skillDescription.SetActive(false);
        mpCost.SetActive(false);
        tpCost.SetActive(false);
        recast.SetActive(false);
        learn.SetActive(false);

        if (transform != null)
        {
            if (transform.Find("SkillInfo") != null)
            {
                if (transform.Find("SkillInfo").gameObject.GetComponent<SkillInfo>().skillInfo != null)
                {
                    Skill skill = transform.Find("SkillInfo").gameObject.GetComponent<SkillInfo>().skillInfo;
                    if (skill != null)
                    {
                        skillIcon.SetActive(true);
                        skillName.SetActive(true);
                        spCost.SetActive(true);
                        skillCategory.SetActive(true);
                        skillDescription.SetActive(true);
                        mpCost.SetActive(true);
                        tpCost.SetActive(true);
                        recast.SetActive(true);
                        learn.SetActive(true);

                        skillIcon.GetComponent<Image>().sprite = skill.icon;
                        skillName.GetComponent<TextMeshProUGUI>().text = skill.skillName;
                        spCost.GetComponent<TextMeshProUGUI>().text = "-";
                        skillCategory.GetComponent<TextMeshProUGUI>().text = CommonController.GetSKillCategoryString(skill.skillCategory);
                        skillDescription.GetComponent<TextMeshProUGUI>().text = skill.description;
                        mpCost.GetComponent<TextMeshProUGUI>().text = "-";
                        tpCost.GetComponent<TextMeshProUGUI>().text = "-";
                        recast.GetComponent<TextMeshProUGUI>().text = "-";
                        learn.GetComponent<TextMeshProUGUI>().text = "-";

                        MagicMiracle magicMiracle = skill as MagicMiracle;
                        Arts arts = skill as Arts;
                        ActiveSkill active = skill as ActiveSkill;
                        PassiveSkill passive = skill as PassiveSkill;

                        // 属性
                        for (int i = 0; i < skill.attributes.Count; i++)
                        {
                            Sprite image = Slash;
                            switch (skill.attributes[i])
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

                        if (magicMiracle != null)
                        {
                            mpCost.GetComponent<TextMeshProUGUI>().text = magicMiracle.MPCost.ToString();

                            //  使用可能クラス(魔法奇跡のみ)
                            foreach (var Class in classList)
                            {
                                Class.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.gray);
                            }

                            if (magicMiracle.usableClasses.Exists(x => x.className == "ウォリアー"))
                            {
                                classList[0].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                            }
                            if (magicMiracle.usableClasses.Exists(x => x.className == "パラディン"))
                            {
                                classList[1].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                            }
                            if (magicMiracle.usableClasses.Exists(x => x.className == "モンク"))
                            {
                                classList[2].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                            }
                            if (magicMiracle.usableClasses.Exists(x => x.className == "シーフ"))
                            {
                                classList[3].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                            }
                            if (magicMiracle.usableClasses.Exists(x => x.className == "レンジャー"))
                            {
                                classList[4].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                            }
                            if (magicMiracle.usableClasses.Exists(x => x.className == "ソーサラー"))
                            {
                                classList[5].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                            }
                            if (magicMiracle.usableClasses.Exists(x => x.className == "クレリック"))
                            {
                                classList[6].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                            }
                            if (magicMiracle.usableClasses.Exists(x => x.className == "スペルソード"))
                            {
                                classList[7].GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                            }

                        }
                        else if (arts != null)
                        {
                            tpCost.GetComponent<TextMeshProUGUI>().text = arts.TPCost.ToString();
                        }
                        else if (active != null)
                        {
                            spCost.GetComponent<TextMeshProUGUI>().text = active.spCost.ToString();
                            recast.GetComponent<TextMeshProUGUI>().text = active.recastTurn.ToString();
                        }
                        else if (passive != null)
                        {
                            spCost.GetComponent<TextMeshProUGUI>().text = passive.spCost.ToString();
                        }

                        // 習得クラス・レベル
                        if (skill.learnClass != null)
                        {
                            string className = skill.learnClass.className;
                            string level = skill.learnLevel.ToString();

                            string str = className + " Lv" + level;
                            learn.GetComponent<TextMeshProUGUI>().text = str;
                        }

                    }
                }
            }
        }
    }

    /// <summary>
    /// スキルボタン押下時
    /// </summary>
    /// <param name="skill"></param>
    public void OnPressSkillButton(Skill skill, Transform button)
    {
        // カーソル位置を記憶するため、選択中の行のインデックスを保存
        Transform transform = EventSystem.current.currentSelectedGameObject.transform;
        lastSelectButtonIndex = transform.parent.transform.GetSiblingIndex();

        scrollViewManager.enabled = false;

        if (skill != null)
        {
            // 奇跡以外の場合
            if (skill.skillCategory != Constants.SkillCategory.Miracle)
            {
                // スキル着脱
                allyStatus.SetSkill(skill, skill.isEquipped);
                button.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.red);

                InitializeSkillMenu();
            }
            // 奇跡の場合
            else
            {
                useSkill = skill;

                Vector3 pos = transform.position;

                // 一覧の下側のアイテムが選択されたらサブウインドウをカーソルの上側に表示
                float offset = pos.y < 320 ? 85 : -100;

                // サブウインドウ表示
                subWindow.transform.position = new Vector3(pos.x + 450, pos.y + offset, pos.z); ;

                // 各キャラクターのHP・MPを表示
                SetHPAndMPToSubWindow();
                subWindow.SetActive(true);

                GameObject topButton = subWindow.transform.GetChild
                    (0).gameObject;

                //ToggleInteractableButtonsInChildren(subWindow.transform, true);
                eventSystem.SetSelectedGameObject(topButton);

                // スキル一覧のボタンを全て選択不可に
                ToggleInteractableButtonsInChildren(content.transform, false);

                // フォーカスが残ったように見せる
                if (lastSelectedButton != null)
                {
                    lastSelectedButton.GetComponent<Image>().sprite = buttonImageOn;
                }
            }
        }
        scrollViewManager.enabled = true;
    }

    /// <summary>
    /// スキル使用
    /// </summary>
    /// <param name="cID"></param>
    public async void OnPressUseSkillToCharacter(int cID)
    {
        AllyStatus target = await CommonController.GetAllyStatus(cID);

        if (target != null && useSkill != null)
        {
            if (useSkill is MagicMiracle)
            {
                allyStatus.UseSkill(useSkill, target);

                // HP・MP再表示
                SetHPAndMPToSubWindow();
                InithializeCharacterStatus();

            }
        }
    }


    /// <summary>
    /// キャンセルボタン押下時処理
    /// </summary>
    public async void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Skill)
            {
                // サブメニュー非表示の場合 ⇒ メイン画面に戻る
                if (!subWindow.activeSelf)
                {
                    SkillScreen.SetActive(false);

                    CommonVariableManager.ShowingMenuState = Constants.MenuState.Main;
                    MainScreen.SetActive(true);
                }
                // サブメニュー表示中の場合⇒ スキル欄に戻る
                else
                {
                    subWindow.SetActive(false);

                    ToggleInteractableButtonsInChildren(content.transform, true);
                    //ToggleInteractableButtonsInChildren(.transform, true);

                    await InitializeSkillMenu();

                    // 記憶した行を選択
                    SelectTargetIndexItem(content, lastSelectButtonIndex);
                }
            }
        }
    }

    /// <summary>
    /// R1(R・RB)ボタン押下
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressRightShoulderButton(InputAction.CallbackContext context)
    {
        if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Skill)
        {
            if (context.performed)
            {
                if (!subWindow.activeSelf)
                {
                    filterState = filterState + 1;
                    if (filterState > 5)
                    {
                        filterState = 0;
                    }
                    await InitializeSkillList();
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
        if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Skill)
        {
            if (context.performed)
            {
                if (!subWindow.activeSelf)
                {
                    filterState = filterState - 1;
                    if (filterState < 0)
                    {
                        filterState = 5;
                    }
                    await InitializeSkillList();
                }
            }
        }
    }

    /// <summary>
    /// R2(ZR・RT)ボタン押下 キャラクター切り替え
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressRightTriggerButton(InputAction.CallbackContext context)
    {
        if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Skill)
        {
            if (context.performed)
            {
                if (!subWindow.activeSelf)
                {
                    showingCharacterID = showingCharacterID + 1;
                    if (showingCharacterID > 4)
                    {
                        showingCharacterID = 1;
                    }
                    await InitializeSkillMenu();
                }
            }
        }
    }

    /// <summary>
    /// L2(ZL・LT)ボタン押下 キャラクター切り替え
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressLeftTriggerButton(InputAction.CallbackContext context)
    {
        if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Skill)
        {
            if (context.performed)
            {
                if (!subWindow.activeSelf)
                {
                    showingCharacterID = showingCharacterID - 1;
                    if (showingCharacterID < 1)
                    {
                        showingCharacterID = 4;
                    }
                    await InitializeSkillMenu();
                }
            }
        }
    }

    /// <summary>
    /// オブジェクト配下の指定の順序のスキルを選択する
    /// </summary>
    public void SelectTargetIndexItem(GameObject target, int index = 0)
    {
        if (target != null && content.transform.childCount > 0)
        {
            if (index > target.transform.childCount - 1)
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
    /// スキル一覧内の子オブジェクトを削除(0.01秒待機)
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
        await Task.Delay(1);
    }

    private async void SetHPAndMPToSubWindow()
    {

        AllyStatus allyStatus = new AllyStatus();

        int loopCount = 0;

        for (int i = 1; i < 5; i++)
        {
            allyStatus = await CommonController.GetAllyStatus(i);
            List<int> texts = new List<int> { allyStatus.maxHp2, allyStatus.hp, allyStatus.maxMp2, allyStatus.mp };

            for (int j = 0; j < 4; j++)
            {
                subWindowHPMP.transform.GetComponentsInChildren<TextMeshProUGUI>()[loopCount].text = texts[j].ToString();
                loopCount++;
            }
        }
    }

}
