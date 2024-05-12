using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "AllyStatus", menuName = "CreateAllyStatus")]
public class AllyStatus : CharacterStatus
{
    //　キャラクターのイメージ
    [SerializeField]
    public Sprite characterImage;
    // カラー
    [SerializeField]
    public Color color;
    // ID
    [SerializeField]
    public int CharacterID;
    //　クラス
    [SerializeField]
    public Class Class;
    //　獲得経験値
    [SerializeField]
    public int earnedExperience;
    [SerializeField]
    public int nextExperience;
    //　累計経験値
    [SerializeField]
    public int totalExperience;

    // 物理攻撃力(左)
    [SerializeField]
    public int pAttackLeft;

    #region クラス別レベル・経験値

    public List<int> classLevels;
    public List<int> classEarnedExps;
    public List<int> classNextExps;

    //public int warLavel;
    //public int warEarnedExp;
    //public int warNextExp;
    //public int pldLavel;
    //public int pldEarnedExp;
    //public int pldNextExp;
    //public int mnkLavel;
    //public int mnkEarnedExp;
    //public int mnkNextExp;
    //public int thfLavel;
    //public int thfEarnedExp;
    //public int thfNextExp;
    //public int rngLavel;
    //public int rngEarnedExp;
    //public int rngNextExp;
    //public int srcLavel;
    //public int srcEarnedExp;
    //public int srcNextExp;
    //public int clcLavel;
    //public int clcEarnedExp;
    //public int clcNextExp;
    //public int spsLavel;
    //public int spsEarnedExp;
    //public int spsNextExp;

    #endregion

    #region 武器レベル
    //　片手剣レベル
    [SerializeField]
    public int swordLevel;
    //　両手剣レベル
    [SerializeField]
    public int bladeLevel;
    //　短剣レベル
    [SerializeField]
    public int daggerLevel;
    //　槍レベル
    [SerializeField]
    public int spearLevel;
    //　斧レベル
    [SerializeField]
    public int axLevel;
    //　弓レベル
    [SerializeField]
    public int bowLevel;
    //　拳レベル
    [SerializeField]
    public int fistLevel;
    //　槌レベル
    [SerializeField]
    public int hammerLevel;
    //　杖レベル
    [SerializeField]
    public int staffLevel;
    //　盾レベル
    [SerializeField]
    public int shieldLevel;
    #endregion

    #region 武器経験値
    //　獲得経験値 片手剣
    [SerializeField]
    public int earnedExperienceSword;
    //　必要経験値 片手剣
    [SerializeField]
    public int nextExperienceSword;
    //　累計経験値 片手剣
    [SerializeField]
    public int totalExperienceSword;
    //　獲得経験値
    [SerializeField]
    public int earnedExperienceBlade;
    //　必要経験値
    [SerializeField]
    public int nextExperienceBlade;
    //　累計経験値
    [SerializeField]
    public int totalExperienceBlade;
    //　獲得経験値
    [SerializeField]
    public int earnedExperienceDagger;
    //　必要経験値
    [SerializeField]
    public int nextExperienceDagger;
    //　累計経験値
    [SerializeField]
    public int totalExperienceDagger;
    //　獲得経験値
    [SerializeField]
    public int earnedExperienceSpear;
    //　必要経験値
    [SerializeField]
    public int nextExperienceSpear;
    //　累計経験値
    [SerializeField]
    public int totalExperienceSpear;
    //　獲得経験値
    [SerializeField]
    public int earnedExperienceAx;
    //　必要経験値
    [SerializeField]
    public int nextExperienceAx;
    //　累計経験値
    [SerializeField]
    public int totalExperienceAx;
    //　獲得経験値
    [SerializeField]
    public int earnedExperienceBow;
    //　必要経験値
    [SerializeField]
    public int nextExperienceBow;
    //　累計経験値
    [SerializeField]
    public int totalExperienceBow;
    //　獲得経験値
    [SerializeField]
    public int earnedExperienceFist;
    //　必要経験値
    [SerializeField]
    public int nextExperienceFist;
    //　累計経験値
    [SerializeField]
    public int totalExperienceFist;
    //　獲得経験値
    [SerializeField]
    public int earnedExperienceHammer;
    //　必要経験値
    [SerializeField]
    public int nextExperienceHammer;
    //　累計経験値
    [SerializeField]
    public int totalExperienceHammer;
    //　獲得経験値
    [SerializeField]
    public int earnedExperienceStaff;
    //　必要経験値
    [SerializeField]
    public int nextExperienceStaff;
    //　累計経験値
    [SerializeField]
    public int totalExperienceStaff;
    //　獲得経験値
    [SerializeField]
    public int earnedExperienceShield;
    //　必要経験値
    [SerializeField]
    public int nextExperienceShield;
    //　累計経験値
    [SerializeField]
    public int totalExperienceShield;

