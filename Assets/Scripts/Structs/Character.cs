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
public abstract class Character : ScriptableObject
{
    private const int maxVisibleStatusEffectCount = 6;
    
    // キャラクターの名前
    [SerializeField] private string characterName;
    public string CharacterName
    {
        get { return characterName; }
        set { characterName = value; }
    }

    [SerializeField] private int hp;
    public int HP
    {
        get { return hp; }
        set
        {
            if (value < 0)
            {
                hp = 0;
            }
            else
            {
                hp = value;
            }
        }
    }

    [SerializeField] private int mp;
    public int MP
    {
        get { return mp; }
        set
        {
            if (value < 0)
            {
                mp = 0;
            }
            else
            {
                mp = value;
            }
        }
    }

    [SerializeField] private int tp;
    public int TP
    {
        get { return tp; }
        set
        {
            if (value < 0)
            {
                tp = 0;
            }
            else
            {
                tp = value;
            }
        }
    }

    #region 基礎ステータス
    [SerializeField] private int level;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    [SerializeField] private int baseMaxHp;
    public int BaseMaxHp
    {
        get { return baseMaxHp; }
        set { baseMaxHp = value; }
    }

    [SerializeField] private int baseMaxMp;
    public int BaseMaxMp
    {
        get { return baseMaxMp; }
        set { baseMaxMp = value; }
    }

    [SerializeField] private int baseStr;
    public int BaseStr
    {
        get { return baseStr; }
        set { baseStr = value; }
    }

    [SerializeField] private int baseVit;
    public int BaseVit
    {
        get { return baseVit; }
        set { baseVit = value; }
    }

    [SerializeField] private int baseDex;
    public int BaseDex
    {
        get { return baseDex; }
        set { baseDex = value; }
    }

    [SerializeField] private int baseAgi;
    public int BaseAgi
    {
        get { return baseAgi; }
        set { baseAgi = value; }
    }

    [SerializeField] private int baseInt;
    public int BaseInt
    {
        get { return baseInt; }
        set { baseInt = value; }
    }

    [SerializeField] private int baseMnd;
    public int BaseMnd
    {
        get { return baseMnd; }
        set { baseMnd = value; }
    }
    #endregion

    #region 補正後ステータス
    private int maxHp;
    public int MaxHp
    {
        get { return maxHp; }
        set { maxHp = value; }
    }

    private int maxMp;
    public int MaxMp
    {
        get { return maxMp; }
        set { maxMp = value; }
    }

    private int str;
    public int Str
    {
        get { return str; }
        set { str = value; }
    }

    private int vit;
    public int Vit
    {
        get { return vit; }
        set { vit = value; }
    }

    private int dex;
    public int Dex
    {
        get { return dex; }
        set { dex = value; }
    }

    private int agi;
    public int Agi
    {
        get { return agi; }
        set { agi = value; }
    }

    private int inte;
    public int Int
    {
        get { return inte; }
        set { inte = value; }
    }

    private int mnd;
    public int Mnd
    {
        get { return mnd; }
        set { mnd = value; }
    }
    #endregion

    #region サブステータス
    [SerializeField] private int pAttack;
    public int PAttack
    {
        get { return pAttack; }
        set { pAttack = value; }
    }

    [SerializeField] private int mAttack;
    public int MAttack
    {
        get { return mAttack; }
        set { mAttack = value; }
    }

    [SerializeField] private int pDefence;
    public int PDefence
    {
        get { return pDefence; }
        set { pDefence = value; }
    }

    [SerializeField] private int mDefence;
    public int MDefence
    {
        get { return mDefence; }
        set { mDefence = value; }
    }

    [SerializeField] private int criticalRate;
    public int CriticalRate
    {
        get { return criticalRate; }
        set { criticalRate = value; }
    }

    [SerializeField] private int evationRate;
    public int EvationRate
    {
        get { return evationRate; }
        set { evationRate = value; }
    }

