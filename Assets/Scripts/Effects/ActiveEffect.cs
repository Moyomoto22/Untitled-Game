using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテム・スキル効果(シングルトン)
/// </summary>
public class ActiveEffect : MonoBehaviour
{
    private Skill skill;
    private Item item;
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

    public static ActiveEffect Instance { get; private set; }

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
    /// <param name="item">使用アイテム</param>
    public void Initialize(Skill skill, Item item)
    {
        this.skill = skill;
        this.item = item;

        healHPTextColor = CommonController.GetColor("#32C883"); // HP回復時のテキストカラー
        healMPTextColor = CommonController.GetColor("#569FE5"); // MP回復時のテキストカラー
    }

    /// <summary>
    /// 効果処理呼び出しメソッド
    /// </summary>
    /// <param name="type">効果区分</param>
    /// <param name="user">使用者</param>
    /// <param name="objective">対象者</param>
    /// <returns></returns>
    public async UniTask<bool> CallEffect(Constants.ActiveEffectType type, CharacterStatus user, CharacterStatus objective)
    {
        bool result = true;
        
        damageStrings = new List<string>();

        if (animatedText != null)
        {
            animationManager = animatedText.GetComponent<TextAnimationManager>();          // ダメージテキストアニメーション管理クラス
        }
        if (objective.spriteObject != null)
        {
            spriteManipulator = objective.spriteObject.GetComponent<SpriteManipulator>();  // スプライト操作クラス
        }

        switch (type)
        {
            case Constants.ActiveEffectType.Guard:                    // 物理ダメージ
                await Guard(objective);
                break;
            case Constants.ActiveEffectType.PhysicalDamage:           // 物理ダメージ
                await PhysicalDamage(user, objective);
                break;
            case Constants.ActiveEffectType.MagicalDamage:            // 魔法ダメージ
                await MagicalDamage(user, objective);
                break;
            case Constants.ActiveEffectType.HealHP:                   // HP回復
                await HealHP(user, objective);
                break;
            case Constants.ActiveEffectType.HealHpByItem:             // HP回復(アイテム)
                result = await HealHPByItem(user, objective);
                break;
            case Constants.ActiveEffectType.HealMpByItem:             // MP回復(アイテム)
                result = await HealMPByItem(user, objective);
                break;
            default:
                break;
        }
        return result;
    }

    /// <summary>
    /// 防御
    /// </summary>
    /// <param name="objective">対象者</param>
    public async UniTask Guard(CharacterStatus objective)
    {
        objective.isGuarded = true;
    }

    /// <summary>
    /// 物理ダメージ
    /// </summary>
    /// <param name="user">使用者</param>
    /// <param name="objective">対象</param>
    /// <returns></returns>
    public async UniTask PhysicalDamage(CharacterStatus user, CharacterStatus objective)
    {
        var attack = user.pAttack;
        var defence = objective.pDefence;
        var times = 1;

        if (user.rightArm != null)
        {
            times = user.rightArm.times;
        }

        for (int i = 1; i < times + 1; i++)
        {
            // 基礎ダメージを計算
            var baseDamage = DamageCalculator.CalcurateBaseDamage(attack, defence);
            // 基礎ダメージへランダム幅を加算
            var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);
            // クリティカルヒット判定
            bool criticalHit = DamageCalculator.JudgeCritical(user.pCrit);

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
    /// 魔法ダメージ
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask MagicalDamage(CharacterStatus user, CharacterStatus objective)
    {
        var magic = skill as MagicMiracle;

        var attack = (int)(user.mAttack * magic.statusRatio);  // 魔法攻撃 * ステータス補正
        var defence = objective.mDefence;                      // 魔法防御
        var times = magic.times;                               // 攻撃回数
        var abstractDamage = magic.baseValue;                  // 基底ダメージ
        var damageRatio = magic.damageRatio;                   // ダメージ倍率

        bool criticalHit = DamageCalculator.JudgeCritical(user.mCrit);

        for (int i = 1; i < times + 1; i++)
        {
            // 基本ダメージ = (基底ダメージ + 魔法攻撃力 * ステータス補正 / 2 - 魔法防御 / 3) * ダメージ倍率
            var baseDamage = (int)((abstractDamage + DamageCalculator.CalcurateBaseDamage(attack, defence)) * damageRatio);
            // 乱数(基本ダメージ / 16 + 1)
            var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);

            if (criticalHit)
            {
                damage = (int)(damage * Constants.criticalDamageRatio);
            }
            damage = DamageCalculator.ReducingByGuard(objective, damage);

            damageStrings.Add(damage.ToString());
            if (criticalHit)
            {
                damageStrings[i - 1] += "!";
            }
            UniTask.WhenAll(
            objective.ReduceHP(damage),
            user.ReduceMP(magic.MPCost));
            
        }
        await ShowDamageEffects(objective);

    }

    /// <summary>
    /// HP回復
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask HealHP(CharacterStatus user, CharacterStatus objective)
    {
        

        var baseAmount = skill.baseValue;                   // 基底回復量
        var status = user.mAttack - user.inte2 + user.mnd2; // 魔法攻撃力 - INT + MND (武器魔法攻撃力 + MND)
        var times = skill.times;
        var ratio = skill.damageRatio;

        for (int i = 1; i < times + 1; i++)
        {
            var baseDamage = baseAmount + (int)(status * ratio);
            var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);

            bool criticalHit = DamageCalculator.JudgeCritical(user.mCrit);

            if (criticalHit)
            {
                damage = (int)(damage * Constants.criticalDamageRatio);
            }
            damageStrings.Add(damage.ToString());
            if (criticalHit)
            {
                damageStrings[i - 1] += "!";
            }

            if (skill is MagicMiracle)
            {
                var magic = skill as MagicMiracle;
                UniTask.WhenAll(
                _ = objective.HealHP(damage),
                _ = user.ReduceMP(magic.MPCost));
            }
            else
            {
                _ = objective.HealHP(damage);
            }
               
            await ShowHealEffects(objective, healHPTextColor);
        }
    }

    /// <summary>
    /// HP回復(アイテム)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask<bool> HealHPByItem(CharacterStatus user, CharacterStatus objective)
    {
        // HP満タンの場合は処理失敗
        if (objective.hp < objective.maxHp2)
        {
            var baseAmount = item.baseValue;           // 基底回復量
            damageStrings.Add(baseAmount.ToString());
            ItemInventory2.Instance.RemoveItem(item);
            await UniTask.WhenAll(
                objective.HealHP(baseAmount),
                ShowHealEffects(objective, healHPTextColor));
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// MP回復(アイテム)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask<bool> HealMPByItem(CharacterStatus user, CharacterStatus objective)
    {
        // MP満タンの場合は処理失敗
        if (objective.mp < objective.maxMp2)
        {
            var baseAmount = item.baseValue;            // 基底回復量
            damageStrings.Add(baseAmount.ToString());
            ItemInventory2.Instance.RemoveItem(item);
            await UniTask.WhenAll(
                objective.HealMP(baseAmount),
                ShowHealEffects(objective, healMPTextColor)
                );
            return true;
        }
        else
        {
            return false;            
        }
    }

    /// <summary>
    /// ダメージ時演出(テキスト表示・スプライトのシェイク・フラッシュ)
    /// </summary>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask ShowDamageEffects(CharacterStatus objective)
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
    private async UniTask ShowHealEffects(CharacterStatus objective, Color color)
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
