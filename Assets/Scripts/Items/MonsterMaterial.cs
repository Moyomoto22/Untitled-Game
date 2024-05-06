using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "MonsterMaterial", menuName = "CreateMonsterMaterial")]
public class MonsterMaterial : Item
{
    //　ドロップするモンスター名
    [SerializeField]
    public string monsterName;

}