    [SerializeField] private int counterRate;
    public int CounterRate
    {
        get { return counterRate; }
        set { counterRate = value; }
    }

    [SerializeField] private int blockRate;
    public int BlockRate
    {
        get { return counterRate; }
        set { counterRate = value; }
    }

    [SerializeField] private int blockReductionRate;
    public int BlockReductionRate
    {
        get { return blockReductionRate; }
        set { blockReductionRate = value; }
    }
    #endregion

    #region ステータス変化・残りターン
    private bool isGuarded;
    public bool IsGuarded
    {
        get { return isGuarded; }
        set { isGuarded = value;
            UpdateActivatingEffects(value, "Guard");
        }
    }

    private bool isStunned;
    public bool IsStunned
    {
        get { return isStunned; }
        set { isStunned = value;
            UpdateActivatingEffects(value, "Stun");
        }
    }

    private int stunRemainingTurn;
    public int StunRemainingTurn
    {
        get { return stunRemainingTurn; }
        set { stunRemainingTurn = value;
            if (stunRemainingTurn <= 0)
            {
                IsStunned = false;
            }
        }
    }

    private bool isPoisoned;
    public bool IsPoisoned
    {
        get { return isPoisoned; }
        set { isPoisoned = value;
            UpdateActivatingEffects(value, "Poison");
        }
    }

    private int poisonRemainingTurn;
    public int PoisonRemainingTurn
    {
        get { return poisonRemainingTurn; }
        set
        {
            poisonRemainingTurn = value;
            if (poisonRemainingTurn <= 0)
            {
                IsPoisoned = false;
            }
        }
    }

    private bool isToxiced;
    public bool IsToxiced
    {
        get { return isToxiced; }
        set { isToxiced = value;
            UpdateActivatingEffects(value, "Toxic");
        }
    }

    private int toxicedRemainingTurn;
    public int ToxicedRemainingTurn
    {
        get { return toxicedRemainingTurn; }
        set
        {
            toxicedRemainingTurn = value;
            if (value <= 0)
            {
                IsToxiced = false;
            }
        }
    }

    private bool isParalyzed;
    public bool IsParalyzed
    {
        get { return isParalyzed; }
        set { isParalyzed = value;
            UpdateActivatingEffects(value, "Paralyze");
        }
    }

    private int paralyzedRemainingTurn;
    public int ParalyzedRemainingTurn
    {
        get { return paralyzedRemainingTurn; }
        set { paralyzedRemainingTurn = value;
            if (value <= 0)
            {
                IsParalyzed = false;
            }
        }
    }

    private bool isSleeped;
    public bool IsSleeped
    {
        get { return isSleeped; }
        set { isSleeped = value;
            UpdateActivatingEffects(value, "Sleep");
        }
    }

    private int sleepedRemainingTurn;
    public int SleepedRemainingTurn
    {
        get { return sleepedRemainingTurn; }
        set { sleepedRemainingTurn = value;
            if (value <= 0)
            {
                IsSleeped = false;
            }
        }
    }

    private bool isSilenced;
    public bool IsSilenced
    {
        get { return isSilenced; }
        set { isSilenced = value;
            UpdateActivatingEffects(value, "Silence");
        }
    }

    private int silencedRemainingTurn;
    public int SilencedRemainingTurn
    {
        get { return silencedRemainingTurn; }
        set { silencedRemainingTurn = value; 
            if (value <= 0)
            {
                IsSilenced = false;
            }
        }
    }

    private bool isDazed;
    public bool IsDazed
    {
        get { return isDazed; }
        set { isDazed = value;
            UpdateActivatingEffects(value, "Daze");
        }
    }

    private int dazedRemainingTurn;
    public int DazedRemainingTurn
    {
        get { return dazedRemainingTurn; }
        set { dazedRemainingTurn = value;
            if (value <= 0)
            {
                IsDazed = false;
            }
        }
    }

