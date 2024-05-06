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

public class ClassMenuController : MonoBehaviour
{

    private int showingCharacterID = 4;

    string path = "Assets/Data/Item/ItemInventory.prefab";

    public GameObject MainScreen;
    public GameObject ClassScreen;

    public GameObject gradation;

    // �L�����N�^�[�X�e�[�^�X
    private AllyStatus allyStatus;

    #region SerializeField
    // �L�����N�^�[�摜
    [SerializeField]
    private GameObject characterImage;
    
    [SerializeField]
    private GameObject characterName;
    [SerializeField]
    private GameObject classAbbreviation;
    [SerializeField]
    private GameObject characterLevel;
    [SerializeField]
    private GameObject classNames;
    [SerializeField]
    private GameObject selectedClassName;
    [SerializeField]
    private GameObject selectedClassLevel;
    [SerializeField]
    private GameObject exp;
    [SerializeField]
    private GameObject nextExp;
    [SerializeField]
    private GameObject classDescription;
    [SerializeField]
    private GameObject statusRank;
    [SerializeField]
    private GameObject weaponIcons;
    [SerializeField]
    private GameObject exSkillName;
    [SerializeField]
    private GameObject exSkillDescription;
    [SerializeField]
    private GameObject nextSkillLevel;
    [SerializeField]
    private GameObject nextSkillIcon;
    [SerializeField]
    private GameObject nextSkillName;
    [SerializeField]
    private GameObject nextSkillLevelTwo;
    [SerializeField]
    private GameObject nextSkillIconTwo;
    [SerializeField]
    private GameObject nextSkillNameTwo;
    [SerializeField]
    private List<GameObject> statuses;
    [SerializeField]
    private List<GameObject> statusesTwo;

    [SerializeField]
    public EventSystem eventSystem;

    [SerializeField]
    private List<BaseClass> classes;

    [SerializeField]
    private InputAction inputActions;
    #endregion

    private GameObject lastSelectedButton;
    private int lastSelectButtonIndex = 0;

    public GameObject inputActionParent;

    public Sprite buttonImageOff;
    public Sprite buttonImageOn;

    public GameObject subWindowHPMP;

    private ItemInventory itemInventory;

    private string selectedItemID;
    private ItemDatabase itemDatabase;

    // Start is called before the first frame update
    void Start()
    {
        //CommonController.LearnAllSkills(allyStatus);
    }

    // Update is called once per frame
    void Update()
    {
        // EventSystem�̌��ݑI������Ă���GameObject���擾
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;

        // �I�����ꂽ�I�u�W�F�N�g���ύX���ꂽ�ꍇ
        if (selectedButton != lastSelectedButton)
        {
            lastSelectedButton = selectedButton;
            lastSelectButtonIndex = selectedButton.transform.GetSiblingIndex();
            // �I�����ꂽ�I�u�W�F�N�g��Button�ł��邩�m�F
            Button button = selectedButton?.GetComponent<Button>();
            if (button != null)
            {
                // �N���X�ڍׂ�\��
                InithializeClassDetail();
            }
        }
    }

    async void OnEnable()
    {
        allyStatus = await CommonController.GetAllyStatus(showingCharacterID);


        // �X�L�����j���[�p�̃A�N�V�����}�b�v��L����
        CommonController.EnableInputActionMap(inputActionParent, "ClassMenu");

        // ��ʏ�����
        await InitializeClassMenu();

        lastSelectButtonIndex = 0;
    }

