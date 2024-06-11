using System;

[Serializable]
public struct ItemForSave
{
    public string ID;
    public int equipedCharacterId;
    public int equipPartIndex;

    public ItemForSave(string id, int equipedCharacterId, int partIndex)
    {
        this.ID = id;
        this.equipedCharacterId = equipedCharacterId;
        this.equipPartIndex = partIndex;
    }
}