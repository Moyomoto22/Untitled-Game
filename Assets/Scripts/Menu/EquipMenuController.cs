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
/// ������ʃR���g���[���[
/// </summary>
public class EquipMenuController : MonoBehaviour
{
    private List<string> characterNames = new List<string>() { "�A���b�N�X", "�j�R", "�^�o�T", "�A���V�A" };
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

    #region �X�e�[�^�X��
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

    #region �ڍ� - �����E�N���X��
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

    // �I���J�[�\�����������ɂ��邩
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

        SetCharacterInfo();     // �L�����N�^�[���ݒ�
        UpdateCharacterName();  // ����L�����N�^�[��������

        SetInputActions();      // InputAction�ݒ�

        UpdateWeaponCategory(); // ����J�e�S��������

        // �������ʑI�𗓂�Interactable��
        ToggleButtonsInteractable(equipPartButtonParent, true);
        ToggleButtonsInteractable(content, false);

        InitializeEquipDetail(); // �����ڍ׏�����

        await SetEquipToList();

        SelectButton(equipPartButtonParent, 0); // �������ʑI�𗓂�I��
        setButtonFillAmount(equipPartButtonParent, 0);

        await FadeIn();          // ��ʃt�F�[�h�C�� 
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
            // InputActionAsset���擾
            var inputActionAsset = playerInput.actions;

            // "Main"�A�N�V�����}�b�v���擾
            var actionMap = inputActionAsset.FindActionMap("Menu");
            // �A�N�V�������擾
            var rs = actionMap.FindAction("RightShoulder");
            var ls = actionMap.FindAction("LeftShoulder");
            var rt = actionMap.FindAction("RightTrigger");
            var lt = actionMap.FindAction("LeftTrigger");
            var cancel = actionMap.FindAction("Cancel");
            var general = actionMap.FindAction("General");

            // �C�x���g���X�i�[��ݒ�
            cancel.performed += OnPressCancelButton;
            general.performed += OnPressGeneralButton;
            rs.performed += OnPressRSButton;
            ls.performed += OnPressLSButton;
            rt.performed += OnPressRTButton;
            lt.performed += OnPressLTButton;

