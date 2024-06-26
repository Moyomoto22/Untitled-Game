using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CI.QuickSave;
using Cysharp.Threading.Tasks;

public class SaveManager : MonoBehaviour
{
    // シングルトン
    public static SaveManager Instance { get; private set; }

    private GameObject player; // プレイヤーオブジェクト
    private PlayerData playerData;

    //セーブ設定
    QuickSaveSettings saveSettings;

    private void Awake()
    {
        // シングルトンインスタンスの設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでオブジェクトを保持

            // QuickSaveSettingsのインスタンスを作成
            saveSettings = new QuickSaveSettings();
            // 暗号化の方法 
            saveSettings.SecurityMode = SecurityMode.Aes;
            // Aesの暗号化キー
            saveSettings.Password = "Password";
            // 圧縮の方法
            saveSettings.CompressionMode = CompressionMode.Gzip;
        }
        else
        {
            Destroy(gameObject); // 重複するインスタンスを破棄
        }

        playerData = new PlayerData();
    }

    public async UniTask SaveGameAsync(int slot)
    {
        // FindObjectOfTypeはメインスレッドでしか実行できない
        //var lName = "";
        //Location l = FindObjectOfType<Location>();
        //if (l != null)
        //{
        //    lName = l.locationName;
        //}

       // await UniTask.RunOnThreadPool(() => SaveGame(slot, lName));
    }

    public void SaveGame(int slot)
    {
        var lName = "";
        Location l = FindObjectOfType<Location>();
        if (l != null)
        {
            lName = l.locationName;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerData = new PlayerData
        {
            // 現在時刻
            saveDateTime = DateTime.Now,

            // プレイ時間
            playTime = PlayTimeManager.Instance.GetPlayTimeForTimeSpan(),

            // シーン名
            currentScene = CommonVariableManager.GetCurrentSceneName(),
            
            // プレイヤー座標
            positionX = player.transform.position.x,
            positionY = player.transform.position.y,
            positionZ = player.transform.position.z,

            location = lName,

            // プレイヤーのステータスを保存
            partyMembers = ConvertAllyToSaveData(),

            // アイテムインベントリ
            items = ConvertItemInventoryToSaveData(),
        };

        // QuickSaveを使用してデータを保存
            QuickSaveWriter.Create("PlayerData" + slot)
            .Write("PlayerData", playerData)
            .Commit();

        // QuickSaveを使用してデータを保存
        QuickSaveWriter.Create("PlayerData" + slot).Write("PlayerData", playerData).Commit();

        string message = "スロット" + slot + "にセーブしました。";
        ToastMessageManager.Instance.ShowToastMessage(message);

        Debug.Log("Game Saved in Slot" + slot + ". Data Stored To:" + Application.persistentDataPath);
    }

    public async UniTask LoadGame(int slot)
    {
        CommonController.ResumeGame();
        
        playerData = new PlayerData();

        // QuickSaveを使用してデータを読み込み
        QuickSaveReader.Create("PlayerData" + slot).Read<PlayerData>("PlayerData", (r) => playerData = r);
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // 暗転
        await FadeController.Instance.FadeIn();

        MainMenuController mainMenuInstance = FindObjectOfType<MainMenuController>();

        if (mainMenuInstance != null)
        {
            await mainMenuInstance.CloseMenu();
        }

        if (playerData != null)
        {

            // プレイヤーの位置を復元
            Vector3 position = new Vector3(playerData.positionX, playerData.positionY, playerData.positionZ);
            //player.transform.position = position;

            await SceneController.Instance.TransitionToNextSceneNoFade(playerData.currentScene, position);

            // プレイヤーのステータスを復元
            List<Ally> restoredParty = RestorePlayerStatuses(playerData.partyMembers);
            PartyMembers.Instance.GetAlliesFromSavedData(restoredParty);

            List<Item> restoredItems = RestoreItemInventory(playerData.items);
            ItemInventory2.Instance.GetItemsFromSavedData(restoredItems);

            // プレイ時間を復元
            PlayTimeManager.Instance.Resume(playerData.playTime);

            Debug.Log("Game Loaded from Slot" + slot);

            await FadeController.Instance.FadeOut();
        }
        else
        {
            Debug.LogWarning("No save data found in Slot " + slot);
        }

        // 暗転解除
        
    }

    public PlayerData GetPlayerData(int slot)
    {
        playerData = new PlayerData();

        // QuickSaveを使用してデータを読み込み
        try
        {
            QuickSaveReader.Create("PlayerData" + slot).Read<PlayerData>("PlayerData", (r) => playerData = r);
            return playerData;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private List<PlayerStatusForSave> ConvertAllyToSaveData()
    {
        var partyMembers = PartyMembers.Instance.partyMembers;
        List<PlayerStatusForSave> data = new List<PlayerStatusForSave>();

        foreach (var member in partyMembers)
        {
            var saveData = new PlayerStatusForSave
            {
                characterID = member.CharacterID,
                characterName = member.CharacterName,
                maxHp = member.MaxHp,
                hp = member.HP,
                maxMp = member.MaxMp,
                mp = member.MP,

                tp = member.TP,
                str = member.Str,
                vit = member.Vit,
                dex = member.Dex,
                agi = member.Agi,
                inte = member.Int,
                mnd = member.Mnd,
                rightArm = member.RightArm?.ID,
                leftArm = member.LeftArm?.ID,
                head = member.Head?.ID,
                body = member.Body?.ID,
                accessary1 = member.Accessary1?.ID,
                accessary2 = member.Accessary2?.ID,
                learnedSkills = member.LearnedSkills.Select(skill => skill.ID).ToList(),
                classID = member.CharacterClass.ID,
                classLevels = new List<int>(member.ClassLevels),
                classEarnedExps = new List<int>(member.ClassEarnedExps),
                classNextExps = new List<int>(member.ClassNextExps),
                totalExperience = member.TotalExperience,
                totalLevel = member.TotalLevel,

                totalExperienceSword = member.TotalExperienceSword,
                totalExperienceBlade = member.TotalExperienceBlade,
                totalExperienceDagger = member.TotalExperienceDagger,
                totalExperienceSpear = member.TotalExperienceSpear,
                totalExperienceAx = member.TotalExperienceAx,
                totalExperienceHammer = member.TotalExperienceHammer,
                totalExperienceFist = member.TotalExperienceFist,
                totalExperienceBow = member.TotalExperienceBow,
                totalExperienceStaff = member.TotalExperienceStaff,
                totalExperienceShield = member.TotalExperienceShield,

                sp = member.SP,
                maxSp = member.MaxSp,
                equipedSkills = member.EquipedSkills.Select(skill => skill.ID).ToList()
            };

            data.Add(saveData);
        }
        return data;
    }

    private List<Ally> RestorePlayerStatuses(List<PlayerStatusForSave> savedData)
    {
        List<Ally> partyStatuses = new List<Ally>();

        foreach (var saveData in savedData)
        {
            var ally = new Ally
            {
                CharacterID = saveData.characterID,
                CharacterName = saveData.characterName,
                Level = saveData.level,
                MaxHp = saveData.maxHp,
                HP = saveData.hp,
                MaxMp = saveData.maxMp,
                MP = saveData.mp,
                TP = saveData.tp,
                Str = saveData.str,
                Vit = saveData.vit,
                Dex = saveData.dex,
                Agi = saveData.agi,
                Int = saveData.inte,
                Mnd = saveData.mnd,
                ClassLevels = new List<int>(saveData.classLevels),
                ClassEarnedExps = new List<int>(saveData.classEarnedExps),
                ClassNextExps = new List<int>(saveData.classNextExps),
                TotalExperience = saveData.totalExperience,
                TotalLevel = saveData.totalLevel,

                LearnedSkills = new List<Skill>(),
                EquipedSkills = new List<Skill>(),

                // 習得スキル復元後に武器経験値復元
                // (武器経験値セッターメソッドでアーツをLeanedSkillにAddするため)
                TotalExperienceSword = saveData.totalExperienceSword,
                TotalExperienceBlade = saveData.totalExperienceBlade,
                TotalExperienceDagger = saveData.totalExperienceDagger,
                TotalExperienceSpear = saveData.totalExperienceSpear,
                TotalExperienceAx = saveData.totalExperienceAx,
                TotalExperienceHammer = saveData.totalExperienceHammer,
                TotalExperienceFist = saveData.totalExperienceFist,
                TotalExperienceBow = saveData.totalExperienceBow,
                TotalExperienceStaff = saveData.totalExperienceStaff,
                TotalExperienceShield = saveData.totalExperienceShield,

                //SP = saveData.sp,
                MaxSp = saveData.maxSp           
            };

            // クラスを設定
            var allyClass = ClassManager.Instance.GetClassByID(saveData.classID);
            if (allyClass != null)
            {
                ally.ChangeClass(allyClass);
            }

            // 習得スキルと装備スキルを設定
            foreach (var skillID in saveData.learnedSkills)
            {
                var skill = SkillManager.Instance.GetSkillByID(skillID);
                if (skill != null)
                {
                    ally.LearnSkill(skill);

                    var equipedSkillIDs = ally.EquipedSkills.Select(s => s.ID).ToList();
                    if (saveData.equipedSkills.Contains(skillID) && !equipedSkillIDs.Contains(skillID))
                    {
                        ally.EquipSkill(skill);
                    }
                }
            }

            partyStatuses.Add(ally);
        }
        return partyStatuses;
    }


    private List<ItemForSave> ConvertItemInventoryToSaveData()
    {
        var itemInventory = ItemInventory2.Instance.items;
        var items = new List<ItemForSave>();

        foreach(var i in itemInventory)
        {
            var item = new ItemForSave(i.ID, i.EquippedAllyID, i.EquippedPart);
            items.Add(item);
        }

        return items;
    }

    /// <summary>
    /// セーブデータからアイテムインベントリを復元する
    /// </summary>
    /// <param name="savedItems"></param>
    /// <returns></returns>
    private List<Item> RestoreItemInventory(List<ItemForSave> savedItems)
    {
        List<Item> items = new List<Item>();

        foreach (var i in savedItems)
        {
            Item copiedItem = ItemInventory2.Instance.GetItemByID(i.ID);
            if (copiedItem != null)
            {
                //copiedItem.EquippedAllyID = i.equipedCharacterId;
                //copiedItem.EquippedPart = i.equipPartIndex;

                // 装備中キャラクターIDが1以上の場合、対象のキャラクターに装備
                if (i.equipedCharacterId > 0 && copiedItem is Equip)
                {
                    Equip equip = copiedItem as Equip;
                    Ally ally = PartyMembers.Instance.partyMembers[i.equipedCharacterId - 1];
                    ally.Equip(equip, copiedItem.EquippedPart);
                }
                items.Add(copiedItem);
            }
        }

        return items;
    }
}
