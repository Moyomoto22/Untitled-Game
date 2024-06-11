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

        SetInputActions();      // InputAction設定

        ToggleButtonsInteractable(mainButtonsParent, true);
        ToggleButtonsInteractable(saveSlotParent, false);

        SelectButton(mainButtonsParent);
        setButtonFillAmount(mainButtonsParent, 0);

        foreach(var controller in saveDataSlotControllers)
        {
            controller.isSaving = true;
        }

        await FadeIn();          // 画面フェードイン 
    }

    public void OnPressSaveButton()
    {
        ToggleButtonsInteractable(mainButtonsParent, false);

        ToggleButtonsInteractable(saveSlotParent, true);

        // オートセーブスロットのみ選択不可
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

        SetInfoMessage("セーブするスロットを選択してください。");
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

        SetInfoMessage("ロードするスロットを選択してください。");
    }

    private void SetInfoMessage(string message)
    {
        if (mainMenuController.info != null)
        {
            mainMenuController.info.text = message;
        }
    }

    /// <summary>
    /// ボタン選択時Info欄の内容を切り替える
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

    ///　--------------------------------------------------------------- ///
    ///　--------------------------- 汎用処理 --------------------------- ///
    ///　--------------------------------------------------------------- ///
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

    /// <summary>
    /// キャンセルボタン押下時処理
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
                // 現在メニューのフェードアウト
                await FadeOut(gameObject, 0.3f);
                if (mainMenuController != null)
                {
                // メインメニューの初期化
                await mainMenuController.InitializeFromChildren("System");
                }
                // 現在メニューインスタンスの破棄
                gameObject.SetActive(false);
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
        }
    }

    /// <summary>
    /// Rボタン押下時処理
    /// </summary>
    public async void OnPressRSButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
        }
    }

    /// <summary>
    /// Lボタン押下時処理
    /// </summary>
    public async void OnPressLSButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
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

    public async UniTask FadeOut(GameObject gameObject, float duration = 0.3f)
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