            // �A�N�V�����}�b�v��L���ɂ���
            actionMap.Enable();
        }
    }

    void RemoveInputActions()
    {
        // �C�x���g���X�i�[������
        if (playerInput != null)
        {
            // InputActionAsset���擾
            var inputActionAsset = playerInput.actions;
            // "Main"�A�N�V�����}�b�v���擾
            var actionMap = inputActionAsset.FindActionMap("Menu");
            // �A�N�V�������擾
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

        characterImage.sprite = ch.CharacterClass.imagesA[i];
        gradation.color = color;
        nameBackGradation.color = color;
        characterClass.text = ch.CharacterClass.classAbbreviation;
        characterLevel.text = ch.Level.ToString();


        #region ������
        leftArm.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        rightArm.GetComponentInChildren<TextMeshProUGUI>().text = ch.RightArm != null ? ch.RightArm.ItemName : "�Ȃ�";
        leftArm.GetComponentInChildren<TextMeshProUGUI>().text = ch.LeftArm != null ? ch.LeftArm.ItemName : "�Ȃ�";
        head.GetComponentInChildren<TextMeshProUGUI>().text = ch.Head != null ? ch.Head.ItemName : "�Ȃ�";
        body.GetComponentInChildren<TextMeshProUGUI>().text = ch.Body != null ? ch.Body.ItemName : "�Ȃ�";
        accessaryOne.GetComponentInChildren<TextMeshProUGUI>().text = ch.Accessary1 != null ? ch.Accessary1.ItemName : "�Ȃ�";
        accessaryTwo.GetComponentInChildren<TextMeshProUGUI>().text = ch.Accessary2 != null ? ch.Accessary2.ItemName : "�Ȃ�";

        if (ch.RightArm != null)
        {
            rightArm.transform.GetChild(1).GetComponent<Image>().enabled = true;
            rightArm.transform.GetChild(1).GetComponent<Image>().sprite = ch.RightArm.IconImage;
        }
        else
        {
            rightArm.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }

        if (ch.LeftArm != null)
        {
            leftArm.transform.GetChild(1).GetComponent<Image>().enabled = true;
            leftArm.transform.GetChild(1).GetComponent<Image>().sprite = ch.LeftArm.IconImage;
        }
        else
        {
            leftArm.transform.GetChild(1).GetComponent<Image>().enabled = false;
            if (ch.RightArm != null)
            {
                if (ch.RightArm.IsTwoHanded)
                {
                    leftArm.GetComponentInChildren<TextMeshProUGUI>().text = ch.RightArm.ItemName;
                    leftArm.GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(Constants.darkGray);
                }
            }
            leftArm.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }

        if (ch.Head != null)
        {
            head.transform.GetChild(1).GetComponent<Image>().enabled = true;
            head.transform.GetChild(1).GetComponent<Image>().sprite = ch.Head.IconImage;
        }
        else
        {
            head.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }

        if (ch.Body != null)
        {
            body.transform.GetChild(1).GetComponent<Image>().enabled = true;
            body.transform.GetChild(1).GetComponent<Image>().sprite = ch.Body.IconImage;
        }
        else
        {
            body.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }

        if (ch.Accessary1 != null)
        {
            accessaryOne.transform.GetChild(1).GetComponent<Image>().enabled = true;
            accessaryOne.transform.GetChild(1).GetComponent<Image>().sprite = ch.Accessary1.IconImage;
        }
        else
        {
            accessaryOne.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }

        if (ch.Accessary2 != null)
        {
            accessaryTwo.transform.GetChild(1).GetComponent<Image>().enabled = true;
            accessaryTwo.transform.GetChild(1).GetComponent<Image>().sprite = ch.Accessary2.IconImage;
        }
        else
        {
            accessaryTwo.transform.GetChild(1).GetComponent<Image>().enabled = false;
        }
        #endregion

        #region �X�e�[�^�X��
        PA.color = Color.white;
        PA.text = ch.PAttack.ToString();
        MA.color = Color.white;
        MA.text = ch.MAttack.ToString();
        PD.color = Color.white;
        PD.text = ch.PDefence.ToString();
        MD.color = Color.white;
        MD.text = ch.MDefence.ToString();
        MH.color = Color.white;
        MH.text = ch.MaxHp.ToString();
        MM.color = Color.white;
        MM.text = ch.MaxMp.ToString();
        ST.color = Color.white;
        ST.text = ch.Str.ToString();
        VI.color = Color.white;
        VI.text = ch.Vit.ToString();
        DE.color = Color.white;
        DE.text = ch.Dex.ToString();
        AG.color = Color.white;
        AG.text = ch.Agi.ToString();
        IT.color = Color.white;
        IT.text = ch.Int.ToString();
        MN.color = Color.white;
        MN.text = ch.Mnd.ToString();
        #endregion
    }

    /// <summary>
    /// �{�^����I����Ԃɂ���
    /// </summary>
    /// <param name="number"></param>
    public void SelectButton(GameObject obj, int number = 0)
    {
        if (eventSystem != null && obj.transform.childCount > 0)
        {
            var buttonToSelect = obj.transform.GetChild(number).GetChild(0).gameObject;

            // �X�N���v�g����I����Ԃɂ���ꍇ�A���ʉ��͖炳�Ȃ�
            var controller = buttonToSelect.GetComponent<MainMenuButtonManager>();
            if (controller != null)
            {
                controller.shouldPlaySound = false;
            }

            eventSystem.SetSelectedGameObject(buttonToSelect);
        }
    }

    /// <summary>
    /// �{�^����Interactable��؂�ւ���
    /// </summary>
    /// <param name="interactable">�L��/����</param>
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
    /// �{�^����FillAmount�𑀍삷��
    /// </summary>
    /// <param name="number">�Ώۃ{�^���̃R�}���h���ł̃C���f�b�N�X</param>
    public void setButtonFillAmount(GameObject obj, int number)
    {
        int numberOfChildren = obj.transform.childCount;

        // �ΏۃC���f�b�N�X�ɊY������{�^���̂�FillAmount��1�ɂ��A����ȊO��0�ɂ���
        for (int i = 0; i < numberOfChildren; i++)
        {
            int fillAmount = i == number ? 1 : 0;
            Transform child = obj.transform.GetChild(i);
            Image buttonImage = child.GetComponentInChildren<Image>();
            buttonImage.fillAmount = fillAmount;
        }
    }

    /// <summary>
    /// ������������
    /// </summary>
    public void OnPressEquipColumnButton()
    {
        SoundManager.Instance.PlaySubmit();

        if (content.transform.childCount > 0)
        {
            // �J�[�\���ʒu���L�����邽�߁A�I�𒆂̍s�̃C���f�b�N�X��ۑ�
            Transform transform = EventSystem.current.currentSelectedGameObject.transform;
            lastSelectButtonIndex = transform.parent.transform.GetSiblingIndex();

            ToggleButtonsInteractable(equipPartButtonParent, false);
            ToggleButtonsInteractable(content, true);
            isSelectingPart = false;

            isSelectEquipColumn = false;

            // �E��A�C�e���ꗗ�̐擪��I��
            setButtonFillAmount(equipPartButtonParent, lastSelectButtonIndex);
            SelectButton(content, 0);
        }
    }

    /// <summary>
    /// �������{�^���I����
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
        // �t�B���^����A�C�e���J�e�S�����擾
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
        filteredWeapons = weapons.Where(x => x.WeaponCategory == weaponCategory).ToList();

        return filteredWeapons.OrderBy(x => x.ID).ToList();
    }

    private List<Equip> GetFilteredEquip(int index)
    {
        Constants.ItemCategory category = Constants.ItemCategory.Head;
        // �t�B���^����A�C�e���J�e�S�����擾
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

        var filteredEquips = equips.Where(x => x.ItemCategory == category).ToList();

        return filteredEquips.OrderBy(x => x.ID).ToList();
    }

    /// <summary>
    /// ������ꗗ�ɃZ�b�g
    /// </summary>
    public async UniTask SetWeapons(List<Weapon> items)
    {
        await DestroyButtons();

        var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);

        //HashSet<string> processedItemIds = new HashSet<string>();  // �o�^�ς݃A�C�e����ID���L�^���邽�߂�HashSet

        // �����A�C�e�����ꗗ�Ƀ{�^���Ƃ��ăZ�b�g���Ă���
        foreach (Equip e in items)
        {
            GameObject obj = Instantiate(equipButton, content.transform, false);    // �ꗗ�ɕ\������{�^���̃x�[�X���C���X�^���X����
            var comp = obj.GetComponent<ItemComponent>();                      // �{�^���ɕR�Â��X�L�������i�[����R���|�[�l���g
            if (comp != null)
            {
                comp.item = e;
                comp.Initialize();
            }

            var newButton = obj.transform.GetChild(0).gameObject;              // �{�^���{��
            AddSelectOrDeselectActionToButtons(newButton, e);               // �I���E�I���������A�N�V�����ݒ�
            // �����\������
            if (e.CanEquip(ch, currentPartIndex))
            {
                // �{�^���������̃A�N�V������ǉ�
                AddOnClickActionToItemButton(newButton, e, ch);
            }
            else
            {
                // �{�^���^�C�g�����O���[�A�E�g
                comp.itemName.color = CommonController.GetColor(Constants.darkGray);
            }
        }
        await UniTask.DelayFrame(1);
    }

    /// <summary>
    /// �h����ꗗ�ɃZ�b�g
    /// </summary>
    public async UniTask SetEquips(List<Equip> items)
    {
        await DestroyButtons();

        var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);

        //HashSet<string> processedItemIds = new HashSet<string>();  // �o�^�ς݃A�C�e����ID���L�^���邽�߂�HashSet

        // �����A�C�e�����ꗗ�Ƀ{�^���Ƃ��ăZ�b�g���Ă���
        foreach (Equip e in items)
        {
            GameObject obj = Instantiate(equipButton, content.transform, false);    // �ꗗ�ɕ\������{�^���̃x�[�X���C���X�^���X����
            var comp = obj.GetComponent<ItemComponent>();                      // �{�^���ɕR�Â��X�L�������i�[����R���|�[�l���g
            if (comp != null)
            {
                comp.item = e;
                comp.Initialize();
            }

            var newButton = obj.transform.GetChild(0).gameObject;              // �{�^���{��
            AddSelectOrDeselectActionToButtons(newButton, e);               // �I���E�I���������A�N�V�����ݒ�
            // �����\������
            if (e.CanEquip(ch, 0))
            {
                // �{�^���������̃A�N�V������ǉ�
                AddOnClickActionToItemButton(newButton, e, ch);
            }
            else
            {
                // �{�^���^�C�g�����O���[�A�E�g
                comp.itemName.color = CommonController.GetColor(Constants.darkGray);
            }
        }
        await UniTask.DelayFrame(1);
    }

    /// </summary>
    /// �����ꗗ�{�^���������̓����ݒ肷��
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="item"></param>
    private void AddOnClickActionToItemButton(GameObject obj, Equip item, Ally ch)
    {
        var button = obj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnClickActionToEquipButton(item, ch));
        }
    }

    /// <summary>
    /// �{�^���������A�N�V�������{�^���ɐݒ�
    /// </summary>
    /// <param name="item"></param>
    private async void OnClickActionToEquipButton(Equip item, Ally ch)
    {
        SoundManager.Instance.PlaySubmit();

        ch.Equip(item, currentPartIndex);

        await Init(currentPartIndex);
    }

    /// <summary>
    /// �{�^���I���E�I���������̓����ݒ�
    /// </summary>
    /// <param name="button"></param>
    /// <param name="item"></param>
    public void AddSelectOrDeselectActionToButtons(GameObject button, Equip e)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Select; // Select�C�x���g�����b�X��
        entry.callback.AddListener((data) =>
        {
            // �A�C�e���ڍׂ��ڍח��ɕ\��
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

        // �G���g�����g���K�[���X�g�ɒǉ�
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// �����J�e�S���A�C�R��������������
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
    /// �ڍח�������������
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
    /// �ڍח��ɕ���̏ڍׂ�\������
    /// </summary>
    /// <param name="item"></param>
    private void SetWeaponDetailInfo(Weapon weapon)
    {
        InitializeEquipDetail();

        if (weapon != null)
        {
            detailIcon.enabled = true;
            detailIcon.sprite = weapon.IconImage;
            detailName.text = weapon.ItemName;
            detailCategory.text = CommonController.GetItemCategoryString(weapon.ItemCategory);
            detailDescription.text = weapon.Description;

            #region �����E�N���X
            if (weapon.Attributes.Count >= 1)
            {
                detailAttributeOne.enabled = true;
                detailAttributeOne.sprite = weapon.Attributes[0].icon;
            }
            if (weapon.Attributes.Count >= 2)
            {
                detailAttributeTwo.enabled = true;
                detailAttributeTwo.sprite = weapon.Attributes[1].icon;
            }
            if (weapon.Attributes.Count >= 3)
            {
                detailAttributeThree.enabled = true;
                detailAttributeThree.sprite = weapon.Attributes[2].icon;
            }
            if (weapon.Attributes.Count >= 4)
            {
                detailAttributeFour.enabled = true;
                detailAttributeFour.sprite = weapon.Attributes[3].icon;
            }
            if (weapon.Attributes.Count >= 5)
            {
                detailAttributeFive.enabled = true;
                detailAttributeFive.sprite = weapon.Attributes[4].icon;
            }
            if (weapon.Attributes.Count >= 6)
            {
                detailAttributeSix.enabled = true;
                detailAttributeSix.sprite = weapon.Attributes[5].icon;
            }

            foreach (var C in weapon.EquipableClasses)
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
    /// �ڍח��ɑ����̏ڍׂ�\������
    /// </summary>
    /// <param name="item"></param>
    private void SetEquipDetailInfo(Equip item)
    {
        InitializeEquipDetail();

        if (item != null)
        {
            detailIcon.enabled = true;
            detailIcon.sprite = item.IconImage;
            detailName.text = item.ItemName;
            detailCategory.text = CommonController.GetItemCategoryString(item.ItemCategory);
            detailDescription.text = item.Description;

            foreach (var C in item.EquipableClasses)
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
    /// �L�����N�^�[�؂�ւ�
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

        //// ��ԏ�̃{�^����I��
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
    /// �L�����N�^�[�؂�ւ�
    /// </summary>
    private void UpdateCharacterName()
    {
        currentCharacterName.text = characterNames[currentCharacterIndex];
        nextCharacterName.text = characterNames[(currentCharacterIndex + 1) % characterNames.Count];
        previousCharacterName.text = characterNames[(currentCharacterIndex - 1 + characterNames.Count) % characterNames.Count];

        //ClearSkillDetailInfo();
    }

    /// <summary>
    /// �E�㕐��E�h��X�g������
    /// </summary>

    /// <summary>
    /// �L�����Z���{�^������������
    /// </summary>
    public async void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SoundManager.Instance.PlayCancel();

            // �������I�𒆂̏ꍇ �� ���C����ʂɖ߂�
            if (isSelectingPart)
            {
                // �A�C�e�����j���[�̃t�F�[�h�A�E�g
                await FadeOutChildren(gameObject, 0.3f);
                gameObject.SetActive(false);
                // ���C�����j���[�̏�����
                await mainMenuController.InitializeFromChildren("Equip");
            }
            // �����ꗗ�I�𒆂̏ꍇ �� �������ɖ߂�
            else
            {
                await Init(currentPartIndex);
            }
        }
    }

    /// <summary>
    /// �ėp�{�^��������
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
    /// R�{�^������������ - ����J�e�S���؂�ւ� - ���y�[�W
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
    /// L�{�^������������ - ����J�e�S���؂�ւ� - �O�y�[�W
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
    /// RT�{�^������������
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
    /// LT�{�^������������
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
    /// �����O��̃X�e�[�^�X���r���ăX�e�[�^�X���ɕ\������
    /// </summary>
    /// <param name=""></param>
    private void CompareStatus(Equip e)
    {
        SetCharacterInfo();

        Ally ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
        Ally dummy = PartyMembers.Instance.CopyAlly(ch);

        dummy.Unequip(currentPartIndex, true);
        dummy.Equip(e, currentPartIndex, true);

        var pA = dummy.PAttack - ch.PAttack;
        var mA = dummy.MAttack - ch.MAttack;
        var pD = dummy.PDefence - ch.PDefence;
        var mD = dummy.MDefence - ch.MDefence;
        var mH = dummy.MaxHp - ch.MaxHp;
        var mM = dummy.MaxMp - ch.MaxMp;
        var sT = dummy.Str - ch.Str;
        var vI = dummy.Vit - ch.Vit;
        var dE = dummy.Dex - ch.Dex;
        var aG = dummy.Agi - ch.Agi;
        var iT = dummy.Int - ch.Int;
        var mN = dummy.Mnd - ch.Mnd;

        SetCompareTextStyle(PA, dummy.PAttack, pa, pA);
        SetCompareTextStyle(MA, dummy.MAttack, ma, mA);
        SetCompareTextStyle(PD, dummy.PDefence, pd, pD);
        SetCompareTextStyle(MD, dummy.MDefence, md, mD);
        SetCompareTextStyle(MH, dummy.MaxHp, mh, mH);
        SetCompareTextStyle(MM, dummy.MaxMp, mm, mM);
        SetCompareTextStyle(ST, dummy.Str, st, sT);
        SetCompareTextStyle(VI, dummy.Vit, vi, vI);
        SetCompareTextStyle(DE, dummy.Dex, de, dE);
        SetCompareTextStyle(AG, dummy.Agi, ag, aG);
        SetCompareTextStyle(IT, dummy.Int, it, iT);
        SetCompareTextStyle(MN, dummy.Mnd, mn, mN);
    }

    /// <summary>
    /// �X�e�[�^�X���̔�r�e�L�X�g�̃X�^�C����ݒ肷��
    /// </summary>
    /// <param name="t">��r��e�L�X�g</param>
    /// <param name="n">��r�㐔�l</param>
    /// <param name="text">�␳�l�e�L�X�g</param>
    /// <param name="number">�␳�l</param>
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
    /// �ꗗ���̃{�^�������ׂĔj������
    /// </summary>
    /// <returns></returns>
    public async UniTask DestroyButtons()
    {
        //var content = GameObject.FindWithTag("ScrollViewContent");
        // �A�C�e���ꗗ���̃I�u�W�F�N�g���폜
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
        // �Q�[���I�u�W�F�N�g�� CanvasGroup �̑��݂��m�F
        if (gameObject != null && gameObject.activeInHierarchy)
        {
            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // �����x��1�ɃA�j���[�V����
                await canvasGroup.DOFade(1, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();
                canvasGroup.interactable = true;
            }
        }
    }

    public async UniTask FadeOutChildren(GameObject gameObject, float duration = 0.3f)
    {
        // �Q�[���I�u�W�F�N�g�� CanvasGroup �̑��݂��m�F
        if (gameObject != null && gameObject.activeInHierarchy)
        {
            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // �����x��0�ɃA�j���[�V����
                await canvasGroup.DOFade(0, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();
                canvasGroup.interactable = false;
            }
            // �A�j���[�V����������ɃQ�[���I�u�W�F�N�g��j��
            if (gameObject != null)
            {
                //Destroy(gameObject);
            }
        }
    }

}
