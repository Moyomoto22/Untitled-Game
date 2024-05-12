using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キャラクター基底クラス
/// </summary>
[Serializable]
public abstract class CharacterStatus : ScriptableObject
{

    //　キャラクターの名前
    [SerializeField]
    public string characterName;

    #region メインステータス
    //　キャラクターのレベル
    [SerializeField]
    public int level;
    //　最大HP
    [SerializeField]
    public int maxHp;
    //　HP
    [SerializeField]
    public int hp;
    //　最大MP
    [SerializeField]
    public int maxMp;
    //　MP
    [SerializeField]
    public int mp;
    //　最大TP
    [SerializeField]
    public int maxTp;
    //　TP
    [SerializeField]
    public int tp;
    //　STR
    [SerializeField]
    public int str;
    //　VIT
    [SerializeField]
    public int vit;
    //　DEX
    [SerializeField]
    public int dex;
    //　AGI
    [SerializeField]
    public int agi;
    //　INT
    [SerializeField]
    public int inte;
    //　MND
    [SerializeField]
    public int mnd;

    // 補正後ステータス
    //　最大HP
    public int maxHp2;
    //　最大MP
    public int maxMp2;
    //　STR
    public int str2;
    //　VIT
    public int vit2;
    //　DEX
    public int dex2;
    //　AGI
    public int agi2;
    //　INT
    public int inte2;
    //　MND
    public int mnd2;
    #endregion

    #region サブステータス
    // 物理攻撃力
    [SerializeField]
    public int pAttack;
    // 魔法攻撃力
    [SerializeField]
    public int mAttack;
    // 物理防御力
    [SerializeField]
    public int pDefence;
    // 魔法防御力
    [SerializeField]
    public int mDefence;
    // 物理クリティカル率
    [SerializeField]
    public int pCrit;
    // 魔法クリティカル率
    [SerializeField]
    public int mCrit;
    // 物理回避率
    [SerializeField]
    public int pAvo;
    // 魔法回避率
    [SerializeField]
    public int mAvo;
    // 物理カウンター率
    [SerializeField]
    public int pCnt;
    // 魔法カウンター率
    [SerializeField]
    public int mCnt;
    // 物理カット率
    [SerializeField]
    public int pCut;
    // 魔法カット率
    [SerializeField]
    public int mCut;
    #endregion

    #region ステータス変化・残りターン
    // 防御 1ターンしか継続しないので残りターンは無し
    public bool isGuarded;
    // スタン 1ターンしか継続しないので残りターンは無し
    public bool isStaned;
    //　毒
    public bool isPoisoned;
    public int poisonRemainingTurn;
    //　猛毒
    public bool isToxiced;
    public int toxicedRemainingTurn;
    //　麻痺
    public bool isParalyzed;
    public int paralyzedRemainingTurn;
    //　睡眠
    public bool isSleeped;
    public int sleepedRemainingTurn;
    //　沈黙
    public bool isSilenced;
    public int silencedRemainingTurn;
    //　幻惑
    public bool isDazed;
    public int dazedRemainingTurn;
    //　魅了
    public bool isTempted;
    public int temptedRemainingTurn;
    //　凍傷
    public bool isFrosted;
    public int frostedRemainingTurn;
    // 戦闘不能
    public bool knockedOut;
    #endregion

    #region ステータスバフ・デバフ
    public float pAttackEffect = 1.0f;      // 物理攻撃力
    public int pAttackEffectRemainingTurn;
    public float mAttackEffect = 1.0f;      // 魔法攻撃力
    public int mAttackEffectRemainingTurn;
    public float pDefenceEffect = 1.0f;     // 物理防御力
    public int pDefenceEffectRemainingTurn;
    public float mDefenceEffect = 1.0f;     // 魔法防御力
    public int mDefenceEffectRemainingTurn;
    public float agiEffect = 1.0f;          // AGI
    public int agiEffectRemainingTurn;
    public float pCritEffect = 1.0f;        // 物理クリティカル
    public int pCritEffectRemainingTurn;
    public float mCritEffect = 1.0f;        // 魔法クリティカル
    public int mCritEffectRemainingTurn;
    public float AvoidEffect = 1.0f;        // 回避率
    public int AvoidEffectRemainingTurn;
    public float pCutEffect = 1.0f;         // 物理カット率
    public int pCutEffectRemainingTurn;
    public float mCutEffect = 1.0f;         // 魔法カット率
    public int mCutEffectRemainingTurn;
    #endregion

