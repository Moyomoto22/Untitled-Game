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

public class BattleResultControler : MonoBehaviour
{
    public BattleController battleController;
    public GameObject canvas;

    private List<AllyStatus> allies;

    public TextMeshProUGUI earnedEXP;
    public TextMeshProUGUI gold;
    public List<GameObject> droppedItems;
    public GameObject itemsOne;
    public GameObject itemsTwo;
    public GameObject itemsThree;
    public GameObject itemsFour;
    public GameObject itemsFive;
    public GameObject itemsSix;

    public List<Image> faces;
    public List<TextMeshProUGUI> classes;
    public List<TextMeshProUGUI> levels;
    public List<GameObject> expGauges;
    public List<GameObject> levelUpTexts;
    public List<GameObject> learnedSkills;

    public int earnedExp;
    public int earnedGold;
    public List<Item> earnedItems;

    private int maxLevelUpTimes;

    private PlayerInput playerInput;

    private bool isComplete = false;

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
        playerInput = FindObjectOfType<PlayerInput>();
        SetInputActions();

        allies = PartyMembers.Instance.GetAllies();
        SetCharactersInfo();
        SetDroppedItems();

        await UniTask.WhenAll(
            FadeIn(gameObject, 0.5f) // ��ʃt�F�[�h�C��
          //SlideCanvas(1.0f)
            );

        await CountText(earnedEXP, earnedExp, 0.5f); // �l���o���l�J�E���g�A�b�v
        await UniTask.DelayFrame(5);                 // 5�t���[���ҋ@
        await CountText(gold, earnedGold, 0.5f);     // �l���S�[���h�J�E���g�A�b�v
        await UniTask.DelayFrame(20);                // 20�t���[���ҋ@

        int surplusExpOne = earnedExp;
        int surplusExpTwo = earnedExp;
        int surplusExpThree = earnedExp;
        int surplusExpFour = earnedExp;

