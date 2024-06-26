using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "Ally", menuName = "CreateAlly")]
public class Ally : Character
{
    // ID
    [SerializeField] private int characterID;
    public int CharacterID
    {
        get { return characterID; }
        set { characterID = value; }
    }

    // �N���X
    [SerializeField] private Class characterClass;
    public Class CharacterClass
    {
        get { return characterClass; }
        set { characterClass = value; }
    }

    // �݌v�o���l
    [SerializeField] private int totalExperience;
    public int TotalExperience
    {
        get { return totalExperience; }
        set { totalExperience = value; }
    }

    // �݌v���x��
    [SerializeField] private int totalLevel;
    public int TotalLevel
    {
        get { return totalLevel; }
        set { totalLevel = value; }
    }

    // �����U����(��)
    [SerializeField] private int pAttackLeft;
    public int PAttackLeft
    {
        get { return pAttackLeft; }
        set { pAttackLeft = value; }
    }

    #region �N���X�ʃ��x���E�o���l
    [SerializeField] private List<int> classLevels;
    public List<int> ClassLevels
    {
        get { return classLevels; }
        set { classLevels = value; }
    }

    [SerializeField] private List<int> classEarnedExps;
    public List<int> ClassEarnedExps
    {
        get { return classEarnedExps; }
        set { classEarnedExps = value; }
    }

    [SerializeField] private List<int> classNextExps;
    public List<int> ClassNextExps
    {
        get { return classNextExps; }
        set { classNextExps = value; }
    }
    #endregion

    #region ���탌�x��
    [SerializeField] private int swordLevel;
    public int SwordLevel
    {
        get { return swordLevel; }
        set { swordLevel = value; }
    }

    [SerializeField] private int bladeLevel;
    public int BladeLevel
    {
        get { return bladeLevel; }
        set { bladeLevel = value; }
    }

    [SerializeField] private int daggerLevel;
    public int DaggerLevel
    {
        get { return daggerLevel; }
        set { daggerLevel = value; }
    }

    [SerializeField] private int spearLevel;
    public int SpearLevel
    {
        get { return spearLevel; }
        set { spearLevel = value; }
    }

    [SerializeField] private int axLevel;
    public int AxLevel
    {
        get { return axLevel; }
        set { axLevel = value; }
    }

    [SerializeField] private int bowLevel;
    public int BowLevel
    {
        get { return bowLevel; }
        set { bowLevel = value; }
    }

    [SerializeField] private int fistLevel;
    public int FistLevel
    {
        get { return fistLevel; }
        set { fistLevel = value; }
    }

    [SerializeField] private int hammerLevel;
    public int HammerLevel
    {
        get { return hammerLevel; }
        set { hammerLevel = value; }
    }

    [SerializeField] private int staffLevel;
    public int StaffLevel
    {
        get { return staffLevel; }
        set { staffLevel = value; }
    }

    [SerializeField] private int shieldLevel;
    public int ShieldLevel
    {
        get { return shieldLevel; }
        set { shieldLevel = value; }
    }
    #endregion

    #region ����o���l
    [SerializeField] private int earnedExperienceSword;
    public int EarnedExperienceSword
    {
        get { return earnedExperienceSword; }
        set { earnedExperienceSword = value; }
    }

    [SerializeField] private int nextExperienceSword;
    public int NextExperienceSword
    {
        get { return nextExperienceSword; }
        set { nextExperienceSword = value; }
    }

    [SerializeField] private int totalExperienceSword;
    public int TotalExperienceSword
    {
        get { return totalExperienceSword; }
        set
        {
            totalExperienceSword = value;
            UpdateWeaponLevelAndNextExp(ref swordLevel, ref earnedExperienceSword, ref nextExperienceSword, totalExperienceSword, Constants.WeaponCategory.Sword);
        }
    }

    [SerializeField] private int earnedExperienceBlade;
    public int EarnedExperienceBlade
    {
        get { return earnedExperienceBlade; }
        set { earnedExperienceBlade = value; }
    }

    [SerializeField] private int nextExperienceBlade;
    public int NextExperienceBlade
    {
        get { return nextExperienceBlade; }
        set { nextExperienceBlade = value; }
    }

