using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スキル効果(シングルトン)
/// </summary>
public class PassiveEffect : MonoBehaviour
{
    private PassiveSkill skill;

    private CharacterController user;
    private CharacterController objective;

    private List<string> damageStrings;
    public GameObject animatedText;

    private TextAnimationManager animationManager;
    private SpriteManipulator spriteManipulator;

    private GaugeManager hpGaugeManager;
    private GaugeManager mpGaugeManager;
    private GaugeManager tpGaugeManager;

    private Color healHPTextColor;
    private Color healMPTextColor;

    public static PassiveEffect Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="skill">使用スキル</param>
    public void Initialize(PassiveSkill skill)
    {
        this.skill = skill;
    }

    /// <summary>
    /// 効果処理呼び出しメソッド
    /// </summary>
    /// <param name="type">効果区分</param>
    /// <param name="user">使用者</param>
    /// <param name="objective">対象者</param>
    /// <returns></returns>
    public void CallEffect(Constants.PassiveEffectType type, Character user, Character objective)
    {
        damageStrings = new List<string>();

        switch (type)
        {
            case Constants.PassiveEffectType.GainMAXHP: // 最大HP上昇
                GainMaxHP(user);
                break;
            case Constants.PassiveEffectType.GainMAXMP: // 最大MP上昇
                GainMaxMP(user);
                break;
            case Constants.PassiveEffectType.GainSTR: // STR上昇
                GainSTR(user);
                break;
            case Constants.PassiveEffectType.GainVIT: // VIT上昇
                GainVIT(user);
                break;
            case Constants.PassiveEffectType.GainDEX: // DEX上昇
                GainDEX(user);
                break;
            case Constants.PassiveEffectType.GainAGI: // AGI上昇
                GainAGI(user);
                break;
            case Constants.PassiveEffectType.GainINT: // INT上昇
                GainINT(user);
                break;
            case Constants.PassiveEffectType.GainMND: // MND上昇
                GainMND(user);
                break;
            case Constants.PassiveEffectType.GainCritical: // クリティカル率上昇
                GainCritical(user);
                break;
            case Constants.PassiveEffectType.GainEvation: // 回避率上昇
                GainEvation(user);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// HP上昇
    /// </summary>
    /// <param name="user"></param>
    public void GainMaxHP(Character user)
    {
        user.MaxHp += skill.BaseValue;
    }

    /// <summary>
    /// MP上昇
    /// </summary>
    /// <param name="user"></param>
    public void GainMaxMP(Character user)
    {
        user.MaxMp += skill.BaseValue;
    }

    /// <summary>
    /// STR上昇
    /// </summary>
    /// <param name="user"></param>
    public void GainSTR(Character user)
    {
        user.Str += skill.BaseValue;
    }

    /// <summary>
    /// VIT上昇
    /// </summary>
    /// <param name="user"></param>
    public void GainVIT(Character user)
    {
        user.Vit += skill.BaseValue;
    }

    /// <summary>
    /// DEX上昇
    /// </summary>
    /// <param name="user"></param>
    public void GainDEX(Character user)
    {
        user.Dex += skill.BaseValue;
    }

    /// <summary>
    /// AGI上昇
    /// </summary>
    /// <param name="user"></param>
    public void GainAGI(Character user)
    {
        user.Agi += skill.BaseValue;
    }

    /// <summary>
    /// DEX上昇
    /// </summary>
    /// <param name="user"></param>
    public void GainINT(Character user)
    {
        user.Int += skill.BaseValue;
    }

    /// <summary>
    /// MND上昇
    /// </summary>
    /// <param name="user"></param>
    public void GainMND(Character user)
    {
        user.Mnd += skill.BaseValue;
    }

    /// <summary>
    /// CRT上昇
    /// </summary>
    /// <param name="user"></param>
    public void GainCritical(Character user)
    {
        user.CriticalRate += skill.BaseValue;
    }

    /// <summary>
    /// AVO上昇
    /// </summary>
    /// <param name="user"></param>
    public void GainEvation(Character user)
    {
        user.EvationRate += skill.BaseValue;
    }

    /// <summary>
    /// ベルセルク(両手武器装備時攻撃力アップ)
    /// </summary>
    /// <param name="user"></param>
    public void Berserk(Character user)
    {
        if (user.RightArm != null)
        {
            if (user.RightArm.IsTwoHanded)
            {
                user.PAttack = (int)(user.PAttack * 1.1);
            }
        }
    }

    /// <summary>
    /// 物理ダメージ
    /// </summary>
    /// <param name="user">使用者</param>
    /// <param name="objective">対象</param>
    /// <returns></returns>
    public async UniTask PhysicalDamage(Character user, Character objective)
    {
        var attack = user.PAttack;
        var defence = objective.PDefence;
        var times = 1;

        if (user.RightArm != null)
        {
            times = user.RightArm.Times;
        }

        for (int i = 1; i < times + 1; i++)
        {
            // 基礎ダメージを計算
            var baseDamage = DamageCalculator.CalcurateBaseDamage(attack, defence);
            // 基礎ダメージへランダム幅を加算
            var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);
            // クリティカルヒット判定
            bool criticalHit = DamageCalculator.JudgeCritical(user.CriticalRate);

            if (criticalHit)
            {
                // クリティカル倍率乗算
                damage = (int)(damage * Constants.criticalDamageRatio);
            }
            // 防御判定
            damage = DamageCalculator.ReducingByGuard(objective, damage);

            // 最終的なダメージを、表示するダメージテキストのリストに追加
            damageStrings.Add(damage.ToString());
            if (criticalHit)
            {
                // クリティカルの場合はテキストの末尾に"！"を追加
                damageStrings[i - 1] += "!";
            }
            // 対象のHPを減算
            _ = objective.ReduceHP(damage);
        }
        await ShowDamageEffects(objective);
    }

    /// <summary>
    /// ダメージ時演出(テキスト表示・スプライトのシェイク・フラッシュ)
    /// </summary>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask ShowDamageEffects(Character objective)
    {
        if (animationManager != null && spriteManipulator != null)
        {
            await animationManager.ShowDamageTextAnimation(objective, damageStrings, spriteManipulator);
        }
    }

    /// <summary>
    /// 回復時演出(テキスト表示・スプライトのシェイク・フラッシュ)
    /// </summary>
    /// <param name="objective"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private async UniTask ShowHealEffects(Character objective, Color color)
    {
        if (animationManager != null && spriteManipulator != null)
        {
            await UniTask.WhenAll(
            animationManager.ShowHealTextAnimation(objective, damageStrings, color),
            spriteManipulator.Flash(0.5f, color)
            );
        }
        else if (animationManager != null)
        {
            await animationManager.ShowHealTextAnimation(objective, damageStrings, color);
        }
    }
}