        while (surplusExpOne >= 0 || surplusExpTwo >= 0 || surplusExpThree >= 0 || surplusExpFour >= 0)
        {
            SetCharactersInfo();

            // �e�L�����N�^�[�Ɍo���l�����Z���A�]��o���l���擾
            var surplusExpTasks = new UniTask<int>[] {
                allies[0].GetExp(surplusExpOne),
                allies[1].GetExp(surplusExpTwo),
                allies[2].GetExp(surplusExpThree),
                allies[3].GetExp(surplusExpFour)
                };

            // �񓯊�������ҋ@
            var results = await UniTask.WhenAll(surplusExpTasks);

            // ���ʂ�ϐ��ɑ��
            surplusExpOne = results[0];
            surplusExpTwo = results[1];
            surplusExpThree = results[2];
            surplusExpFour = results[3];

            await UniTask.DelayFrame(30);

            // ���x���A�b�v�ƏK���X�L���\���̃^�X�N���쐬
            var levelUpTasks = new List<UniTask>();

            if (surplusExpOne >= 0)
            {
                levelUpTasks.Add(LevelUpAndDisplaySkill(0));
            }
            if (surplusExpTwo >= 0)
            {
                levelUpTasks.Add(LevelUpAndDisplaySkill(1));
            }
            if (surplusExpThree >= 0)
            {
                levelUpTasks.Add(LevelUpAndDisplaySkill(2));
            }
            if (surplusExpFour >= 0)
            {
                levelUpTasks.Add(LevelUpAndDisplaySkill(3));
            }

            await UniTask.WhenAll(levelUpTasks);
            await UniTask.DelayFrame(20);
        }
        await UniTask.DelayFrame(30);
        isComplete = true;
    }

    /// <summary>
    /// �w�肳�ꂽ�L�����N�^�[�̃��x���A�b�v�ƃX�L���\�����s��
    /// </summary>
    private async UniTask LevelUpAndDisplaySkill(int index)
    {
        await LevelUp(index);
        await DisplayLearnedSkill(index);
    }

    private async UniTask LevelUp(int index)
    {
        var m = levelUpTexts[index].GetComponent<TextAnimationManager>();
        var ma = levels[index].GetComponent<TextAnimationManager>();

        levels[index].text = (int.Parse(levels[index].text) + 1).ToString();

        await UniTask.WhenAll(
            m.LevelUpAnimation(),
            ma.TextScaleFlash());
    }

    private async UniTask DisplayLearnedSkill(int index)
    {
        AllyStatus ch = PartyMembers.Instance.GetAllyByIndex(index);
        int levelIndex = ch.level - 1;

        if (ch.Class.LearnSkills.Count >= ch.level)
        {
            Skill learnedSkill = ch.Class.LearnSkills[levelIndex];

            var image = learnedSkills[index].GetComponentInChildren<Image>();
            var skillName = learnedSkills[index].GetComponentInChildren<TextMeshProUGUI>();

            if (learnedSkill != null && image != null && skillName != null)
            {
                image.sprite = learnedSkill.icon;
                skillName.text = learnedSkill.skillName;
            }

            await FadeIn(learnedSkills[index]);
        }
    }

    private void SetCharactersInfo()
    {

        for (int i = 0; i < allies.Count; i++)
        {
            AllyStatus ch = allies[i];

            faces[i].sprite = ch.Class.imagesC[i];
            classes[i].text = ch.Class.classAbbreviation;
            levels[i].text = ch.level.ToString();

            ch.EXPGauge = expGauges[i];

            var manager = ch.EXPGauge.GetComponent<GaugeManager>();
            if (manager != null)
            {
                manager.maxValueText.text = ch.GetCurrentClassNextExp().ToString();
                manager.currentValueText.text = ch.GetCurrentClassEarnedExp().ToString();
                manager.updateGaugeByText();
            }
        }
    }

    private void SetDroppedItems()
    {
        foreach (var droppedItem in droppedItems)
        {
            droppedItem.SetActive(false);
        }

        if (earnedItems.Count > 0)
        {
            for (int i = 0; i < earnedItems.Count; i++)
            {
                droppedItems[i].SetActive(true);
                droppedItems[i].GetComponentInChildren<Image>().sprite = earnedItems[i].iconImage;
                droppedItems[i].GetComponentInChildren<TextMeshProUGUI>().text = earnedItems[i].itemName;
            }
        }
    }

    /// <summary>
    /// �e�L�X�g���w��̒l�܂ŃJ�E���g�A�b�v�E�_�E��������
    /// </summary>
    /// <param name="targetValue">�J�E���g������ڕW�l</param>
    /// <param name="duration">�J�E���g�����鎞�ԊԊu</param>
    /// <returns></returns>
    public async UniTask CountText(TextMeshProUGUI tm, int targetValue, float duration = 1.0f)
    {
        if (!int.TryParse(tm.text, out var currentValue))
        {
            return;
        }
        await DOTween.To(
            () => currentValue,                        // �J�n�l
            x => tm.text = x.ToString(), // �e�L�X�g�X�V�̃A�N�V����
            targetValue,                            �@ // �ڕW�l
            duration                                �@ // �p������
        ).SetEase(Ease.OutQuad).SetUpdate(true);                       // �A�j���[�V�����̃C�[�W���O�ݒ�
    }

    public async UniTask SlideCanvas(float duration)
    {
        var rect = canvas.GetComponent<RectTransform>();

        await rect.DOAnchorPos(new Vector2(rect.anchoredPosition.x + 1000.0f, rect.anchoredPosition.y), duration).SetEase(Ease.InOutQuad).SetUpdate(true);
    }

    private void SetInputActions()
    {
        if (playerInput != null)
        {
            // InputActionAsset���擾
            var inputActionAsset = playerInput.actions;

            // "Main"�A�N�V�����}�b�v���擾
            var actionMap = inputActionAsset.FindActionMap("Main");
            // �A�N�V�������擾
            var submit = actionMap.FindAction("SubMit");

            // �C�x���g���X�i�[��ݒ�
            submit.performed += OnPressSubmitButton;

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
            var actionMap = inputActionAsset.FindActionMap("Main");
            // �A�N�V�������擾
            var submit = actionMap.FindAction("SubMit");

            submit.performed -= OnPressSubmitButton;
        }
    }

    private void OnPressSubmitButton(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isComplete)
            {
                EndScene();
            }
        }
    }

    public async UniTask FadeIn(GameObject gameObject, float duration = 0.3f)
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

    private async void EndScene()
    {
        if (SceneController.Instance != null)
        {
            //RemoveInputActions();
            EnemyManager.Instance.Initialize();

            await SceneController.Instance.SwitchFieldAndBattleScene("AbandonedFortress1F");
            CommonVariableManager.playerCanMove = true;
        }
    }
}
