using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Weapon", menuName = "CreateWeapon")]
public class Weapon : Equip
{
    // ���
    [SerializeField]
    public Constants.WeaponCategory weaponCategory;
    // ����
    [SerializeField]
    public List<Constants.Attribute> attributes;
    // �ˑ��X�e�[�^�X 
    // 0: STR 1: DEX 2: INT 3:MND
    [SerializeField]
    public int dependentStatus = 0;
    // ���莝��
    [SerializeField]
    public bool isTwoHanded = false;
    // �U����
    [SerializeField]
    public int times = 1;

}