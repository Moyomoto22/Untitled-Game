using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using HighlightPlus;

/// <summary>
/// �퓬��ʃR���g���[���[
/// </summary>
public class BattleController : MonoBehaviour
{

    public EventSystem eventSystem;
    public BattleCommandManager battleCommandManager;

    public GameObject mainCanvas;
    public GameObject resultCanvas;
    public BattleResultControler resultController;

    public GameObject currentName;
    public GameObject nameBackground;

    public GameObject attackButton;
    public GameObject skillButton;
    public GameObject itemButton;
    public GameObject blockButton;
    public GameObject runButton;

    public List<GameObject> faceImages;
    public List<GameObject> HPGauges;
    public List<GameObject> MPGauges;
    public List<GameObject> TPGauges;
    public List<GameObject> hates;
    public List<GameObject> effectSpriteObjectsParents;

    private Dictionary<int, int> speeds = new Dictionary<int, int>();
    public GameObject MainCamera;

    public GameObject animatedText;

    private CancellationTokenSource cancellationTokenSource;

    private static readonly System.Random random = new System.Random();

    public EnemyPartyStatus dummyEnemyParty;

    async void Start()
    {
        // ����������
        await Initialize();
    }

    /// <summary>
    /// ����������
    /// </summary>
    /// <returns></returns>
    private async Task Initialize()
    {
        CommonVariableManager.turns = 0;
        // �����̃X�e�[�^�X
        await GetAllyes();

        // GetAllyes�̏����������������SetEnemies�����s
        SetEnemies();

        OrderManager.Instance.Initialize();
        var order = OrderManager.Instance.GetActionOrder(CommonVariableManager.turns);

        SetCommnadHeaderState();
    }

    /// <summary>
    /// �����L�����N�^�[�̃X�e�[�^�X���擾����
    /// </summary>
    /// <returns></returns>
    private async Task GetAllyes()
    {
        //await PartyMembers.Instance.Initialize();
        List<Ally> allies = PartyMembers.Instance.GetAllies();

        for (int i = 0; i < 4; i++)
        {
            Ally Ally = allies[i]; //await CommonController.GetAlly(i + 1);
            Ally.spriteObject = faceImages[i];                                    // ��ʉ��� �o�X�g�A�b�v�摜
            Ally.HPGauge = HPGauges[i];                                           // HP�Q�[�W
            Ally.MPGauge = MPGauges[i];                                           // MP�Q�[�W
            Ally.TPGauge = TPGauges[i];                                           // TP�Q�[�W

            GaugeManager hpGaugeManager = HPGauges[i].GetComponent<GaugeManager>();     // HP�Q�[�W�Ǘ��N���X
            GaugeManager mpGaugeManager = MPGauges[i].GetComponent<GaugeManager>();     // MP�Q�[�W�Ǘ��N���X
            GaugeManager tpGaugeManager = TPGauges[i].GetComponent<GaugeManager>();     // TP�Q�[�W�Ǘ��N���X

            hpGaugeManager.maxValueText.text = Ally.MaxHp.ToString();            // �ő�HP�e�L�X�g
            hpGaugeManager.currentValueText.text = Ally.HP.ToString();            // ����HP�e�L�X�g
            mpGaugeManager.maxValueText.text = Ally.MaxMp.ToString();            // �ő�MP�e�L�X�g
            mpGaugeManager.currentValueText.text = Ally.MP.ToString();            // ����MP�e�L�X�g
            tpGaugeManager.currentValueText.text = Ally.TP.ToString();            // ����TP�e�L�X�g

            hpGaugeManager.updateGaugeByText();
            mpGaugeManager.updateGaugeByText();
            tpGaugeManager.updateGaugeByText();

            speeds.Add(i, Ally.Agi);
            faceImages[i].GetComponent<SpriteRenderer>().sprite = Ally.CharacterClass.imagesC[i];
            Ally.positionInScreen = GetPositionInScreen(Ally.spriteObject.transform.parent.gameObject);

            
            if (effectSpriteObjectsParents[i] != null)
            {
                int childCount = effectSpriteObjectsParents[i].transform.childCount;
                Ally.effectSpriteObjects = new List<GameObject>(childCount);
                for (int j = 0; j < childCount; j++)
                {
                    var child = effectSpriteObjectsParents[i].transform.GetChild(j);
                    if (child != null)
                    {
                        Ally.effectSpriteObjects.Add(child.gameObject);
                    }
                }

                //foreach (Transform child in effectSpriteObjectsParents[i].transform)
                //{
                //    Ally.effectSpriteObjects.Add(child.gameObject);
                //}
            }
            // �p�[�e�B�����o�[�̃V���O���g���ɒǉ�
            //PartyMembers.Instance.AddCharacterToParty(Ally);   
        }
    }