    private bool isTempted;
    public bool IsTempted
    {
        get { return isTempted; }
        set { isTempted = value;
            UpdateActivatingEffects(value, "Temp");
        }
    }

    private int temptedRemainingTurn;
    public int TemptedRemainingTurn
    {
        get { return temptedRemainingTurn; }
        set { temptedRemainingTurn = value;
            if (value <= 0)
            {
                IsTempted = false;
            }
        }
    }

    private bool isFrosted;
    public bool IsFrosted
    {
        get { return isFrosted; }
        set { isFrosted = value;
            UpdateActivatingEffects(value, "Frost");
        }
    }

    private int frostedRemainingTurn;
    public int FrostedRemainingTurn
    {
        get { return frostedRemainingTurn; }
        set { frostedRemainingTurn = value;
            if (value <= 0)
            {
                IsFrosted = false;
            }
        }
    }

    private bool knockedOut;
    public bool KnockedOut
    {
        get { return knockedOut; }
        set { knockedOut = value;
        }
    }
    #endregion

    #region ステータスバフ・デバフ
    private float pAttackEffect = 1.0f;
    public float PAttackEffect
    {
        get { return pAttackEffect; }
        set { pAttackEffect = value;
            bool condition = value != 1.0f; 
            string effectName = value > 1.0f ? "PAttackUp" : "PAttackDown";
            UpdateActivatingEffects(condition, effectName);
        }
    }

    private int pAttackEffectRemainingTurn;
    public int PAttackEffectRemainingTurn
    {
        get { return pAttackEffectRemainingTurn; }
        set { pAttackEffectRemainingTurn = value;
            if (value <= 0)
            {
                PAttackEffect = 1.0f;
            }
        }
    }

    private float mAttackEffect = 1.0f;
    public float MAttackEffect
    {
        get { return mAttackEffect; }
        set
        {
            mAttackEffect = value;
            bool condition = value != 1.0f;
            string effectName = value > 1.0f ? "PAttackUp" : "PAttackDown";
            UpdateActivatingEffects(condition, effectName);
        }
    }

    private int mAttackEffectRemainingTurn;
    public int MAttackEffectRemainingTurn
    {
        get { return mAttackEffectRemainingTurn; }
        set { mAttackEffectRemainingTurn = value;
            if (value <= 0)
            {
                MAttackEffect = 1.0f;
            }
        }
    }

    private float pDefenceEffect = 1.0f;
    public float PDefenceEffect
    {
        get { return pDefenceEffect; }
        set
        {
            pDefenceEffect = value;
            bool condition = value != 1.0f;
            string effectName = value > 1.0f ? "PAttackUp" : "PAttackDown";
            UpdateActivatingEffects(condition, effectName);
        }
    }

    private int pDefenceEffectRemainingTurn;
    public int PDefenceEffectRemainingTurn
    {
        get { return pDefenceEffectRemainingTurn; }
        set { pDefenceEffectRemainingTurn = value;
            if (value <= 0)
            {
                PDefenceEffect = 1.0f;
            }
        }
    }

    private float mDefenceEffect = 1.0f;
    public float MDefenceEffect
    {
        get { return mDefenceEffect; }
        set
        {
            mDefenceEffect = value;
            bool condition = value != 1.0f;
            string effectName = value > 1.0f ? "PAttackUp" : "PAttackDown";
            UpdateActivatingEffects(condition, effectName);
        }
    }

    private int mDefenceEffectRemainingTurn;
    public int MDefenceEffectRemainingTurn
    {
        get { return mDefenceEffectRemainingTurn; }
        set { mDefenceEffectRemainingTurn = value;
            if (value <= 0)
            {
                MDefenceEffect = 1.0f;
            }
        }
    }

    private float agiEffect = 1.0f;
    public float AgiEffect
    {
        get { return agiEffect; }
        set
        {
            agiEffect = value;
            bool condition = value != 1.0f;
            string effectName = value > 1.0f ? "PAttackUp" : "PAttackDown";
            UpdateActivatingEffects(condition, effectName);
        }
    }

