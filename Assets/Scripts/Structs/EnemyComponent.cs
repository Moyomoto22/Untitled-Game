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
    // 表示する最大ステータス変化数
    private const int MaxVisibleStatusEffects = 6;

    public EnemyBehaviour behaviour;

    // 戦闘時検索用インデックス
    public int indexInBattle;
    public bool isFadedOut;
    // 画面上での座標
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
            Canvas canvas = FindObjectOfType<Canvas>(); // Canvasを見つける
            if (canvas.renderMode != RenderMode.WorldSpace) // World Space以外のCanvasを想定
            {
                var spriteHeight = GetSpriteHeight();

                // カメラを通してワールド座標からスクリーン座標へ変換
                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, gameObject.transform.position);

                // スクリーン座標をCanvas内のアンカー座標に変換
                Vector2 canvasPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPosition, canvas.worldCamera, out canvasPosition);

                // RectTransformの位置を設定
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
    /// 使用するスキルの名称を表示する
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
