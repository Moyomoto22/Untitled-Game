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

    // 戦闘時検索用インデックス
    public int indexInBattle;
    public bool isFadedOut;
    // 画面上での座標
    public Vector2 positionInScreen;

    // Start is called before the first frame update
    void Awake()
    {
        status = new EnemyStatus();

        status.characterName = enemyStatus.characterName;

        //　毒
        status.isPoisoned = enemyStatus.isPoisoned;
        //　猛毒

        status.isToxiced = enemyStatus.isToxiced;
        //　麻痺

        status.isParalyzed = enemyStatus.isParalyzed;
        //　睡眠

        status.isSleeped = enemyStatus.isSleeped;
        //　沈黙

        status.isSilenced = enemyStatus.isSilenced;
        //　幻惑

        status.isDazed = enemyStatus.isDazed;
        //　魅了

        status.isTempted = enemyStatus.isTempted;
        //　凍傷

        status.isFrosted = enemyStatus.isFrosted;
        // 戦闘不能

        status.knockedOut = enemyStatus.knockedOut;
        //　キャラクターのレベル

        status.level = enemyStatus.level;
        //　最大HP

        status.maxHp = enemyStatus.maxHp2;
        //　HP

        status.hp = enemyStatus.hp;
        //　最大MP

        status.maxMp = enemyStatus.maxMp;
        //　MP

        status.mp = enemyStatus.mp;
        //　最大TP

        status.maxTp = enemyStatus.maxTp;
        //　TP

        status.tp = enemyStatus.tp;
        //　STR

        status.str = enemyStatus.str;
        //　VIT

        status.vit = enemyStatus.vit;
        //　DEX

        status.dex = enemyStatus.dex;
        //　AGI

        status.agi = enemyStatus.agi;
        //　INT

        status.inte = enemyStatus.inte2;
        //　MND

        status.mnd = enemyStatus.mnd;

        //　最大HP
        //
        status.maxHp2 = enemyStatus.maxHp2;
        //　最大MP
        //
        status.maxMp2 = enemyStatus.maxMp2;
        //　STR
        //
        status.str2 = enemyStatus.str2;
        //　VIT
        //
        status.vit2 = enemyStatus.vit2;
        //　DEX
        //
        status.dex2 = enemyStatus.dex2;
        //　AGI
        //
        status.agi2 = enemyStatus.agi2;
        //　INT
        //
        status.inte2 = enemyStatus.inte2;
        //　MND
        //
        status.mnd2 = enemyStatus.mnd2;
        // 物理攻撃力

        status.pAttack = enemyStatus.pAttack;
        // 魔法攻撃力

        status.mAttack = enemyStatus.mAttack;
        // 物理防御力

        status.pDefence = enemyStatus.pDefence;
        // 魔法防御力

        status.mDefence = enemyStatus.mDefence;
        // 物理クリティカル率

        status.pCrit = enemyStatus.pCrit;
        // 魔法クリティカル率

        status.mCrit = enemyStatus.mCrit;
        // 物理回避率

        status.pAvo = enemyStatus.pAvo;
        // 魔法回避率

        status.mAvo = enemyStatus.mAvo;
        // 物理カウンター率

        status.pCnt = enemyStatus.pCnt;
        // 魔法カウンター率

        status.mCnt = enemyStatus.mCnt;

        // 右手

        status.rightArm = enemyStatus.rightArm;
        // 左手

        status.leftArm = enemyStatus.leftArm;
        // 頭

        status.head = enemyStatus.head;
        // 胴

        status.body = enemyStatus.body;
        // 装飾品1

        status.accessary1 = enemyStatus.accessary1;
        // 装飾品2

        status.accessary2 = enemyStatus.accessary2;

        status.resistPhysical = enemyStatus.resistPhysical;
        // 魔法

        status.resistMagic = enemyStatus.resistMagic;
        // 斬撃

        status.resistSlash = enemyStatus.resistSlash;
        // 刺突

        status.resistThrast = enemyStatus.resistThrast;
        // 打撃

        status.resistBlow = enemyStatus.resistBlow;
        // 炎

        status.resistFire = enemyStatus.resistFire;
        // 氷

        status.resistIce = enemyStatus.resistIce;
        // 雷

        status.resistThunder = enemyStatus.resistThunder;
        // 風

        status.resistWind = enemyStatus.resistWind;
        // 毒

        status.resistPoison = enemyStatus.resistPoison;
        // 麻痺

        status.resistParalyze = enemyStatus.resistParalyze;
        // 睡眠

        status.resistAsleep = enemyStatus.resistAsleep;
        // 沈黙

        status.resistSilence = enemyStatus.resistSilence;
        // 幻影

        status.resistDaze = enemyStatus.resistDaze;
        // 魅了

        status.resistCharm = enemyStatus.resistCharm;
        // 凍傷

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

        ShowSkillName(skillName);                    // スキル名称表示
        await FlashWithSpriteGlow(0.2f, 3.0f);       // 敵スプライトをフラッシュ
        await status.UseSkill(skill, objective);     // スキル効果適用
        skillNameWindow.SetActive(false);            // スキル名称非表示
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
    /// 使用するスキルの名称を表示する
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
    /// SpriteGlowでスプライトをフラッシュさせる
    /// </summary>
    /// <param name="duration">フラッシュ間隔</param>
    /// <param name="brightness">輝度</param>
    /// <returns></returns>
    private async UniTask FlashWithSpriteGlow(float duration, float brightness)
    {
        if (spriteGlow != null)
        {
            spriteGlow.enabled = true;
            float originalBrightness = spriteGlow.GlowBrightness; // 元の輝度を保存
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                // 経過時間に基づいて輝度を調整する
                float t = elapsed / duration; // 0から1の値
                spriteGlow.GlowBrightness = Mathf.Lerp(originalBrightness, brightness, t);
                await UniTask.Yield(PlayerLoopTiming.Update); // フレームの更新毎に待機
                elapsed += Time.deltaTime;
            }

            spriteGlow.GlowBrightness = brightness;

            // 元の輝度に戻す
            elapsed = 0.0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration; // 0から1の値
                spriteGlow.GlowBrightness = Mathf.Lerp(brightness, 1.0f, t);
                await UniTask.Yield(PlayerLoopTiming.Update); // フレームの更新毎に待機
                elapsed += Time.deltaTime;
            }

            spriteGlow.GlowBrightness = 1.0f;
            spriteGlow.enabled = false;
        }
    }
}
