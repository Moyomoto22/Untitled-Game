using SpriteGlow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// �퓬��� - �X�L���I���T�u���j���[�R���g���[��
/// </summary>
public class SelectSkillSubMenuController : MonoBehaviour
{
    private List<string> skillCategoryNames = new List<string>() { "���@", "���", "�A�[�c", "�A�N�e�B�u" };
    private int currentCategoryIndex = 0;

    private EventSystem eventSystems;

    private GameObject targetSelectMenuInstance;

    private BattleCommandManager battleCommandManager;

    public TextMeshProUGUI currentCategoryText;
    public TextMeshProUGUI nextCategoryText;
    public TextMeshProUGUI previousCategoryText;

    public GameObject content;
    public GameObject button;

    public Image detailImage;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailCost;
    public TextMeshProUGUI detailPoint;
    public TextMeshProUGUI description;
    public GameObject attributes;
    public GameObject classes;

    public GameObject targetSelectMenu;

    private CharacterStatus turnCharacter;
    private List<Skill> skills;

    void Awake()
    {
        eventSystems = FindObjectOfType<EventSystem>();
        battleCommandManager = FindObjectOfType<BattleCommandManager>();

        //�I�𒆃J�e�S����������
        UpdateCategoryTexts();
        
        // �^�[���L�����N�^�[�̎g�p�\�X�L�����ꗗ�ɃZ�b�g
        turnCharacter = TurnCharacter.Instance.CurrentCharacter;
        skills = turnCharacter.learnedSkills;
        SetSkills();
    }

    /// <summary>
    /// �I�𒆃J�e�S���̃X�L�����ꗗ�ɃZ�b�g����
    /// </summary>
    public void SetSkills()
    {
        switch (currentCategoryIndex)
        {
            case 0:
                // ���@
                SetMagicsOrMiracles(true);
                break;
            case 1:
                // ���
                SetMagicsOrMiracles(false);
                break;
            case 2:
                // �A�[�c
                SetArts();
                break;
            case 3:
                // �A�N�e�B�u�X�L��
                SetActives();
                break;
        }
        // �ꗗ�̈�ԏ��I��
        SelectButton(0);
    }

    /// <summary>
    /// ���@/��Ղ��ꗗ�ɃZ�b�g
    /// </summary>
    /// <param name="isMagics">���@��</param>
    private void SetMagicsOrMiracles(bool isMagics)
    {
        List<Skill> ms = new List<Skill>();

        if (isMagics)
        {
            ms = skills.Where(x => x.skillCategory == Constants.SkillCategory.Magic).ToList();
        }
        else
        {
            ms = skills.Where(x => x.skillCategory == Constants.SkillCategory.Miracle).ToList();
        }

        foreach (MagicMiracle m in ms)
        {
            GameObject obj = Instantiate(button, content.transform, false); // �ꗗ�ɕ\������{�^���̃x�[�X���C���X�^���X����
            var comp = obj.GetComponent<SkillComponent>();                  // �{�^���ɕR�Â��X�L�������i�[����R���|�[�l���g
            var newButton = obj.transform.GetChild(0).gameObject;           // �{�^���{��

            comp.icon.sprite = m.icon;                                      // �A�C�R��
            comp.skillName.text = m.skillName;                              // �X�L������
            comp.cost.text = "MP";                                          // �R�X�g�̎��
            comp.point.text = m.MPCost.ToString();                          // ����MP
            AddSelectOrDeselectActionToButtons(newButton, m);               // �{�^���̑I���E�I���������̃A�N�V������ݒ�

            // �X�L�����g�p�\������
            if (turnCharacter.CanUseSkill(m))
            {
                // �{�^���������̃A�N�V������ǉ�
                AddOnClickActionToSkillButton(newButton, m);
            }
            else
            {
                // �{�^���^�C�g�����O���[�A�E�g
                comp.skillName.color = CommonController.GetColor(Constants.darkGray);
            }


        }
    }

    /// <summary>
    /// �A�[�c���ꗗ�ɃZ�b�g
    /// </summary>
    private void SetArts()
    {
        var actives = skills.Where(x => x.skillCategory == Constants.SkillCategory.Arts).ToList();

        foreach (Arts art in actives)
        {
            GameObject obj = Instantiate(button, content.transform, false);
            var comp = obj.GetComponent<SkillComponent>();
            var newButton = obj.transform.GetChild(0).gameObject;

            comp.icon.sprite = art.icon;
            comp.skillName.text = art.skillName;
            comp.cost.text = "TP";
            comp.point.text = art.TPCost.ToString();
            AddSelectOrDeselectActionToButtons(newButton, art);
            if (turnCharacter.CanUseSkill(art))
            {
                AddOnClickActionToSkillButton(newButton, art);
            }
            else
            {
                comp.skillName.color = Color.gray;
            }
        }
    }

