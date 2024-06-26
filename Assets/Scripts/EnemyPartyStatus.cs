using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "EnemyPartyStatus", menuName = "CreateEnemyPartyStatus")]
public class EnemyPartyStatus : MonoBehaviour
{
    [System.Serializable]
    public struct PartyMember 
    {
        [SerializeField] public GameObject enemy;
        [SerializeField] public Vector3 position;
    }

    [SerializeField] public string partyName = null;

    [SerializeField] public List<PartyMember> partyMembers;

    public string GetPartyName()
    {
        return partyName;
    }

    public List<PartyMember> GetPartyMembers()
    {
        List<PartyMember> members = new List<PartyMember>();
       
        foreach(var partyMember in partyMembers)
        {
            members.Add(partyMember);
        }
        return members;
    }
}