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
    /// ����������
    /// </summary>
    /// <returns></returns>
    public async UniTask Initialize()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        SetInputActions();

        var duration = 0.5f;

        // �w�i�X�v���C�g�̃t�F�[�h�C��
        SpriteManipulator manipulator = BackGroundImage.GetComponent<SpriteManipulator>();
        await manipulator.FadeIn(0.5f);

        // �X�e�[�^�X�ݒ�
        SetAllyes();

        // �{�^���ƃL�����N�^�[�g�̃A�j���[�V����
        await UniTask.WhenAll(
            SlideButtons(duration),
            SlideFrames(duration)
            );

        // �擪�̃{�^����I��
        SelectButton();

        //var image = BackGroundImage.GetComponent<Image>();

        //image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        //await image.DOFade(1, 1.0f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
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
            var cancel = actionMap.FindAction("Cancel");
            var openMenu = actionMap.FindAction("OpenMenu");
            var general = actionMap.FindAction("General");

            cancel.performed += OnPressCancelButton;
            openMenu.performed += OnPressMenuButton;
            general.performed += OnPressGeneralButton;
            // �A�N�V�����}�b�v��L���ɂ���
            actionMap.Enable();
        }
    }

    private void RemoveInputActions()
    {
        // �C�x���g���X�i�[������
        if (playerInput != null)
        {
            // InputActionAsset���擾
            var inputActionAsset = playerInput.actions;
            // "Main"�A�N�V�����}�b�v���擾
            var actionMap = inputActionAsset.FindActionMap("Menu");
            // �A�N�V�������擾
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

            characterImages[i].sprite = ally.CharacterClass.imagesB[i];                    // �L�����N�^�[�摜
            classNames[i].text = ally.CharacterClass.classAbbreviation;
            levels[i].text = ally.Level.ToString();
            ally.HPGauge = HPGauges[i];                                           // HP�Q�[�W
            ally.MPGauge = MPGauges[i];                                           // MP�Q�[�W
            ally.TPGauge = TPGauges[i];                                           // TP�Q�[�W

            GaugeManager hpGaugeManager = HPGauges[i].GetComponent<GaugeManager>();     // HP�Q�[�W�Ǘ��N���X
            GaugeManager mpGaugeManager = MPGauges[i].GetComponent<GaugeManager>();     // MP�Q�[�W�Ǘ��N���X
            GaugeManager tpGaugeManager = TPGauges[i].GetComponent<GaugeManager>();     // TP�Q�[�W�Ǘ��N���X
            GaugeManager expGaugeManager = EXPGauges[i].GetComponent<GaugeManager>();     // EXP�Q�[�W�Ǘ��N���X

            hpGaugeManager.maxValueText.text = ally.MaxHp.ToString();            // �ő�HP�e�L�X�g
            hpGaugeManager.currentValueText.text = ally.HP.ToString();            // ����HP�e�L�X�g
            mpGaugeManager.maxValueText.text = ally.MaxMp.ToString();            // �ő�MP�e�L�X�g
            mpGaugeManager.currentValueText.text = ally.MP.ToString();            // ����MP�e�L�X�g
            tpGaugeManager.currentValueText.text = ally.TP.ToString();            // ����TP�e�L�X�g
            expGaugeManager.maxValueText.text = ally.GetCurrentClassNextExp().ToString();            // �ő�EXP�e�L�X�g
            expGaugeManager.currentValueText.text = ally.GetCurrentClassEarnedExp().ToString();            // ����EXP�e�L�X�g

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
    /// �L�����Z���{�^������
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
    /// ���j���[�{�^������
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
    /// �ėp�{�^��(��)����
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
    /// ���j���[��ʂ��J��
    /// </summary>
    public void OpenMenu()
    {
        // �v���C���[�̓������~�߂�
        CommonVariableManager.playerCanMove = false;

        CommonController.PauseGame();
        MenuOutline.SetActive(true);
    }

    /// <summary>
    /// ���j���[��ʂ����
    /// </summary>
    public async UniTask CloseMenu()
    {
        await FadeOutChildren(gameObject);
        Destroy(gameObject);
        CommonController.ResumeGame();
    }

    /// <summary>
    /// ���C�����j���[ Item�{�^��������
    /// �A�C�e����ʑJ�ڏ���
    /// </summary>
    public async void OnPressItemButton()
    {
        SoundManager.Instance.PlaySubmit();
        
        eventSystem.enabled = false;
        await FadeOutChildren(main);
        RemoveInputActions();
        header.text = "�A�C�e��";
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
        header.text = "����";
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
        header.text = "�X�L��";
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
        header.text = "�N���X";
        var controller = classMenuObject.GetComponent<ClassMenuController>();
        controller.currentCharacterIndex = characterIndex;
        classMenuObject.SetActive(true);
        eventSystem.enabled = true;
    }

    /// <summary>
    /// ���C�����j���[ Status�{�^��������
    /// �A�C�e����ʑJ�ڏ���
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
        header.text = "�X�e�[�^�X";
        var controller = statusMenuObject.GetComponent<StatusMenuController>();
        controller.currentCharacterIndex = characterIndex;
        statusMenuObject.SetActive(true);
        eventSystem.enabled = true;
    }

    /// <summary>
    /// ���C�����j���[ �V�X�e���{�^��������
    /// �V�X�e����ʑJ�ڏ���
    /// </summary>
    public async void OnPressSystemButton()
    {
        SoundManager.Instance.PlaySubmit();

        eventSystem.enabled = false;
        await FadeOutChildren(main);
        RemoveInputActions();
        header.text = "�V�X�e��";
        systemMenuObject.SetActive(true);
        //var controller = systemMenuObject.GetComponent<SystemMenuController>();
        eventSystem.enabled = true;
    }

    /// <summary>
    /// �L�����N�^�[�I���T�u���j���[��\������
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
    /// �{�^���I��
    /// </summary>
    /// <param name="number"></param>
    public void SelectButton(int number = 0)
    {
        if (eventSystem != null)
        {
            var button = buttons[number];

            // �X�N���v�g����I����Ԃɂ���ꍇ�A���ʉ��͖炳�Ȃ�
            var controller = button.GetComponent<MainMenuButtonManager>();
            if (controller != null)
            {
                controller.shouldPlaySound = false;
            }

            eventSystem.SetSelectedGameObject(button.transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// �{�^���I����Info���̓��e��؂�ւ���
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
    /// �{�^����Interactable��؂�ւ���
    /// </summary>
    /// <param name="interactable">�L��/����</param>
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
    /// �{�^����FillAmount�𑀍삷��
    /// </summary>
    /// <param name="number">�Ώۃ{�^���̃R�}���h���ł̃C���f�b�N�X</param>
    public void setButtonFillAmount(int number)
    {
        int numberOfChildren = buttons.Count;

        // �ΏۃC���f�b�N�X�ɊY������{�^���̂�FillAmount��1�ɂ��A����ȊO��0�ɂ���
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
        // ����̖��O��InputAction��T���ėL����
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
        header.text = "���C�����j���[";
        //EnableInputActionMap("Main", fromMenuName);
        // �X�e�[�^�X�ݒ�
        SetAllyes();
        SetInputActions();
        await FadeInChildren(main);
        SelectButton(lastSelectButtonIndex);
    }

    public async UniTask FadeInChildren(GameObject gameObject, float duration = 0.3f)
    {
        // �eGameObject�̎q�v�f���ׂĂɑ΂��ăt�F�[�h�C����K�p
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            // �����x��0�ɃA�j���[�V����
            await canvasGroup.DOFade(1, duration).SetEase(Ease.InOutQuad).SetUpdate(true).ToUniTask();
            canvasGroup.interactable = true;
            //gameObject.SetActive(false);
        }
    }

    public async UniTask FadeOutChildren(GameObject gameObject, float duration = 0.5f)
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