    [SerializeField] private int totalExperienceBlade;
    public int TotalExperienceBlade
    {
        get { return totalExperienceBlade; }
        set { totalExperienceBlade = value;
              UpdateWeaponLevelAndNextExp(ref bladeLevel, ref earnedExperienceBlade, ref nextExperienceBlade, totalExperienceBlade, Constants.WeaponCategory.Blade);
        }
    }

    [SerializeField] private int earnedExperienceDagger;
    public int EarnedExperienceDagger
    {
        get { return earnedExperienceDagger; }
        set { earnedExperienceDagger = value; }
    }

    [SerializeField] private int nextExperienceDagger;
    public int NextExperienceDagger
    {
        get { return nextExperienceDagger; }
        set { nextExperienceDagger = value; }
    }

    [SerializeField] private int totalExperienceDagger;
    public int TotalExperienceDagger
    {
        get { return totalExperienceDagger; }
        set { totalExperienceDagger = value;
              UpdateWeaponLevelAndNextExp(ref daggerLevel, ref earnedExperienceDagger, ref nextExperienceDagger, totalExperienceDagger, Constants.WeaponCategory.Dagger);
        }
    }

    [SerializeField] private int earnedExperienceSpear;
    public int EarnedExperienceSpear
    {
        get { return earnedExperienceSpear; }
        set { earnedExperienceSpear = value; }
    }

    [SerializeField] private int nextExperienceSpear;
    public int NextExperienceSpear
    {
        get { return nextExperienceSpear; }
        set { nextExperienceSpear = value; }
    }

    [SerializeField] private int totalExperienceSpear;
    public int TotalExperienceSpear
    {
        get { return totalExperienceSpear; }
        set { totalExperienceSpear = value;
              UpdateWeaponLevelAndNextExp(ref spearLevel, ref earnedExperienceSpear, ref nextExperienceSpear, totalExperienceSpear, Constants.WeaponCategory.Spear);
        }
    }

    [SerializeField] private int earnedExperienceAx;
    public int EarnedExperienceAx
    {
        get { return earnedExperienceAx; }
        set { earnedExperienceAx = value; }
    }

    [SerializeField] private int nextExperienceAx;
    public int NextExperienceAx
    {
        get { return nextExperienceAx; }
        set { nextExperienceAx = value; }
    }

    [SerializeField] private int totalExperienceAx;
    public int TotalExperienceAx
    {
        get { return totalExperienceAx; }
        set { totalExperienceAx = value;
              UpdateWeaponLevelAndNextExp(ref axLevel, ref earnedExperienceAx, ref nextExperienceAx, totalExperienceAx, Constants.WeaponCategory.Ax);
        }
    }

    [SerializeField] private int earnedExperienceBow;
    public int EarnedExperienceBow
    {
        get { return earnedExperienceBow; }
        set { earnedExperienceBow = value; }
    }

    [SerializeField] private int nextExperienceBow;
    public int NextExperienceBow
    {
        get { return nextExperienceBow; }
        set { nextExperienceBow = value; }
    }

    [SerializeField] private int totalExperienceBow;
    public int TotalExperienceBow
    {
        get { return totalExperienceBow; }
        set { totalExperienceBow = value;
              UpdateWeaponLevelAndNextExp(ref bowLevel, ref earnedExperienceBow, ref nextExperienceBow, totalExperienceBow, Constants.WeaponCategory.Bow);
        }
    }

    [SerializeField] private int earnedExperienceFist;
    public int EarnedExperienceFist
    {
        get { return earnedExperienceFist; }
        set { earnedExperienceFist = value; }
    }

    [SerializeField] private int nextExperienceFist;
    public int NextExperienceFist
    {
        get { return nextExperienceFist; }
        set { nextExperienceFist = value; }
    }

    [SerializeField] private int totalExperienceFist;
    public int TotalExperienceFist
    {
        get { return totalExperienceFist; }
        set { totalExperienceFist = value;
              UpdateWeaponLevelAndNextExp(ref fistLevel, ref earnedExperienceFist, ref nextExperienceFist, totalExperienceFist, Constants.WeaponCategory.Fist);
        }
    }

    [SerializeField] private int earnedExperienceHammer;
    public int EarnedExperienceHammer
    {
        get { return earnedExperienceHammer; }
        set { earnedExperienceHammer = value; }
    }

