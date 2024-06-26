using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    public Enemy Enemy = null;
    public Enemy status;

    public GameObject HPGauge;
    public GameObject skillNameWindow;

    public SpriteRenderer mainSprite;
    public SpriteRenderer shadowSprite;
    public SpriteManipulator manipulator;

    public List<GameObject> effectSprites;
    // �\������ő�X�e�[�^�X�ω���
    private const int MaxVisibleStatusEffects = 6;

    public EnemyBehaviour behaviour;

    // �퓬�������p�C���f�b�N�X
    public int indexInBattle;
    public bool isFadedOut;
    // ��ʏ�ł̍��W
    public Vector2 positionInScreen;

    // Start is called before the first frame update
    void Awake()
    {
        positionInScreen = GetPositionInScreen();
        manipulator = mainSprite.GetComponent<SpriteManipulator>();

        status = new Enemy
        {
            CharacterName = Enemy.CharacterName,
            IsPoisoned = Enemy.IsPoisoned,
            IsToxiced = Enemy.IsToxiced,
            IsParalyzed = Enemy.IsParalyzed,
            IsSleeped = Enemy.IsSleeped,
            IsSilenced = Enemy.IsSilenced,
            IsDazed = Enemy.IsDazed,
            IsTempted = Enemy.IsTempted,
            IsFrosted = Enemy.IsFrosted,
            KnockedOut = Enemy.KnockedOut,
            Level = Enemy.Level,
            MaxHp = Enemy.BaseMaxHp,
            HP = Enemy.HP,
            MaxMp = Enemy.BaseMaxMp,
            MP = Enemy.MP,
            TP = Enemy.TP,
            Str = Enemy.Str,
            Vit = Enemy.Vit,
            Dex = Enemy.Dex,
            Agi = Enemy.Agi,
            Int = Enemy.Int,
            Mnd = Enemy.Mnd,
            PAttack = Enemy.PAttack,
            MAttack = Enemy.MAttack,
            PDefence = Enemy.PDefence,
            MDefence = Enemy.MDefence,
            CritEffect = Enemy.CritEffect,
            EvationRate = Enemy.EvationRate,
            CounterRate = Enemy.CounterRate,
            RightArm = Enemy.RightArm,
            LeftArm = Enemy.LeftArm,
            Head = Enemy.Head,
            Body = Enemy.Body,
            Accessary1 = Enemy.Accessary1,
            Accessary2 = Enemy.Accessary2,
            ResistPhysical = Enemy.ResistPhysical,
            ResistMagic = Enemy.ResistMagic,
            ResistSlash = Enemy.ResistSlash,
            ResistThrast = Enemy.ResistThrast,
            ResistBlow = Enemy.ResistBlow,
            ResistFire = Enemy.ResistFire,
            ResistIce = Enemy.ResistIce,
            ResistThunder = Enemy.ResistThunder,
            ResistWind = Enemy.ResistWind,
            ResistPoison = Enemy.ResistPoison,
            ResistParalyze = Enemy.ResistParalyze,
            ResistAsleep = Enemy.ResistAsleep,
            ResistSilence = Enemy.ResistSilence,
            ResistDaze = Enemy.ResistDaze,
            ResistCharm = Enemy.ResistCharm,
            ResistFrostBite = Enemy.ResistFrostBite,
            HPGauge = HPGauge,
            Exp = Enemy.Exp,
            Gold = Enemy.Gold,
            DropItemOne = Enemy.DropItemOne,
            DropRateOne = Enemy.DropRateOne,
            DropItemTwo = Enemy.DropItemTwo,
            DropRateTwo = Enemy.DropRateTwo,
            StealItemOne = Enemy.StealItemOne,
            StealRateOne = Enemy.StealRateOne,
            StealItemTwo = Enemy.StealItemTwo,
            StealRateTwo = Enemy.StealRateTwo,
            EyesSprite = Enemy.EyesSprite,
            positionInScreen = this.positionInScreen,
            spriteObject = this.mainSprite.gameObject,
            Component = this,
            effectSpriteObjects = effectSprites
        };
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
                return new Vector2(canvasPosition.x, canvasPosition.y);// - spriteHeight);// / 2);
            }
        }
        return new Vector3(0.0f, 0.0f, 1);
    }

    private float GetSpriteHeight()
    {
        float height = 0.0f;

        var sprite = mainSprite.sprite;
        if (sprite != null)
        {
            height = sprite.textureRect.height;
        }
        return height;
    }

    /// <summary>
    /// �g�p����X�L���̖��̂�\������
    /// </summary>
    /// <returns></returns>
    public void ShowSkillName(string skillName)
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

    private Sprite GetSpriteForEffect(string effectName)
    {
        var sprite = Resources.Load<Sprite>("UI/Icon/StatusEffect/" + effectName);
        return sprite;
    }
}