    /// <summary>
    /// �G�I�u�W�F�N�g�̔z�u
    /// </summary>
    private void SetEnemies()
    {
        //EnemyManager.Instance.Initialize(); //PlayerController�ɂēG�Ƃ̐ڐG�����łɏ��������Ă���̂ł����ł͏��������Ȃ�
        //SetTestEnemies();

        Vector3 position;
        GameObject ins;
        var party = EnemyManager.Instance.EnemyPartyMembers;

        // ���X�g�̒��̓G�v���n�u���C���X�^���X��
        for (int i = 0; i < party.Count; i++)
        {
            var enemy = party[i].enemy;
            var component = enemy.GetComponent<EnemyComponent>();
            component.indexInBattle = i;

            position = party[i].position;
            ins = Instantiate<GameObject>(enemy, position, Quaternion.Euler(0, -0, 0));
            EnemyManager.Instance.AddEnemyIns(ins);

            // �X�L���u�I�E���A�C�v�������̃L�����N�^�[������ꍇ�A�G��HP�Q�[�W��\��
            if (PartyMembers.Instance.IsOwlEyeActivate())
            {
                EnemyManager.Instance.ShowHPGauges();
            }

            // �����\�����A�X�v���C�g���o�O��̂�SpriteGlow�𖳌������Ă���
            //var glowEffect = ins.GetComponent<SpriteGlowEffect>();
            //if (glowEffect != null)
            //{
            //    glowEffect.enabled = false;
            //}
        }
    }

    /// <summary>
    /// �����^�[���J�n
    /// </summary>
    public void StartAllysTurn()
    {
        // �R�}���h�E�C���h�E�̃w�b�_�[�̏�Ԃ�ݒ�
        SetCommnadHeaderState();

        // �L�����N�^�[�摜�X�v���C�g���n�C���C�g
        var turnCharacterIndex = TurnCharacter.Instance.CurrentCharacterIndex;
        for (int i = 0; i < 4; i++)
        {
            var manipulator = faceImages[i].GetComponent<SpriteManipulator>();          
            if (manipulator != null)
            {
                manipulator.StopGlowingEffect();
                if (i == turnCharacterIndex)
                {
                    manipulator.StartGlowingEffect(0.001f, 3.0f, 2.0f);
                }  
            }
        }

        var turnCharacter = TurnCharacter.Instance.CurrentCharacter;
        // �s���\�ȏꍇ
        if (turnCharacter.CanAct())
        {
            // �R�}���h�E�C���h�E�̑����L����
            battleCommandManager.ToggleButtonsInteractable(true);
            // �R�}���h - �U���{�^����I��
            battleCommandManager.SelectButton(0);
        }
    }

    /// <summary>
    /// �G�^�[���J�n
    /// </summary>
    /// <param name="index">�^�[�����̓G�̃��X�g���ł̃C���f�b�N�X</param>
    /// <returns></returns>
    public async UniTask StartEnemiesTurn(int index)
    {
        var targetIndex = HateManager.Instance.GetTargetIndex();
        var target = PartyMembers.Instance.GetAllyByIndex(targetIndex);

        // �R�}���h�E�C���h�E�̑���𖳌���
        battleCommandManager.ToggleButtonsInteractable(false);

        var enemy = EnemyManager.Instance.InstantiatedEnemies[index];
        var component = enemy.GetComponent<EnemyComponent>();

        if (component != null)
        {
            var behaviour = component.behaviour;
            if (behaviour != null)
            {
                var turnCharacter = TurnCharacter.Instance.CurrentCharacter;
                // �s���\�ȏꍇ
                if (turnCharacter.CanAct())
                {
                    await behaviour.PerformAction(target);
                }
                else
                {
                    await behaviour.Stunned();
                }
            }
            else
            {
                Debug.LogError(enemy.name + "has no EnemyBehaviour!");
            }
        }
        else
        {
            Debug.LogError(enemy.name + "has no EnemyComponent!");
        }
        await ChangeNextTurn();
    }

