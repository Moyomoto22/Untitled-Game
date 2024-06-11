using System;
using System.Collections.Generic;

// セーブデータ
[Serializable]
public class PlayerData
{
    public DateTime saveDateTime;
    public TimeSpan playTime;

    public string currentScene;
    public float positionX;
    public float positionY;
    public float positionZ;

    public string location;
    public List<PlayerStatusForSave> partyMembers;
    public List<ItemForSave> items;
}