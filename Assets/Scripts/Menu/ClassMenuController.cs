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

    private List<string> characterNames = new List<string>() { "�A���b�N�X", "�j�R", "�^�o�T", "�A���V�A" };
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
    /// �N���X��ʏ�����
    /// </summary>
    public async UniTask Initialize()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        playerInput = FindObjectOfType<PlayerInput>();
        var index = GetCharacterClassIndex();
        currentClassIndex = index;

        SetCharacterInfo();     // �L�����N�^�[���ݒ�
        UpdateCharacterName();  // ����L�����N�^�[��������
        InitializeClassList(); // �N���X�I�𗓏�����
        SetClassDetail(); // �N���X�ڍ׏�����
        CompareStatus();  // �X�e�[�^�X��r�\��

        SetInputActions();      // InputAction�ݒ�

        setButtonFillAmount(classList, index);
        SelectButton(classList, index); // �������ʑI�𗓂�I��

        await FadeIn();          // ��ʃt�F�[�h�C�� 
    }

    private int GetCharacterClassIndex()
    {
        var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
        var cl = ch.Class;

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
            // InputActionAsset���擾
            var inputActionAsset = playerInput.actions;

            // "Main"�A�N�V�����}�b�v���擾
            var actionMap = inputActionAsset.FindActionMap("Menu");
            // �A�N�V�������擾
            var rt = actionMap.FindAction("RightTrigger");
            var lt = actionMap.FindAction("LeftTrigger");
            var cancel = actionMap.FindAction("Cancel");

            // Always remove listeners first to prevent duplication
            cancel.performed -= OnPressCancelButton;
            rt.performed -= OnPressRTButton;
            lt.performed -= OnPressLTButton;

            // �C�x���g���X�i�[��ݒ�
            cancel.performed += OnPressCancelButton;
            rt.performed += OnPressRTButton;
            lt.performed += OnPressLTButton;

            // �A�N�V�����}�b�v��L���ɂ���
            actionMap.Enable();

            Debug.Log("SetInpuActions has done.");
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
        characterClass.text = ch.Class.classAbbreviation;
        characterLevel.text = ch.level.ToString();

        #region �X�e�[�^�X��
        MH.text = ch.maxHp2.ToString();
        MM.text = ch.maxMp2.ToString();
        ST.text = ch.str2.ToString();
        VI.text = ch.vit2.ToString();
        DE.text = ch.dex2.ToString();
        AG.text = ch.agi2.ToString();
        IT.text = ch.inte2.ToString();
        MN.text = ch.mnd2.ToString();
        #endregion
    }

    /// <summary>
    /// �����O��̃X�e�[�^�X���r���ăX�e�[�^�X���ɕ\������
    /// </summary>
    /// <param name=""></param>
    private void CompareStatus()
    {
        AllyStatus ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
        AllyStatus dummy = PartyMembers.Instance.CopyAllyStatus(ch);
        Class cl = ClassManager.Instance.GetClassByIndex(currentClassIndex);

        dummy.ChangeClass(cl);

        var pA = dummy.pAttack - ch.pAttack;
        var mA = dummy.mAttack - ch.mAttack;
        var pD = dummy.pDefence - ch.pDefence;
        var mD = dummy.mDefence - ch.mDefence;
        var mH = dummy.maxHp2 - ch.maxHp2;
        var mM = dummy.maxMp2 - ch.maxMp2;
        var sT = dummy.str2 - ch.str2;
        var vI = dummy.vit2 - ch.vit2;
        var dE = dummy.dex2 - ch.dex2;
        var aG = dummy.agi2 - ch.agi2;
        var iT = dummy.inte2 - ch.inte2;
        var mN = dummy.mnd2 - ch.mnd2;

        SetCompareTextStyle(MH, dummy.maxHp2, mh, mH);
        SetCompareTextStyle(MM, dummy.maxMp2, mm, mM);
        SetCompareTextStyle(ST, dummy.str2, st, sT);
        SetCompareTextStyle(VI, dummy.vit2, vi, vI);
        SetCompareTextStyle(DE, dummy.dex2, de, dE);
        SetCompareTextStyle(AG, dummy.agi2, ag, aG);
        SetCompareTextStyle(IT, dummy.inte2, it, iT);
        SetCompareTextStyle(MN, dummy.mnd2, mn, mN);
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

    private void SetClassDetail()
    {
        var ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
        var cl = ClassManager.Instance.GetClassByIndex(currentClassIndex);
        
        var lv = ch.classLevels[currentClassIndex];
        var next = ch.classNextExps[currentClassIndex];
        var earned = ch.classEarnedExps[currentClassIndex];

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

        exSkillIcon.sprite = cl.exSkill.icon;
        exSkillName.text = cl.exSkill.skillName;
        exSkillDescription.text = cl.exSkill.description;

        Skill nextSkill = null;
        Skill nextNextSkill = null;

        var nextLv = ch.classLevels[currentCharacterIndex] + 1;
        var nextNextLv = ch.classLevels[currentCharacterIndex] + 2;
        
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
            nextSkillIcon.sprite = nextSkill.icon;
            nextSkillName.text = nextSkill.skillName;
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
            nextNextSkillIcon.sprite = nextNextSkill.icon;
            nextNextSkillName.text = nextNextSkill.skillName;
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
    /// �N���X�{�^��������
    /// </summary>
    /// <param name="skill"></param>
    public async void OnPressClassButton(int index)
    {
        Class cl = ClassManager.Instance.GetClassByIndex(index);
        int currentCharacterClassIndex = GetCharacterClassIndex();
        if (index != currentCharacterClassIndex)
        {
            AllyStatus ch = PartyMembers.Instance.GetAllyByIndex(currentCharacterIndex);
            ch.ChangeClass(cl);

            await Initialize();
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// �L�����Z���{�^������������
    /// </summary>
    public async void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed && gameObject.activeSelf == true)
        {
            // �N���X���j���[�̃t�F�[�h�A�E�g
            await FadeOutChildren(gameObject, 0.3f);
            // �N���X���j���[�C���X�^���X�̔j��
            gameObject.SetActive(false);
            await mainMenuController.InitializeFromChildren("Class");
        }
    }

    /// <summary>
    /// R2(ZR�ERT)�{�^������ �L�����N�^�[�؂�ւ�
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
    /// L2(ZL�ELT)�{�^������ �L�����N�^�[�؂�ւ�
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
    /// �L�����N�^�[�؂�ւ�
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

        //// ��ԏ�̃{�^����I��
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
    /// �{�^����I����Ԃɂ���
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