    /// <summary>
    /// �^�[���ڍs�O����
    /// </summary>
    public async UniTask ChangeNextTurn()
    {
        TurnCharacter.Instance.EndTurn();
        
        // �L�����N�^�[���������Ă��邩�`�F�b�N
        CheckCharactersAreAlive();

        var aliveEnemies = EnemyManager.Instance.GetEnemiesInsExceptKnockedOut();

        // �������Ă���G������Α��s
        if (aliveEnemies.Count > 0)
        {

            // �^�[���L�����N�^�[�̍s���񐔂��C���N�������g
            var index = TurnCharacter.Instance.CurrentCharacterIndex;
            OrderManager.Instance.IncrementActionsTaken(index);

            // �^�[�������C���N�������g
            CommonVariableManager.turns += 1;


            // �s�����擾�E�^�[���L�����N�^�[�؂�ւ�
            OrderManager.Instance.GetActionOrder(CommonVariableManager.turns);

            var turnCharacterIndex = TurnCharacter.Instance.CurrentCharacterIndex;

            if (turnCharacterIndex < 4)
            {
                StartAllysTurn();
            }
            else
            {
                var enemyIndex = turnCharacterIndex - 4;
                await StartEnemiesTurn(enemyIndex);
            }
        }
        else
        {
            await UniTask.DelayFrame(120);
            // �I������
            EndBattle();
        }
    }

    /// <summary>
    /// �R�}���h�E�C���h�E�̃w�b�_�[�̏�Ԃ�ݒ肷��
    /// </summary>
    public void SetCommnadHeaderState()
    {
        var turnCharacter = TurnCharacter.Instance.CurrentCharacter;
        var tmPro = currentName.GetComponent<TextMeshProUGUI>();
        var image = nameBackground.GetComponent<Image>();

        if (turnCharacter != null && tmPro != null && image != null)
        {
            if (turnCharacter is Ally)
            {
                var c = turnCharacter as Ally;
                var index = PartyMembers.Instance.GetIndex(c);

                tmPro.text = c.CharacterName;�@// �L�����N�^�[����
                image.color = CommonController.GetCharacterColorByIndex(index);      �@ // ���O�w�i�̐F
            }
            else
            {
                tmPro.text = turnCharacter.CharacterName;
                image.color = CommonController.GetColor(Constants.gray);
            }
        }
    }

    /// <summary>
    /// �L�����N�^�[���퓬�s�\���`�F�b�N����
    /// </summary>
    private void CheckCharactersAreAlive()
    {
        // ����
        var allies = PartyMembers.Instance.GetAllies();
        foreach (var ally in allies)
        {
            if (ally.HP <= 0 || ally.KnockedOut)
            {
                // �O�̂��ߐ퓬�s�\��Ԃɂ��Ă���
                ally.KnockedOut = true;
            }
        }
        // �G
        var enemies = EnemyManager.Instance.InstantiatedEnemies;
        var count = enemies.Count;
        for (int i = 0; i < count; i++)
        {
            var comp = enemies[i].GetComponent<EnemyComponent>();
            var st = comp.status;
            if (st.HP <= 0 || st.KnockedOut)
            {
                // �O�̂��ߐ퓬�s�\��Ԃɂ��Ă���
                st.KnockedOut = true;
                if (!comp.isFadedOut)
                {
                    // �t�F�[�h�A�E�g
                    _ = EnemyManager.Instance.FadeOutEnemyIns(enemies[i]);
                    EnemyManager.Instance.removeIndexFromAliveEnemyIndexes(i);
                }

            }
        }
    }

    /// <summary>
    /// �G�X�v���C�g�_�ŏ���
    /// </summary>
    /// <param name="index">�_�ł�����X�v���C�g�̓G�p�[�e�B���ł̃C���f�b�N�X</param>
    /// <returns></returns>
    public async UniTask StartFlashingGlowEffect(int index)
    {
        cancellationTokenSource?.Cancel();  // �O�̃^�X�N���L�����Z��
        cancellationTokenSource = new CancellationTokenSource();

        var color = nameBackground.GetComponent<Image>().color;

        GameObject obj = EnemyManager.Instance.GetEnemyIns(index);
        if (obj != null)
        {
            HighlightEffect effect = obj.GetComponentInChildren<HighlightEffect>();
            if (effect != null)
            {
                await AnimateGlowBrightness(effect, 0.001f, 0.5f, 0.8f, color, cancellationTokenSource.Token);
            }
        }
    }

