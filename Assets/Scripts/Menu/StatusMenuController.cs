using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class StatusMenuController : MonoBehaviour
{
    private int showingCharacterID = 1;
    private AllyStatus allyStatus;

    private Dictionary<string, string> nameVariableDict = new Dictionary<string, string>();

    public GameObject inputActionParent;

    public EventSystem eventSystem;

    public GameObject mainScreen;
    public GameObject statusScreen;

    public GameObject gradation;
    public GameObject characterName;
    public GameObject characterImage;
    public GameObject classAbbreviation;
    public GameObject level;
    public GameObject exp;
    public GameObject nextExp;
    public GameObject totalExp;
    public GameObject className;
    public GameObject hp;
    public GameObject maxHp;
    public GameObject mp;
    public GameObject maxMp;
    public GameObject tp;
    public GameObject maxTp;
    public GameObject pAtk;
    public GameObject mAtk;
    public GameObject pDef;
    public GameObject mDef;
    public GameObject str;
    public GameObject vit;
    public GameObject dex;
    public GameObject agi;
    public GameObject inte;
    public GameObject mnd;
    public GameObject pCrt;
    public GameObject mCrt;
    public GameObject pAvo;
    public GameObject mAvo;

    public List<GameObject> equipIcons;
    public List<GameObject> equipNames;
    public List<GameObject> resists;
    public List<GameObject> weaponLevels;
    public List<GameObject> traits;
    public List<GameObject> classLevels;

    // Start is called before the first frame update
    void Start()
    {
        // シーン内のすべてのTextMeshProを取得
        //allTMP = status.GetComponentsInChildren<TextMeshProUGUI>();

        InitializeStatusMenu(showingCharacterID);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        // ステータスメュー用のアクションマップを有効化
        CommonController.EnableInputActionMap(inputActionParent, "StatusMenu");

        InitializeStatusMenu(showingCharacterID);
    }

    /// <summary>
    /// キャンセルボタン押下時処理
    /// </summary>
    public void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Status)
            {

                statusScreen.SetActive(false);

                CommonVariableManager.ShowingMenuState = Constants.MenuState.Main;
                mainScreen.SetActive(true);
            }
        }
    }

    public void OnPressRightTriggerButton(InputAction.CallbackContext context)
    {
        if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Status)
        {
            if (context.performed)
            {
                showingCharacterID = showingCharacterID + 1;
                if (showingCharacterID > 4)
                {
                    showingCharacterID = 1;
                }
                InitializeStatusMenu(showingCharacterID);
            }
        }
    }

    public void OnPressLeftTriggerButton(InputAction.CallbackContext context)
    {
        if (CommonVariableManager.ShowingMenuState == Constants.MenuState.Status)
        {
            if (context.performed)
            {
                showingCharacterID = showingCharacterID - 1;
                if (showingCharacterID < 1)
                {
                    showingCharacterID = 4;
                }
                InitializeStatusMenu(showingCharacterID);
            }
        }
    }

    public async void InitializeStatusMenu(int id = 1)
    {
        string color = Constants.gradationBlue;

        switch (showingCharacterID)
        {
            case 1:
                color = Constants.gradationBlue;
                break;
            case 2:
                color = Constants.gradationRed;
                break;
            case 3:
                color = Constants.gradationPurple;
                break;
            case 4:
                color = Constants.gradationGreen;
                break;
            default:
                color = Constants.gradationBlue;
                break;
        }

        gradation.GetComponent<Image>().color = CommonController.GetColor(color);
        allyStatus = await CommonController.GetAllyStatus(showingCharacterID);

        characterImage.GetComponent<Image>().sprite = allyStatus.Class.imagesA[showingCharacterID - 1];

        characterName.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.characterName;
        level.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.level.ToString();
        classAbbreviation.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.Class.classAbbreviation;

        nextExp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.nextExperience.ToString();
        exp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.earnedExperience.ToString();
        totalExp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.totalExperience.ToString();

        className.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.Class.className;
        hp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.hp.ToString();
        maxHp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.maxHp2.ToString();
        mp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.mp.ToString();
        maxMp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.maxMp2.ToString();
        tp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.tp.ToString();
        maxTp.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.maxTp.ToString();

        pAtk.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.pAttack.ToString();
        mAtk.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.mAttack.ToString();
        pDef.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.pDefence.ToString();
        mDef.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.mDefence.ToString();

        str.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.str2.ToString();
        vit.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.vit2.ToString();
        dex.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.dex2.ToString();
        agi.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.agi2.ToString();
        inte.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.inte2.ToString();
        mnd.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.mnd2.ToString();

        pCrt.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.pCrit.ToString();
        mCrt.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.mCrit.ToString();
        pAvo.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.pAvo.ToString();
        mAvo.GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.mAvo.ToString();

        List<Item> equips = new List<Item> { allyStatus.rightArm, allyStatus.leftArm, allyStatus.head, allyStatus.body, allyStatus.accessary1, allyStatus.accessary2 };

        for (int i = 0; i < 6; i++)
        {
            if (equips[i] != null)
            {
                equipNames[i].GetComponentInChildren<TextMeshProUGUI>().text = equips[i].itemName;
                equipIcons[i].GetComponent<Image>().sprite = equips[i].iconImage;
            }
            else
            {
                equipNames[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                equipIcons[i].GetComponent<Image>().sprite = null;
            }
        }

        int[] rList = { allyStatus.resistPhysical, allyStatus.resistMagic, allyStatus.resistSlash, allyStatus.resistThrast,
                          allyStatus.resistBlow, allyStatus.resistFire, allyStatus.resistIce,  allyStatus.resistThunder,
                          allyStatus.resistWind, allyStatus.resistPoison,  allyStatus.resistParalyze, allyStatus.resistAsleep,
                          allyStatus.resistSilence, allyStatus.resistDaze,  allyStatus.resistCharm, allyStatus.resistFrostBite};

        for (int i = 0; i < 16; i++)
        {
            resists[i].GetComponentInChildren<TextMeshProUGUI>().text = rList[i].ToString();
        }

        int[] wLevels = { allyStatus.swordLevel, allyStatus.bladeLevel, allyStatus.daggerLevel, allyStatus.spearLevel,
                          allyStatus.axLevel, allyStatus.hammerLevel, allyStatus.fistLevel,  allyStatus.bowLevel,
                          allyStatus.staffLevel, allyStatus.shieldLevel};

        for (int i = 0; i < 10; i++)
        {
            weaponLevels[i].GetComponentInChildren<TextMeshProUGUI>().text = wLevels[i].ToString();
        }

        for (int i = 0; i < 8; i++)
        {
            classLevels[i].GetComponentInChildren<TextMeshProUGUI>().text = allyStatus.classLevels[i].ToString();
        }
    }

    private void SetTextByVariableName(string o)
    {
        //name
    }
}
