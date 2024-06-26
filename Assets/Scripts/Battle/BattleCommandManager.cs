using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// �퓬��� - �R�}���h�Ǘ�
/// </summary>
public class BattleCommandManager : MonoBehaviour
{
    public BattleController battleController;
    public InputActionManager inputAction;
    public EventSystem eventSystems;
    
    public GameObject targetSelectMenu;
    public GameObject skillSelectMenu;
    public GameObject itemSelectMenu;
    
    public List<GameObject> buttons;
    public GameObject animatedText;

    public Skill selectedSkill;
    public Item selectedItem;

    private int lastSelectedButtonIndex = 0;

    private GameObject targetSelectMenuInstance;
    private GameObject skillSelectMenuInstance;
    private GameObject itemSelectMenuInstance;

    private List<GameObject> displayedWindows;

    private void Start()
    {
    }

    private void Awake()
    {
        displayedWindows = new List<GameObject>();
    }

    /// <summary>
    /// �U���{�^������
    /// </summary>
    public void OnPressAttackButton()
    {
        // ID:"0000"�̃X�L��(�U��)��I��
        var attackSkillID = Constants.attackSkillID;
        selectedSkill = SkillManager.Instance.GetSkillByID(attackSkillID);
        lastSelectedButtonIndex = 0;

        // �G�I���T�u���j���[��\��
        DisplayEnemyTargetSubMenu(0);

        setButtonFillAmount(0);
        ToggleButtonsInteractable(false);
    }

    /// <summary>
    /// �X�L���{�^������
    /// </summary>
    public void OnPressSkillButton()
    {
        lastSelectedButtonIndex = 1;

        skillSelectMenuInstance = Instantiate(skillSelectMenu, this.transform, false);
        displayedWindows.Add(skillSelectMenuInstance);

        setButtonFillAmount(1);
        ToggleButtonsInteractable(false);
    }

    /// <summary>
    /// �A�C�e���{�^������
    /// </summary>
    public void OnPressItemButton()
    {
        lastSelectedButtonIndex = 2;

        itemSelectMenuInstance = Instantiate(itemSelectMenu, this.transform, false);
        displayedWindows.Add(itemSelectMenuInstance);

        setButtonFillAmount(2);
        ToggleButtonsInteractable(false);
    }

    /// <summary>
    /// �h��{�^������
    /// </summary>
    public async void OnPressGuardButton()
    {
        // ID:"9999"�̃X�L��(�h��)��I��
        var skillID = Constants.guardSkillID;
        selectedSkill = SkillManager.Instance.GetSkillByID(skillID);
        lastSelectedButtonIndex = 3;

        ToggleButtonsInteractable(false);

        // �^�[���L�����N�^�[�̃C���f�b�N�X���擾
        var index = TurnCharacter.Instance.CurrentCharacterIndex;
        // ������ΏۂɃX�L�����g�p
        await SkillToSelf(index);

        ToggleButtonsInteractable(true);
    }

    /// <summary>
    /// ������{�^������
    /// </summary>
    public async void OnPressRunButton()
    {
        if (SceneController.Instance != null)
        {
            EnemyManager.Instance.Initialize();

            await SceneController.Instance.SwitchFieldAndBattleScene("AbandonedFortress1F");
            CommonVariableManager.playerCanMove = true;
        }
    }

