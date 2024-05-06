using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "Class", menuName = "CreateClass")]
public class BaseClass : ScriptableObject
{
    //�@ID
    [SerializeField]
    public int ID;
    //�@�N���X��
    [SerializeField]
    public string className;
    //�@�N���X������
    [SerializeField]
    public string classAbbreviation;
    //  �L�����N�^�[�摜 �S�g
    [SerializeField]
    public List<Sprite> imagesA;
    //  �L�����N�^�[�摜 �l�p
    [SerializeField]
    public List<Sprite> imagesB;
    //  �L�����N�^�[�摜 �o�X�g�A�b�v
    [SerializeField]
    public List<Sprite> imagesC;
    //  �L�����N�^�[�摜 �ڐ�
    [SerializeField]
    public List<Sprite> imagesD;
    //�@HP�{��
    [SerializeField]
    public double hpRate;
    //�@MP�{��
    [SerializeField]
    public double mpRate;
    //�@STR�{��
    [SerializeField]
    public double strRate;
    //�@VIT�{��
    [SerializeField]
    public double vitRate;
    //�@DEX�{��
    [SerializeField]
    public double dexRate;
    //�@AGI�{��
    [SerializeField]
    public double agiRate;
    //�@INT�{��
    [SerializeField]
    public double intRate;
    //�@MND�{��
    [SerializeField]
    public double mndRate;
    // �X�e�[�^�X�]�� 0:HP ~ 7:MND
    [SerializeField]
    public List<string> statusRank;
    // �����\������
    [SerializeField]
    public List<Constants.WeaponCategory> equippableWeapons;
    //�@����
    [SerializeField, Multiline(4)]
    public string description;
    // Ex�X�L��
    [SerializeField]
    public Skill exSkill;
    // �K���X�L��
    [SerializeField]
    public List<Skill> LearnSkills;
}