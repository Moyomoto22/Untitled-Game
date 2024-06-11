using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMembers: MonoBehaviour
{ 

    private static PartyMembers instance;

    public static PartyMembers Instance { get; private set; }

    private void Start()
    {
        
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            partyMembers = new List<AllyStatus>();
            Initialize();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // パーティメンバー
    public List<AllyStatus> partyMembers { get; private set; }

    /// <summary>
    /// Addressableからパーティメンバーを取得する
    /// </summary>
    private async UniTask GetAlliesFromAddressables()
    {
        for (int i = 1; i < 5; i++)
        {
            
            var originalAlly = await CommonController.GetAllyStatus(i);
            var allyInstance = CopyAllyStatus(originalAlly);

            if (allyInstance != null)
            {
                AddCharacterToParty(allyInstance);
            }
            else
            {
                Debug.LogError("Failed to load ally status");
            }
        }
    }

    /// <summary>
    /// セーブデータからパーティメンバーを取得する
    /// </summary>
    public void GetAlliesFromSavedData(List<AllyStatus> loadedData)
    {

        RemoveAllAllies();

        for (int i = 0; i < 4; i++)
        {
            var allyInstance = loadedData[i];

            if (allyInstance != null)
            {
                AddCharacterToParty(allyInstance);
            }
            else
            {
                Debug.LogError("Failed to load ally status");
            }
        }
    }

    /// <summary>
    /// AllyStatusのディープコピーを作成する
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public AllyStatus CopyAllyStatus(AllyStatus original)
    {
        // オリジナルのAllyStatusをJSONにシリアライズ
        string json = JsonUtility.ToJson(original);

        // JSONから新しいAllyStatusをデシリアライズ
        var newInstance = ScriptableObject.CreateInstance<AllyStatus>();
        JsonUtility.FromJsonOverwrite(json, newInstance);

        return newInstance;
    }


    // プレイヤーをターンに設定
    public void AddCharacterToParty(AllyStatus ally)
    {
        partyMembers.Add(ally);
    }

    public async UniTaskVoid Initialize()
    {
        RemoveAllAllies();
        await GetAlliesFromAddressables();
    }



    public AllyStatus GetAllyByIndex(int index)
    {
        if (partyMembers != null)
        {
            if (index >= 0 && index < partyMembers.Count)
            {
                return partyMembers[index];
            }
            return null;
        }
        return null;
    }

    public List<AllyStatus> GetAllies()
    {
        List<AllyStatus> allies = new List<AllyStatus>();
        foreach (var ally in partyMembers)
        {
            if (ally != null)
            {
                allies.Add(ally);
            }
        }
        return allies;
    }

    private void RemoveAllAllies()
    {
        if (partyMembers != null)
        {
            partyMembers.Clear();
        }
    }
}