    /// <summary>
    /// �N���X��ʏ�����
    /// </summary>
    public async Task InitializeClassMenu()
    {
        string color = Constants.gradationBlue;
        string strColor = Constants.blue;

        switch (showingCharacterID)
        {
            case 1:
                color = Constants.gradationBlue;
                strColor = Constants.blue;
                break;
            case 2:
                color = Constants.gradationRed;
                strColor = Constants.red;
                break;
            case 3:
                color = Constants.gradationPurple;
                strColor = Constants.purple;
                break;
            case 4:
                color = Constants.gradationGreen;
                strColor = Constants.green;
                break;
            default:
                color = Constants.gradationBlue;
                break;
        }

        gradation.GetComponent<Image>().color = CommonController.GetColor(color);
        allyStatus = await CommonController.GetAllyStatus(showingCharacterID);

        characterName.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.characterName;
        characterLevel.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.level.ToString();
        classAbbreviation.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.Class.classAbbreviation;

        List<GameObject> icons = CommonController.GetChildrenGameObjects(weaponIcons);

        List<GameObject> Buttons = CommonController.GetChildrenGameObjects(classNames);

        for (int i = 0; i < Buttons.Count; i++)
        {
            Transform textTransform = Buttons[i].transform.GetChild(0);
            Transform equipMarkTransform = Buttons[i].transform.GetChild(1);

            equipMarkTransform.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(strColor);

            if (textTransform != null && equipMarkTransform != null)
            {
                if (allyStatus.Class.ID - 1 != i)
                {
                    //Buttons[i].GetComponent<Button>().interactable = true;
                    textTransform.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.white);
                    equipMarkTransform.gameObject.SetActive(false);
                }
                else
                {
                    //Buttons[i].GetComponent<Button>().interactable = false;
                    textTransform.GetComponent<TextMeshProUGUI>().color = CommonController.GetColor(Constants.gray);
                    equipMarkTransform.gameObject.SetActive(true);
                }
            }
        }

        SelectTargetIndexItem(classNames, allyStatus.Class.ID - 1);

        InithializeClassDetail();

