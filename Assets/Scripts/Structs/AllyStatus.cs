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
    //�@�L�����N�^�[�̃C���[�W
    [SerializeField]
    public Sprite characterImage;
    // �J���[
    [SerializeField]
    public Color color;
    // ID
    [SerializeField]
    public int CharacterID;
    //�@�N���X
    [SerializeField]
    public Class Class;
    //�@�݌v�o���l
    public int totalExperience;
    public int totalLevel;

    // �����U����(��)
    [SerializeField]
    public int pAttackLeft;

    #region �N���X�ʃ��x���E�o���l
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

    #region ���탌�x��
    //�@�Ў茕���x��
    [SerializeField]
    public int swordLevel;
    //�@���茕���x��
    [SerializeField]
    public int bladeLevel;
    //�@�Z�����x��
    [SerializeField]
    public int daggerLevel;
    //�@�����x��
    [SerializeField]
    public int spearLevel;
    //�@�����x��
    [SerializeField]
    public int axLevel;
    //�@�|���x��
    [SerializeField]
    public int bowLevel;
    //�@�����x��
    [SerializeField]
    public int fistLevel;
    //�@�ƃ��x��
    [SerializeField]
    public int hammerLevel;
    //�@�񃌃x��
    [SerializeField]
    public int staffLevel;
    //�@�����x��
    [SerializeField]
    public int shieldLevel;
    #endregion

    #region ����o���l
    //�@�l���o���l �Ў茕
    [SerializeField]
    public int earnedExperienceSword;
    //�@�K�v�o���l �Ў茕
    [SerializeField]
    public int nextExperienceSword;
    //�@�݌v�o���l �Ў茕
    [SerializeField]
    public int totalExperienceSword;
    //�@�l���o���l
    [SerializeField]
    public int earnedExperienceBlade;
    //�@�K�v�o���l
    [SerializeField]
    public int nextExperienceBlade;
    //�@�݌v�o���l
    [SerializeField]
    public int totalExperienceBlade;
    //�@�l���o���l
    [SerializeField]
    public int earnedExperienceDagger;
    //�@�K�v�o���l
    [SerializeField]
    public int nextExperienceDagger;
    //�@�݌v�o���l
    [SerializeField]
    public int totalExperienceDagger;
    //�@�l���o���l
    [SerializeField]
    public int earnedExperienceSpear;
    //�@�K�v�o���l
    [SerializeField]
    public int nextExperienceSpear;
    //�@�݌v�o���l
    [SerializeField]
    public int totalExperienceSpear;
    //�@�l���o���l
    [SerializeField]
    public int earnedExperienceAx;
    //�@�K�v�o���l
    [SerializeField]
    public int nextExperienceAx;
    //�@�݌v�o���l
    [SerializeField]
    public int totalExperienceAx;
    //�@�l���o���l
    [SerializeField]
    public int earnedExperienceBow;
    //�@�K�v�o���l
    [SerializeField]
    public int nextExperienceBow;
    //�@�݌v�o���l
    [SerializeField]
    public int totalExperienceBow;
    //�@�l���o���l
    [SerializeField]
    public int earnedExperienceFist;
    //�@�K�v�o���l
    [SerializeField]
    public int nextExperienceFist;
    //�@�݌v�o���l
    [SerializeField]
    public int totalExperienceFist;
    //�@�l���o���l
    [SerializeField]
    public int earnedExperienceHammer;
    //�@�K�v�o���l
    [SerializeField]
    public int nextExperienceHammer;
    //�@�݌v�o���l
    [SerializeField]
    public int totalExperienceHammer;
    //�@�l���o���l
    [SerializeField]
    public int earnedExperienceStaff;
    //�@�K�v�o���l
    [SerializeField]
    public int nextExperienceStaff;
    //�@�݌v�o���l
    [SerializeField]
    public int totalExperienceStaff;
    //�@�l���o���l
    [SerializeField]
    public int earnedExperienceShield;
    //�@�K�v�o���l
    [SerializeField]
    public int nextExperienceShield;
    //�@�݌v�o���l
    [SerializeField]
    public int totalExperienceShield;

    #endregion

    // SP
    public int sp;
    //�ő�SP
    public int maxSp;
    // �������X�L��
    public List<Skill> equipedSkills;

    /// <summary>
    /// �w�蕔�ʂɃA�C�e���𑕔�
    /// </summary>
    /// <param name="item"></param>
    /// <param name="index">0�F�E�� 1:���� 2:�� 3:�� 4:�����i1 5:�����i2</param>
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
                item.equippedPart = index;
            }
            // �X�e�[�^�X�Čv�Z
            CalcStatus();
        }
    }

    /// <summary>
    /// �w�蕔�ʂ̑�������
    /// </summary>
    /// <param name="item"></param>
    /// <param name="index">0�F�E�� 1:���� 2:�� 3:�� 4:�����i1 5:�����i2</param>
    public void Unequip(int index, bool isEquipDummy = false)
    {
        Item item = null;

        switch (index)
        {
            // �E��
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
            // ����
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
            // ��
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
            // ��
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
            // �����i1
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
            // �����i2
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
        
        // �X�e�[�^�X�Čv�Z
        CalcStatus();

    }

    /// <summary>
    /// �X�L���𑕔�����
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
    /// �X�L�����͂���
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
    /// �A�C�e���������\���`�F�b�N����
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
                // �E��
                case 0:
                    Weapon weapon = equip as Weapon;
                    if (weapon != null)
                    {
                        // �E��ɏ��͑����s��
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
                // ����
                case 1:
                    Weapon weapon2 = equip as Weapon;
                    if (weapon2 != null)
                    {
                        // ����͊�{���̂�
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
                // ���E��
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
                // �����i�P�E�Q
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
    /// ������X�L�����l�������X�e�[�^�X���Čv�Z����
    /// </summary>
    public void CalcStatus()
    {
        // ��b�X�e�[�^�X
        int index = level - 1;

        maxHp = Constants.hpGrow[index];
        maxMp = Constants.mpGrow[index];
        str = Constants.prmGrow[index];
        vit = Constants.prmGrow[index];
        dex = Constants.prmGrow[index];
        agi = Constants.prmGrow[index];
        inte = Constants.prmGrow[index];
        mnd = Constants.prmGrow[index];
        maxSp = Constants.spGrow[index];

        // �N���X�␳
        maxHp2 = (int)(maxHp * Class.hpRate);
        maxMp2 = (int)(maxMp * Class.mpRate);
        str2 = (int)(str * Class.strRate);
        vit2 = (int)(vit * Class.vitRate);
        dex2 = (int)(dex * Class.dexRate);
        agi2 = (int)(agi * Class.agiRate);
        inte2 = (int)(inte * Class.intRate);
        mnd2 = (int)(mnd * Class.mndRate);

        // ��{�X�e�[�^�X
        maxHp2 = maxHp2 + (rightArm?.maxHp ?? 0) + (leftArm?.maxHp ?? 0) + (head?.maxHp ?? 0) + (body?.maxHp ?? 0) + (accessary1?.maxHp ?? 0) + (accessary2?.maxHp ?? 0);
        maxMp2 = maxMp2 + (rightArm?.maxMp ?? 0) + (leftArm?.maxMp ?? 0) + (head?.maxMp ?? 0) + (body?.maxMp ?? 0) + (accessary1?.maxMp ?? 0) + (accessary2?.maxMp ?? 0);
        str2 = str2 + (rightArm?.str ?? 0) + (leftArm?.str ?? 0) + (head?.str ?? 0) + (body?.str ?? 0) + (accessary1?.str ?? 0) + (accessary2?.str ?? 0);
        vit2 = vit2 + (rightArm?.vit ?? 0) + (leftArm?.vit ?? 0) + (head?.vit ?? 0) + (body?.vit ?? 0) + (accessary1?.vit ?? 0) + (accessary2?.vit ?? 0);
        dex2 = dex2 + (rightArm?.dex ?? 0) + (leftArm?.dex ?? 0) + (head?.dex ?? 0) + (body?.dex ?? 0) + (accessary1?.dex ?? 0) + (accessary2?.dex ?? 0);
        agi2 = agi2 + (rightArm?.agi ?? 0) + (leftArm?.agi ?? 0) + (head?.agi ?? 0) + (body?.agi ?? 0) + (accessary1?.agi ?? 0) + (accessary2?.agi ?? 0);
        inte2 = inte2 + (rightArm?.inte ?? 0) + (leftArm?.inte ?? 0) + (head?.inte ?? 0) + (body?.inte ?? 0) + (accessary1?.inte ?? 0) + (accessary2?.inte ?? 0);
        mnd2 = mnd2 + (rightArm?.mnd ?? 0) + (leftArm?.mnd ?? 0) + (head?.mnd ?? 0) + (body?.mnd ?? 0) + (accessary1?.mnd ?? 0) + (accessary2?.mnd ?? 0);

        // �p�b�V�u�X�L��
        ApplyPassiveSkillsEffect();

        // �����U���͈ˑ��l ����ɂ����STR or DEX or INT or MND���U���͂ɉ��Z
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

        // �T�u�X�e�[�^�X
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

        // �K�v�o���l
        int classIndex = ClassManager.Instance.AllClasses.IndexOf(Class);
        classNextExps[classIndex] = CalcurateNextExp();
    }

    /// <summary>
    /// �X�L���𒅒E����
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
    /// �������̃X�L�������ׂĊO��
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
    /// �X�L���R�X�g������
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
    /// �N���X�`�F���W
    /// </summary>
    public void ChangeClass(Class newClass)
    {
        int index = ClassManager.Instance.AllClasses.IndexOf(newClass);
        // ���x���ݒ�
        level = classLevels[index];
        // �N���X�ύX
        Class = newClass;

        // ��������
        for (int i = 0; i < 6; i++)
        {
            Unequip(i);
        }
        // �X�L���S����
        RemoveAllSkills();
        
        // �摜�ݒ�
        if (CharacterID > 0)
        {
           // characterImage = newClass.imagesA[CharacterID - 1];
        }

        // �X�e�[�^�X�Čv�Z
        CalcStatus();

    }

    public async UniTask<int> GetExp(int exp)
    {
        int classIndex = ClassManager.Instance.AllClasses.IndexOf(Class);
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

    public void LevelUp()
    {
        level++;

        int index = ClassManager.Instance.AllClasses.IndexOf(Class);
        classLevels[index]++;

        // �X�L���K��
        if (Class.LearnSkills.Count >= level)
        {
            Skill newSkill = Class.LearnSkills[level - 1];
            LearnSkill(newSkill);
        }

        // �o���l���Z�b�g
        classEarnedExps[index] = 0;

        // �X�e�[�^�X�Čv�Z
        CalcStatus();
    }

    /// <summary>
    /// �X�L�����K������
    /// </summary>
    /// <param name="skill"></param>
    public void LearnSkill(Skill skill)
    {
        if (skill != null)
        {
            // �N���[�����쐬
            Skill newSkill = Instantiate(skill);
            if (!learnedSkills.Contains(newSkill))
            {
                learnedSkills.Add(newSkill);
            }
        }
    }

    /// <summary>
    /// ���̃��x���܂ł̕K�v�o���l���v�Z����
    /// </summary>
    public int CalcurateNextExp()
    {
        if (Constants.requiredExp.Length >= level - 1)
        {
            // ��b�K�v�o���l
            var baseRequiredEXP = Constants.requiredExp[level - 1];
            // ���̃��x���ɓ��B�ς݂̑��N���X���������邩
            var count = classLevels.Where(x => x >= level + 1).ToList().Count;
            // ��b�l + ��b�l/2 * ���B�ςݑ��N���X�� (�琬�ς݃N���X�������قǕK�v�o���l��������)
            var newRequiredEXP = baseRequiredEXP + baseRequiredEXP / 2 * count;

            return newRequiredEXP;
        }
        else
        {
            return 0;
        }
    }

    public int GetCurrentClassEarnedExp()
    {
        int index = ClassManager.Instance.AllClasses.IndexOf(Class);
        return classEarnedExps[index];
    }

    public int GetCurrentClassNextExp()
    {
        int index = ClassManager.Instance.AllClasses.IndexOf(Class);
        return classNextExps[index];
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