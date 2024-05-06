using Cysharp.Threading.Tasks;
using SpriteGlow;
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

/// <summary>
/// �퓬��ʃR���g���[���[
/// </summary>
public class BattleController : MonoBehaviour
{

    public EventSystem eventSystem;
    public BattleCommandManager battleCommandManager;

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

    public List<GameObject> timelineFaces;

    private Dictionary<int, int> speeds = new Dictionary<int, int>();
    public GameObject MainCamera;

    public GameObject animatedText;

    private CancellationTokenSource cancellationTokenSource;
    
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
        await GetAllyStatuses();

        // GetAllyStatuses�̏����������������SetEnemies�����s
        SetEnemies();

        OrderManager.Instance.Initialize();
        var order = OrderManager.Instance.GetActionOrder(CommonVariableManager.turns);


        SetCommnadHeaderState();
    }

    /// <summary>
    /// �����L�����N�^�[�̃X�e�[�^�X���擾����
    /// </summary>
    /// <returns></returns>
    private async Task GetAllyStatuses()
    {
        PartyMembers.Instance.Initialize();

        for (int i = 0; i < 4; i++) 
        {
            AllyStatus allyStatus = await CommonController.GetAllyStatus(i + 1);
            allyStatus.spriteObject = faceImages[i];                                    // ��ʉ��� �o�X�g�A�b�v�摜
            allyStatus.HPGauge = HPGauges[i];                                           // HP�Q�[�W
            allyStatus.MPGauge = MPGauges[i];                                           // MP�Q�[�W
            allyStatus.TPGauge = TPGauges[i];                                           // TP�Q�[�W

            GaugeManager hpGaugeManager = HPGauges[i].GetComponent<GaugeManager>();     // HP�Q�[�W�Ǘ��N���X
            GaugeManager mpGaugeManager = MPGauges[i].GetComponent<GaugeManager>();     // MP�Q�[�W�Ǘ��N���X
            GaugeManager tpGaugeManager = TPGauges[i].GetComponent<GaugeManager>();     // TP�Q�[�W�Ǘ��N���X

            hpGaugeManager.maxValueText.text = allyStatus.maxHp2.ToString();            // �ő�HP�e�L�X�g
            hpGaugeManager.currentValueText.text = allyStatus.hp.ToString();            // ����HP�e�L�X�g
            mpGaugeManager.maxValueText.text = allyStatus.maxMp2.ToString();            // �ő�MP�e�L�X�g
            mpGaugeManager.currentValueText.text = allyStatus.mp.ToString();            // ����MP�e�L�X�g
            tpGaugeManager.currentValueText.text = allyStatus.tp.ToString();            // ����TP�e�L�X�g

            hpGaugeManager.updateGaugeByText();
            mpGaugeManager.updateGaugeByText();
            tpGaugeManager.updateGaugeByText();

            speeds.Add(i, allyStatus.agi2);
            faceImages[i].GetComponent<Image>().sprite = allyStatus.Class.imagesC[i];

            // �p�[�e�B�����o�[�̃V���O���g���ɒǉ�
            PartyMembers.Instance.AddCharacterToParty(allyStatus);   
        }
    }

    /// <summary>
    /// �G�I�u�W�F�N�g�̔z�u
    /// </summary>
    private void SetEnemies()
    {
        //EnemyManager.Instance.Initialize(); PlayerController�ɂēG�Ƃ̐ڐG�����łɏ��������Ă���̂ł����ł͏��������Ȃ�

        Vector3 position;
        GameObject ins;
        List<EnemyPartyStatus.PartyMember> enemyPartyMembers = new List<EnemyPartyStatus.PartyMember>();
        var party = EnemyManager.Instance.EnemyPartyMembers;

        // ���X�g�̒��̓G�v���n�u���C���X�^���X��
        for (int i = 0; i < party.Count; i++)
        {
            var enemy = party[i].enemy;
            position = party[i].position;
            ins = Instantiate<GameObject>(enemy, position, Quaternion.Euler(0, -22, 0));
            ins.GetComponent<EnemyBehaviour>().indexInBattle = i;
            EnemyManager.Instance.AddEnemyIns(ins);

            // �����\�����A�X�v���C�g���o�O��̂�SpriteGlow�𖳌������Ă���
            var glowEffect = ins.GetComponent<SpriteGlowEffect>();
            if (glowEffect != null)
            {
                glowEffect.enabled = false;
            }
        }
    }

    /// <summary>
    /// �����^�[���J�n
    /// </summary>
    public void StartAllysTurn()
    {
        // �R�}���h�E�C���h�E�̃w�b�_�[�̏�Ԃ�ݒ�
        SetCommnadHeaderState();
        // �R�}���h�E�C���h�E�̑����L����
        battleCommandManager.ToggleButtonsInteractable(true);
        // �R�}���h - �U���{�^����I��
        battleCommandManager.SelectButton(0);
    }

    /// <summary>
    /// �G�^�[���J�n
    /// </summary>
    /// <param name="index">�^�[�����̓G�̃��X�g���ł̃C���f�b�N�X</param>
    /// <returns></returns>
    public async UniTask StartEnemiesTurn(int index)
    {
        // �R�}���h�E�C���h�E�̑���𖳌���
        battleCommandManager.ToggleButtonsInteractable(true);

        var enemy = EnemyManager.Instance.InstantiatedEnemies[index];
        var behaviour = enemy.GetComponent<EnemyBehaviour>();

        var target = HateManager.Instance.GetTargetWithRandom();

        await behaviour.PerformAction(target);

        ChangeNextTurn();
    }

    /// <summary>
    /// �^�[���ڍs�O����
    /// </summary>
    public async void ChangeNextTurn()
    {
        // �L�����N�^�[���������Ă��邩�`�F�b�N
        CheckCharactersAreAlive();
        
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
            if (turnCharacter is AllyStatus)
            {
                var c = turnCharacter as AllyStatus;
                tmPro.text = c.characterName;�@// �L�����N�^�[����
                image.color = c.color;      �@ // ���O�w�i�̐F
            }
            else
            {
                tmPro.text = turnCharacter.characterName;
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
        foreach(var ally in allies)
        {
            if (ally.hp <= 0 || ally.knockedOut)
            {
                // �O�̂��ߐ퓬�s�\��Ԃɂ��Ă���
                ally.knockedOut = true;
            }
        }
        // �G
        var enemies = EnemyManager.Instance.InstantiatedEnemies;
        var count = enemies.Count;
        for(int i = 0; i < count; i++)
        {
            var comp = enemies[i].GetComponent<EnemyBehaviour>();
            var st = comp.status;
            if (st.hp <= 0 || st.knockedOut)
            {
                // �O�̂��ߐ퓬�s�\��Ԃɂ��Ă���
                st.knockedOut = true;
                if (!comp.isFadedOut)
                {
                    // �t�F�[�h�A�E�g
                    EnemyManager.Instance.FadeOutEnemyIns(enemies[i]);
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

        GameObject obj = EnemyManager.Instance.GetEnemyIns(index);
        if (obj != null)
        {
            SpriteGlowEffect glowEffect = obj.GetComponent<SpriteGlowEffect>();
            if (glowEffect != null)
            {
                glowEffect.enabled = true;
                if (glowEffect != null)
                {
                    await AnimateGlowBrightness(glowEffect, 1, 2, 0.8f, cancellationTokenSource.Token);
                }
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
    private async UniTask AnimateGlowBrightness(SpriteGlowEffect glowEffect, float minBrightness, float maxBrightness, float totalDuration, CancellationToken cancellationToken)
    {
        float startTime = Time.time;
        while (!cancellationToken.IsCancellationRequested)
        {
            float elapsed = Time.time - startTime;
            float cycleTime = elapsed % totalDuration; // totalDuration���ƂɃ��Z�b�g
            float value = Mathf.PingPong(cycleTime / totalDuration * 2 * (maxBrightness - minBrightness), maxBrightness - minBrightness) + minBrightness;
            glowEffect.GlowBrightness = value;

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken); // ���̃t���[���܂őҋ@
        }
        glowEffect.enabled = false;
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
            var glowEffect = obj.GetComponent<SpriteGlowEffect>();
            if (glowEffect != null)
            {
                glowEffect.enabled = false;
            }
            cancellationTokenSource?.Cancel();
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
}