        //await InitializeClassList(allyStatus);
    }

    public void InithializeClassDetail()
    {
        BaseClass selectedClass = classes[lastSelectButtonIndex];
        int nextLevel = allyStatus.classLevels[lastSelectButtonIndex] + 1;
        int nextNextLevel = allyStatus.classLevels[lastSelectButtonIndex] + 2;

        characterImage.GetComponent<Image>().sprite = selectedClass.imagesA[showingCharacterID - 1];

        selectedClassName.GetComponentInChildren<TextMeshProUGUI>().text = selectedClass.className;
        classDescription.GetComponentInChildren<TextMeshProUGUI>().text = selectedClass.description;
        selectedClassLevel.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.classLevels[lastSelectButtonIndex].ToString();
        exp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.classEarnedExps[lastSelectButtonIndex].ToString();
        nextExp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.classNextExps[lastSelectButtonIndex].ToString();
        exSkillName.GetComponentInChildren<TextMeshProUGUI>().text = selectedClass.exSkill.skillName;
        exSkillDescription.GetComponentInChildren<TextMeshProUGUI>().text = selectedClass.exSkill.description;

        nextSkillLevel.GetComponentInChildren<TextMeshProUGUI>().text = "-";
        nextSkillIcon.GetComponent<Image>().sprite = null;
        nextSkillName.GetComponentInChildren<TextMeshProUGUI>().text = "-";
        nextSkillLevelTwo.GetComponentInChildren<TextMeshProUGUI>().text = "-";
        nextSkillIconTwo.GetComponent<Image>().sprite = null;
        nextSkillNameTwo.GetComponentInChildren<TextMeshProUGUI>().text = "-";

        if (nextLevel <= 30)
        {
            Skill nextSkill = selectedClass.LearnSkills[nextLevel - 1];

            nextSkillLevel.GetComponentInChildren<TextMeshProUGUI>().text = "Lv " + nextLevel.ToString();
            nextSkillIcon.GetComponent<Image>().sprite = nextSkill.icon;
            nextSkillName.GetComponentInChildren<TextMeshProUGUI>().text = nextSkill.skillName;
        }
        if (nextNextLevel <= 30)
        {
            Skill nextSkill = selectedClass.LearnSkills[nextNextLevel - 1];

            nextSkillLevelTwo.GetComponentInChildren<TextMeshProUGUI>().text = "Lv " + nextNextLevel.ToString();
            nextSkillIconTwo.GetComponent<Image>().sprite = nextSkill.icon;
            nextSkillNameTwo.GetComponentInChildren<TextMeshProUGUI>().text = nextSkill.skillName;
        }

        List<GameObject> ranks = CommonController.GetChildrenGameObjects(statusRank);

        for (int i = 0; i < ranks.Count; i++)
        {
            ranks[i].GetComponent<TextMeshProUGUI>().text = selectedClass.statusRank[i];
        }

        List<GameObject> icons = CommonController.GetChildrenGameObjects(weaponIcons);

        foreach (GameObject icon in icons)
        {
            icon.GetComponent<Image>().color = CommonController.GetColor(Constants.gray);
        }

        #region ����A�C�R���F�ύX
        if (selectedClass.equippableWeapons.Exists(x => x.Equals(Constants.WeaponCategory.Sword)))
        {
            icons[0].GetComponent<Image>().color = CommonController.GetColor(Constants.white);
        }
        if (selectedClass.equippableWeapons.Exists(x => x.Equals(Constants.WeaponCategory.Blade)))
        {
            icons[1].GetComponent<Image>().color = CommonController.GetColor(Constants.white);
        }
        if (selectedClass.equippableWeapons.Exists(x => x.Equals(Constants.WeaponCategory.Dagger)))
        {
            icons[2].GetComponent<Image>().color = CommonController.GetColor(Constants.white);
        }
        if (selectedClass.equippableWeapons.Exists(x => x.Equals(Constants.WeaponCategory.Spear)))
        {
            icons[3].GetComponent<Image>().color = CommonController.GetColor(Constants.white);
        }
        if (selectedClass.equippableWeapons.Exists(x => x.Equals(Constants.WeaponCategory.Ax)))
        {
            icons[4].GetComponent<Image>().color = CommonController.GetColor(Constants.white);
        }
        if (selectedClass.equippableWeapons.Exists(x => x.Equals(Constants.WeaponCategory.Hammer)))
        {
            icons[5].GetComponent<Image>().color = CommonController.GetColor(Constants.white);
        }
        if (selectedClass.equippableWeapons.Exists(x => x.Equals(Constants.WeaponCategory.Fist)))
        {
            icons[6].GetComponent<Image>().color = CommonController.GetColor(Constants.white);
        }
        if (selectedClass.equippableWeapons.Exists(x => x.Equals(Constants.WeaponCategory.Bow)))
        {
            icons[7].GetComponent<Image>().color = CommonController.GetColor(Constants.white);
        }
        if (selectedClass.equippableWeapons.Exists(x => x.Equals(Constants.WeaponCategory.Staff)))
        {
            icons[8].GetComponent<Image>().color = CommonController.GetColor(Constants.white);
        }
        if (selectedClass.equippableWeapons.Exists(x => x.Equals(Constants.WeaponCategory.Shield)))
        {
            icons[9].GetComponent<Image>().color = CommonController.GetColor(Constants.white);
        }
        #endregion

        InithializeStatusList(selectedClass);

    }

    public void InithializeStatusList(BaseClass selectedClass)
    {
        List<int> statusOne = new List<int> { allyStatus.maxHp2, allyStatus.maxMp2, allyStatus.str2, allyStatus.vit2, allyStatus.dex2, allyStatus.agi2, allyStatus.inte2, allyStatus.mnd2,
                                           allyStatus.pAttack, allyStatus.mAttack, allyStatus.pDefence, allyStatus.mDefence, allyStatus.pCrit, allyStatus.mCrit, allyStatus.pAvo, allyStatus.mAvo, allyStatus.maxSp };

        string str = "0";

        List<int> statusTwo = allyStatus.ReturnStatusList(lastSelectButtonIndex, selectedClass);

        List<int> statusThree = new List<int>();

        for (int i = 0; i < 17; i++)
        {
            string color = Constants.white;
            int correction = statusTwo[i] - statusOne[i];

            statuses[i].GetComponentInChildren<TextMeshProUGUI>().text = statusTwo[i].ToString();
            if (selectedClass != allyStatus.Class)
            {
                if (correction > 0)
                {
                    color = Constants.blue;
                    str = "+" + correction.ToString();
                }
                else if (correction < 0)
                {
                    color = Constants.red;
                    str = correction.ToString();
                }
                else
                {
                    str = "+" + correction.ToString();
                }
            }
            else
            {
                str = "+0";
            }

            statusesTwo[i].GetComponentInChildren<TextMeshProUGUI>().text = str;
            statuses[i].GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(color);
            statusesTwo[i].GetComponentInChildren<TextMeshProUGUI>().color = CommonController.GetColor(color);

        }

    }

    /// <summary>
    /// �N���X�{�^��������
    /// </summary>
    /// <param name="skill"></param>
    public async void OnPressClassButton(BaseClass selectedClass)
    {
        // �J�[�\���ʒu���L�����邽�߁A�I�𒆂̍s�̃C���f�b�N�X��ۑ�
        //Transform transform = EventSystem.current.currentSelectedGameObject.transform;
        //lastSelectButtonIndex = transform.parent.transform.GetSiblingIndex();

        allyStatus.ChangeClass(selectedClass);

        await InitializeClassMenu();
    }

    /// <summary>
    /// �L�����Z���{�^������������
    /// </summary>
    public async void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Class)
            {
                // ���C����ʂɖ߂�
                ClassScreen.SetActive(false);

                CommonVariableManager.ShowingMenuState = Constants.MenuState.Main;
                MainScreen.SetActive(true);
            }
        }
    }

    /// <summary>
    /// R2(ZR�ERT)�{�^������ �L�����N�^�[�؂�ւ�
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressRightTriggerButton(InputAction.CallbackContext context)
    {
        if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Class)
        {
            if (context.performed)
            {
                showingCharacterID = showingCharacterID + 1;
                if (showingCharacterID > 4)
                {
                    showingCharacterID = 1;
                }
                await InitializeClassMenu();
            }
        }
    }

    /// <summary>
    /// L2(ZL�ELT)�{�^������ �L�����N�^�[�؂�ւ�
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressLeftTriggerButton(InputAction.CallbackContext context)
    {
        if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Class)
        {
            if (context.performed)
            {
                showingCharacterID = showingCharacterID - 1;
                if (showingCharacterID < 1)
                {
                    showingCharacterID = 4;
                }
                await InitializeClassMenu();
            }
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�z���̎w��̏����̃{�^����I������
    /// </summary>
    public void SelectTargetIndexItem(GameObject target, int index = 0)
    {
        if (target != null)
        {
            if (index > target.transform.childCount - 1)
            {
                index = 0;
            }

            Transform targetItemTF = target.transform.GetChild(index);

            if (targetItemTF != null)
            {
                //Transform firstItemButton = targetItemTF.Find("Button");

                //if (firstItemButton != null)
                //{
                GameObject gameObject = targetItemTF.gameObject;
                Button button = gameObject.GetComponent<Button>();
                eventSystem.SetSelectedGameObject(gameObject);
                //}
                //SetItemInformation(targetItemTF);
            }
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�z���̑S�Ẵ{�^����Interactable��؂�ւ���
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="toggle"></param>
    private void ToggleInteractableButtonsInChildren(Transform parent, bool toggle)
    {
        // �q�I�u�W�F�N�g�̐����擾
        int childCount = parent.childCount;

        // �q�I�u�W�F�N�g�����ԂɃ`�F�b�N����Button�R���|�[�l���g�������m�F
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // Button�R���|�[�l���g�����ꍇ�AInteractable��ύX����
            Button buttonComponent = child.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = toggle;
            }

            // �ċA�I�Ɏq�I�u�W�F�N�g�̎q�I�u�W�F�N�g����������
            ToggleInteractableButtonsInChildren(child, toggle);
        }
    }
}