    #endregion

    // SP
    public int sp;
    //最大SP
    public int maxSp;
    // 装備中スキル
    public List<Skill> equipedSkills;

    /// <summary>
    /// 指定部位にアイテムを装備
    /// </summary>
    /// <param name="item"></param>
    /// <param name="index">0：右手 1:左手 2:頭 3:胴 4:装飾品1 5:装飾品2</param>
    public void Equip(Equip item, int index, bool isEquipDummy = false)
    {
        Unequip(index);
        
        if (item != null)
        {
            Constants.ItemCategory category = item.itemCategory;
            
            switch (category)
            {
                case Constants.ItemCategory.Weapon:
                    var weapon = item as Weapon;
                    if (index == 0)
                    {
                        rightArm = weapon;
                    }
                    else if (index == 1)
                    {
                        leftArm = weapon;
                    }
                    break;
                case Constants.ItemCategory.Head:
                    var h = item as Head;
                    head = h;
                    break;
                case Constants.ItemCategory.Body:
                    var b = item as Body;
                    body = b;
                    break;
                case Constants.ItemCategory.Accessary:
                    var a = item as Accessary;
                    if (index == 4)
                    {
                        accessary1 = a;
                    }
                    else if (index == 5)
                    {
                        accessary2 = a;
                    }
                    break;

            }
            if (!isEquipDummy)
            {
                item.equippedAllyID = CharacterID;
            }
            // ステータス再計算
            CalcStatus();
        }
    }

    /// <summary>
    /// 指定部位の装備解除
    /// </summary>
    /// <param name="item"></param>
    /// <param name="index">0：右手 1:左手 2:頭 3:胴 4:装飾品1 5:装飾品2</param>
    public void Unequip(int index, bool isEquipDummy = false)
    {
        Item item = null;

        switch (index)
        {
            // 右手
            case 0:
                if (rightArm != null)
                {
                    if (!isEquipDummy)
                    {
                        rightArm.equippedAllyID = 0;
                    }
                    rightArm = null;
                }
                break;
            // 左手
            case 1:
                if (leftArm != null)
                {
                    if (!isEquipDummy)
                    {
                        leftArm.equippedAllyID = 0;
                    }
                    leftArm = null;
                }
                break;
            // 頭
            case 2:
                if (head != null)
                {
                    if (!isEquipDummy)
                    {
                        head.equippedAllyID = 0;
                    }
                    head = null;
                }
                break;
            // 胴
            case 3:
                if (body != null)
                {
                    if (!isEquipDummy)
                    {
                        body.equippedAllyID = 0;
                    }
                    body = null;
                }
                break;
            // 装飾品1
            case 4:
                if (accessary1 != null)
                {
                    if (!isEquipDummy)
                    {
                        accessary1.equippedAllyID = 0;
                    }
                    accessary1 = null;
                }
                break;
            // 装飾品2
            case 5:
                if (accessary2 != null)
                {
                    if (!isEquipDummy)
                    {
                        accessary2.equippedAllyID = 0;
                    }
                    accessary2 = null;
                }
                break;
            default:
                break;
        }
        
        // ステータス再計算
        CalcStatus();

    }

    /// <summary>
    /// スキルを装備する
    /// </summary>
    /// <param name="skill"></param>
    public async void EquipSkill(Skill skill)
    {
        if (skill.CanEquip(this))
        {
            equipedSkills.Add(skill);
            CalcStatus();

            sp += skill.spCost;
            if (sp > maxSp)
            {
                sp = maxSp;
            }

            var manager = SPGauge.GetComponent<GaugeManager>();
            if (manager != null)
            {
                await manager.AnimateTextAndGauge(sp, 0.2f);
            }
        }
    }

    /// <summary>
    /// スキルをはずす
    /// </summary>
    /// <param name="skill"></param>
    public async UniTask UnEquipSkill(Skill skill)
    {
        if (equipedSkills.Contains(skill))
        {
            equipedSkills.Remove(skill);
            CalcStatus();
            sp -= skill.spCost;
            if (sp < 0)
            {
                sp = 0;
            }

            var manager = SPGauge.GetComponent<GaugeManager>();
            if (manager != null)
            {
                await manager.AnimateTextAndGauge(sp, 0.2f);
            }
        }
    }

