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

public class StatusMenuController : MonoBehaviour
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

    public TextMeshProUGUI characterClassAb;
    public TextMeshProUGUI characterLevel;

    public TextMeshProUGUI currentExp;
    public TextMeshProUGUI nextExp;
    public GameObject EXPGauge;
    public TextMeshProUGUI totalExp;

    public Image rightArmIcon;
    public TextMeshProUGUI rightArm;
    public Image leftArmIcon;
    public TextMeshProUGUI leftArm;
    public Image headIcon;
    public TextMeshProUGUI head;
    public Image bodyIcon;
    public TextMeshProUGUI body;
    public Image accessaryOneIcon;
    public TextMeshProUGUI accessaryOne;
    public Image accessaryTwoIcon;
    public TextMeshProUGUI accessaryTwo;

    #region ステータス欄
    public TextMeshProUGUI CL;
    public TextMeshProUGUI MH;
    public TextMeshProUGUI H;
    public TextMeshProUGUI MM;
    public TextMeshProUGUI M;
    public TextMeshProUGUI MT;
    public TextMeshProUGUI T;
    public TextMeshProUGUI PA;
    public TextMeshProUGUI MA;
    public TextMeshProUGUI PD;
    public TextMeshProUGUI MD;
    public TextMeshProUGUI ST;
    public TextMeshProUGUI VI;
    public TextMeshProUGUI DE;
    public TextMeshProUGUI AG;
    public TextMeshProUGUI IT;
    public TextMeshProUGUI MN;
    public TextMeshProUGUI PC;
    public TextMeshProUGUI MC;
    public TextMeshProUGUI PAV;
    public TextMeshProUGUI MAV;
    #endregion

    public TextMeshProUGUI slash;
    public TextMeshProUGUI thrast;
    public TextMeshProUGUI blow;
    public TextMeshProUGUI magic;
    public TextMeshProUGUI fire;
    public TextMeshProUGUI ice;
    public TextMeshProUGUI thunder;
    public TextMeshProUGUI wind;

    public TextMeshProUGUI poison;
    public TextMeshProUGUI paralyze;
    public TextMeshProUGUI asleep;
    public TextMeshProUGUI silence;
    public TextMeshProUGUI daze;
    public TextMeshProUGUI charm;
    public TextMeshProUGUI flostbite;

    public TextMeshProUGUI swordLevel;
    public TextMeshProUGUI bladeLevel;
    public TextMeshProUGUI daggerLevel;
    public TextMeshProUGUI spearLevel;
    public TextMeshProUGUI axLevel;
    public TextMeshProUGUI hammerLevel;
    public TextMeshProUGUI fistLevel;
    public TextMeshProUGUI bowLevel;
    public TextMeshProUGUI staffLevel;
    public TextMeshProUGUI shieldLevel;

    public GameObject swordGauge;
    public GameObject bladeGauge;
    public GameObject daggerGauge;
    public GameObject spearGauge;
    public GameObject axGauge;
    public GameObject hammerGauge;
    public GameObject fistGauge;
    public GameObject bowGauge;
    public GameObject staffGauge;
    public GameObject shieldGauge;

    public TextMeshProUGUI war;
    public TextMeshProUGUI pld;
    public TextMeshProUGUI mnk;
    public TextMeshProUGUI thf;
    public TextMeshProUGUI rng;
    public TextMeshProUGUI src;
    public TextMeshProUGUI clc;
    public TextMeshProUGUI sps;

    public GameObject content;

    public EventSystem eventSystem;
    public PlayerInput playerInput;

    private int lastSelectButtonIndex;

    async void OnEnable()
    {
        await Initialize();
    }

    private void OnDisable()
    {
        RemoveInputActions();
    }

    /// <summary>
    /// スキル画面初期化
    /// </summary>
    public async UniTask Initialize()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        playerInput = FindObjectOfType<PlayerInput>();

        SetCharacterInfo();     // キャラクター情報設定
        UpdateCharacterName();  // 左上キャラクター名初期化

        SetInputActions();      // InputAction設定

        await FadeIn();          // 画面フェードイン 
    }

    public async UniTask Init()
    {
        //selectedSkill = null;
        //await SetSkillList(); // スキル選択欄設定

        //setButtonFillAmount(content, lastSelectButtonIndex);
        //SelectButton(content, lastSelectButtonIndex); // 装備部位選択欄を選択
    }

    /// <summary>
    /// キャラクター情報を設定する
    /// </summary>
    public void SetCharacterInfo()
    {
        var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
        var color = CommonController.GetCharacterColorByIndex(currentCharacterIndex);

        gradation.color = color;
        nameBackGradation.color = color;

        characterImage.sprite = ch.Class.imagesA[currentCharacterIndex];
        characterLevel.text = ch.level.ToString();
        characterClassAb.text = ch.Class.classAbbreviation;

        currentExp.text = ch.earnedExperience.ToString();
        nextExp.text = ch.nextExperience.ToString();
        totalExp.text = ch.totalExperience.ToString();

        CL.text = ch.Class.className;
        MH.text = ch.maxHp2.ToString();
        H.text = ch.hp.ToString();
        MM.text = ch.maxMp2.ToString();
        M.text = ch.mp.ToString();
        MT.text = ch.maxTp.ToString();
        T.text = ch.tp.ToString();
        PA.text = ch.pAttack.ToString();
        MA.text = ch.mAttack.ToString();
        PD.text = ch.pDefence.ToString();
        MD.text = ch.mDefence.ToString();
        ST.text = ch.str2.ToString();
        VI.text = ch.vit2.ToString();
        DE.text = ch.dex2.ToString();
        AG.text = ch.agi2.ToString();
        IT.text = ch.inte2.ToString();
        MN.text = ch.mnd2.ToString();
        PC.text = ch.pCrit.ToString() + " %";
        MC.text = ch.mCrit.ToString() + " %";
        PAV.text = ch.pAvo.ToString() + " %";
        MAV.text = ch.mAvo.ToString() + " %";

        leftArm.color = Color.white;

        rightArm.text = ch.rightArm != null ? ch.rightArm.itemName : "なし";
        leftArm.text = ch.leftArm != null ? ch.leftArm.itemName : "なし";
        head.text = ch.head != null ? ch.head.itemName : "なし";
        body.text = ch.body != null ? ch.body.itemName : "なし";
        accessaryOne.text = ch.accessary1 != null ? ch.accessary1.itemName : "なし";
        accessaryTwo.text = ch.accessary2 != null ? ch.accessary2.itemName : "なし";

        if (ch.rightArm != null)
        {
            rightArmIcon.enabled = true;
            rightArmIcon.sprite = ch.rightArm.iconImage;
        }
        else
        {
            rightArmIcon.enabled = false;
        }

        if (ch.leftArm != null)
        {
            leftArmIcon.enabled = true;
            leftArmIcon.sprite = ch.leftArm.iconImage;
        }
        else
        {
            leftArmIcon.enabled = false;
            if (ch.rightArm != null)
            {
                if (ch.rightArm.isTwoHanded)
                {
                    leftArm.text = ch.rightArm.itemName;
                    leftArm.color = CommonController.GetColor(Constants.darkGray);
                }
            }
            leftArmIcon.enabled = false;
        }

        if (ch.head != null)
        {
            headIcon.enabled = true;
            headIcon.sprite = ch.head.iconImage;
        }
        else
        {
            headIcon.enabled = false;
        }

        if (ch.body != null)
        {
            bodyIcon.enabled = true;
            bodyIcon.sprite = ch.body.iconImage;
        }
        else
        {
            bodyIcon.enabled = false;
        }

        if (ch.accessary1 != null)
        {
            accessaryOneIcon.enabled = true;
            accessaryOneIcon.sprite = ch.accessary1.iconImage;
        }
        else
        {
            accessaryOneIcon.enabled = false;
        }

        if (ch.accessary2 != null)
        {
            accessaryTwoIcon.enabled = true;
            accessaryTwoIcon.sprite = ch.accessary2.iconImage;
        }
        else
        {
            accessaryTwoIcon.enabled = false;
        }

        slash.text = ch.resistSlash.ToString() + " %";
        thrast.text = ch.resistThrast.ToString() + " %";
        blow.text = ch.resistBlow.ToString() + " %";
        magic.text = ch.resistMagic.ToString() + " %";
        fire.text = ch.resistFire.ToString() + " %";
        ice.text = ch.resistIce.ToString() + " %";
        thunder.text = ch.resistThunder.ToString() + " %";
        wind.text = ch.resistWind.ToString() + " %";
        poison.text = ch.resistPoison.ToString() + " %";
        paralyze.text = ch.resistParalyze.ToString() + " %";
        asleep.text = ch.resistAsleep.ToString() + " %";
        silence.text = ch.resistSilence.ToString() + " %";
        daze.text = ch.resistDaze.ToString() + " %";
        charm.text = ch.resistCharm.ToString() + " %";
        flostbite.text = ch.resistFrostBite.ToString() + " %";

        swordLevel.text = ch.swordLevel.ToString();
        bladeLevel.text = ch.bladeLevel.ToString();
        daggerLevel.text = ch.daggerLevel.ToString();
        spearLevel.text = ch.spearLevel.ToString();
        axLevel.text = ch.axLevel.ToString();
        hammerLevel.text = ch.hammerLevel.ToString();
        fistLevel.text = ch.fistLevel.ToString();
        bowLevel.text = ch.bowLevel.ToString();
        staffLevel.text = ch.staffLevel.ToString();
        shieldLevel.text = ch.shieldLevel.ToString();

        war.text = ch.classLevels[0].ToString();
        pld.text = ch.classLevels[1].ToString();
        mnk.text = ch.classLevels[2].ToString();
        thf.text = ch.classLevels[3].ToString();
        rng.text = ch.classLevels[4].ToString();
        src.text = ch.classLevels[5].ToString();
        clc.text = ch.classLevels[6].ToString();
        sps.text = ch.classLevels[7].ToString();

        //GaugeManager gaugeManager = SPGauge.GetComponent<GaugeManager>();
        //gaugeManager.updateGaugeByText();
    }

    /// <summary>
    /// キャラクター切り替え
    /// </summary>
    /// <returns></returns>
    private async UniTask ChangeCharacter()
    {
        float duration = 0.2f;

        await characterImage.gameObject.GetComponent<SpriteManipulator>().FadeOut(duration);
        SetCharacterInfo();

        Color color = CommonController.GetCharacterColorByIndex(currentCharacterIndex);

        //ToggleButtonsInteractable(content, true);

        SetCharacterInfo();

        await UniTask.WhenAll(
            characterImage.gameObject.GetComponent<SpriteManipulator>().FadeIn(duration),
            gradation.gameObject.GetComponent<SpriteManipulator>().AnimateColor(color, duration * 2),
            nameBackGradation.gameObject.GetComponent<SpriteManipulator>().AnimateColor(color, duration * 2)
            );
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
            //if (subWindowInstance != null)
            //{
            //    Destroy(subWindowInstance);
            //    await Init();
            //}
            //else
            //{
                // アイテムメニューのフェードアウト
                await FadeOut(gameObject, 0.3f);
                if (mainMenuController != null)
                {
                    // メインメニューの初期化
                    await mainMenuController.InitializeFromChildren("Status");
                }
                // アイテムメニューインスタンスの破棄
                gameObject.SetActive(false);
            //}
        }
    }

    /// <summary>
    /// 汎用ボタン押下時 - 装備中スキルをはずす
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressGeneralButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //if (selectedSkill != null)
            //{
            //    var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
            //    await ch.UnEquipSkill(selectedSkill);

            //    await Init();
            //}
        }
    }

    /// <summary>
    /// Rボタン押下時処理 - スキルカテゴリ切り替え - 次ページ
    /// </summary>
    public async void OnPressRSButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
        }
    }

    /// <summary>
    /// Lボタン押下時処理 - スキルカテゴリ切り替え - 前ページ
    /// </summary>
    public async void OnPressLSButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
        }
    }

    /// <summary>
    /// RTボタン押下時処理 - キャラクター切り替え - 次ページ
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
    /// LTボタン押下時処理 - キャラクター切り替え - 前ページ
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
    /// 左上キャラクター名称更新
    /// </summary>
    private void UpdateCharacterName()
    {
        currentCharacterName.text = characterNames[currentCharacterIndex];
        nextCharacterName.text = characterNames[(currentCharacterIndex + 1) % characterNames.Count];
        previousCharacterName.text = characterNames[(currentCharacterIndex - 1 + characterNames.Count) % characterNames.Count];

        //ClearSkillDetailInfo();
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
