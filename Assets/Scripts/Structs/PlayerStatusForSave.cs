using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// セーブデータ用味方ステータスデータ
public class PlayerStatusForSave
{
    //　キャラクターの名前
    public string characterName;
    // ID
    public int characterID;
    //　キャラクターのレベル
    public int level;
    //　最大HP
    public int maxHp;
    //　HP    
    public int hp;
    //　最大MP   
    public int maxMp;
    //　MP    
    public int mp;
    //　最大TP    
    public int maxTp;
    //　TP    
    public int tp;
    //　STR    
    public int str;
    //　VIT    
    public int vit;
    //　DEX    
    public int dex;
    //　AGI
    public int agi;
    //　INT
    public int inte;
    //　MND
    public int mnd;

    // 装備はアイコン画像を持ち保存不可のため、IDから復元
    // 右手    
    public string rightArm;
    // 左手    
    public string leftArm;
    // 頭    
    public string head;
    // 胴    
    public string body;
    // 装飾品1    
    public string accessary1;
    // 装飾品2    
    public string accessary2;

    // スキルはアイコン画像を持ち保存不可のため、IDから復元
    // 習得済みスキル
    public List<string> learnedSkills;
    // 装備中スキル
    public List<string> equipedSkills;

    // クラスID
    // クラスはキャラクター画像を持ち保存不可のため、IDから復元
    public string classID;

    // クラス別レベル・経験値
    public List<int> classLevels;
    public List<int> classEarnedExps;
    public List<int> classNextExps;

    //　累計経験値
    public int totalExperience;
    public int totalLevel;

    //　片手剣レベル    
    public int swordLevel;
    //　両手剣レベル    
    public int bladeLevel;
    //　短剣レベル    
    public int daggerLevel;
    //　槍レベル    
    public int spearLevel;
    //　斧レベル    
    public int axLevel;
    //　弓レベル    
    public int bowLevel;
    //　拳レベル    
    public int fistLevel;
    //　槌レベル    
    public int hammerLevel;
    //　杖レベル    
    public int staffLevel;
    //　盾レベル    
    public int shieldLevel;

    //　獲得経験値 片手剣    
    public int earnedExperienceSword;
    //　必要経験値 片手剣    
    public int nextExperienceSword;
    //　累計経験値 片手剣    
    public int totalExperienceSword;
    //　獲得経験値    
    public int earnedExperienceBlade;
    //　必要経験値    
    public int nextExperienceBlade;
    //　累計経験値    
    public int totalExperienceBlade;
    //　獲得経験値    
    public int earnedExperienceDagger;
    //　必要経験値    
    public int nextExperienceDagger;
    //　累計経験値    
    public int totalExperienceDagger;
    //　獲得経験値    
    public int earnedExperienceSpear;
    //　必要経験値    
    public int nextExperienceSpear;
    //　累計経験値    
    public int totalExperienceSpear;
    //　獲得経験値    
    public int earnedExperienceAx;
    //　必要経験値    
    public int nextExperienceAx;
    //　累計経験値    
    public int totalExperienceAx;
    //　獲得経験値    
    public int earnedExperienceBow;
    //　必要経験値    
    public int nextExperienceBow;
    //　累計経験値    
    public int totalExperienceBow;
    //　獲得経験値    
    public int earnedExperienceFist;
    //　必要経験値    
    public int nextExperienceFist;
    //　累計経験値    
    public int totalExperienceFist;
    //　獲得経験値    
    public int earnedExperienceHammer;
    //　必要経験値    
    public int nextExperienceHammer;
    //　累計経験値    
    public int totalExperienceHammer;
    //　獲得経験値    
    public int earnedExperienceStaff;
    //　必要経験値    
    public int nextExperienceStaff;
    //　累計経験値    
    public int totalExperienceStaff;
    //　獲得経験値    
    public int earnedExperienceShield;
    //　必要経験値    
    public int nextExperienceShield;
    //　累計経験値    
    public int totalExperienceShield;

    // SP
    public int sp;
    //最大SP
    public int maxSp;
}