    [SerializeField] private int nextExperienceHammer;
    public int NextExperienceHammer
    {
        get { return nextExperienceHammer; }
        set { nextExperienceHammer = value; }
    }

    [SerializeField] private int totalExperienceHammer;
    public int TotalExperienceHammer
    {
        get { return totalExperienceHammer; }
        set { totalExperienceHammer = value;
              UpdateWeaponLevelAndNextExp(ref hammerLevel, ref earnedExperienceHammer, ref nextExperienceHammer, totalExperienceHammer, Constants.WeaponCategory.Hammer);
        }
    }

    [SerializeField] private int earnedExperienceStaff;
    public int EarnedExperienceStaff
    {
        get { return earnedExperienceStaff; }
        set { earnedExperienceStaff = value; }
    }

    [SerializeField] private int nextExperienceStaff;
    public int NextExperienceStaff
    {
        get { return nextExperienceStaff; }
        set { nextExperienceStaff = value; }
    }

    [SerializeField] private int totalExperienceStaff;
    public int TotalExperienceStaff
    {
        get { return totalExperienceStaff; }
        set { totalExperienceStaff = value;
              UpdateWeaponLevelAndNextExp(ref staffLevel, ref earnedExperienceStaff, ref nextExperienceStaff, totalExperienceStaff, Constants.WeaponCategory.Staff);
        }
    }

    [SerializeField] private int earnedExperienceShield;
    public int EarnedExperienceShield
    {
        get { return earnedExperienceShield; }
        set { earnedExperienceShield = value; }
    }

    [SerializeField] private int nextExperienceShield;
    public int NextExperienceShield
    {
        get { return nextExperienceShield; }
        set { nextExperienceShield = value; }
    }

    [SerializeField] private int totalExperienceShield;
    public int TotalExperienceShield
    {
        get { return totalExperienceShield; }
        set { totalExperienceShield = value;
              UpdateWeaponLevelAndNextExp(ref shieldLevel, ref earnedExperienceShield, ref nextExperienceShield, totalExperienceShield, Constants.WeaponCategory.Shield);
        }
    }
    #endregion

    // SP
    [SerializeField] private int sp;
    public int SP
    {
        get { return sp; }
        set { sp = value; }
    }

    // �ő�SP
    [SerializeField] private int maxSp;
    public int MaxSp
    {
        get { return maxSp; }
        set { maxSp = value; }
    }

    // �������X�L��
    [SerializeField] private List<Skill> equipedSkills;
    public List<Skill> EquipedSkills
    {
        get { return equipedSkills; }
        set { equipedSkills = value; }
    }

    // �R���X�g���N�^
    public Ally()
    {
        // �R���X�g���N�^���Ŕ񓯊��������s���Ȃ����߃��\�b�h�ŏ�����
        Initialize();
    }

    /// <summary>
    /// ������
    /// </summary>
    private async void Initialize()
    {
        // ClassManager�����p�\�ɂȂ�܂őҋ@
        while (ClassManager.Instance == null)
        {
            await UniTask.DelayFrame(1);
        }

        // ����������
        Level = 1;
        BaseMaxHp = 36;
        BaseMaxMp = 20;
        BaseStr = 15;
        BaseVit = 15;
        BaseDex = 15;
        BaseAgi = 15;
        BaseInt = 15;
        BaseMnd = 15;

        HP = 36;
        MP = 20;

        CharacterClass = ClassManager.Instance.GetClassByID("01"); // �E�H���A�[
        ClassLevels = new List<int> { 1, 1, 1, 1, 1, 1, 1, 1 };
        ClassEarnedExps = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 };
        ClassNextExps = new List<int> { 40, 40, 40, 40, 40, 40, 40, 40 };
        
        SwordLevel = 1;
        BladeLevel = 1;
        DaggerLevel = 1;
        SpearLevel = 1;
        AxLevel = 1;
        HammerLevel = 1;
        FistLevel = 1;
        BowLevel = 1;
        StaffLevel = 1;
        ShieldLevel = 1;

