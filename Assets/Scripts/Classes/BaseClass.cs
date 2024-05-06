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
    //@ID
    [SerializeField]
    public int ID;
    //@NXΌ
    [SerializeField]
    public string className;
    //@NXΌͺΜ
    [SerializeField]
    public string classAbbreviation;
    //  LN^[ζ Sg
    [SerializeField]
    public List<Sprite> imagesA;
    //  LN^[ζ lp
    [SerializeField]
    public List<Sprite> imagesB;
    //  LN^[ζ oXgAbv
    [SerializeField]
    public List<Sprite> imagesC;
    //  LN^[ζ Ϊό
    [SerializeField]
    public List<Sprite> imagesD;
    //@HP{¦
    [SerializeField]
    public double hpRate;
    //@MP{¦
    [SerializeField]
    public double mpRate;
    //@STR{¦
    [SerializeField]
    public double strRate;
    //@VIT{¦
    [SerializeField]
    public double vitRate;
    //@DEX{¦
    [SerializeField]
    public double dexRate;
    //@AGI{¦
    [SerializeField]
    public double agiRate;
    //@INT{¦
    [SerializeField]
    public double intRate;
    //@MND{¦
    [SerializeField]
    public double mndRate;
    // Xe[^X]Ώ 0:HP ~ 7:MND
    [SerializeField]
    public List<string> statusRank;
    // υΒ\ννΚ
    [SerializeField]
    public List<Constants.WeaponCategory> equippableWeapons;
    //@ΰΎ
    [SerializeField, Multiline(4)]
    public string description;
    // ExXL
    [SerializeField]
    public Skill exSkill;
    // KΎXL
    [SerializeField]
    public List<Skill> LearnSkills;
}