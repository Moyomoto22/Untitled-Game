using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Z�[�u�f�[�^�p�����X�e�[�^�X�f�[�^
public class PlayerStatusForSave
{
    //�@�L�����N�^�[�̖��O
    public string characterName;
    // ID
    public int characterID;
    //�@�L�����N�^�[�̃��x��
    public int level;
    //�@�ő�HP
    public int maxHp;
    //�@HP    
    public int hp;
    //�@�ő�MP   
    public int maxMp;
    //�@MP    
    public int mp;
    //�@�ő�TP    
    public int maxTp;
    //�@TP    
    public int tp;
    //�@STR    
    public int str;
    //�@VIT    
    public int vit;
    //�@DEX    
    public int dex;
    //�@AGI
    public int agi;
    //�@INT
    public int inte;
    //�@MND
    public int mnd;

    // �����̓A�C�R���摜�������ۑ��s�̂��߁AID���畜��
    // �E��    
    public string rightArm;
    // ����    
    public string leftArm;
    // ��    
    public string head;
    // ��    
    public string body;
    // �����i1    
    public string accessary1;
    // �����i2    
    public string accessary2;

    // �X�L���̓A�C�R���摜�������ۑ��s�̂��߁AID���畜��
    // �K���ς݃X�L��
    public List<string> learnedSkills;
    // �������X�L��
    public List<string> equipedSkills;

    // �N���XID
    // �N���X�̓L�����N�^�[�摜�������ۑ��s�̂��߁AID���畜��
    public string classID;

    // �N���X�ʃ��x���E�o���l
    public List<int> classLevels;
    public List<int> classEarnedExps;
    public List<int> classNextExps;

    //�@�݌v�o���l
    public int totalExperience;
    public int totalLevel;

    //�@�Ў茕���x��    
    public int swordLevel;
    //�@���茕���x��    
    public int bladeLevel;
    //�@�Z�����x��    
    public int daggerLevel;
    //�@�����x��    
    public int spearLevel;
    //�@�����x��    
    public int axLevel;
    //�@�|���x��    
    public int bowLevel;
    //�@�����x��    
    public int fistLevel;
    //�@�ƃ��x��    
    public int hammerLevel;
    //�@�񃌃x��    
    public int staffLevel;
    //�@�����x��    
    public int shieldLevel;

    //�@�l���o���l �Ў茕    
    public int earnedExperienceSword;
    //�@�K�v�o���l �Ў茕    
    public int nextExperienceSword;
    //�@�݌v�o���l �Ў茕    
    public int totalExperienceSword;
    //�@�l���o���l    
    public int earnedExperienceBlade;
    //�@�K�v�o���l    
    public int nextExperienceBlade;
    //�@�݌v�o���l    
    public int totalExperienceBlade;
    //�@�l���o���l    
    public int earnedExperienceDagger;
    //�@�K�v�o���l    
    public int nextExperienceDagger;
    //�@�݌v�o���l    
    public int totalExperienceDagger;
    //�@�l���o���l    
    public int earnedExperienceSpear;
    //�@�K�v�o���l    
    public int nextExperienceSpear;
    //�@�݌v�o���l    
    public int totalExperienceSpear;
    //�@�l���o���l    
    public int earnedExperienceAx;
    //�@�K�v�o���l    
    public int nextExperienceAx;
    //�@�݌v�o���l    
    public int totalExperienceAx;
    //�@�l���o���l    
    public int earnedExperienceBow;
    //�@�K�v�o���l    
    public int nextExperienceBow;
    //�@�݌v�o���l    
    public int totalExperienceBow;
    //�@�l���o���l    
    public int earnedExperienceFist;
    //�@�K�v�o���l    
    public int nextExperienceFist;
    //�@�݌v�o���l    
    public int totalExperienceFist;
    //�@�l���o���l    
    public int earnedExperienceHammer;
    //�@�K�v�o���l    
    public int nextExperienceHammer;
    //�@�݌v�o���l    
    public int totalExperienceHammer;
    //�@�l���o���l    
    public int earnedExperienceStaff;
    //�@�K�v�o���l    
    public int nextExperienceStaff;
    //�@�݌v�o���l    
    public int totalExperienceStaff;
    //�@�l���o���l    
    public int earnedExperienceShield;
    //�@�K�v�o���l    
    public int nextExperienceShield;
    //�@�݌v�o���l    
    public int totalExperienceShield;

    // SP
    public int sp;
    //�ő�SP
    public int maxSp;
}