    /// <summary>
    /// �G�X�v���C�g�_�ŊJ�n
    /// </summary>
    /// <param name="glowEffect">SpriteGlow�R���|�[�l���g</param>
    /// <param name="minBrightness">�ŏ����x</param>
    /// <param name="maxBrightness">�ő喾�x</param>
    /// <param name="totalDuration">�_�ŊԊu</param>
    /// <param name="cancellationToken">�L�����Z���g�[�N��</param>
    /// <returns></returns>
    private async UniTask AnimateGlowBrightness(HighlightEffect effect, float minBrightness, float maxBrightness, float totalDuration, Color color, CancellationToken cancellationToken)
    {
        float startTime = Time.time;
        effect.outlineColor = color;
        while (!cancellationToken.IsCancellationRequested)
        {
            float elapsed = Time.time - startTime;
            float cycleTime = elapsed % totalDuration; // totalDuration���ƂɃ��Z�b�g
            float value = Mathf.PingPong(cycleTime / totalDuration * 2 * (maxBrightness - minBrightness), maxBrightness - minBrightness) + minBrightness;
            effect.outline = value;

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken); // ���̃t���[���܂őҋ@
        }
        effect.outline = 0.001f;
    }

    /// <summary>
    /// �G�X�v���C�g�_�Œ�~
    /// </summary>
    /// <param name="index">�_�Œ�~������X�v���C�g�̓G�p�[�e�B���ł̃C���f�b�N�X</param>
    /// <returns></returns>
    public void StopFlashingGlowEffect(int index)
    {
        GameObject obj = EnemyManager.Instance.GetEnemyIns(index);
        if (obj != null)
        {
            HighlightEffect effect = obj.GetComponentInChildren<HighlightEffect>();
            if (effect != null)
            {
                effect.outline = 0.001f;
            }
            cancellationTokenSource?.Cancel();
        }
    }

    private async void EndBattle()
    {
        await FadeOut(mainCanvas);

        resultController.earnedExp = CalculateEarnedExp();
        resultController.earnedGold = CalculateEarnedGold();
        resultController.earnedItems = GetDropItems();

        resultCanvas.SetActive(true);
    }

    private int CalculateEarnedExp()
    {
        int exp = 0;
        var enemies = EnemyManager.Instance.GetAllEnemiesStatus();
        
        foreach(Enemy enemy in enemies)
        {
            exp += enemy.Exp;
        }

        return exp;
    }

    private int CalculateEarnedGold()
    {
        int gold = 0;
        var enemies = EnemyManager.Instance.GetAllEnemiesStatus();

        foreach (Enemy enemy in enemies)
        {
            gold += enemy.Gold;
        }

        return gold;
    }

    private List<Item> GetDropItems()
    {
        List<Item> earnedItems = new List<Item>();
        var enemies = EnemyManager.Instance.GetAllEnemiesStatus();

        foreach (Enemy enemy in enemies)
        {
            if (enemy.DropItemOne != null)
            {
                int result = random.Next(1);
                if (enemy.DropRateOne > result)
                {
                    earnedItems.Add(enemy.DropItemOne);
                    continue;
                }
            }
            if (enemy.DropItemTwo != null)
            {
                int result = random.Next(1);
                if (enemy.DropRateTwo > result)
                {
                    earnedItems.Add(enemy.DropItemTwo);
                }
            }
        }
        return earnedItems;
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

    /// <summary>
    /// �V�[���j��
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator UnloadOldScene(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
    }

    private Vector2 GetPositionInScreen(GameObject obj)
    {
        if (Camera.main != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>(); // Canvas��������
            //Canvas canvas = mainCanvas.GetComponentInParent<Canvas>(); // Canvas��������
            if (canvas.renderMode != RenderMode.WorldSpace) // World Space�ȊO��Canvas��z��
            {

                // �J������ʂ��ă��[���h���W����X�N���[�����W�֕ϊ�
                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, obj.transform.position);

                // �X�N���[�����W��Canvas���̃A���J�[���W�ɕϊ�
                Vector2 canvasPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPosition, canvas.worldCamera, out canvasPosition);

                // RectTransform�̈ʒu��ݒ�
                return new Vector2(canvasPosition.x, canvasPosition.y);// - spriteHeight);// / 2);
            }
        }
        return new Vector3(0.0f, 0.0f, 1);
    }

    private void SetTestEnemies()
    {
        // �G�Ǘ��V���O���g���ɓG�̃��X�g���i�[
        var party = dummyEnemyParty.GetPartyMembers();
        EnemyManager.Instance.Initialize();
        EnemyManager.Instance.SetEnemies(party);
    }
}