    private int agiEffectRemainingTurn;
    public int AgiEffectRemainingTurn
    {
        get { return agiEffectRemainingTurn; }
        set { agiEffectRemainingTurn = value;
            if (value <= 0)
            {
                AgiEffect = 1.0f;
            }
        }
    }

    private float critEffect = 1.0f;
    public float CritEffect
    {
        get { return critEffect; }
        set
        {
            critEffect = value;
            bool condition = value != 1.0f;
            string effectName = value > 1.0f ? "PAttackUp" : "PAttackDown";
            UpdateActivatingEffects(condition, effectName);
        }
    }

    private int critEffectRemainingTurn;
    public int CritEffectRemainingTurn
    {
        get { return critEffectRemainingTurn; }
        set { critEffectRemainingTurn = value;
            if (value <= 0)
            {
                CritEffect = 1.0f;
            }
        }
    }

    private float evationEffect = 1.0f;
    public float EvationEffect
    {
        get { return evationEffect; }
        set
        {
            evationEffect = value;
            bool condition = value != 1.0f;
            string effectName = value > 1.0f ? "EvationUp" : "EvationDown";
            UpdateActivatingEffects(condition, effectName);
        }
    }

    private int evationEffectRemainingTurn;
    public int EvationEffectRemainingTurn
    {
        get { return evationEffectRemainingTurn; }
        set { evationEffectRemainingTurn = value;
            if (value <= 0)
            {
                EvationEffect = 1.0f;
            }
        }
    }

    #endregion

    #region 装備 敵のステータスを設定する際に役立つつかもしれないので一応SerializeFIeld
    [SerializeField] private Weapon rightArm;
    public Weapon RightArm
    {
        get { return rightArm; }
        set { rightArm = value; }
    }

    [SerializeField] private Weapon leftArm;
    public Weapon LeftArm
    {
        get { return leftArm; }
        set { leftArm = value; }
    }

    [SerializeField] private Head head;
    public Head Head
    {
        get { return head; }
        set { head = value; }
    }

    [SerializeField] private Body body;
    public Body Body
    {
        get { return body; }
        set { body = value; }
    }

    [SerializeField] private Accessary accessary1;
    public Accessary Accessary1
    {
        get { return accessary1; }
        set { accessary1 = value; }
    }

    [SerializeField] private Accessary accessary2;
    public Accessary Accessary2
    {
        get { return accessary2; }
        set { accessary2 = value; }
    }
    #endregion

    // 習得済みスキル
    private List<Skill> learnedSkills;
    public List<Skill> LearnedSkills
    {
        get { return learnedSkills; }
        set { learnedSkills = value; }
    }

    #region 耐性
    [SerializeField] private double resistPhysical;
    public double ResistPhysical
    {
        get { return resistPhysical; }
        set { resistPhysical = value; }
    }

    [SerializeField] private double resistMagic;
    public double ResistMagic
    {
        get { return resistMagic; }
        set { resistMagic = value; }
    }

    [SerializeField] private double resistSlash;
    public double ResistSlash
    {
        get { return resistSlash; }
        set { resistSlash = value; }
    }

    [SerializeField] private double resistThrast;
    public double ResistThrast
    {
        get { return resistThrast; }
        set { resistThrast = value; }
    }

    [SerializeField] private double resistBlow;
    public double ResistBlow
    {
        get { return resistBlow; }
        set { resistBlow = value; }
    }

    [SerializeField] private double resistFire;
    public double ResistFire
    {
        get { return resistFire; }
        set { resistFire = value; }
    }

    [SerializeField] private double resistIce;
    public double ResistIce
    {
        get { return resistIce; }
        set { resistIce = value; }
    }

    [SerializeField] private double resistThunder;
    public double ResistThunder
    {
        get { return resistThunder; }
        set { resistThunder = value; }
    }

