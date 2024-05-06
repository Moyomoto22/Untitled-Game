using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "EnemyPartyStatus", menuName = "CreateEnemyPartyStatus")]
public class EnemyPartyStatus : ScriptableObject
{
    [System.Serializable]
    public struct PartyMember 
    {
        [SerializeField]
        public GameObject enemy;
        [SerializeField]
        public Vector3 position;
    }

    [SerializeField]
    private string partyName = null;

    [SerializeField]
    List<PartyMember> partyMembers;

    public string GetPartyName()
    {
        return partyName;
    }

    public List<PartyMember> GetPartyMembers()
    {
        return partyMembers;
    }
}