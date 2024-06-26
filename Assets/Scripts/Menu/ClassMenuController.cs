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

public class ClassMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public MainMenuController mainMenuController;

    private List<string> characterNames = new List<string>() { "アレックス", "ニコ", "タバサ", "アリシア" };
    public int currentCharacterIndex = 0;

    public EventSystem eventSystem;
    public PlayerInput playerInput;

    private List<Class> classes;

    public Image gradation;
    public Image nameBackGradation;

    public Image characterImage;
    public TextMeshProUGUI characterClass;
    public TextMeshProUGUI characterLevel;

    public TextMeshProUGUI currentCharacterName;
    public TextMeshProUGUI nextCharacterName;
    public TextMeshProUGUI previousCharacterName;

    public GameObject classList;
    private int currentClassIndex = 0;
    private Class currentClass;

    public TextMeshProUGUI className;
    public TextMeshProUGUI classLevel;
    public TextMeshProUGUI currentExp;
    public TextMeshProUGUI nextExp;
    public TextMeshProUGUI description;
    public TextMeshProUGUI hpRank;
    public TextMeshProUGUI mpRank;
    public TextMeshProUGUI strRank;
    public TextMeshProUGUI vitRank;
    public TextMeshProUGUI dexRank;
    public TextMeshProUGUI agiRank;
    public TextMeshProUGUI intRank;
    public TextMeshProUGUI mndRank;

    public Image sword;
    public Image blade;
    public Image dagger;
    public Image spear;
    public Image ax;
    public Image hammer;
    public Image fist;
    public Image bow;
    public Image staff;
    public Image shield;

    public Image exSkillIcon;
    public TextMeshProUGUI exSkillName;
    public TextMeshProUGUI exSkillDescription;
    public Image nextSkillIcon;
    public TextMeshProUGUI nextSkillName;
    public Image nextNextSkillIcon;
    public TextMeshProUGUI nextNextSkillName;
    public TextMeshProUGUI nextLevel;
    public TextMeshProUGUI nextNextLevel;

    public TextMeshProUGUI MH;
    public TextMeshProUGUI MM;
    public TextMeshProUGUI ST;
    public TextMeshProUGUI VI;
    public TextMeshProUGUI DE;
    public TextMeshProUGUI AG;
    public TextMeshProUGUI IT;
    public TextMeshProUGUI MN;
    public TextMeshProUGUI MS;

    public TextMeshProUGUI mh;
    public TextMeshProUGUI mm;
    public TextMeshProUGUI st;
    public TextMeshProUGUI vi;
    public TextMeshProUGUI de;
    public TextMeshProUGUI ag;
    public TextMeshProUGUI it;
    public TextMeshProUGUI mn;
    public TextMeshProUGUI ms;

    private GameObject lastSelectedButton;
    private int lastSelectButtonIndex = 0;


    private void Start()
    {
        
    }

    private async void OnEnable()
    {
        classes = ClassManager.Instance.AllClasses;
        await Initialize();
    }

    private void OnDisable()
    {
        RemoveInputActions();
    }

    /// <summary>
    /// クラス画面初期化
    /// </summary>
    public async UniTask Initialize()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        playerInput = FindObjectOfType<PlayerInput>();
        var index = GetCharacterClassIndex();
        currentClassIndex = index;

        SetCharacterInfo();     // キャラクター情報設定
        UpdateCharacterName();  // 左上キャラクター名初期化
        InitializeClassList(); // クラス選択欄初期化
        SetClassDetail(); // クラス詳細初期化
        CompareStatus();  // ステータス比較表示

        SetInputActions();      // InputAction設定

        setButtonFillAmount(classList, index);
        SelectButton(classList, index); // 装備部位選択欄を選択

        await FadeIn();          // 画面フェードイン 
    }

    private int GetCharacterClassIndex()
    {
        var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
        var cl = ch.CharacterClass;

        var index = classes.IndexOf(cl);

        if (index > 0)
        {
            return index;
        }
        else
        {
            return 0;
        }
    }

    private void SetInputActions()
    {
        if (playerInput != null)
        {
            // InputActionAssetを取得
            var inputActionAsset = playerInput.actions;

            // "Main"アクションマップを取得
            var actionMap = inputActionAsset.FindActionMap("Menu");
            // アクションを取得
            var rt = actionMap.FindAction("RightTrigger");
            var lt = actionMap.FindAction("LeftTrigger");
            var cancel = actionMap.FindAction("Cancel");

            // Always remove listeners first to prevent duplication
            cancel.performed -= OnPressCancelButton;
            rt.performed -= OnPressRTButton;
            lt.performed -= OnPressLTButton;

            // イベントリスナーを設定
            cancel.performed += OnPressCancelButton;
            rt.performed += OnPressRTButton;
            lt.performed += OnPressLTButton;

            // アクションマップを有効にする
            actionMap.Enable();

            Debug.Log("SetInpuActions has done.");
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
            var rt = actionMap.FindAction("RightTrigger");
            var lt = actionMap.FindAction("LeftTrigger");
            var cancel = actionMap.FindAction("Cancel");

            cancel.performed -= OnPressCancelButton;
            rt.performed -= OnPressRTButton;
            lt.performed -= OnPressLTButton;
        }
    }

    private void InitializeClassList()
    {
        var index = GetCharacterClassIndex();
        int numberOfChildren = classList.transform.childCount;

        for (int i = 0; i < numberOfChildren - 1; i++)
        {
            Color color = i == index ? CommonController.GetColor(Constants.darkGray) : Color.white;
            classList.transform.GetChild(i).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = color;
        }
    }

    private void SetCharacterInfo()
    {
        var i = currentCharacterIndex;
        var ch = PartyMembers.Instance.GetAllyByIndex(i);
        var color = CommonController.GetCharacterColorByIndex(i);
        var index = GetCharacterClassIndex();

        //await manip.FadeOut(0.1f);
        characterImage.sprite = classes[currentClassIndex].imagesA[i];
        //await manip.FadeIn(0.1f);
        
        gradation.color = color;
        nameBackGradation.color = color;
        characterClass.text = ch.CharacterClass.classAbbreviation;
        characterLevel.text = ch.Level.ToString();

        #region ステータス欄
        MH.text = ch.MaxHp.ToString();
        MM.text = ch.MaxMp.ToString();
        ST.text = ch.Str.ToString();
        VI.text = ch.Vit.ToString();
        DE.text = ch.Dex.ToString();
        AG.text = ch.Agi.ToString();
        IT.text = ch.Int.ToString();
        MN.text = ch.Mnd.ToString();
        #endregion
    }

    /// <summary>
    /// 装備前後のステータスを比較してステータス欄に表示する
    /// </summary>
    /// <param name=""></param>
    private void CompareStatus()
    {
        Ally ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
        Ally dummy = PartyMembers.Instance.CopyAlly(ch);
        Class cl = ClassManager.Instance.GetClassByIndex(currentClassIndex);

        dummy.ChangeClass(cl);

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
    /// ステータス欄の比較テキストのスタイルを設定する
    /// </summary>
    /// <param name="t">比較後テキスト</param>
    /// <param name="n">比較後数値</param>
    /// <param name="text">補正値テキスト</param>
    /// <param name="number">補正値</param>
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

    private void SetClassDetail()
    {
        var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
        var cl = ClassManager.Instance.GetClassByIndex(currentClassIndex);
        
        var lv = ch.ClassLevels[currentClassIndex];
        var next = ch.ClassNextExps[currentClassIndex];
        var earned = ch.ClassEarnedExps[currentClassIndex];

        className.text = cl.className;
        classLevel.text = lv.ToString();
        currentExp.text = earned.ToString();
        nextExp.text = next.ToString();

        description.text = cl.description;

        hpRank.text = cl.statusRank[0];
        mpRank.text = cl.statusRank[1];
        strRank.text = cl.statusRank[2];
        vitRank.text = cl.statusRank[3];
        dexRank.text = cl.statusRank[4];
        agiRank.text = cl.statusRank[5];
        intRank.text = cl.statusRank[6];
        mndRank.text = cl.statusRank[7];

        Color darkGray = CommonController.GetColor(Constants.darkGray);

        sword.color = darkGray;
        blade.color = darkGray;
        spear.color = darkGray;
        ax.color = darkGray;
        hammer.color = darkGray;
        fist.color = darkGray;
        bow.color = darkGray;
        staff.color = darkGray;
        shield.color = darkGray;

        if (cl.equippableWeapons.Contains(Constants.WeaponCategory.Sword))
        {
            sword.color = Color.white;
        }
        if (cl.equippableWeapons.Contains(Constants.WeaponCategory.Blade))
        {
            blade.color = Color.white;
        }
        if (cl.equippableWeapons.Contains(Constants.WeaponCategory.Dagger))
        {
            dagger.color = Color.white;
        }
        if (cl.equippableWeapons.Contains(Constants.WeaponCategory.Spear))
        {
            spear.color = Color.white;
        }
        if (cl.equippableWeapons.Contains(Constants.WeaponCategory.Ax))
        {
            ax.color = Color.white;
        }
        if (cl.equippableWeapons.Contains(Constants.WeaponCategory.Hammer))
        {
            hammer.color = Color.white;
        }
        if (cl.equippableWeapons.Contains(Constants.WeaponCategory.Fist))
        {
            fist.color = Color.white;
        }
        if (cl.equippableWeapons.Contains(Constants.WeaponCategory.Bow))
        {
            bow.color = Color.white;
        }
        if (cl.equippableWeapons.Contains(Constants.WeaponCategory.Staff))
        {
            staff.color = Color.white;
        }
        if (cl.equippableWeapons.Contains(Constants.WeaponCategory.Shield))
        {
            shield.color = Color.white;
        }

        exSkillIcon.sprite = cl.exSkill.Icon;
        exSkillName.text = cl.exSkill.SkillName;
        exSkillDescription.text = cl.exSkill.Description;

        Skill nextSkill = null;
        Skill nextNextSkill = null;

        var nextLv = ch.ClassLevels[currentCharacterIndex] + 1;
        var nextNextLv = ch.ClassLevels[currentCharacterIndex] + 2;
        
        if (cl.LearnSkills.Count >= nextLv - 1)
        {
            nextSkill = cl.LearnSkills[nextLv - 1];
        }
        if (cl.LearnSkills.Count >= nextNextLv - 1)
        {
            nextNextSkill = cl.LearnSkills[nextNextLv - 1];
        }

        if (nextSkill != null)
        {
            nextLevel.text = nextLv.ToString();
            nextSkillIcon.enabled = true;
            nextSkillIcon.sprite = nextSkill.Icon;
            nextSkillName.text = nextSkill.SkillName;
        }
        else
        {
            nextLevel.text = nextLv.ToString();
            nextSkillIcon.enabled = false;
            nextSkillName.text = "-";
        }
        if (nextNextSkill != null)
        {
            nextNextLevel.text = nextNextLv.ToString();
            nextNextSkillIcon.enabled = true;
            nextNextSkillIcon.sprite = nextNextSkill.Icon;
            nextNextSkillName.text = nextNextSkill.SkillName;
        }
        else
        {
            nextNextLevel.text = "-";
            nextNextSkillIcon.enabled = false;
            nextNextSkillName.text = "-";
        }

    }

    public void OnSelectClassButtons(int index)
    {
        currentClassIndex = index;

        SetClassDetail();    
        
        SetCharacterInfo();
        CompareStatus();
    }

    /// <summary>
    /// クラスボタン押下時
    /// </summary>
    /// <param name="skill"></param>
    public async void OnPressClassButton(int index)
    {
        SoundManager.Instance.PlaySubmit();

        Class cl = ClassManager.Instance.GetClassByIndex(index);
        int currentCharacterClassIndex = GetCharacterClassIndex();
        if (index != currentCharacterClassIndex)
        {
            Ally ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
            ch.ChangeClass(cl);

            await Initialize();
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// キャンセルボタン押下時処理
    /// </summary>
    public async void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeSelf == true)
        {
            SoundManager.Instance.PlayCancel();
            
            // クラスメニューのフェードアウト
            await FadeOutChildren(gameObject, 0.3f);
            // クラスメニューインスタンスの破棄
            gameObject.SetActive(false);
            await mainMenuController.InitializeFromChildren("Class");
        }
    }

    /// <summary>
    /// R2(ZR・RT)ボタン押下 キャラクター切り替え
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressRTButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("RTButton pressed.");
            
            currentCharacterIndex = (currentCharacterIndex + 1) % characterNames.Count;
            UpdateCharacterName();

            await ChangeCharacter();
        }
    }

    /// <summary>
    /// L2(ZL・LT)ボタン押下 キャラクター切り替え
    /// </summary>
    /// <param name="context"></param>
    public async void OnPressLTButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("LTButton pressed.");

            currentCharacterIndex = (currentCharacterIndex - 1 + characterNames.Count) % characterNames.Count;
            UpdateCharacterName();

            await ChangeCharacter();
        }
    }

    /// <summary>
    /// キャラクター切り替え
    /// </summary>
    private void UpdateCharacterName()
    {
        currentCharacterName.text = characterNames[currentCharacterIndex];
        nextCharacterName.text = characterNames[(currentCharacterIndex + 1) % characterNames.Count];
        previousCharacterName.text = characterNames[(currentCharacterIndex - 1 + characterNames.Count) % characterNames.Count];

        //ClearSkillDetailInfo();
    }

    private async UniTask ChangeCharacter()
    {
        SetCharacterInfo();
        InitializeClassList();

        Color color = CommonController.GetCharacterColorByIndex(currentCharacterIndex);

        var index = GetCharacterClassIndex();

        //// 一番上のボタンを選択
        SelectButton(classList, index);
        setButtonFillAmount(classList, index);

        //Init();

        float duration = 0.2f;
        await UniTask.WhenAll(
            characterImage.gameObject.GetComponent<SpriteManipulator>().FadeOut(duration),
            characterImage.gameObject.GetComponent<SpriteManipulator>().FadeIn(duration),
            gradation.gameObject.GetComponent<SpriteManipulator>().AnimateColor(color, duration * 2),
            nameBackGradation.gameObject.GetComponent<SpriteManipulator>().AnimateColor(color, duration * 2)
            );
    }

    /// <summary>
    /// ボタンを選択状態にする
    /// </summary>
    /// <param name="number"></param>
    public void SelectButton(GameObject obj, int number = 0)
    {
        if (eventSystem != null && obj.transform.childCount > 0)
        {
            var buttonToSelect = obj.transform.GetChild(number).gameObject;
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

    public async UniTask FadeOutChildren(GameObject gameObject, float duration = 0.3f)
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