    /// <summary>
    /// �G��Ώۂɂ���T�u���j���[��\������
    /// </summary>
    /// <param name="callerButtonID">�Ăяo�����{�^��ID 0:�U��, 1:�X�L��, 2:�A�C�e��</param>
    public void DisplayEnemyTargetSubMenu(int callerButtonID = 0)
    {
        // �T�u���j���[�̃C���X�^���X����
        targetSelectMenuInstance = Instantiate(targetSelectMenu, this.transform, false);
        displayedWindows.Add(targetSelectMenu);

        // �C���X�^���X�̃��[�J���X�P�[����ݒ�
        targetSelectMenuInstance.transform.localScale = new Vector3(1f, 1f, 1.0f);
        targetSelectMenuInstance.transform.localPosition = new Vector3(450f, 180.0f, 0.0f);

        // �퓬�s�\�łȂ��G���擾
        var enemies = EnemyManager.Instance.GetAllEnemiesStatus(true);
        var enemyNames = enemies.Select(enemies => enemies.CharacterName).ToList();


        if (EnemyManager.Instance.AliveEnemyIndexes.Count > 0)
        {
            var firstEnemyIndex = EnemyManager.Instance.AliveEnemyIndexes.First();

            // �ΏۑI���T�u���j���[���쐬
            var subMenuController = targetSelectMenuInstance.GetComponent<TargetSelectSubMenuController>();
            subMenuController.CreateSubMenuSingleRow(enemyNames.Count);
            subMenuController.SetButtonTitles(enemyNames);
            subMenuController.SelectButton();
            subMenuController.AddSelectOrDeselectActionToButtonsSelectEnemy(true);
            subMenuController.AddSelectOrDeselectActionToButtonsSelectEnemy(false);
            AddClickActionToButtonsForEnemy(subMenuController, callerButtonID);

            //SelectButton(callerButtonID);

            // �X�L���I������Ăяo���ꂽ�ꍇ
            if (callerButtonID == 1)
            {
                Destroy(skillSelectMenuInstance);
            }
            else if (callerButtonID == 2)
            {
                Destroy(skillSelectMenuInstance);
            }
            subMenuController.SetHeader(selectedSkill, selectedItem);
            battleController.StartFlashingGlowEffect(firstEnemyIndex);
        }
    }

    /// <summary>
    /// ���Ԃ�Ώۂɂ���T�u���j���[��\������
    /// </summary>
    public void DisplayAllyTargetSubMenu(int callerButtonID = 0)
    {
        // �T�u���j���[�̃C���X�^���X����
        targetSelectMenuInstance = Instantiate(targetSelectMenu, this.transform, false);
        displayedWindows.Add(targetSelectMenu);

        // �C���X�^���X�̃��[�J���X�P�[����ݒ�
        targetSelectMenuInstance.transform.localScale = new Vector3(1.6f, 1.6f, 1.0f);
        targetSelectMenuInstance.transform.localPosition = new Vector3(-50.0f, 140.0f, 0.0f);

        // ���Ԃ��擾
        var allies = PartyMembers.Instance.GetAllies();
        var allyNames = allies.Select(allies => allies.CharacterName).ToList();

        // �ΏۑI���T�u���j���[���쐬
        var subMenuController = targetSelectMenuInstance.GetComponent<TargetSelectSubMenuController>();
        subMenuController.CreateSubMenuSingleRow(allyNames.Count);
        subMenuController.SetButtonTitles(allyNames);
        subMenuController.SelectButton();
        subMenuController.SetHeader(selectedSkill, selectedItem);
        AddClickActionToButtonsForAlly(subMenuController, callerButtonID);

        Destroy(skillSelectMenuInstance);
        Destroy(itemSelectMenuInstance);
    }

    /// <summary>
    /// �L�����Z��(�Z)�{�^������
    /// </summary>
    /// <param name="context"></param>
    public void OnPressCancelButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SoundManager.Instance.PlayCancel();

