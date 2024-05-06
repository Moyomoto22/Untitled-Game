using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "MonsterMaterial", menuName = "CreateMonsterMaterial")]
public class MonsterMaterial : Item
{
    //�@�h���b�v���郂���X�^�[��
    [SerializeField]
    public string monsterName;

}