        EarnedExperienceSword = 0;
        TotalExperienceSword = 0;
        EarnedExperienceBlade = 0;
        TotalExperienceBlade = 0;
        EarnedExperienceDagger = 0;
        TotalExperienceDagger = 0;
        EarnedExperienceSpear = 0;
        TotalExperienceSpear = 0;
        EarnedExperienceAx = 0;
        TotalExperienceAx = 0;
        EarnedExperienceHammer = 0;
        TotalExperienceHammer = 0;
        EarnedExperienceFist = 0;
        TotalExperienceFist = 0;
        EarnedExperienceBow = 0;
        TotalExperienceBow = 0;
        EarnedExperienceStaff = 0;
        TotalExperienceStaff = 0;
        EarnedExperienceShield = 0;
        TotalExperienceShield = 0;

        LearnedSkills = new List<Skill>();
        EquipedSkills = new List<Skill>();
        effectSpriteObjects = new List<GameObject>(6);

        CalcStatus();
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="item"></param>
    /// <param name="index"></param>
    /// <param name="isEquipDummy"></param>
    public void Equip(Equip item, int index, bool isEquipDummy = false)
    {
        Unequip(index);
        if (item != null)
        {
            Constants.ItemCategory category = item.ItemCategory;
            switch (category)
            {
                case Constants.ItemCategory.Weapon:
                    var weapon = item as Weapon;
                    if (index == 0)
                    {
                        RightArm = weapon;
                    }
                    else if (index == 1)
                    {
                        LeftArm = weapon;
                    }
                    break;
                case Constants.ItemCategory.Head:
                    var h = item as Head;
                    Head = h;
                    break;
                case Constants.ItemCategory.Body:
                    var b = item as Body;
                    Body = b;
                    break;
                case Constants.ItemCategory.Accessary:
                    var a = item as Accessary;
                    if (index == 4)
                    {
                        Accessary1 = a;
                    }
                    else if (index == 5)
                    {
                        Accessary2 = a;
                    }
                    break;
            }
            if (!isEquipDummy)
            {
                item.EquippedAllyID = CharacterID;
                item.EquippedPart = index;
            }
            // �X�e�[�^�X�Čv�Z
            CalcStatus();
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="index"></param>
    /// <param name="isEquipDummy"></param>
    public void Unequip(int index, bool isEquipDummy = false)
    {
        switch (index)
        {
            case 0:
                if (RightArm != null)
                {
                    if (!isEquipDummy)
                    {
                        RightArm.EquippedAllyID = 0;
                    }
                    RightArm = null;
                }
                break;
            case 1:
                if (LeftArm != null)
                {
                    if (!isEquipDummy)
                    {
                        LeftArm.EquippedAllyID = 0;
                    }
                    LeftArm = null;
                }
                break;
            case 2:
                if (Head != null)
                {
                    if (!isEquipDummy)
                    {
                        Head.EquippedAllyID = 0;
                    }
                    Head = null;
                }
                break;
            case 3:
                if (Body != null)
                {
                    if (!isEquipDummy)
                    {
                        Body.EquippedAllyID = 0;
                    }
                    Body = null;
                }
                break;
            case 4:
                if (Accessary1 != null)
                {
                    if (!isEquipDummy)
                    {
                        Accessary1.EquippedAllyID = 0;
                    }
                    Accessary1 = null;
                }
                break;
            case 5:
                if (Accessary2 != null)
                {
                    if (!isEquipDummy)
                    {
                        Accessary2.EquippedAllyID = 0;
                    }
                    Accessary2 = null;
                }
                break;
            default:
                break;
        }
        // �X�e�[�^�X�Čv�Z
        CalcStatus();
    }

    /// <summary>
    /// �X�L������
    /// </summary>
    /// <param name="skill"></param>
    public async void EquipSkill(Skill skill)
    {
        if (skill.CanEquip(this))
        {
            equipedSkills.Add(skill);
            CalcStatus();

            sp += skill.SpCost;
            if (sp > maxSp)
            {
                sp = maxSp;
            }

            if (SPGauge != null)
            {
                var manager = SPGauge.GetComponent<GaugeManager>();
                if (manager != null)
                {
                    await manager.AnimateTextAndGauge(sp, 0.2f);
                }
            }
        }
    }

    /// <summary>
    /// �X�L����������
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public async UniTask UnEquipSkill(Skill skill)
    {
        if (equipedSkills.Contains(skill))
        {
            equipedSkills.Remove(skill);
            CalcStatus();
            sp -= skill.SpCost;
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

    /// <summary>
    /// �X�L���S����
    /// </summary>
    public void RemoveAllSkills()
    {
        foreach (Skill skill in LearnedSkills)
        {
            if (skill.IsEquipped)
            {
                skill.IsEquipped = false;
                sp = Math.Min(sp - skill.SpCost, 0);
            }
        }
    }

    /// <summary>
    /// �p�b�V�u�X�L�����ʓK�p
    /// </summary>
    private void ApplyPassiveSkillsEffect()
    {
        foreach (Skill skill in equipedSkills)
        {
            if (skill is PassiveSkill)
            {
                var p = skill as PassiveSkill;
                p.ApplyPassiveEffect(this);
            }
        }
    }

    /// <summary>
    /// �X�e�[�^�X�v�Z
    /// </summary>
    public void CalcStatus()
    {
        // ��b�X�e�[�^�X
        int index = Level - 1;

        BaseMaxHp = Constants.hpGrow[index];
        BaseMaxMp = Constants.mpGrow[index];
        BaseStr = Constants.prmGrow[index];
        BaseVit = Constants.prmGrow[index];
        BaseDex = Constants.prmGrow[index];
        BaseAgi = Constants.prmGrow[index];
        BaseInt = Constants.prmGrow[index];
        BaseMnd = Constants.prmGrow[index];
        maxSp = Constants.spGrow[index];

        // �N���X�␳
        MaxHp = (int)(BaseMaxHp * CharacterClass.hpRate);
        MaxMp = (int)(BaseMaxMp * CharacterClass.mpRate);
        Str = (int)(BaseStr * CharacterClass.strRate);
        Vit = (int)(BaseVit * CharacterClass.vitRate);
        Dex = (int)(BaseDex * CharacterClass.dexRate);
        Agi = (int)(BaseAgi * CharacterClass.agiRate);
        Int = (int)(BaseInt * CharacterClass.intRate);
        Mnd = (int)(maxSp * CharacterClass.mndRate);

        // ����
        MaxHp = MaxHp + (RightArm?.MaxHp ?? 0) + (LeftArm?.MaxHp ?? 0) + (Head?.MaxHp ?? 0) + (Body?.MaxHp ?? 0) + (Accessary1?.MaxHp ?? 0) + (Accessary2?.MaxHp ?? 0);
        MaxMp = MaxMp + (RightArm?.MaxMp ?? 0) + (LeftArm?.MaxMp ?? 0) + (Head?.MaxMp ?? 0) + (Body?.MaxMp ?? 0) + (Accessary1?.MaxMp ?? 0) + (Accessary2?.MaxMp ?? 0);
        Str = Str + (RightArm?.Str ?? 0) + (LeftArm?.Str ?? 0) + (Head?.Str ?? 0) + (Body?.Str ?? 0) + (Accessary1?.Str ?? 0) + (Accessary2?.Str ?? 0);
        Vit = Vit + (RightArm?.Vit ?? 0) + (LeftArm?.Vit ?? 0) + (Head?.Vit ?? 0) + (Body?.Vit ?? 0) + (Accessary1?.Vit ?? 0) + (Accessary2?.Vit ?? 0);
        Dex = Dex + (RightArm?.Dex ?? 0) + (LeftArm?.Dex ?? 0) + (Head?.Dex ?? 0) + (Body?.Dex ?? 0) + (Accessary1?.Dex ?? 0) + (Accessary2?.Dex ?? 0);
        Agi = Agi + (RightArm?.Agi ?? 0) + (LeftArm?.Agi ?? 0) + (Head?.Agi ?? 0) + (Body?.Agi ?? 0) + (Accessary1?.Agi ?? 0) + (Accessary2?.Agi ?? 0);
        Int = Int + (RightArm?.Int ?? 0) + (LeftArm?.Int ?? 0) + (Head?.Int ?? 0) + (Body?.Int ?? 0) + (Accessary1?.Int ?? 0) + (Accessary2?.Int ?? 0);
        Mnd = Mnd + (RightArm?.Mnd ?? 0) + (LeftArm?.Mnd ?? 0) + (Head?.Mnd ?? 0) + (Body?.Mnd ?? 0) + (Accessary1?.Mnd ?? 0) + (Accessary2?.Mnd ?? 0);

        // �p�b�V�u�X�L��
        ApplyPassiveSkillsEffect();

        // �����U���͈ˑ��l ����ɂ����STR or DEX or INT or MND���U���͂ɉ��Z
        int pAttackCorect = Str;
        int pAttackCorectLeft = 0;
        if (RightArm != null)
        {
            switch (RightArm.DependentStatus)
            {
                case Weapon.DependStatus.STR:
                    pAttackCorect = Str;
                    break;
                case Weapon.DependStatus.DEX:
                    pAttackCorect = Dex;
                    break;
                case Weapon.DependStatus.INT:
                    pAttackCorect = Int;
                    break;
                case Weapon.DependStatus.MND:
                    pAttackCorect = Mnd;
                    break;
            }
        }

        if (LeftArm != null)
        {
            if (LeftArm.WeaponCategory != Constants.WeaponCategory.Shield)
            {
                switch (LeftArm.DependentStatus)
                {
                    case Weapon.DependStatus.STR:
                        pAttackCorectLeft = Str;
                        break;
                    case Weapon.DependStatus.DEX:
                        pAttackCorectLeft = Dex;
                        break;
                    case Weapon.DependStatus.INT:
                        pAttackCorectLeft = Int;
                        break;
                    case Weapon.DependStatus.MND:
                        pAttackCorectLeft = Mnd;
                        break;
                }
            }
        }

        // �T�u�X�e�[�^�X
        PAttackLeft = pAttackCorectLeft + (LeftArm?.PAttack ?? 0);
        PAttack = pAttackCorect + (RightArm?.PAttack ?? 0) + (LeftArm?.PAttack ?? 0) + (Head?.PAttack ?? 0) + (Body?.PAttack ?? 0) + (Accessary1?.PAttack ?? 0) + (Accessary2?.PAttack ?? 0);
        MAttack = Int + (RightArm?.MAttack ?? 0) + (LeftArm?.MAttack ?? 0) + (Head?.MAttack ?? 0) + (Body?.MAttack ?? 0) + (Accessary1?.MAttack ?? 0) + (Accessary2?.MAttack ?? 0);
        PDefence = Vit + (RightArm?.PDefence ?? 0) + (LeftArm?.PDefence ?? 0) + (Head?.PDefence ?? 0) + (Body?.PDefence ?? 0) + (Accessary1?.PDefence ?? 0) + (Accessary2?.PDefence ?? 0);
        MDefence = Mnd / 2 + (RightArm?.MDefence ?? 0) + (LeftArm?.MDefence ?? 0) + (Head?.MDefence ?? 0) + (Body?.MDefence ?? 0) + (Accessary1?.MDefence ?? 0) + (Accessary2?.MDefence ?? 0);
        CriticalRate = CriticalRate + (RightArm?.CriticalRate ?? 0) + (LeftArm?.CriticalRate ?? 0) + (Head?.CriticalRate ?? 0) + (Body?.CriticalRate ?? 0) + (Accessary1?.CriticalRate ?? 0) + (Accessary2?.CriticalRate ?? 0);
        EvationRate = EvationRate + (RightArm?.EvationRate ?? 0) + (LeftArm?.EvationRate ?? 0) + (Head?.EvationRate ?? 0) + (Body?.EvationRate ?? 0) + (Accessary1?.EvationRate ?? 0) + (Accessary2?.EvationRate ?? 0);
        CounterRate = CounterRate + (RightArm?.CounterRate ?? 0) + (LeftArm?.CounterRate ?? 0) + (Head?.CounterRate ?? 0) + (Body?.CounterRate ?? 0) + (Accessary1?.CounterRate ?? 0) + (Accessary2?.CounterRate ?? 0);
        BlockRate = BlockRate + (RightArm?.BlockRate ?? 0) + (LeftArm?.BlockRate ?? 0) + (Head?.BlockRate ?? 0) + (Body?.BlockRate ?? 0) + (Accessary1?.BlockRate ?? 0) + (Accessary2?.BlockRate ?? 0);
        BlockReductionRate = BlockReductionRate + (RightArm?.BlockReductionRate ?? 0) + (LeftArm?.BlockReductionRate ?? 0) + (Head?.BlockReductionRate ?? 0) + (Body?.BlockReductionRate ?? 0) + (Accessary1?.BlockReductionRate ?? 0) + (Accessary2?.BlockReductionRate ?? 0);

        if (HP > MaxHp)
        {
            HP = MaxHp;
        }
        if (MP > MaxMp)
        {
            MP = MaxMp;
        }

        // �K�v�o���l
        int classIndex = ClassManager.Instance.AllClasses.IndexOf(CharacterClass);
        classNextExps[classIndex] = CalcurateNextExp();
    }

    /// <summary>
    /// �N���X�`�F���W
    /// </summary>
    /// <param name="newClass"></param>
    public void ChangeClass(Class newClass)
    {
        int index = ClassManager.Instance.AllClasses.IndexOf(newClass);
        // ���x���ݒ�
        Level = classLevels[index];
        // �N���X�ύX
        CharacterClass = newClass;

        // ��������
        for (int i = 0; i < 6; i++)
        {
            Unequip(i);
        }
        // �X�L���S����
        RemoveAllSkills();

        // �X�e�[�^�X�Čv�Z
        CalcStatus();
    }

    /// <summary>
    /// �o���l�l��
    /// </summary>
    /// <param name="exp"></param>
    /// <returns></returns>
    public async UniTask<int> GetExp(int exp)
    {
        int classIndex = ClassManager.Instance.AllClasses.IndexOf(CharacterClass);
        bool doesLevelUp = false;

        // �]��o���l = �l���όo���l + �l���o���l - �K�v�o���l
        int surplusExp = classEarnedExps[classIndex] + exp - classNextExps[classIndex];
        classEarnedExps[classIndex] += exp;

        if (surplusExp >= 0)
        {
            doesLevelUp = true;
            classEarnedExps[classIndex] = classNextExps[classIndex];
            totalExperience += exp - surplusExp;
        }
        else
        {
            totalExperience += exp;
        }

        var manager = EXPGauge.GetComponent<GaugeManager>();
        if (manager != null)
        {
            await manager.AnimateTextAndGauge(classEarnedExps[classIndex], 0.2f);
        }

        if (doesLevelUp)
        {
            LevelUp();
        }

        return surplusExp;
    }

    /// <summary>
    /// ���x���A�b�v
    /// </summary>
    public void LevelUp()
    {
        Level++;

        int index = ClassManager.Instance.AllClasses.IndexOf(CharacterClass);
        classLevels[index]++;

        // �X�L���K��
        if (CharacterClass.LearnSkills.Count >= Level)
        {
            Skill newSkill = CharacterClass.LearnSkills[Level - 1];
            LearnSkill(newSkill);
        }

        // �o���l���Z�b�g
        classEarnedExps[index] = 0;

        // �X�e�[�^�X�Čv�Z
        CalcStatus();
    }

    /// <summary>
    /// �X�L���K��
    /// </summary>
    /// <param name="skill"></param>
    public void LearnSkill(Skill skill)
    {
        if (skill != null)
        {
            // �N���[�����쐬
            //Skill newSkill = Instantiate(skill);
            var learnedSkillIDs =LearnedSkills.Select(s => s.ID).ToList();
            if (!learnedSkillIDs.Contains(skill.ID))
            {
                LearnedSkills.Add(skill);
            }
        }
    }

    /// <summary>
    /// ���̃��x���A�b�v�܂ł̌o���l���v�Z
    /// </summary>
    /// <returns></returns>
    public int CalcurateNextExp()
    {
        if (Constants.requiredExp.Length >= Level - 1)
        {
            // ��b�K�v�o���l
            var baseRequiredEXP = Constants.requiredExp[Level - 1];
            // ���̃��x���ɓ��B�ς݂̑��N���X���������邩
            var count = classLevels.Where(x => x >= Level + 1).ToList().Count;
            // ��b�l + ��b�l/2 * ���B�ςݑ��N���X�� (�琬�ς݃N���X�������قǕK�v�o���l��������)
            var newRequiredEXP = baseRequiredEXP + baseRequiredEXP / 2 * count;

            return newRequiredEXP;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// ���݂̃N���X�̊l���o���l
    /// </summary>
    /// <returns></returns>
    public int GetCurrentClassEarnedExp()
    {
        int index = ClassManager.Instance.AllClasses.IndexOf(CharacterClass);
        return classEarnedExps[index];
    }

    /// <summary>
    /// ���݂̃N���X�̕K�v�o���l
    /// </summary>
    /// <returns></returns>
    public int GetCurrentClassNextExp()
    {
        int index = ClassManager.Instance.AllClasses.IndexOf(CharacterClass);
        return classNextExps[index];
    }

    /// <summary>
    /// ��Ԉُ�E�X�e�[�^�X�ω��A�C�R���X�V
    /// </summary>
    public void UpdateStatusEffectSprites()
    {
        if (effectSpriteObjects == null)
        {
            effectSpriteObjects = new List<GameObject>();
        }

        int maxVisibleEffectCount = 6;
        int activateEffectCount = ActivateStatusEffects.Count;

        for (int i = 0; i < maxVisibleEffectCount; i++)
        {
            Image imageComponent = effectSpriteObjects[i].GetComponent<Image>();
            if (imageComponent != null)
            {
                if (i < activateEffectCount)
                {
                    imageComponent.enabled = true;
                    var effectName = ActivateStatusEffects[i];
                    imageComponent.sprite = CommonController.GetSpriteForEffect(effectName);
                }
                else
                {
                    imageComponent.enabled = false;
                }
            }
        }
    }

    public void GetWeaponExp(Constants.WeaponCategory weaponCategory, int value)
    {
        switch (weaponCategory)
        {
            case Constants.WeaponCategory.Sword:
                TotalExperienceSword += value;
                break;
            case Constants.WeaponCategory.Blade:
                TotalExperienceBlade += value;
                break;
            case Constants.WeaponCategory.Dagger:
                TotalExperienceDagger += value;
                break;
            case Constants.WeaponCategory.Spear:
                TotalExperienceSpear += value;
                break;
            case Constants.WeaponCategory.Ax:
                TotalExperienceAx += value;
                break;
            case Constants.WeaponCategory.Hammer:
                TotalExperienceHammer += value;
                break;
            case Constants.WeaponCategory.Fist:
                TotalExperienceFist += value;
                break;
            case Constants.WeaponCategory.Bow:
                TotalExperienceBow += value;
                break;
            case Constants.WeaponCategory.Staff:
                TotalExperienceStaff += value;
                break;
            case Constants.WeaponCategory.Shield:
                TotalExperienceShield += value;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ���탌�x���Ǝ��̃��x���܂ł̕K�v�o���l���X�V����
    /// </summary>
    /// <param name="level">����̃��x���̎Q��</param>
    /// <param name="nextExp">���̌o���l�̎Q��</param>
    /// <param name="totalExp">���v�o���l</param>
    private void UpdateWeaponLevelAndNextExp(ref int level, ref int earnedExp, ref int nextExp, int totalExp, Constants.WeaponCategory weaponCategory)
    {
        int newLevel = 0;
        int cumulativeExp = 0;

        for (int i = 0; i < Constants.requiredWeaponExp.Length; i++)
        {
            cumulativeExp = Constants.requiredWeaponExp[i + 1];

            if (totalExp < cumulativeExp)
            {
                newLevel = i + 1;
                earnedExp = totalExp - Constants.requiredWeaponExp[i];
                nextExp = Constants.requiredWeaponExp[i + 1] - Constants.requiredWeaponExp[i];
                break;
            }

            if (i == Constants.requiredWeaponExp.Length - 1)
            {
                // �Ō�̃��x���ɓ��B�����ꍇ
                newLevel = Constants.requiredWeaponExp.Length;
                earnedExp = 0;
                nextExp = 0;
            }
        }

        if (newLevel > level)
        {
            // ���x���A�b�v���ɃA�[�c���K��
            Skill learningArts = SkillManager.Instance.GetLearningArtsID(weaponCategory, newLevel);
            if (learningArts != null)
            {
                List<string> ids = LearnedSkills.Select(x => x.ID).ToList();
                if (!ids.Contains(learningArts.ID))
                {
                    LearnSkill(learningArts);
                }  
            }     
        }

        level = newLevel;

    }
}
