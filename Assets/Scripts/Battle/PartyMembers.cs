using Cysharp.Threading.Tasks;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PartyMembers: MonoBehaviour
{
    const int numberOfPartyMembers = 4;
    
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
            partyMembers = new List<Ally>();
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // パーティメンバー
    public List<Ally> partyMembers { get; private set; }

    /// <summary>
    /// Addressableからパーティメンバーを取得する
    /// </summary>
    private async UniTask GetAlliesFromAddressables()
    {
        for (int i = 1; i < 5; i++)
        {
            
            var originalAlly = await CommonController.GetAlly(i);
            var allyInstance = CopyAlly(originalAlly);

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
    public void GetAlliesFromSavedData(List<Ally> loadedData)
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
    /// Allyのディープコピーを作成する
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public Ally CopyAlly(Ally original)
    {
        // オリジナルのAllyをJSONにシリアライズ
        string json = JsonUtility.ToJson(original);

        // JSONから新しいAllyをデシリアライズ
        var newInstance = ScriptableObject.CreateInstance<Ally>();
        JsonUtility.FromJsonOverwrite(json, newInstance);

        return newInstance;
    }


    // プレイヤーをターンに設定
    public void AddCharacterToParty(Ally ally)
    {
            partyMembers.Add(ally);
    }

    public async UniTask Initialize()
    {
        RemoveAllAllies();

        for (int i = 0; i < numberOfPartyMembers; i++)
        {
            Ally ally = new Ally();
            ally.CharacterName = Constants.characterNames[i];
            ally.CharacterID = i + 1;
            partyMembers.Add(ally);
        }
        await UniTask.Delay(10);
        //await GetAlliesFromAddressables();
    }

    public int GetIndex(Ally ally)
    {
        int index = partyMembers.IndexOf(ally);
        return index;
    }

    public Ally GetAllyByIndex(int index)
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

    public List<Ally> GetAllies()
    {
        List<Ally> allies = new List<Ally>();
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

    public bool IsOwlEyeActivate()
    {
        bool isOwlEyeActivate = false;
        
        foreach(var member in partyMembers)
        {
            List<string> equipedSkills = member.EquipedSkills.Select(s => s.ID).ToList();
            if (equipedSkills.Contains("30403"))
            {
                isOwlEyeActivate = true;
                continue;
            }
        }
        return isOwlEyeActivate;
    }
}