    #region 装備
    // 右手
    [SerializeField]
    public Weapon rightArm;
    // 左手
    [SerializeField]
    public Weapon leftArm;
    // 頭
    [SerializeField]
    public Head head;
    // 胴
    [SerializeField]
    public Body body;
    // 装飾品1
    [SerializeField]
    public Accessary accessary1;
    // 装飾品2
    [SerializeField]
    public Accessary accessary2;
    #endregion

    // 習得済みスキル
    public List<Skill> learnedSkills;

    #region 耐性
    // 物理
    [SerializeField]
    public int resistPhysical;
    // 魔法
    [SerializeField]
    public int resistMagic;
    // 斬撃
    [SerializeField]
    public int resistSlash;
    // 刺突
    [SerializeField]
    public int resistThrast;
    // 打撃
    [SerializeField]
    public int resistBlow;
    // 炎
    [SerializeField]
    public int resistFire;
    // 氷
    [SerializeField]
    public int resistIce;
    // 雷
    [SerializeField]
    public int resistThunder;
    // 風
    [SerializeField]
    public int resistWind;
    // 毒
    [SerializeField]
    public int resistPoison;
    // 麻痺
    [SerializeField]
    public int resistParalyze;
    // 睡眠
    [SerializeField]
    public int resistAsleep;
    // 沈黙
    [SerializeField]
    public int resistSilence;
    // 幻影
    [SerializeField]
    public int resistDaze;
    // 魅了
    [SerializeField]
    public int resistCharm;
    // 凍傷
    [SerializeField]
    public int resistFrostBite;
    #endregion

    public Vector2 positionInScreen;

    public GameObject spriteObject;

    public GameObject HPGauge;
    public GameObject MPGauge;
    public GameObject TPGauge;
    public GameObject SPGauge;
    public GameObject EXPGauge;


    public CharacterStatus()
    {
        maxHp2 = maxHp + (rightArm?.maxHp ?? 0) + (leftArm?.maxHp ?? 0) + (head?.maxHp ?? 0) + (body?.maxHp ?? 0) + (accessary1?.maxHp ?? 0) + (accessary2?.maxHp ?? 0);
        maxMp2 = maxMp + (rightArm?.maxMp ?? 0) + (leftArm?.maxMp ?? 0) + (head?.maxMp ?? 0) + (body?.maxMp ?? 0) + (accessary1?.maxMp ?? 0) + (accessary2?.maxMp ?? 0);
        str2 = str + (rightArm?.str ?? 0) + (leftArm?.str ?? 0) + (head?.str ?? 0) + (body?.str ?? 0) + (accessary1?.str ?? 0) + (accessary2?.str ?? 0);
        vit2 = vit + (rightArm?.vit ?? 0) + (leftArm?.vit ?? 0) + (head?.vit ?? 0) + (body?.vit ?? 0) + (accessary1?.vit ?? 0) + (accessary2?.vit ?? 0);
        dex2 = dex + (rightArm?.dex ?? 0) + (leftArm?.dex ?? 0) + (head?.dex ?? 0) + (body?.dex ?? 0) + (accessary1?.dex ?? 0) + (accessary2?.dex ?? 0);
        agi2 = agi + (rightArm?.agi ?? 0) + (leftArm?.agi ?? 0) + (head?.agi ?? 0) + (body?.agi ?? 0) + (accessary1?.agi ?? 0) + (accessary2?.agi ?? 0);
        inte2 = inte + (rightArm?.inte ?? 0) + (leftArm?.inte ?? 0) + (head?.inte ?? 0) + (body?.inte ?? 0) + (accessary1?.inte ?? 0) + (accessary2?.inte ?? 0);
        mnd2 = mnd + (rightArm?.mnd ?? 0) + (leftArm?.mnd ?? 0) + (head?.mnd ?? 0) + (body?.mnd ?? 0) + (accessary1?.mnd ?? 0) + (accessary2?.mnd ?? 0);

        // 物理攻撃力依存値 武器によってSTR or DEX or INT or MNDを攻撃力に加算
        int pAttackCorect = str;
        int pAttackCorectLeft = 0;
        if (rightArm != null)
        {
            switch (rightArm.dependentStatus)
            {
                case 1:
                    pAttackCorect = dex;
                    break;
                case 2:
                    pAttackCorect = inte;
                    break;
                case 3:
                    pAttackCorect = mnd;
                    break;
                default:
                    pAttackCorect = str;
                    break;
            }
        }

        if (leftArm != null)
        {
            if (leftArm.weaponCategory != Constants.WeaponCategory.Shield)
            {
                switch (leftArm.dependentStatus)
                {
                    case 1:
                        pAttackCorectLeft = dex;
                        break;
                    case 2:
                        pAttackCorectLeft = inte;
                        break;
                    case 3:
                        pAttackCorectLeft = mnd;
                        break;
                    default:
                        pAttackCorectLeft = str;
                        break;
                }
            }
        }

        // サブステータス
        int pAttackLeft = pAttackCorectLeft + (leftArm?.pAttack ?? 0);
        pAttack = pAttackCorect + (rightArm?.pAttack ?? 0) + pAttackLeft + (head?.pAttack ?? 0) + (body?.pAttack ?? 0) + (accessary1?.pAttack ?? 0) + (accessary2?.pAttack ?? 0);
        mAttack = inte + (rightArm?.mAttack ?? 0) + (leftArm?.mAttack ?? 0) + (head?.mAttack ?? 0) + (body?.mAttack ?? 0) + (accessary1?.mAttack ?? 0) + (accessary2?.mAttack ?? 0);
        pDefence = vit + (rightArm?.pDefence ?? 0) + (leftArm?.pDefence ?? 0) + (head?.pDefence ?? 0) + (body?.pDefence ?? 0) + (accessary1?.pDefence ?? 0) + (accessary2?.pDefence ?? 0);
        mDefence = mnd / 2 + (rightArm?.mDefence ?? 0) + (leftArm?.mDefence ?? 0) + (head?.mDefence ?? 0) + (body?.mDefence ?? 0) + (accessary1?.mDefence ?? 0) + (accessary2?.mDefence ?? 0);
    }