    private void ApplyPassiveSkillsEffect()
    {
        foreach(Skill skill in equipedSkills)
        {
            if (skill is PassiveSkill)
            {
                var p = skill as PassiveSkill;
                p.applyPassiveEffect(this);
            }
        }
    }

    /// <summary>
    /// アイテムが装備可能かチェックする
    /// </summary>
    /// <param name="status"></param>
    /// <param name="item"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public async Task<bool> CheckEquippable(Item item, int index)
    {
        bool isEquippable = false;
        bool isNitouryu = false;

        List<AllyStatus> statuses = new List<AllyStatus>();

        for (int i = 1; i < 5; i++)
        {
            if (i != CharacterID)
            {
                statuses.Add(await CommonController.GetAllyStatus(i));
            }
        }


        Equip equip = item as Equip;

        if (equip != null && Class != null)
        {

            switch (index)
            {
                // 右手
                case 0:
                    Weapon weapon = equip as Weapon;
                    if (weapon != null)
                    {
                        // 右手に盾は装備不可
                        if (weapon.weaponCategory != Constants.WeaponCategory.Shield)
                        {
                            if (weapon.equipableClasses.Exists(x => x.name == Class.name))
                            {
                                foreach (var status in statuses)
                                {
                                    if (status.rightArm == weapon)
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                    }
                    break;
                // 左手
                case 1:
                    Weapon weapon2 = equip as Weapon;
                    if (weapon2 != null)
                    {
                        // 左手は基本盾のみ
                        if (weapon2.weaponCategory == Constants.WeaponCategory.Shield || isNitouryu)
                        {
                            if (weapon2.equipableClasses.Exists(x => x.name == Class.name) && weapon2.equippedAllyID == 0)
                            {
                                foreach (var status in statuses)
                                {
                                    if (status.leftArm == weapon2)
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                    }
                    break;
                // 頭・胴
                case 2:
                case 3:
                    if (equip.equipableClasses.Exists(x => x.name == Class.name) && equip.equippedAllyID == 0)
                    {
                        foreach (var status in statuses)
                        {
                            if (status.head == equip || status.body == equip)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    break;
                // 装飾品１・２
                case 4:
                case 5:
                    if (equip.equippedAllyID == 0)
                    {
                        foreach (var status in statuses)
                        {
                            if (status.accessary1 == equip || status.accessary2 == equip)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    break;
                default:
                    return false;
            }
        }

        return isEquippable;
    }

    /// <summary>
    /// 装備やスキルを考慮したステータスを再計算する
    /// </summary>
    public void CalcStatus()
    {
        // 基礎ステータス
        //int level = classLevels[Class.ID - 1] - 1;

        //maxHp = Constants.hpGrow[level];
        //maxMp = Constants.mpGrow[level];
        //str = Constants.prmGrow[level];
        //vit = Constants.prmGrow[level];
        //dex = Constants.prmGrow[level];
        //agi = Constants.prmGrow[level];
        //inte = Constants.prmGrow[level];
        //mnd = Constants.prmGrow[level];
        //maxSp = Constants.spGrow[level];

        // クラス補正
        maxHp2 = (int)(maxHp * Class.hpRate);
        maxMp2 = (int)(maxMp * Class.mpRate);
        str2 = (int)(str * Class.strRate);
        vit2 = (int)(vit * Class.vitRate);
        dex2 = (int)(dex * Class.dexRate);
        agi2 = (int)(agi * Class.agiRate);
        inte2 = (int)(inte * Class.intRate);
        mnd2 = (int)(mnd * Class.mndRate);

        // 基本ステータス
        maxHp2 = maxHp2 + (rightArm?.maxHp ?? 0) + (leftArm?.maxHp ?? 0) + (head?.maxHp ?? 0) + (body?.maxHp ?? 0) + (accessary1?.maxHp ?? 0) + (accessary2?.maxHp ?? 0);
        maxMp2 = maxMp2 + (rightArm?.maxMp ?? 0) + (leftArm?.maxMp ?? 0) + (head?.maxMp ?? 0) + (body?.maxMp ?? 0) + (accessary1?.maxMp ?? 0) + (accessary2?.maxMp ?? 0);
        str2 = str2 + (rightArm?.str ?? 0) + (leftArm?.str ?? 0) + (head?.str ?? 0) + (body?.str ?? 0) + (accessary1?.str ?? 0) + (accessary2?.str ?? 0);
        vit2 = vit2 + (rightArm?.vit ?? 0) + (leftArm?.vit ?? 0) + (head?.vit ?? 0) + (body?.vit ?? 0) + (accessary1?.vit ?? 0) + (accessary2?.vit ?? 0);
        dex2 = dex2 + (rightArm?.dex ?? 0) + (leftArm?.dex ?? 0) + (head?.dex ?? 0) + (body?.dex ?? 0) + (accessary1?.dex ?? 0) + (accessary2?.dex ?? 0);
        agi2 = agi2 + (rightArm?.agi ?? 0) + (leftArm?.agi ?? 0) + (head?.agi ?? 0) + (body?.agi ?? 0) + (accessary1?.agi ?? 0) + (accessary2?.agi ?? 0);
        inte2 = inte2 + (rightArm?.inte ?? 0) + (leftArm?.inte ?? 0) + (head?.inte ?? 0) + (body?.inte ?? 0) + (accessary1?.inte ?? 0) + (accessary2?.inte ?? 0);
        mnd2 = mnd2 + (rightArm?.mnd ?? 0) + (leftArm?.mnd ?? 0) + (head?.mnd ?? 0) + (body?.mnd ?? 0) + (accessary1?.mnd ?? 0) + (accessary2?.mnd ?? 0);

        // パッシブスキル
        ApplyPassiveSkillsEffect();

        // 物理攻撃力依存値 武器によってSTR or DEX or INT or MNDを攻撃力に加算
        int pAttackCorect = str2;
        int pAttackCorectLeft = 0;
        if (rightArm != null)
        {
            switch (rightArm.dependentStatus)
            {
                case 1:
                    pAttackCorect = dex2;
                    break;
                case 2:
                    pAttackCorect = inte2;
                    break;
                case 3:
                    pAttackCorect = mnd2;
                    break;
                default:
                    pAttackCorect = str2;
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
                        pAttackCorectLeft = dex2;
                        break;
                    case 2:
                        pAttackCorectLeft = inte2;
                        break;
                    case 3:
                        pAttackCorectLeft = mnd2;
                        break;
                    default:
                        pAttackCorectLeft = str2;
                        break;
                }
            }
        }

        // サブステータス
        //pAttackLeft = pAttackCorectLeft + (leftArm?.pAttack ?? 0);
        pAttack = pAttackCorect + (rightArm?.pAttack ?? 0) + (leftArm?.pAttack ?? 0) + (head?.pAttack ?? 0) + (body?.pAttack ?? 0) + (accessary1?.pAttack ?? 0) + (accessary2?.pAttack ?? 0);
        mAttack = inte2 + (rightArm?.mAttack ?? 0) + (leftArm?.mAttack ?? 0) + (head?.mAttack ?? 0) + (body?.mAttack ?? 0) + (accessary1?.mAttack ?? 0) + (accessary2?.mAttack ?? 0);
        pDefence = vit2 + (rightArm?.pDefence ?? 0) + (leftArm?.pDefence ?? 0) + (head?.pDefence ?? 0) + (body?.pDefence ?? 0) + (accessary1?.pDefence ?? 0) + (accessary2?.pDefence ?? 0);
        mDefence = mnd2 / 2 + (rightArm?.mDefence ?? 0) + (leftArm?.mDefence ?? 0) + (head?.mDefence ?? 0) + (body?.mDefence ?? 0) + (accessary1?.mDefence ?? 0) + (accessary2?.mDefence ?? 0);

        if(hp > maxHp2)
        {
            hp = maxHp2;
        }
        if (mp > maxMp2)
        {
            mp = maxMp2;
        }
    }

    /// <summary>
    /// スキルを習得する
    /// </summary>
    /// <param name="skill"></param>
    public void LearnSkill(Skill skill)
    {
        // クローンを作成
        Skill newSkill = Instantiate(skill);
        learnedSkills.Add(newSkill);
    }

    /// <summary>
    /// スキルを着脱する
    /// </summary>
    /// <param name="skill"></param>
    public void EquipSkill(Skill skill, bool isEquip)
    {
        if (!skill.isEquipped)
        {
            sp = Math.Min(sp + skill.spCost, maxSp);
        }
        else
        {
            sp = Math.Max(sp - skill.spCost, 0);
        }
        skill.isEquipped = !isEquip;
    }

    /// <summary>
    /// 装備中のスキルをすべて外す
    /// </summary>
    public void RemoveAllSkills()
    {
        foreach(Skill skill in learnedSkills)
        {
            if (skill.isEquipped)
            {
                skill.isEquipped = false;
                sp = Math.Min(sp - skill.spCost, 0);
            }
        }
    }

    /// <summary>
    /// スキルが装備可能か判定する
    /// エラーメッセージを返却し、戻り値が空白なら装備可能とする
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public string CanEquipSkill(Skill skill)
    {
        // エラーメッセージ
        const string overSp = "SPが不足しています";

        int leftSp = maxSp - sp;

        // sp判定
        if (skill.spCost > leftSp)
        {
            return overSp;
        }

        //if (skill is PassiveSkill)
        //{
        //    PassiveSkill passive = skill as PassiveSkill;
        //    if (passive != null)
        //    {
        //        if (passive.spCost > leftSp)
        //        {
        //            return overSp;
        //        }
        //    }
        //}
        //else if (skill is ActiveSkill)
        //{
        //    ActiveSkill active = skill as ActiveSkill;
        //    if (active != null)
        //    {
        //        if (active.spCost > leftSp)
        //        {
        //            return overSp;
        //        }
        //    }
        //}
        return "";
    }

    /// <summary>
    /// スキルが使用可能か判定する
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public bool CanUseSKill(Skill skill)
    {
        MagicMiracle magicMiracle = skill as MagicMiracle;
        Arts arts = skill as Arts;
        ActiveSkill active = skill as ActiveSkill;
        //PassiveSkill passive = skill as PassiveSkill;

        if (magicMiracle != null)
        {
            if (magicMiracle.usableClasses.Exists(x => x.name == Class.name) && magicMiracle.MPCost <= mp)
            {
                return true;
            }
        }
        else if (arts != null)
        {
            if (arts.TPCost <= tp)
            {
                return true;
            }
        }
        else if (active != null)
        {

        }
        return false;
    }

    /// <summary>
    /// スキルコストを消費
    /// </summary>
    private void ConsumeSkillCost(Skill skill)
    {
        MagicMiracle magicMiracle = skill as MagicMiracle;
        Arts arts = skill as Arts;
        //ActiveSkill active = skill as ActiveSkill;
        //PassiveSkill passive = skill as PassiveSkill;

        if (magicMiracle != null)
        {
            mp = mp - magicMiracle.MPCost;
        }
        if (arts != null)
        {
            tp = tp - arts.TPCost;
        }
    }

    /// <summary>
    /// クラスチェンジ
    /// </summary>
    public void ChangeClass(Class newClass)
    {
        // 装備解除
        for (int i = 0; i < 6; i++)
        {
            Unequip(i);
        }
        // スキル全解除
        RemoveAllSkills();
        // クラス変更
        Class = newClass;
        // レベル設定
        //level = classLevels[newClass.ID - 1];
        // 画像設定
        if (CharacterID > 0)
        {
            characterImage = newClass.imagesA[CharacterID - 1];
        }

        // ステータス再計算
        CalcStatus();

    }

    public List<int> ReturnStatusList(int classNo, Class cl)
    {

        int level = classLevels[classNo] - 1;

        int hp = Constants.hpGrow[level];
        int mp = Constants.mpGrow[level];
        int str = Constants.prmGrow[level];
        int vit = Constants.prmGrow[level];
        int dex = Constants.prmGrow[level];
        int agi = Constants.prmGrow[level];
        int inte = Constants.prmGrow[level];
        int mnd = Constants.prmGrow[level];


        hp = (int)(hp * cl.hpRate);
        mp = (int)(mp * cl.mpRate);
        str = (int)(str * cl.strRate);
        vit = (int)(vit * cl.vitRate);
        dex = (int)(dex * cl.dexRate);
        agi = (int)(agi * cl.agiRate);
        inte = (int)(inte * cl.intRate);
        mnd = (int)(mnd * cl.mndRate);

        int pAtk = str;
        int mAtk = inte;
        int pDef = vit;
        int mDef = mnd / 2;
        int pCrt = dex / 50 + 2;
        int mCrt = 2;
        int pAvo = agi / 40 + 1;
        int mAvo = agi / 40 + 1; ;
        int sp = Constants.spGrow[level];

        List<int> statusList = new List<int>() { hp, mp, str, vit, dex, agi, inte, mnd, pAtk, mAtk, pDef, mDef, pCrt, mCrt, pAvo, mAvo, sp };

        return statusList;
    }
}