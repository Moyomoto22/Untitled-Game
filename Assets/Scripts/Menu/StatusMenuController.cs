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
    public GameObject HPGauge;
    public TextMeshProUGUI MM;
    public TextMeshProUGUI M;
    public GameObject MPGauge;
    public TextMeshProUGUI MT;
    public TextMeshProUGUI T;
    public GameObject TPGauge;
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

        characterImage.sprite = ch.CharacterClass.imagesA[currentCharacterIndex];
        characterLevel.text = ch.Level.ToString();
        characterClassAb.text = ch.CharacterClass.classAbbreviation;

        currentExp.text = ch.GetCurrentClassEarnedExp().ToString();
        nextExp.text = ch.GetCurrentClassNextExp().ToString();
        totalExp.text = ch.TotalExperience.ToString();

        CL.text = ch.CharacterClass.className;
        MH.text = ch.MaxHp.ToString();
        H.text = ch.HP.ToString();
        MM.text = ch.MaxMp.ToString();
        M.text = ch.MP.ToString();
        MT.text = "100";
        T.text = ch.TP.ToString();
        PA.text = ch.PAttack.ToString();
        MA.text = ch.MAttack.ToString();
        PD.text = ch.PDefence.ToString();
        MD.text = ch.MDefence.ToString();
        ST.text = ch.Str.ToString();
        VI.text = ch.Vit.ToString();
        DE.text = ch.Dex.ToString();
        AG.text = ch.Agi.ToString();
        IT.text = ch.Int.ToString();
        MN.text = ch.Mnd.ToString();
        PC.text = ch.CriticalRate.ToString() + " %";
        MC.text = " %";
        PAV.text = ch.EvationRate.ToString() + " %";
        MAV.text = " %";

        UpdateGauges();

        leftArm.color = Color.white;

        rightArm.text = ch.RightArm != null ? ch.RightArm.ItemName : "なし";
        leftArm.text = ch.LeftArm != null ? ch.LeftArm.ItemName : "なし";
        head.text = ch.Head != null ? ch.Head.ItemName : "なし";
        body.text = ch.Body != null ? ch.Body.ItemName : "なし";
        accessaryOne.text = ch.Accessary1 != null ? ch.Accessary1.ItemName : "なし";
        accessaryTwo.text = ch.Accessary2 != null ? ch.Accessary2.ItemName : "なし";

        if (ch.RightArm != null)
        {
            rightArmIcon.enabled = true;
            rightArmIcon.sprite = ch.RightArm.IconImage;
        }
        else
        {
            rightArmIcon.enabled = false;
        }

        if (ch.LeftArm != null)
        {
            leftArmIcon.enabled = true;
            leftArmIcon.sprite = ch.LeftArm.IconImage;
        }
        else
        {
            leftArmIcon.enabled = false;
            if (ch.RightArm != null)
            {
                if (ch.RightArm.IsTwoHanded)
                {
                    leftArm.text = ch.RightArm.ItemName;
                    leftArm.color = CommonController.GetColor(Constants.darkGray);
                }
            }
            leftArmIcon.enabled = false;
        }

        if (ch.Head != null)
        {
            headIcon.enabled = true;
            headIcon.sprite = ch.Head.IconImage;
        }
        else
        {
            headIcon.enabled = false;
        }

        if (ch.Body != null)
        {
            bodyIcon.enabled = true;
            bodyIcon.sprite = ch.Body.IconImage;
        }
        else
        {
            bodyIcon.enabled = false;
        }

        if (ch.Accessary1 != null)
        {
            accessaryOneIcon.enabled = true;
            accessaryOneIcon.sprite = ch.Accessary1.IconImage;
        }
        else
        {
            accessaryOneIcon.enabled = false;
        }

        if (ch.Accessary2 != null)
        {
            accessaryTwoIcon.enabled = true;
            accessaryTwoIcon.sprite = ch.Accessary2.IconImage;
        }
        else
        {
            accessaryTwoIcon.enabled = false;
        }

        slash.text = ch.ResistSlash.ToString() + " %";
        thrast.text = ch.ResistThrast.ToString() + " %";
        blow.text = ch.ResistBlow.ToString() + " %";
        magic.text = ch.ResistMagic.ToString() + " %";
        fire.text = ch.ResistFire.ToString() + " %";
        ice.text = ch.ResistIce.ToString() + " %";
        thunder.text = ch.ResistThunder.ToString() + " %";
        wind.text = ch.ResistWind.ToString() + " %";
        poison.text = ch.ResistPoison.ToString() + " %";
        paralyze.text = ch.ResistParalyze.ToString() + " %";
        asleep.text = ch.ResistAsleep.ToString() + " %";
        silence.text = ch.ResistSilence.ToString() + " %";
        daze.text = ch.ResistDaze.ToString() + " %";
        charm.text = ch.ResistCharm.ToString() + " %";
        flostbite.text = ch.ResistFrostBite.ToString() + " %";

        swordLevel.text = ch.SwordLevel.ToString();
        bladeLevel.text = ch.BladeLevel.ToString();
        daggerLevel.text = ch.DaggerLevel.ToString();
        spearLevel.text = ch.SpearLevel.ToString();
        axLevel.text = ch.AxLevel.ToString();
        hammerLevel.text = ch.HammerLevel.ToString();
        fistLevel.text = ch.FistLevel.ToString();
        bowLevel.text = ch.BowLevel.ToString();
        staffLevel.text = ch.StaffLevel.ToString();
        shieldLevel.text = ch.ShieldLevel.ToString();

        GaugeManager manager = swordGauge.GetComponent<GaugeManager>();
        manager.updateGauge(ch.NextExperienceSword, ch.EarnedExperienceSword);
        manager = bladeGauge.GetComponent<GaugeManager>();
        manager.updateGauge(ch.NextExperienceBlade, ch.EarnedExperienceBlade);
        manager = daggerGauge.GetComponent<GaugeManager>();
        manager.updateGauge(ch.NextExperienceDagger, ch.EarnedExperienceDagger);
        manager = spearGauge.GetComponent<GaugeManager>();
        manager.updateGauge(ch.NextExperienceSpear, ch.EarnedExperienceSpear);
        manager = axGauge.GetComponent<GaugeManager>();
        manager.updateGauge(ch.NextExperienceAx, ch.EarnedExperienceAx);
        manager = hammerGauge.GetComponent<GaugeManager>();
        manager.updateGauge(ch.NextExperienceHammer, ch.EarnedExperienceHammer);
        manager = fistGauge.GetComponent<GaugeManager>();
        manager.updateGauge(ch.NextExperienceFist, ch.EarnedExperienceFist);
        manager = bowGauge.GetComponent<GaugeManager>();
        manager.updateGauge(ch.NextExperienceBow, ch.EarnedExperienceBow);
        manager = staffGauge.GetComponent<GaugeManager>();
        manager.updateGauge(ch.NextExperienceStaff, ch.EarnedExperienceStaff);
        manager = shieldGauge.GetComponent<GaugeManager>();
        manager.updateGauge(ch.NextExperienceShield, ch.EarnedExperienceShield);

        war.text = ch.ClassLevels[0].ToString();
        pld.text = ch.ClassLevels[1].ToString();
        mnk.text = ch.ClassLevels[2].ToString();
        thf.text = ch.ClassLevels[3].ToString();
        rng.text = ch.ClassLevels[4].ToString();
        src.text = ch.ClassLevels[5].ToString();
        clc.text = ch.ClassLevels[6].ToString();
        sps.text = ch.ClassLevels[7].ToString();

        //GaugeManager gaugeManager = SPGauge.GetComponent<GaugeManager>();
        //gaugeManager.updateGaugeByText();
    }

    private void UpdateGauges()
    {
        var e = EXPGauge.GetComponent<GaugeManager>();
        var h = HPGauge.GetComponent<GaugeManager>();
        var m = MPGauge.GetComponent<GaugeManager>();
        var t = TPGauge.GetComponent<GaugeManager>();

        e.updateGaugeByText();
        h.updateGaugeByText();
        m.updateGaugeByText();
        t.updateGaugeByText();
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
            SoundManager.Instance.PlayCancel();

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
            SoundManager.Instance.PlaySelect(0.5f);

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
            SoundManager.Instance.PlaySelect(0.5f);

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
