using Cysharp.Threading.Tasks;
using SpriteGlow;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class EnemyBehaviour : MonoBehaviour
{
    public EnemyStatus enemyStatus = null;
    public EnemyStatus status;

    public GameObject HPGauge;
    public GameObject skillNameWindow;

    public SpriteManipulator manipulator;
    public SpriteGlowEffect spriteGlow;

    // �퓬�������p�C���f�b�N�X
    public int indexInBattle;
    public bool isFadedOut;
    // ��ʏ�ł̍��W
    public Vector2 positionInScreen;

    // Start is called before the first frame update
    void Awake()
    {
        status = new EnemyStatus();

        status.characterName = enemyStatus.characterName;

        //�@��
        status.isPoisoned = enemyStatus.isPoisoned;
        //�@�ғ�

        status.isToxiced = enemyStatus.isToxiced;
        //�@���

        status.isParalyzed = enemyStatus.isParalyzed;
        //�@����

        status.isSleeped = enemyStatus.isSleeped;
        //�@����

        status.isSilenced = enemyStatus.isSilenced;
        //�@���f

        status.isDazed = enemyStatus.isDazed;
        //�@����

        status.isTempted = enemyStatus.isTempted;
        //�@����

        status.isFrosted = enemyStatus.isFrosted;
        // �퓬�s�\

        status.knockedOut = enemyStatus.knockedOut;
        //�@�L�����N�^�[�̃��x��

        status.level = enemyStatus.level;
        //�@�ő�HP

        status.maxHp = enemyStatus.maxHp2;
        //�@HP

        status.hp = enemyStatus.hp;
        //�@�ő�MP

        status.maxMp = enemyStatus.maxMp;
        //�@MP

        status.mp = enemyStatus.mp;
        //�@�ő�TP

        status.maxTp = enemyStatus.maxTp;
        //�@TP

        status.tp = enemyStatus.tp;
        //�@STR

        status.str = enemyStatus.str;
        //�@VIT

        status.vit = enemyStatus.vit;
        //�@DEX

        status.dex = enemyStatus.dex;
        //�@AGI

        status.agi = enemyStatus.agi;
        //�@INT

        status.inte = enemyStatus.inte2;
        //�@MND

        status.mnd = enemyStatus.mnd;

        //�@�ő�HP
        //
        status.maxHp2 = enemyStatus.maxHp2;
        //�@�ő�MP
        //
        status.maxMp2 = enemyStatus.maxMp2;
        //�@STR
        //
        status.str2 = enemyStatus.str2;
        //�@VIT
        //
        status.vit2 = enemyStatus.vit2;
        //�@DEX
        //
        status.dex2 = enemyStatus.dex2;
        //�@AGI
        //
        status.agi2 = enemyStatus.agi2;
        //�@INT
        //
        status.inte2 = enemyStatus.inte2;
        //�@MND
        //
        status.mnd2 = enemyStatus.mnd2;
        // �����U����

        status.pAttack = enemyStatus.pAttack;
        // ���@�U����

        status.mAttack = enemyStatus.mAttack;
        // �����h���

        status.pDefence = enemyStatus.pDefence;
        // ���@�h���

        status.mDefence = enemyStatus.mDefence;
        // �����N���e�B�J����

        status.pCrit = enemyStatus.pCrit;
        // ���@�N���e�B�J����

        status.mCrit = enemyStatus.mCrit;
        // �������

        status.pAvo = enemyStatus.pAvo;
        // ���@���

        status.mAvo = enemyStatus.mAvo;
        // �����J�E���^�[��

        status.pCnt = enemyStatus.pCnt;
        // ���@�J�E���^�[��

        status.mCnt = enemyStatus.mCnt;

        // �E��

        status.rightArm = enemyStatus.rightArm;
        // ����

        status.leftArm = enemyStatus.leftArm;
        // ��

        status.head = enemyStatus.head;
        // ��

        status.body = enemyStatus.body;
        // �����i1

        status.accessary1 = enemyStatus.accessary1;
        // �����i2

        status.accessary2 = enemyStatus.accessary2;

        status.resistPhysical = enemyStatus.resistPhysical;
        // ���@

        status.resistMagic = enemyStatus.resistMagic;
        // �a��

        status.resistSlash = enemyStatus.resistSlash;
        // �h��

        status.resistThrast = enemyStatus.resistThrast;
        // �Ō�

        status.resistBlow = enemyStatus.resistBlow;
        // ��

        status.resistFire = enemyStatus.resistFire;
        // �X

        status.resistIce = enemyStatus.resistIce;
        // ��

        status.resistThunder = enemyStatus.resistThunder;
        // ��

        status.resistWind = enemyStatus.resistWind;
        // ��

        status.resistPoison = enemyStatus.resistPoison;
        // ���

        status.resistParalyze = enemyStatus.resistParalyze;
        // ����

        status.resistAsleep = enemyStatus.resistAsleep;
        // ����

        status.resistSilence = enemyStatus.resistSilence;
        // ���e

        status.resistDaze = enemyStatus.resistDaze;
        // ����

        status.resistCharm = enemyStatus.resistCharm;
        // ����

        status.resistFrostBite = enemyStatus.resistFrostBite;

        status.positionInScreen = GetPositionInScreen();
        status.spriteObject = gameObject;

        status.HPGauge = HPGauge;

        manipulator = gameObject.GetComponent<SpriteManipulator>();
        spriteGlow = gameObject.GetComponent<SpriteGlowEffect>();
    }

    private Vector2 GetPositionInScreen()
    {
        if (Camera.main != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>(); // Canvas��������
            if (canvas.renderMode != RenderMode.WorldSpace) // World Space�ȊO��Canvas��z��
            {
                var spriteHeight = GetSpriteHeight();

                // �J������ʂ��ă��[���h���W����X�N���[�����W�֕ϊ�
                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, gameObject.transform.position);

                // �X�N���[�����W��Canvas���̃A���J�[���W�ɕϊ�
                Vector2 canvasPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPosition, canvas.worldCamera, out canvasPosition);

                // RectTransform�̈ʒu��ݒ�
                return new Vector2(canvasPosition.x, canvasPosition.y - spriteHeight / 2);
            }
        }
        return new Vector3(0.0f, 0.0f, 1);
    }

    private float GetSpriteHeight()
    {
        float height = 0.0f;

        var sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        if (sprite != null)
        {
            height = sprite.textureRect.height;
        }
        return height;
    }

    public abstract UniTask PerformAction(CharacterStatus objective);

    public async UniTask Action(CharacterStatus objective, Skill skill)
    {
        var skillName = skill.skillName;

        ShowSkillName(skillName);                    // �X�L�����̕\��
        await FlashWithSpriteGlow(0.2f, 3.0f);       // �G�X�v���C�g���t���b�V��
        await status.UseSkill(skill, objective);     // �X�L�����ʓK�p
        skillNameWindow.SetActive(false);            // �X�L�����̔�\��
    }

    public async UniTask Attack(CharacterStatus objective)
    {
        var skillID = Constants.attackSkillID;
        Skill attack = SkillManager.Instance.GetSkillByID(skillID);
        await Action(objective, attack);
    }

    public async UniTask Guard(CharacterStatus objective)
    {
        var skillID = Constants.guardSkillID;
        Skill guard = SkillManager.Instance.GetSkillByID(skillID);
        await Action(objective, guard);
    }

    /// <summary>
    /// �g�p����X�L���̖��̂�\������
    /// </summary>
    /// <returns></returns>
    private void ShowSkillName(string skillName)
    {
        if (skillNameWindow != null)
        {
            TextMeshPro tmpro = skillNameWindow.GetComponentInChildren<TextMeshPro>();
            if (tmpro != null)
            {
                tmpro.text = skillName;
            }
            skillNameWindow.SetActive(true);
        }
    }

    /// <summary>
    /// SpriteGlow�ŃX�v���C�g���t���b�V��������
    /// </summary>
    /// <param name="duration">�t���b�V���Ԋu</param>
    /// <param name="brightness">�P�x</param>
    /// <returns></returns>
    private async UniTask FlashWithSpriteGlow(float duration, float brightness)
    {
        if (spriteGlow != null)
        {
            spriteGlow.enabled = true;
            float originalBrightness = spriteGlow.GlowBrightness; // ���̋P�x��ۑ�
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                // �o�ߎ��ԂɊ�Â��ċP�x�𒲐�����
                float t = elapsed / duration; // 0����1�̒l
                spriteGlow.GlowBrightness = Mathf.Lerp(originalBrightness, brightness, t);
                await UniTask.Yield(PlayerLoopTiming.Update); // �t���[���̍X�V���ɑҋ@
                elapsed += Time.deltaTime;
            }

            spriteGlow.GlowBrightness = brightness;

            // ���̋P�x�ɖ߂�
            elapsed = 0.0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration; // 0����1�̒l
                spriteGlow.GlowBrightness = Mathf.Lerp(brightness, 1.0f, t);
                await UniTask.Yield(PlayerLoopTiming.Update); // �t���[���̍X�V���ɑҋ@
                elapsed += Time.deltaTime;
            }

            spriteGlow.GlowBrightness = 1.0f;
            spriteGlow.enabled = false;
        }
    }
}
