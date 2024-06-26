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
    private List<string> characterNames = new List<string>() { "�A���b�N�X", "�j�R", "�^�o�T", "�A���V�A" };
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

    #region �X�e�[�^�X��
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
    /// �X�L����ʏ�����
    /// </summary>
    public async UniTask Initialize()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        playerInput = FindObjectOfType<PlayerInput>();

        SetCharacterInfo();     // �L�����N�^�[���ݒ�
        UpdateCharacterName();  // ����L�����N�^�[��������

        SetInputActions();      // InputAction�ݒ�

        await FadeIn();          // ��ʃt�F�[�h�C�� 
    }

    public async UniTask Init()
    {
        //selectedSkill = null;
        //await SetSkillList(); // �X�L���I�𗓐ݒ�

        //setButtonFillAmount(content, lastSelectButtonIndex);
        //SelectButton(content, lastSelectButtonIndex); // �������ʑI�𗓂�I��
    }

    /// <summary>
    /// �L�����N�^�[����ݒ肷��
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

        rightArm.text = ch.RightArm != null ? ch.RightArm.ItemName : "�Ȃ�";
        leftArm.text = ch.LeftArm != null ? ch.LeftArm.ItemName : "�Ȃ�";
        head.text = ch.Head != null ? ch.Head.ItemName : "�Ȃ�";
        body.text = ch.Body != null ? ch.Body.ItemName : "�Ȃ�";
        accessaryOne.text = ch.Accessary1 != null ? ch.Accessary1.ItemName : "�Ȃ�";
        accessaryTwo.text = ch.Accessary2 != null ? ch.Accessary2.ItemName : "�Ȃ�";

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
    /// �L�����N�^�[�؂�ւ�
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

            //if (subWindowInstance != null)
            //{
            //    Destroy(subWindowInstance);
            //    await Init();
            //}
            //else
            //{
            // �A�C�e�����j���[�̃t�F�[�h�A�E�g
            await FadeOut(gameObject, 0.3f);
                if (mainMenuController != null)
                {
                    // ���C�����j���[�̏�����
                    await mainMenuController.InitializeFromChildren("Status");
                }
                // �A�C�e�����j���[�C���X�^���X�̔j��
                gameObject.SetActive(false);
            //}
        }
    }

    /// <summary>
    /// �ėp�{�^�������� - �������X�L�����͂���
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
    /// R�{�^������������ - �X�L���J�e�S���؂�ւ� - ���y�[�W
    /// </summary>
    public async void OnPressRSButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
        }
    }

    /// <summary>
    /// L�{�^������������ - �X�L���J�e�S���؂�ւ� - �O�y�[�W
    /// </summary>
    public async void OnPressLSButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
        }
    }

    /// <summary>
    /// RT�{�^������������ - �L�����N�^�[�؂�ւ� - ���y�[�W
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
    /// LT�{�^������������ - �L�����N�^�[�؂�ւ� - �O�y�[�W
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