    /// <summary>
    /// �A�N�e�B�u�X�L�����ꗗ�ɃZ�b�g
    /// </summary>
    private void SetActives()
    {
        var actives = skills.Where(x => x.skillCategory == Constants.SkillCategory.Active).ToList();

        foreach (ActiveSkill act in actives)
        {
            GameObject obj = Instantiate(button, content.transform, false);
            var comp = obj.GetComponent<SkillComponent>();
            var newButton = obj.transform.GetChild(0).gameObject;

            comp.icon.sprite = act.icon;
            comp.skillName.text = act.skillName;
            comp.cost.text = "���L���X�g";
            comp.point.text = act.recastTurn.ToString();
            AddOnClickActionToSkillButton(newButton, act);
            AddSelectOrDeselectActionToButtons(newButton, act);
        }
    }

    /// <summary>
    /// �{�^����I����Ԃɂ���
    /// </summary>
    /// <param name="number"></param>
    public void SelectButton(int number = 0)
    {
        if (eventSystems != null && content.transform.childCount > 0)
        {
            var buttonToSelect = content.transform.GetChild(0).GetChild(0).gameObject;
            Debug.Log(buttonToSelect.name);
            eventSystems.SetSelectedGameObject(buttonToSelect);
        }
    }

    /// <summary>
    /// �{�^���������̓����ݒ肷��
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="skill"></param>
    private void AddOnClickActionToSkillButton(GameObject obj, Skill skill)
    {
        var button = obj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnClickActionToSkillButton(skill));
        }
    }

    /// <summary>
    /// �{�^���������A�N�V�������{�^���ɐݒ�
    /// </summary>
    /// <param name="skill"></param>
    private void OnClickActionToSkillButton(Skill skill)
    {
        battleCommandManager.selectedSkill = skill;
        // �X�L���̑Ώۂ��G���P�̂̎�
        if (skill.target == 3 && !skill.isTargetAll)
        {  
            battleCommandManager.DisplayEnemyTargetSubMenu(1);
        }
        // �Ώۂ��������P�̂̎�
        else if (skill.target == 2 && !skill.isTargetAll)
        {
            battleCommandManager.DisplayAllyTargetSubMenu(1);
        }

    }

    /// <summary>
    /// �{�^���I���E�I���������̓����ݒ�
    /// </summary>
    /// <param name="button"></param>
    /// <param name="skill"></param>
    public void AddSelectOrDeselectActionToButtons(GameObject button, Skill skill)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.Select; // Select�C�x���g�����b�X��
        entry.callback.AddListener((data) =>
        {
            // �X�L���ڍׂ�\��
            SetSkillDetailInfo(skill);
        });
        // �G���g�����g���K�[���X�g�ɒǉ�
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// �ڍח��ɃX�L���̏ڍׂ�\������
    /// </summary>
    /// <param name="skill"></param>
    private void SetSkillDetailInfo(Skill skill)
    {
        if (skill != null)
        {
            detailImage.enabled = true;
            detailImage.sprite = skill.icon;
            detailName.text = skill.skillName;
            description.text = skill.description;
            if (skill is MagicMiracle)
            {
                var magic = skill as MagicMiracle;
                detailCost.text = "����MP";
                detailPoint.text = magic.MPCost.ToString();
            }
            else if (skill is Arts)
            {
                var arts = skill as Arts;
                detailCost.text = "����TP";
                detailPoint.text = arts.TPCost.ToString();
            }
            else if (skill is ActiveSkill)
            {
                var act = skill as ActiveSkill;
                detailCost.text = "���L���X�g";
                detailPoint.text = act.recastTurn.ToString();
            }
        }
    }

    /// <summary>
    /// �X�L���ڍׂ��N���A����
    /// </summary>
    private void ClearSkillDetailInfo()
    {
        detailImage.enabled = false;
        detailName.text = "";
        description.text = "";
        detailCost.text = "";
        detailPoint.text = "";
    }

    /// <summary>
    /// �ꗗ���̃{�^�������ׂĔj������
    /// </summary>
    /// <returns></returns>
    private async UniTask DestroyButtons()
    {
        // �A�C�e���ꗗ���̃I�u�W�F�N�g���폜
        int childCount = content.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = content.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        await UniTask.Delay(10);
    }

    /// <summary>
    /// �X�L���J�e�S���؂�ւ� - ���y�[�W
    /// </summary>
    public async void NextCategory()
    {
        currentCategoryIndex = (currentCategoryIndex + 1) % skillCategoryNames.Count;
        UpdateCategoryTexts();

        await DestroyButtons();
        SetSkills();
    }

    /// <summary>
    /// �X�L���J�e�S���؂�ւ� - �O�y�[�W
    /// </summary>
    public async void PreviousCategory()
    {
        currentCategoryIndex = (currentCategoryIndex - 1 + skillCategoryNames.Count) % skillCategoryNames.Count;
        UpdateCategoryTexts();

        await DestroyButtons();
        SetSkills();
    }

    /// <summary>
    /// �X�L���J�e�S���؂�ւ�
    /// </summary>
    private void UpdateCategoryTexts()
    {
        currentCategoryText.text = skillCategoryNames[currentCategoryIndex];
        nextCategoryText.text = skillCategoryNames[(currentCategoryIndex + 1) % skillCategoryNames.Count];
        previousCategoryText.text = skillCategoryNames[(currentCategoryIndex - 1 + skillCategoryNames.Count) % skillCategoryNames.Count];

        ClearSkillDetailInfo();
    }
}