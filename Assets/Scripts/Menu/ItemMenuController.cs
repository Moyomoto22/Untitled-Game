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

        /// �t�B���^����J�e�S����������
        currentCategoryIndex = 0;
        SetCategoryImage();

        ToggleButtonsInteractable(content);

        await SetItems();
        // ��ԏ�̃{�^����I��
        SelectButton(0);
        setButtonFillAmount(0);

        await FadeIn();
        await UniTask.DelayFrame(1);
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
            var cancel = actionMap.FindAction("Cancel");
            if (rs == null || ls == null || cancel == null)
            {
                Debug.LogError("Actions not found!");
                return;
            }

            // �C�x���g���X�i�[��ݒ�
            cancel.performed += OnPressCancelButton;
            rs.performed += OnPressNextButton;
            ls.performed += OnPressPreviousButton;

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
    /// �L�����Z���{�^������
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
                // �A�C�e�����j���[�̃t�F�[�h�A�E�g
                await FadeOutChildren(gameObject, 0.3f);
                if (mainMenuController != null)
                {
                    // ���C�����j���[�̏�����
                    await mainMenuController.InitializeFromChildren("Item");
                }
                // �A�C�e�����j���[�C���X�^���X�̔j��
                gameObject.SetActive(false);
            }
            isClosing = false;
        }
    }

    /// <summary>
    /// ����(R1)�{�^������
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressNextButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("RightShoulderButton has pressed.");
            await DestroyButtons();
            // �J�e�S���؂�ւ�
            await NextCategory();
            // ��ԏ�̃{�^����I��
            SelectButton(0);
        }
    }

    /// <summary>
    /// �O��(L1)�{�^������
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressPreviousButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("LeftShoulderButton has pressed.");
            await DestroyButtons();
            // �J�e�S���؂�ւ�
            await PreviousCategory();
            // ��ԏ�̃{�^����I��
            SelectButton(0);
        }
    }

    /// <summary>
    /// �A�C�e���{�^�����ꗗ�ɃZ�b�g
    /// </summary>
    public async UniTask SetItems()
    {
        await DestroyButtons();

        // �t�B���^����A�C�e���J�e�S�����擾
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

        // �V���O���g�����珊���A�C�e���ꗗ���擾���A�J�e�S���Ńt�B���^
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

        HashSet<string> processedItemIds = new HashSet<string>();  // �o�^�ς݃A�C�e����ID���L�^���邽�߂�HashSet

        // �����A�C�e�����ꗗ�Ƀ{�^���Ƃ��ăZ�b�g���Ă���
        foreach (Item item in sortedItems)
        {
            if (processedItemIds.Contains(item.ID))
            {
                continue;  // ���̃A�C�e��ID�����łɏ�������Ă���΁A���̃A�C�e���փX�L�b�v
            }
            //var content = GameObject.FindWithTag("ScrollViewContent");
            GameObject obj = Instantiate(button, content.transform, false);    // �ꗗ�ɕ\������{�^���̃x�[�X���C���X�^���X����
            var comp = obj.GetComponent<ItemComponent>();                      // �{�^���ɕR�Â��X�L�������i�[����R���|�[�l���g
            var newButton = obj.transform.GetChild(0).gameObject;              // �{�^���{��
            var amount = items.Where(i => i.ID == item.ID).ToList().Count;     // �A�C�e��������

            comp.icon.sprite = item.iconImage;                                 // �A�C�R��
            comp.itemName.text = item.itemName;                                // �A�C�e������
            comp.amount.text = amount.ToString();                              // ������
            AddSelectOrDeselectActionToButtons(newButton, item);               // �I���E�I���������A�N�V�����ݒ�
            // �A�C�e�����g�p�\������
            if (item.usable)
            {
                // �{�^���������̃A�N�V������ǉ�
                AddOnClickActionToItemButton(newButton, item);
            }
            else
            {
                // �{�^���^�C�g�����O���[�A�E�g
                comp.itemName.color = CommonController.GetColor(Constants.darkGray);
            }
            processedItemIds.Add(item.ID);                                     // ���̃A�C�e��ID�������ς݂Ƃ��ċL�^
        }
        await UniTask.DelayFrame(1);
    }

    /// <summary>
    /// �{�^����I����Ԃɂ���
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
    /// �{�^���������̓����ݒ肷��
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
    /// �{�^���������A�N�V�������{�^���ɐݒ�
    /// </summary>
    /// <param name="item"></param>
    private void OnClickActionToItemButton(Item item)
    {
        // �Ώۂ��������P�̂̎�
        if (item.target == 2 && !item.isTargetAll)
        {
            DisplaySubMenu(item);
        }
        else if (item.target == 2 && item.isTargetAll)
        {

        }

    }

    /// <summary>
    /// �L�����N�^�[�I���T�u���j���[��\������
    /// </summary>
    /// <param name="item"></param>
    private void DisplaySubMenu(Item item)
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
            controller.item = item;
            controller.itemMenuController = this;
        }
        ToggleButtonsInteractable(false);
        setButtonFillAmount(lastSelectButtonIndex);
    }

    /// <summary>
    /// �{�^����Interactable��؂�ւ���
    /// </summary>
    /// <param name="interactable">�L��/����</param>
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
    /// �{�^����FillAmount�𑀍삷��
    /// </summary>
    /// <param name="number">�Ώۃ{�^���̃R�}���h���ł̃C���f�b�N�X</param>
    public void setButtonFillAmount(int number)
    {
        int numberOfChildren = content.transform.childCount;

        // �ΏۃC���f�b�N�X�ɊY������{�^���̂�FillAmount��1�ɂ��A����ȊO��0�ɂ���
        for (int i = 0; i < numberOfChildren - 1; i++)
        {
            int fillAmount = i == number ? 1 : 0;
            Transform child = content.transform.GetChild(i);
            Image buttonImage = child.GetComponentInChildren<Image>();
            buttonImage.fillAmount = fillAmount;
        }
    }

    /// <summary>
    /// �{�^���I���E�I���������̓����ݒ�
    /// </summary>
    /// <param name="button"></param>
    /// <param name="item"></param>
    public void AddSelectOrDeselectActionToButtons(GameObject button, Item item)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Select; // Select�C�x���g�����b�X��
        entry.callback.AddListener((data) =>
        {
            // �A�C�e���ڍׂ��ڍח��ɕ\��
            SetItemDetailInfo(item);
        });

        // �G���g�����g���K�[���X�g�ɒǉ�
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// �ڍח��ɃA�C�e���̏ڍׂ�\������
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
    /// �X�L���J�e�S���؂�ւ� - ���y�[�W
    /// </summary>
    public async UniTask NextCategory()
    {
        currentCategoryIndex = (currentCategoryIndex + 1) % categoryImages.Count;

        SetCategoryImage();
        await SetItems();
    }

    /// <summary>
    /// �X�L���J�e�S���؂�ւ� - �O�y�[�W
    /// </summary>
    public async UniTask PreviousCategory()
    {
        currentCategoryIndex = (currentCategoryIndex - 1 + categoryImages.Count) % categoryImages.Count;

        SetCategoryImage();
        await SetItems();
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
        }
    }
}
