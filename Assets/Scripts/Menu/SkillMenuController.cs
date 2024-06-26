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

    private List<string> characterNames = new List<string>() { "�A���b�N�X", "�j�R", "�^�o�T", "�A���V�A" };
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

    // �X�L���ꗓ �X�N���[���r���[��
    public GameObject content;

    private int lastSelectButtonIndex = 0;

    // �g�p����X�L��
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
    /// �X�L����ʏ�����
    /// </summary>
    public async UniTask Initialize()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        playerInput = FindObjectOfType<PlayerInput>();
        currentSkillCategoryIndex = 0;
        lastSelectButtonIndex = 0;

        SetCharacterInfo();     // �L�����N�^�[���ݒ�
        UpdateCharacterName();  // ����L�����N�^�[��������
        UpdateSkillCategory();  // �X�L���J�e�S���ݒ�
        await SetSkillList(); // �X�L���I�𗓐ݒ�
        InitializeSkillDetail(); // �N���X�ڍ׏�����

        SetInputActions();      // InputAction�ݒ�

        setButtonFillAmount(content, 0);
        SelectButton(content, 0); // �������ʑI�𗓂�I��

        var rect = underBar.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(underBarPositions[0], rect.anchoredPosition.y);

        await FadeIn();          // ��ʃt�F�[�h�C�� 
    }

    public async UniTask Init()
    {
        selectedSkill = null;
        await SetSkillList(); // �X�L���I�𗓐ݒ�

        setButtonFillAmount(content, lastSelectButtonIndex);
        SelectButton(content, lastSelectButtonIndex); // �������ʑI�𗓂�I��
    }

    /// <summary>
    /// �L�����N�^�[����ݒ肷��(�摜����)
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
    /// �X�L���ꗗ������
    /// </summary>
    /// <summary>
    /// �X�L���ꗗ������
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

        // �K���X�L�����ꗗ�Ƀ{�^���Ƃ��ăZ�b�g���Ă���
        foreach (Skill s in orderedSkills)
        {
            GameObject obj = Instantiate(skillButton, content.transform, false);    // �ꗗ�ɕ\������{�^���̃x�[�X���C���X�^���X����
            var comp = obj.GetComponent<SkillComponent>();                           // �{�^���ɕR�Â��X�L�������i�[����R���|�[�l���g
            if (comp != null)
            {
                comp.skill = s;
                comp.Initialize();
            }

            var newButton = obj.transform.GetChild(0).gameObject;              // �{�^���{��
            AddSelectActionToButtons(newButton, s);               // �I���E�I���������A�N�V�����ݒ�
            var equipedSkillIDs = ch.EquipedSkills.Select(skill => skill.ID).ToList();
            // �����\�E�g�p�\������
            if (s.CanEquip(ch) || (s.CanUseInMenu && s.CanUse(ch)))
            {
                // �{�^���������̃A�N�V������ǉ�
                AddOnClickActionToSkillButton(newButton, s, ch);
            }
            // ������
            else if (equipedSkillIDs.Contains(s.ID))
            {
                comp.skillName.color = CommonController.GetColor(colors[currentCharacterIndex]);
            }
            else
            {
                // �{�^���^�C�g�����O���[�A�E�g
                comp.skillName.color = CommonController.GetColor(Constants.darkGray);
            }
        }
        await UniTask.DelayFrame(1);
    }

    /// <summary>
    /// �{�^���I�����̓����ݒ�
    /// </summary>
    /// <param name="button"></param>
    /// <param name="item"></param>
    public void AddSelectActionToButtons(GameObject button, Skill s)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Select; // Select�C�x���g�����b�X��
        entry.callback.AddListener((data) =>
        {
            SetSkillDetail(button, s);
        });

        // �G���g�����g���K�[���X�g�ɒǉ�
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// �X�L���ڍח�������������
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
    /// �X�L���ڍח���ݒ肷��
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

        #region �����E�N���X
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
    /// �����ꗗ�{�^���������̓����ݒ肷��
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
    /// �{�^���������A�N�V�������{�^���ɐݒ�
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
    /// �L�����N�^�[�I���T�u���j���[��\������
    /// </summary>
    /// <param name="item"></param>
    private void DisplaySubMenu(Skill skill)
    {

        var selectedButton = EventSystem.current.currentSelectedGameObject;
        // �A�C�e���I����
        Vector3 pos = selectedButton.transform.position;

        // �ꗗ�̉����̃A�C�e�����I�����ꂽ��T�u�E�C���h�E���J�[�\���̏㑤�ɕ\��
        float offset = pos.y < 320 ? 85 : -100;

        // �J�[�\���ʒu���L�����邽�߁A�I�𒆂̃A�C�e���̃C���f�b�N�X��ۑ�
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
    /// �L�����N�^�[�؂�ւ�
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

    ///�@--------------------------------------------------------------- ///
    ///�@--------------------------- �ėp���� --------------------------- ///
    ///�@--------------------------------------------------------------- ///

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

    /// <summary>
    /// �L�����Z���{�^������������
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
                // �A�C�e�����j���[�̃t�F�[�h�A�E�g
                await FadeOut(gameObject, 0.3f);
                if (mainMenuController != null)
                {
                    // ���C�����j���[�̏�����
                    await mainMenuController.InitializeFromChildren("Skill");
                }
                // �A�C�e�����j���[�C���X�^���X�̔j��
                gameObject.SetActive(false);
            }
            isClosing = false;
        }
    }

    /// <summary>
    /// �ėp�{�^�������� - �������X�L�����͂���
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
    /// R�{�^������������ - �X�L���J�e�S���؂�ւ� - ���y�[�W
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
    /// L�{�^������������ - �X�L���J�e�S���؂�ւ� - �O�y�[�W
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
    /// RT�{�^������������ - �L�����N�^�[�؂�ւ� - ���y�[�W
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
    /// LT�{�^������������ - �L�����N�^�[�؂�ւ� - �O�y�[�W
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
    /// ����L�����N�^�[���̍X�V
    /// </summary>
    private void UpdateCharacterName()
    {
        currentCharacterName.text = characterNames[currentCharacterIndex];
        nextCharacterName.text = characterNames[(currentCharacterIndex + 1) % characterNames.Count];
        previousCharacterName.text = characterNames[(currentCharacterIndex - 1 + characterNames.Count) % characterNames.Count];

        //ClearSkillDetailInfo();
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

    public async UniTask FadeOut(GameObject gameObject, float duration = 0.3f)
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