    /// <summary>
    /// 1ターン切りの効果を解除する
    /// </summary>
    /// <returns></returns>
    public void RefreshEffectsRemainOneTurn()
    {
        this.isStaned = false;
        this.isGuarded = false;
    }

    public bool CanUseSkill(Skill skill)
    {
        return skill.CanUse(this);
    }

    public async UniTask<bool> UseSkill(Skill skill, CharacterStatus objective)
    {
        bool result = true;
        if (skill != null && skill.usable && CanUseSkill(skill))
        {
            // スキルの効果を適用する
            skill.User = this;
            skill.Objective = objective;
            result = await skill.applyActiveEffect();
        }
        else
        {
            return false;
        }
        return result;
    }

    public async UniTask<bool> UseItem(Item item, CharacterStatus objective)
    {
        bool result = true;
        if (item != null && item.usable)
        {
            // スキルの効果を適用する
            item.User = this;
            item.Objective = objective;
            result = await item.applyEffect();
        }
        return result;
    }

    public async UniTask<int> ReduceHP(int amount)
    {
        hp -= amount;
        if (hp < 0)
        {
            hp = 0;
            knockedOut = true;
        }
        // HPがゲージと紐づいている場合アニメーション開始
        if (HPGauge != null && this is AllyStatus)
        {
            var manager = HPGauge.GetComponent<GaugeManager>();
            await manager.AnimateTextAndGauge(hp);
        }
        else if (HPGauge != null && this is EnemyStatus)
        {
            var manager = HPGauge.GetComponent<GaugeManager>();
            float targetFillAmount = (float)hp / maxHp2;
            await manager.AnimateGaugeRenderer(targetFillAmount);
        }
        return hp;
    }

    public async UniTask<int> ReduceMP(int amount)
    {
        mp -= amount;
        if (mp < 0)
        {
            mp = 0;
        }
        // MPがゲージと紐づいている場合アニメーション開始
        if (MPGauge != null)
        {
            var manager = MPGauge.GetComponent<GaugeManager>();
            await manager.AnimateTextAndGauge(mp);
        }
        return mp;
    }

    public async UniTask<int> HealHP(int amount)
    {
        hp += amount;
        if (hp > maxHp2)
        {
            hp = maxHp2;
        }
        // HPがゲージと紐づいている場合アニメーション開始
        if (HPGauge != null)
        {
            var manager = HPGauge.GetComponent<GaugeManager>();
            await manager.AnimateTextAndGauge(hp);
        }
        return hp;
    }

    public async UniTask<int> HealMP(int amount)
    {
        mp += amount;
        if (mp > maxMp2)
        {
            mp = maxMp2;
        }
        // MPがゲージと紐づいている場合アニメーション開始
        if (MPGauge != null)
        {
            var manager = MPGauge.GetComponent<GaugeManager>();
            await manager.AnimateTextAndGauge(mp);
        }
        return mp;
    }
}