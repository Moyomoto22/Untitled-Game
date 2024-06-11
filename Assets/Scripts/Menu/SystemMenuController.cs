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

public class SystemMenuController : MonoBehaviour
{
    public EventSystem eventSystem;
    public PlayerInput playerInput;

    public GameObject mainMenu;
    public MainMenuController mainMenuController;

    public GameObject mainButtonsParent;
    public GameObject saveButton;
    public GameObject loadButton;
    public GameObject quitButton;

    public GameObject saveSlotParent;
    public List<SaveDataSlotController> saveDataSlotControllers;

    private int lastSelectButtonIndex = 0;
    private bool isSelectingSaveSlot = false;

    async void OnEnable()
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

        SetInputActions();      // InputAction�ݒ�

        ToggleButtonsInteractable(mainButtonsParent, true);
        ToggleButtonsInteractable(saveSlotParent, false);

        SelectButton(mainButtonsParent);
        setButtonFillAmount(mainButtonsParent, 0);

        foreach(var controller in saveDataSlotControllers)
        {
            controller.isSaving = true;
        }

        await FadeIn();          // ��ʃt�F�[�h�C�� 
    }

    public void OnPressSaveButton()
    {
        ToggleButtonsInteractable(mainButtonsParent, false);

        ToggleButtonsInteractable(saveSlotParent, true);

        // �I�[�g�Z�[�u�X���b�g�̂ݑI��s��
        Transform child = saveSlotParent.transform.GetChild(0);
        Button button = child.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.interactable = false;
        }

        SelectButton(saveSlotParent, 1);

        isSelectingSaveSlot = true;

        foreach (var controller in saveDataSlotControllers)
        {
            controller.isSaving = true;
        }

        SetInfoMessage("�Z�[�u����X���b�g��I�����Ă��������B");
    }

    public void OnPressLoadButton()
    {
        ToggleButtonsInteractable(mainButtonsParent, false);

        ToggleButtonsInteractable(saveSlotParent, true);

        SelectButton(saveSlotParent, 0);

        isSelectingSaveSlot = true;

        foreach (var controller in saveDataSlotControllers)
        {
            controller.isSaving = false;
        }

        SetInfoMessage("���[�h����X���b�g��I�����Ă��������B");
    }

    private void SetInfoMessage(string message)
    {
        if (mainMenuController.info != null)
        {
            mainMenuController.info.text = message;
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
                infoMessage = Messages.SaveButton;
                break;
            case 1:
                infoMessage = Messages.LoadButton;
                break;
            case 2:
                infoMessage = Messages.QuitButton;
                break;
        }
        SetInfoMessage(infoMessage);
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
        if (context.performed)
        {
            SoundManager.Instance.PlayCancel();

            if (isSelectingSaveSlot)
            {
                ToggleButtonsInteractable(mainButtonsParent, true);
                ToggleButtonsInteractable(saveSlotParent, false);

                SelectButton(mainButtonsParent, lastSelectButtonIndex);
                setButtonFillAmount(mainButtonsParent, lastSelectButtonIndex);

                isSelectingSaveSlot = false;
            }
            else
            {
                // ���݃��j���[�̃t�F�[�h�A�E�g
                await FadeOut(gameObject, 0.3f);
                if (mainMenuController != null)
                {
                // ���C�����j���[�̏�����
                await mainMenuController.InitializeFromChildren("System");
                }
                // ���݃��j���[�C���X�^���X�̔j��
                gameObject.SetActive(false);
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
        }
    }

    /// <summary>
    /// R�{�^������������
    /// </summary>
    public async void OnPressRSButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
        }
    }

    /// <summary>
    /// L�{�^������������
    /// </summary>
    public async void OnPressLSButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
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
