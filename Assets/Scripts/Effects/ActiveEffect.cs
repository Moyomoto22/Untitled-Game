using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    /// 効果処理呼び出し
    /// </summary>
    /// <param name="type">効果区分</param>
    /// <param name="user">使用者</param>
    /// <param name="objective">対象者</param>
    /// <returns></returns>
    public async UniTask<bool> CallEffect(Constants.ActiveEffectType type, Character user, Character objective)
    {
        bool result = true;

        damageStrings = new List<string>();

        if (animatedText != null)
        {
            animationManager = animatedText.GetComponent<TextAnimationManager>();          // ダメージテキストアニメーション管理クラス
        }
        if (objective.spriteObject != null)
        {
            spriteManipulator = objective.spriteObject.GetComponentInChildren<SpriteManipulator>();  // スプライト操作クラス
        }

        switch (type)
        {
            case Constants.ActiveEffectType.AttackWithWeapon:           // 物理ダメージ
                await AttackWithWeapon(user, objective);
                break;
            case Constants.ActiveEffectType.Guard:                    // 防御
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
            case Constants.ActiveEffectType.HealHPByStatus:                   // HP回復
                await HealHPByStatus(user, objective);
                break;
            case Constants.ActiveEffectType.CurePoison:                // 毒治療
                await CurePoison(user, objective);
                break;
            case Constants.ActiveEffectType.AddStan:                  // スタン付与
                await AddStan(user, objective);
                break;
            case Constants.ActiveEffectType.UpdateHate:
                await UpdateHate(user);
                break;
            case Constants.ActiveEffectType.ReduceHate:
                await UpdateHateByRatio(user);
                break;
            case Constants.ActiveEffectType.HealHpByItem:             // HP回復(アイテム)
                result = await HealHPByItem(user, objective);
                break;
            case Constants.ActiveEffectType.HealMpByItem:             // MP回復(アイテム)
                result = await HealMPByItem(user, objective);
                break;
            case Constants.ActiveEffectType.Steal:             // MP回復(アイテム)
                await Steal(user, objective);
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
    public async UniTask Guard(Character objective)
    {
        objective.IsGuarded = true;
    }

    /// <summary>
    /// 装備武器で攻撃
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask AttackWithWeapon(Character user, Character objective)
    {
        var attack = user.PAttack;
        var defence = objective.PDefence;
        var attibutes = new List<Constants.Attribute>();
        var times = 1;
        var weaponCategory = Constants.WeaponCategory.Fist;

        if (user.RightArm != null)
        {
            times = user.RightArm.Times;
            attibutes = user.RightArm.Attributes.Select(a => a.attribute).ToList();
            weaponCategory = user.RightArm.WeaponCategory;
        }
        else
        {
            // 素手は打撃属性
            attibutes.Add(Constants.Attribute.Blow);
        }

        for (int i = 1; i < times + 1; i++)
        {
            // 回避判定
            if (!DamageCalculator.Dodge(objective.EvationRate))
            {
                // 基礎ダメージを計算
                var baseDamage = DamageCalculator.CalcurateBaseDamage(attack, defence);
                // 属性によるダメージ増減
                baseDamage = DamageCalculator.CalculateDamageWithAttributes(baseDamage, attibutes, objective);
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

                if (damage < 0)
                {
                    damage = 0;
                }

                // 最終的なダメージを、表示するダメージテキストのリストに追加
                damageStrings.Add(damage.ToString());
                if (criticalHit)
                {
                    // クリティカルの場合はテキストの末尾に"！"を追加
                    damageStrings[i - 1] += "!";
                }
                // 対象のHPを減算
                _ = objective.ReduceHP(damage);

                // ヘイト計算・武器経験値可算
                if (user is Ally)
                {
                    Ally ally = user as Ally;
                    int index = PartyMembers.Instance.GetIndex(ally);
                    HateManager.Instance.IncreaseHate(index, damage, skill.HateRatio);

                    ally.GetWeaponExp(weaponCategory, 3);
                }
            }
            else
            {
                damageStrings.Add("Dodge");
            }
        }
        await ShowDamageEffects(objective);
    }

    /// <summary>
    /// 物理ダメージ
    /// </summary>
    /// </summary>
    /// <param name="user">使用者</param>
    /// <param name="objective">対象</param>
    /// <returns></returns>
    public async UniTask PhysicalDamage(Character user, Character objective)
    {
        var attack = user.PAttack;
        var defence = objective.PDefence;
        var attibutes = skill.Attributes.Select(s => s.attribute).ToList();
        var times = skill.Times;

        for (int i = 1; i < times + 1; i++)
        {
            // 回避判定
            if (!DamageCalculator.Dodge(objective.EvationRate))
            {
                // 基礎ダメージを計算
                var baseDamage = DamageCalculator.CalcurateBaseDamage(attack, defence);
                // 属性によるダメージ増減
                baseDamage = DamageCalculator.CalculateDamageWithAttributes(baseDamage, attibutes, objective);
                // 基礎ダメージへランダム幅を加算
                var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);
                // クリティカルヒット判定
                bool criticalHit = DamageCalculator.JudgeCritical(user.CriticalRate);
                // ダメージ倍率
                damage = (int)(damage * skill.DamageRatio);

                if (criticalHit)
                {
                    // クリティカル倍率乗算
                    damage = (int)(damage * Constants.criticalDamageRatio);
                }
                // 防御判定
                damage = DamageCalculator.ReducingByGuard(objective, damage);

                if (damage < 0)
                {
                    damage = 0;
                }

                // 最終的なダメージを、表示するダメージテキストのリストに追加
                damageStrings.Add(damage.ToString());
                if (criticalHit)
                {
                    // クリティカルの場合はテキストの末尾に"！"を追加
                    damageStrings[i - 1] += "!";
                }
                // 対象のHPを減算
                // ゲージ増減と同時に実行するため同期処理
                _ = UniTask.WhenAll(
                objective.ReduceHP(damage),
                user.ReduceTP(skill.TpCost));

                // ヘイト計算
                if (user is Ally)
                {
                    Ally ally = user as Ally;
                    int index = PartyMembers.Instance.GetIndex(ally);
                    HateManager.Instance.IncreaseHate(index, damage, skill.HateRatio);
                }
            }
            else
            {
                damageStrings.Add("Dodge");
                await user.ReduceTP(skill.TpCost);
            }
        }
        await ShowDamageEffects(objective);
    }

    /// <summary>
    /// 魔法ダメージ
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask MagicalDamage(Character user, Character objective)
    {
        var magic = skill as MagicMiracle;

        var attack = (int)(user.MAttack * magic.StatusRatio);  // 魔法攻撃 * ステータス補正
        var defence = objective.MDefence;                      // 魔法防御
        var times = magic.Times;                               // 攻撃回数
        var abstractDamage = magic.BaseValue;                  // 基底ダメージ
        var damageRatio = magic.DamageRatio;                   // ダメージ倍率

        bool criticalHit = DamageCalculator.JudgeCritical(user.CriticalRate);

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
            // ゲージ増減と同時に実行するため同期処理
            _ = UniTask.WhenAll(
            objective.ReduceHP(damage),
            user.ReduceMP(magic.MpCost));

        }
        await ShowDamageEffects(objective);
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <param name="user">使用者</param>
    /// <param name="objective">対象</param>
    /// <returns></returns>
    public string DealDamage(Character user, Character objective, int offenceValue, int defenceValue)
    {
        string damageString;

        // 基礎ダメージを計算
        var baseDamage = DamageCalculator.CalcurateBaseDamage(offenceValue, defenceValue);
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

        if (damage < 0)
        {
            damage = 0;
        }

        // 最終的なダメージ
        damageString = (damage.ToString());
        if (criticalHit)
        {
            // クリティカルの場合はテキストの末尾に"！"を追加
            damageString += "!";
        }
        // 対象のHPを減算
        _ = objective.ReduceHP(damage);

        // ヘイト計算
        if (user is Ally)
        {
            Ally ally = user as Ally;
            int index = PartyMembers.Instance.GetIndex(ally);
            HateManager.Instance.IncreaseHate(index, damage, skill.HateRatio);
        }

        return damageString;

    }

    /// <summary>
    /// HP回復(魔法攻撃力依存)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask HealHP(Character user, Character objective)
    {
        var baseAmount = skill.BaseValue;                   // 基底回復量
        var status = user.MAttack - user.Int + user.Mnd;    // 魔法攻撃力 - INT + MND (武器魔法攻撃力 + MND)
        var times = skill.Times;
        var ratio = skill.DamageRatio;

        for (int i = 1; i < times + 1; i++)
        {
            var baseDamage = baseAmount + (int)(status * ratio);
            var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);

            bool criticalHit = DamageCalculator.JudgeCritical(user.CriticalRate);

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
                _ = user.ReduceMP(magic.MpCost));
            }
            else
            {
                _ = objective.HealHP(damage);
            }

            await ShowHealEffects(objective, healHPTextColor);
        }
    }

    /// <summary>
    /// HP回復(ステータス依存)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask HealHPByStatus(Character user, Character objective)
    {
        var baseAmount = skill.BaseValue;
        var status = GetValueDependStatus(user);
        var times = skill.Times;
        var ratio = skill.DamageRatio;

        for (int i = 1; i < times + 1; i++)
        {
            var baseDamage = baseAmount + (int)(status * ratio);
            var damage = DamageCalculator.AddRandomValueToBaseDamage(baseDamage);

            damageStrings.Add(damage.ToString());

            if (skill is MagicMiracle)
            {
                var magic = skill as MagicMiracle;
                UniTask.WhenAll(
                _ = objective.HealHP(damage),
                _ = user.ReduceMP(magic.MpCost));
            }
            else
            {
                _ = objective.HealHP(damage);
            }

            await ShowHealEffects(objective, healHPTextColor);
        }
    }

    /// <summary>
    /// 毒治療
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask CurePoison(Character user, Character objective)
    {
        if (objective.IsPoisoned)
        {
            objective.IsPoisoned = false;
        }
    }

    /// <summary>
    /// スタン付与
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask AddStan(Character user, Character objective)
    {
        // (1 - スタン耐性)%が成功率
        double successRate = 1 - objective.ResistStan;
        System.Random random = new System.Random();
        int randomValue = random.Next(0, 1);
        bool isSuccess = randomValue < successRate;

        if (isSuccess)
        {
            var str = new List<string>() { "スタン" };

            objective.IsStunned = true;
            objective.StunRemainingTurn = 2;
            await animationManager.ShowDamageTextAnimation(objective, str, spriteManipulator);
        }
    }

    /// <summary>
    /// 使用者のヘイト操作
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask UpdateHate(Character user)
    {
        int baseValue = GetValueDependStatus(user);
        int value = (int)(baseValue * skill.DamageRatio);

        int index = PartyMembers.Instance.GetIndex(user as Ally);
        double reduceRate = skill.DamageRatio;

        HateManager.Instance.UpdateHateByConst(index, value);

        await UniTask.Delay(1);
    }

    /// <summary>
    /// 使用者のヘイト操作(割合)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask UpdateHateByRatio(Character user)
    {
        int index = PartyMembers.Instance.GetIndex(user as Ally);
        double reduceRate = skill.DamageRatio;

        // スキル - ダメージ倍率を現在ヘイトに乗算
        HateManager.Instance.ReduceHate(index, reduceRate);

        await UniTask.Delay(1);
    }

    /// <summary>
    /// スティール
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    private async UniTask Steal(Character user, Character objective)
    {
        Enemy enemy = objective as Enemy;

        if (!enemy.IsStolen)
        {
            double successRate = 1 - objective.ResistStan;
            System.Random random = new System.Random();

            // DEXボーナスを成功率に加算
            float dexBonus = user.Dex / 1000f;
            float successRateOne = enemy.StealRateOne + dexBonus;
            float successRateTwo = enemy.StealRateTwo + dexBonus;

            float randomValue1 = (float)random.NextDouble();
            float randomValue2 = (float)random.NextDouble();

            // 盗めるアイテム1
            if (successRateOne > randomValue1)
            {
                enemy.IsStolen = true;
                ItemInventory2.Instance.AddCopyItem(enemy.StealItemOne);

                var message = enemy.StealItemOne.ItemName + " を盗んだ！";
                var sprite = enemy.StealItemOne.IconImage;

                ToastMessageManager.Instance.ShowToastMessage(message, sprite);
            }
            // 盗めるアイテム2
            else if (successRateTwo > randomValue2)
            {
                enemy.IsStolen = true;
                ItemInventory2.Instance.AddCopyItem(enemy.StealItemTwo);

                var message = enemy.StealItemTwo.ItemName + " を盗んだ！";
                var sprite = enemy.StealItemTwo.IconImage;

                ToastMessageManager.Instance.ShowToastMessage(message, sprite);
            }
            else
            {
                var message = "盗めなかった！";
                ToastMessageManager.Instance.ShowToastMessage(message);
            }
        }
        else
        {
            var message = "何も持っていない！";
            ToastMessageManager.Instance.ShowToastMessage(message);
        }
    }


    /// <summary>
    /// HP回復(アイテム)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    public async UniTask<bool> HealHPByItem(Character user, Character objective)
    {
        // HP満タンの場合は処理失敗
        if (objective.HP < objective.MaxHp)
        {
            var baseAmount = item.BaseValue;           // 基底回復量
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
    public async UniTask<bool> HealMPByItem(Character user, Character objective)
    {
        // MP満タンの場合は処理失敗
        if (objective.MP < objective.MaxMp)
        {
            var baseAmount = item.BaseValue;            // 基底回復量
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

    private int GetValueDependStatus(Character user)
    {
        int value = 0;
        switch (skill.DependentStatus)
        {
            case Skill.DependStatus.STR:
                value = user.Str;
                break;
            case Skill.DependStatus.VIT:
                value = user.Vit;
                break;
            case Skill.DependStatus.DEX:
                value = user.Dex;
                break;
            case Skill.DependStatus.AGI:
                value = user.Agi;
                break;
            case Skill.DependStatus.INT:
                value = user.Int;
                break;
            case Skill.DependStatus.MND:
                value = user.Mnd;
                break;
        }
        return value;
    }
}