    [SerializeField] private double resistWind;
    public double ResistWind
    {
        get { return resistWind; }
        set { resistWind = value; }
    }

    [SerializeField] private double resistPoison;
    public double ResistPoison
    {
        get { return resistPoison; }
        set { resistPoison = value; }
    }

    [SerializeField] private double resistParalyze;
    public double ResistParalyze
    {
        get { return resistParalyze; }
        set { resistParalyze = value; }
    }

    [SerializeField] private double resistAsleep;
    public double ResistAsleep
    {
        get { return resistAsleep; }
        set { resistAsleep = value; }
    }

    [SerializeField] private double resistSilence;
    public double ResistSilence
    {
        get { return resistSilence; }
        set { resistSilence = value; }
    }

    [SerializeField] private double resistDaze;
    public double ResistDaze
    {
        get { return resistDaze; }
        set { resistDaze = value; }
    }

    [SerializeField] private double resistCharm;
    public double ResistCharm
    {
        get { return resistCharm; }
        set { resistCharm = value; }
    }

    [SerializeField] private double resistFrostBite;
    public double ResistFrostBite
    {
        get { return resistFrostBite; }
        set { resistFrostBite = value; }
    }

    [SerializeField] private double resistStan;
    public double ResistStan
    {
        get { return resistStan; }
        set { resistStan = value; }
    }
    #endregion

    #region その他特殊効果

    // 魔法消費MP軽減
    public bool reduceMagicMPCost;

    #endregion

    public Vector2 positionInScreen;

    public GameObject spriteObject;
    public GameObject HPGauge;
    public GameObject MPGauge;
    public GameObject TPGauge;
    public GameObject SPGauge;
    public GameObject EXPGauge;

    private List<string> activateStatusEffects;
    public List<string> ActivateStatusEffects
    {
        get { return activateStatusEffects; }
    }

    public List<GameObject> effectSpriteObjects;

    private void UpdateActivatingEffects(bool condition, string effectName)
    {
        if (condition)
        {
            if (!activateStatusEffects.Contains(effectName))
            {
                if (activateStatusEffects.Count >= 6)
                {
                    activateStatusEffects.RemoveAt(0);
                }
                activateStatusEffects.Add(effectName);
            }
        }
        else
        {
            activateStatusEffects.Remove(effectName);
        }

        if (this is Ally)
        {
            Ally ally = this as Ally;
            ally.UpdateStatusEffectSprites();
        }
        else if (this is Enemy)
        {
            Enemy enemy = this as Enemy;
            enemy.UpdateStatusEffectSprites();
        }
    }

    public Character()
    {
        activateStatusEffects = new List<string>();
    }

    /// <summary>
    /// ターン開始時リフレッシュ
    /// </summary>
    /// <returns></returns>
    public void RefreshWhenTurnBegins()
    {
        // 1ターン限りの効果を解除
        IsGuarded = false;

        // 各ステータス変化の残りターン経過
        ReduceEffectRamainingTurns();

        if (learnedSkills != null)
        {
            // スキルのリキャストターンを更新
            foreach (var skill in learnedSkills)
            {
                skill.CountCoolDown();
            }
        }
    }

    /// <summary>
    /// ターン終了時リフレッシュ
    /// </summary>
    /// <returns></returns>
    public void RefreshWhenEndTurn()
    {
        
    }

    public bool CanUseSkill(Skill skill)
    {
        return skill.CanUse(this);
    }

    public async UniTask<bool> UseSkill(Skill skill, Character objective)
    {

        if (skill != null && skill.Usable && CanUseSkill(skill))
        {
            Debug.Log("Skill is usable and can be used.");

            // スキルの効果を適用する
            skill.User = this;
            skill.Objective = objective;

            // 再使用までのターンをセット
            skill.RemainingTurn = skill.RecastTurn;

            // 効果適用・使用者、対象者のTP獲得を同時に実行
            await UniTask.WhenAll(skill.ApplyActiveEffect(), GetTP(skill.GetTP), objective.GetTP(skill.GiveTP));

            return true;
        }
        else
        {
            return false;
        }
    }