            DestroyAllWindowInstances();
            ToggleButtonsInteractable(true);
            SelectButton(lastSelectedButtonIndex);
        }
    }

    /// <summary>
    /// ����(R1)�{�^������
    /// </summary>
    /// <param name="context"></param>
    public void OnPressNextButton(InputAction.CallbackContext context)
    {
        if (context.performed && skillSelectMenuInstance != null)
        {
            var controller = skillSelectMenuInstance.GetComponent<SelectSkillSubMenuController>();
            // �X�L���E�C���h�E�̃J�e�S���؂�ւ�
            controller.NextCategory();
        }
    }

    /// <summary>
    /// �O��(L1)�{�^������
    /// </summary>
    /// <param name="context"></param>
    public void OnPressPreviousButton(InputAction.CallbackContext context)
    {
        if (context.performed && skillSelectMenuInstance != null)
        {
            var controller = skillSelectMenuInstance.GetComponent<SelectSkillSubMenuController>();
            // �X�L���E�C���h�E�̃J�e�S���؂�ւ�
            controller.PreviousCategory();
        }
    }

    /// <summary>
    /// �{�^����Interactable��؂�ւ���
    /// </summary>
    /// <param name="interactable">�L��/����</param>
    public void ToggleButtonsInteractable(bool interactable)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i].transform.GetChild(0).GetComponent<Button>();
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
        int numberOfChildren = buttons.Count;

        // �ΏۃC���f�b�N�X�ɊY������{�^���̂�FillAmount��1�ɂ��A����ȊO��0�ɂ���
        for (int i = 0; i < numberOfChildren - 1; i++)
        {
            int fillAmount = i == number ? 1 : 0;
            Transform child = buttons[i].transform.GetChild(0);
            Image buttonImage = child.GetComponentInChildren<Image>();
            buttonImage.fillAmount = fillAmount;
        }
    }

    /// <summary>
    /// �R�}���h�{�^����I����Ԃɂ���
    /// </summary>
    /// <param name="number">�Ώۃ{�^���̃R�}���h���ł̃C���f�b�N�X</param>
    public void SelectButton(int number = 0)
    {
        if (eventSystems != null)
        {
            var button = buttons[number];

            // �X�N���v�g����I����Ԃɂ���ꍇ�A���ʉ��͖炳�Ȃ�
            var controller = button.GetComponent<MainMenuButtonManager>();
            if (controller != null)
            {
                controller.shouldPlaySound = false;
            }

            eventSystems.SetSelectedGameObject(button.transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// �ΏۑI���T�u���j���[�̊e�{�^���֑I�����A�N�V������ݒ肷��
    /// </summary>
    /// <param name="subMenuController"></param>
    /// <param name="callerButtonID"></param>
    private void AddClickActionToButtonsForEnemy(TargetSelectSubMenuController subMenuController, int callerButtonID)
    {
        var buttons = subMenuController.createdButtons;

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = EnemyManager.Instance.AliveEnemyIndexes[i];

            var buttonToSelect = buttons[i].transform.GetComponentInChildren<Button>();
            if (callerButtonID == 2)
            {
                buttonToSelect.onClick.AddListener(() => ItemToEnemy(index));  // �A�C�e���g�p
            }
            else
            {
                buttonToSelect.onClick.AddListener(() => SkillToEnemy(index)); // �X�L���g�p
            }
        }
    }

    private void AddClickActionToButtonsForAlly(TargetSelectSubMenuController subMenuController, int callerButtonID)
    {
        var buttons = subMenuController.createdButtons;

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;

            var buttonToSelect = buttons[i].transform.GetComponentInChildren<Button>();
            if (callerButtonID == 2)
            {
                
                buttonToSelect.onClick.AddListener(() => ItemToAlly(index));  // �A�C�e���g�p
            }
            else
            {
                buttonToSelect.onClick.AddListener(() => SkillToAlly(index)); // �X�L���g�p
            }
        }
    }

    /// <summary>
    /// �����փX�L�����g�p����
    /// </summary>
    /// <param name="index">�g�p�҂̃p�[�e�B���C���f�b�N�X</param>
    /// <returns></returns>
    public async UniTask SkillToAlly(int index)
    {
        Destroy(targetSelectMenuInstance);
        ToggleButtonsInteractable(true);
        SelectButton(1);

        TextAnimationManager textAnimationManager = new TextAnimationManager();

        // �Ώۂ̒��Ԃ̃C���X�^���X���擾
        var target = PartyMembers.Instance.GetAllyByIndex(index);
        GameObject faceImage = battleController.faceImages[index];

        if (target != null)
        {
            var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

            if (turnCharacter != null && selectedSkill != null)
            {
                // �X�L�����g�p
                await turnCharacter.UseSkill(selectedSkill, target);
            }
        }
        // �^�[���I��
        battleController.ChangeNextTurn();
    }

    /// <summary>
    /// �����փX�L�����g�p����
    /// </summary>
    /// <param name="index">�g�p�҂̃p�[�e�B���C���f�b�N�X</param>
    /// <returns></returns>
    public async UniTask SkillToSelf(int index)
    {
        Destroy(targetSelectMenuInstance);
        Destroy(skillSelectMenuInstance);
        ToggleButtonsInteractable(false);

        TextAnimationManager textAnimationManager = new TextAnimationManager();

        // �Ώۂ̒��Ԃ̃C���X�^���X���擾
        var target = PartyMembers.Instance.GetAllyByIndex(index);
        GameObject faceImage = battleController.faceImages[index];

        if (target != null)
        {
            var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

            if (turnCharacter != null && selectedSkill != null)
            {
                // �X�L�����g�p
                await turnCharacter.UseSkill(selectedSkill, target);
            }
        }
        ToggleButtonsInteractable(true);
        // �^�[���I��
        battleController.ChangeNextTurn();
    }

    /// <summary>
    /// �G�փX�L�����g�p����
    /// </summary>
    /// <param name="index">�g�p�҂̃p�[�e�B���C���f�b�N�X</param>
    /// <returns></returns>
    private async UniTask SkillToEnemy(int index)
    {
        Destroy(targetSelectMenuInstance);
        SelectButton(lastSelectedButtonIndex);

        // �Ώۂ̓G�̃C���X�^���X���擾
        var enemy = EnemyManager.Instance.GetEnemyIns(index);
        if (enemy != null)
        {
            var comp = enemy.GetComponent<EnemyComponent>();
            var e = comp.status;
            var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

            if (e != null && turnCharacter != null && selectedSkill != null)
            {
                // �X�L�����g�p
                await turnCharacter.UseSkill(selectedSkill, e);
            }
        }
        ToggleButtonsInteractable(true);
        // �^�[���I��
        battleController.ChangeNextTurn();
    }

    /// <summary>
    /// �G�S�̂փX�L�����g�p����
    /// </summary>
    /// <returns></returns>
    public async UniTask SkillToAllEnemies()
    {
        Destroy(targetSelectMenuInstance);
        Destroy(skillSelectMenuInstance);
        ToggleButtonsInteractable(false);

        // �������̓G�̃C���X�^���X���擾
        var aliveEnemies = EnemyManager.Instance.GetEnemiesInsExceptKnockedOut();
        var skillTasks = new List<UniTask>();

        foreach (var enemy in aliveEnemies)
        {
            if (enemy != null)
            {
                var comp = enemy.GetComponent<EnemyComponent>();
                var e = comp.status;
                var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

                if (e != null && turnCharacter != null && selectedSkill != null)
                {
                    Debug.Log($"Using skill on {e.CharacterName}");

                    // �X�L�����g�p����^�X�N�����X�g�ɒǉ�
                    skillTasks.Add(turnCharacter.UseSkill(selectedSkill, e));
                }
            }
        }
        // �S�ẴX�L���g�p�^�X�N����������܂őҋ@
        await UniTask.WhenAll(skillTasks);

        ToggleButtonsInteractable(true);
        // �^�[���I��
        battleController.ChangeNextTurn();
    }

    /// <summary>
    /// �����փA�C�e�����g�p����
    /// </summary>
    /// <param name="index">�g�p�҂̃p�[�e�B���C���f�b�N�X</param>
    /// <returns></returns>
    private async UniTask ItemToAlly(int index)
    {
        Destroy(targetSelectMenuInstance);
        ToggleButtonsInteractable(true);
        SelectButton(2);

        if (selectedItem != null)
        {
            TextAnimationManager textAnimationManager = new TextAnimationManager();

            // �Ώۂ̒��Ԃ̃C���X�^���X���擾
            var target = PartyMembers.Instance.GetAllyByIndex(index);
            GameObject faceImage = battleController.faceImages[index];

            if (target != null)
            {
                var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

                if (turnCharacter != null)
                {
                    // �X�L�����g�p
                    await turnCharacter.UseItem(selectedItem, target);
                }
            }
            battleController.ChangeNextTurn();
        }
    }

    /// <summary>
    /// �G�փA�C�e�����g�p����
    /// </summary>
    /// <param name="index">�g�p�҂̃p�[�e�B���C���f�b�N�X</param>
    /// <returns></returns>
    private async UniTask ItemToEnemy(int index)
    {
        Destroy(targetSelectMenuInstance);
        SelectButton(2);

        if (selectedItem != null)
        {
            // �Ώۂ̓G�̃C���X�^���X���擾
            var enemy = EnemyManager.Instance.GetEnemyIns(index);
            if (enemy != null)
            {
                var comp = enemy.GetComponent<EnemyComponent>();
                var e = comp.status;
                var turnCharacter = TurnCharacter.Instance.CurrentCharacter;

                if (e != null && turnCharacter != null)
                {
                    // �A�C�e�����g�p
                    await turnCharacter.UseItem(selectedItem, e);
                }
            }
            ToggleButtonsInteractable(true);
            battleController.ChangeNextTurn();
        }
    }

    /// <summary>
    /// �T�u���j���[�̃C���X�^���X�����ׂĔj������
    /// </summary>
    private void DestroyAllWindowInstances()
    {
        Destroy(targetSelectMenuInstance);
        Destroy(skillSelectMenuInstance);
        Destroy(itemSelectMenuInstance);
    }
}
