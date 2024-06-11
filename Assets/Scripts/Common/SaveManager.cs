using System;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;
using Cysharp.Threading.Tasks;

public class SaveManager : MonoBehaviour
{
    // �V���O���g��
    public static SaveManager Instance { get; private set; }

    private GameObject player; // �v���C���[�I�u�W�F�N�g
    private PlayerData playerData;

    //�Z�[�u�ݒ�
    QuickSaveSettings saveSettings;

    private void Awake()
    {
        // �V���O���g���C���X�^���X�̐ݒ�
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����ׂ��ŃI�u�W�F�N�g��ێ�

            // QuickSaveSettings�̃C���X�^���X���쐬
            saveSettings = new QuickSaveSettings();
            // �Í����̕��@ 
            saveSettings.SecurityMode = SecurityMode.Aes;
            // Aes�̈Í����L�[
            saveSettings.Password = "Password";
            // ���k�̕��@
            saveSettings.CompressionMode = CompressionMode.Gzip;
        }
        else
        {
            Destroy(gameObject); // �d������C���X�^���X��j��
        }

        playerData = new PlayerData();
    }

    public async UniTask SaveGameAsync(int slot)
    {
        // FindObjectOfType�̓��C���X���b�h�ł������s�ł��Ȃ�
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
            // ���ݎ���
            saveDateTime = DateTime.Now,

            // �v���C����
            playTime = PlayTimeManager.Instance.GetPlayTimeForTimeSpan(),

            // �V�[����
            currentScene = CommonVariableManager.GetCurrentSceneName(),
            
            // �v���C���[���W
            positionX = player.transform.position.x,
            positionY = player.transform.position.y,
            positionZ = player.transform.position.z,

            location = lName,

            // �v���C���[�̃X�e�[�^�X��ۑ�
            partyMembers = ConvertAllyStatusToSaveData(),

            // �A�C�e���C���x���g��
            items = ConvertItemInventoryToSaveData(),
        };

        // QuickSave���g�p���ăf�[�^��ۑ�
            QuickSaveWriter.Create("PlayerData" + slot)
            .Write("PlayerData", playerData)
            .Commit();

        // QuickSave���g�p���ăf�[�^��ۑ�
        QuickSaveWriter.Create("PlayerData" + slot).Write("PlayerData", playerData).Commit();

        Debug.Log("Game Saved in Slot" + slot + ". Data Stored To:" + Application.persistentDataPath);
    }

    public async UniTask LoadGame(int slot)
    {
        CommonController.ResumeGame();
        
        playerData = new PlayerData();

        // QuickSave���g�p���ăf�[�^��ǂݍ���
        QuickSaveReader.Create("PlayerData" + slot).Read<PlayerData>("PlayerData", (r) => playerData = r);
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // �Ó]
        await FadeController.Instance.FadeIn();

        MainMenuController mainMenuInstance = FindObjectOfType<MainMenuController>();

        if (mainMenuInstance != null)
        {
            await mainMenuInstance.CloseMenu();
        }

        if (playerData != null)
        {

            // �v���C���[�̈ʒu�𕜌�
            Vector3 position = new Vector3(playerData.positionX, playerData.positionY, playerData.positionZ);
            //player.transform.position = position;

            await SceneController.Instance.TransitionToNextSceneNoFade(playerData.currentScene, position);

            // �v���C���[�̃X�e�[�^�X�𕜌�
            List<AllyStatus> restoredParty = RestorePlayerStatuses(playerData.partyMembers);
            PartyMembers.Instance.GetAlliesFromSavedData(restoredParty);

            List<Item> restoredItems = RestoreItemInventory(playerData.items);
            ItemInventory2.Instance.GetItemsFromSavedData(restoredItems);

            // �v���C���Ԃ𕜌�
            PlayTimeManager.Instance.Resume(playerData.playTime);

            Debug.Log("Game Loaded from Slot" + slot);

            await FadeController.Instance.FadeOut();
        }
        else
        {
            Debug.LogWarning("No save data found in Slot " + slot);
        }

        // �Ó]����
        
    }

    public PlayerData GetPlayerData(int slot)
    {
        playerData = new PlayerData();

        // QuickSave���g�p���ăf�[�^��ǂݍ���
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

    private List<PlayerStatusForSave> ConvertAllyStatusToSaveData()
    {
        var partyMembers = PartyMembers.Instance.partyMembers;
        List<PlayerStatusForSave> data = new List<PlayerStatusForSave>();

        foreach (var m in partyMembers)
        {
            PlayerStatusForSave d = new PlayerStatusForSave();
            d.characterID = m.CharacterID;
            d.characterName = m.characterName;
            d.maxHp = m.maxHp;
            d.hp = m.hp;
            d.maxMp = m.maxMp;
            d.mp = m.mp;
            d.maxTp = m.maxTp;
            d.tp = m.tp;
            d.str = m.str;
            d.vit = m.vit;
            d.dex = m.dex;
            d.agi = m.agi;
            d.inte = m.inte;
            d.mnd = m.mnd;

            d.rightArm = m.rightArm != null ? m.rightArm.ID : null;
            d.leftArm = m.leftArm != null ? m.leftArm.ID : null;
            d.head = m.head != null ? m.head.ID : null;
            d.body = m.body != null ? m.body.ID : null;
            d.accessary1 = m.accessary1 != null ? m.accessary1.ID : null;
            d.accessary2 = m.accessary2 != null ? m.accessary2.ID : null;

            d.learnedSkills = new List<string>();
            foreach (var skill in m.learnedSkills)
            {
                d.learnedSkills.Add(skill.ID);
            }

            d.classID = m.Class.ID;

            d.classLevels = m.classLevels;
            d.classEarnedExps = m.classEarnedExps;
            d.classNextExps = m.classNextExps;

            d.totalExperience = m.totalExperience;
            d.totalLevel = m.totalLevel;

            d.sp = m.sp;
            d.maxSp = m.maxSp;

            d.equipedSkills = new List<string>();
            foreach (var skill in m.equipedSkills)
            {
                d.equipedSkills.Add(skill.ID);
            }

            data.Add(d);
        }
        return data;
    }

    private List<AllyStatus> RestorePlayerStatuses(List<PlayerStatusForSave> savedData)
    {
        List<AllyStatus> partyStatuses = new List<AllyStatus>();

        foreach(var m in savedData)
        {
            AllyStatus d = new AllyStatus();
            d.CharacterID = m.characterID;
            d.characterName = m.characterName;
            d.level = m.level;
            d.maxHp = m.maxHp;
            d.hp = m.hp;
            d.maxMp = m.maxMp;
            d.mp = m.mp;
            d.maxTp = m.maxTp;
            d.tp = m.tp;
            d.str = m.str;
            d.vit = m.vit;
            d.dex = m.dex;
            d.agi = m.agi;
            d.inte = m.inte;
            d.mnd = m.mnd;

            // ������RestoreItemInventory�ŕ�������

            d.classLevels = new List<int>(m.classLevels);
            d.classEarnedExps = new List<int>(m.classEarnedExps);
            d.classNextExps = new List<int>(m.classNextExps);

            d.totalExperience = m.totalExperience;
            d.totalLevel = m.totalLevel;

            d.learnedSkills = new List<Skill>();
            d.equipedSkills = new List<Skill>();

            // �N���X
            Class cl = ClassManager.Instance.GetClassByID(m.classID);
            if (cl != null)
            {
                d.ChangeClass(cl);
            }

            // �K���X�L��
            foreach (var skillID in m.learnedSkills)
            {
                Skill skill = SkillManager.Instance.GetSkillByID(skillID);
                if (skill != null)
                {
                    d.LearnSkill(skill);

                    // �X�L��ID���������X�L���Ɋ܂܂��ꍇ�A�X�L������
                    if (m.equipedSkills.Contains(skillID))
                    {
                        d.EquipSkill(skill);
                    }

                } 
            }            

            d.sp = m.sp;
            d.maxSp = m.maxSp;

            partyStatuses.Add(d);
        }
        return partyStatuses;
    }

    private List<ItemForSave> ConvertItemInventoryToSaveData()
    {
        var itemInventory = ItemInventory2.Instance.items;
        var items = new List<ItemForSave>();

        foreach(var i in itemInventory)
        {
            var item = new ItemForSave(i.ID, i.equippedAllyID, i.equippedPart);
            items.Add(item);
        }

        return items;
    }

    private List<Item> RestoreItemInventory(List<ItemForSave> savedItems)
    {
        List<Item> items = new List<Item>();

        foreach (var i in savedItems)
        {
            Item item = ItemManager.Instance.GetItemByID(i.ID);
            if (item != null)
            {
                item.equippedAllyID = i.equipedCharacterId;
                item.equippedPart = i.equipPartIndex;

                // �������L�����N�^�[ID��1�ȏ�̏ꍇ�A�Ώۂ̃L�����N�^�[�ɑ���
                if (item.equippedAllyID > 0 && item is Equip) {

                    Equip equip = item as Equip;
                    AllyStatus ally = PartyMembers.Instance.partyMembers[item.equippedAllyID - 1];
                    ally.Equip(equip, item.equippedPart);

                }

                items.Add(item);
            }

        }

        return items;
    }
}