    public async UniTask<bool> UseItem(Item item, Character objective)
    {
        bool result = true;
        if (item != null && item.Usable)
        {
            // スキルの効果を適用する
            item.User = this;
            item.Objective = objective;
            result = await item.ApplyEffect();
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
        if (HPGauge != null && this is Ally)
        {
            var manager = HPGauge.GetComponent<GaugeManager>();
            await manager.AnimateTextAndGauge(hp);
        }
        else if (HPGauge != null && this is Enemy)
        {
            var manager = HPGauge.GetComponent<GaugeManager>();
            float targetFillAmount = (float)hp / MaxHp;
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
        if (hp > maxHp)
        {
            hp = maxHp;
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
        if (mp > maxMp)
        {
            mp = maxMp;
        }
        // MPがゲージと紐づいている場合アニメーション開始
        if (MPGauge != null)
        {
            var manager = MPGauge.GetComponent<GaugeManager>();
            await manager.AnimateTextAndGauge(mp);
        }
        return mp;
    }

    public async UniTask GetTP(int amount)
    {
        tp += amount;
        if (amount > 0)
        {
            if (tp > 100)
            {
                tp = 100;
            }
            // TPがゲージと紐づいている場合アニメーション開始
            if (TPGauge != null)
            {
                var manager = TPGauge.GetComponent<GaugeManager>();
                await manager.AnimateTextAndGauge(tp);
            }
        }    
    }

    public async UniTask<int> ReduceTP(int amount)
    {
        if (amount >= 0)
        {
            tp -= amount;
            if (tp < 0)
            {
                tp = 0;
            }
            // MPがゲージと紐づいている場合アニメーション開始
            if (TPGauge != null)
            {
                var manager = TPGauge.GetComponent<GaugeManager>();
                await manager.AnimateTextAndGauge(tp);
            }
        }
        return mp;
    }

    /// <summary>
    /// ターンが回ってきたときに行動可能か判定する
    /// </summary>
    /// <returns></returns>
    public bool CanAct()
    {
        // スタン
        if (isStunned) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// スキルのMPコストを変化させる
    /// </summary>
    /// <param name="ratio"></param>
    public void UpdateMagicMPCost(double ratio)
    {
        var filteredSkills = learnedSkills.Where(x => x.SkillCategory == Constants.SkillCategory.Magic).ToList();

        foreach (var skill in filteredSkills)
        {
            int reducedCost = (int)(skill.MpCost * ratio);

            skill.MpCost = reducedCost;
        }
    }

    /// <summary>
    /// スキルのMPコストを元々の値に戻す
    /// </summary>
    /// <param name="ratio"></param>
    public void RestoreMagicMPCost(double ratio)
    {
        var filteredSkills = learnedSkills.Where(x => x.SkillCategory == Constants.SkillCategory.Magic).ToList();

        foreach (var skill in filteredSkills)
        {
            var originalSkill = SkillManager.Instance.GetSkillByID(skill.ID);
            if (originalSkill != null)
            {
                var originalMPCost = originalSkill.MpCost;

                skill.MpCost = originalMPCost;
            }      
        }
    }

    public void ReduceEffectRamainingTurns()
    {
        StunRemainingTurn--;
        PoisonRemainingTurn--;
        ParalyzedRemainingTurn--;
        SleepedRemainingTurn--;
        SilencedRemainingTurn--;
        DazedRemainingTurn--;
        TemptedRemainingTurn--;
        FrostedRemainingTurn--;
        PAttackEffectRemainingTurn--;
        PDefenceEffectRemainingTurn--;
        MAttackEffectRemainingTurn--;
        MDefenceEffectRemainingTurn--;
        AgiEffectRemainingTurn--;
        CritEffectRemainingTurn--;
        EvationEffectRemainingTurn--;
    }
}