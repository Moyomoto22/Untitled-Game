using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    public List<Skill> AllSkills;

    [SerializeField] private List<Skill> swordArtsList;
    public List<Skill> SwordArtsList
    {
        get { return swordArtsList; }
    }

    [SerializeField] private List<Skill> bladeArtsList;
    public List<Skill> BladeArtsList
    {
        get { return bladeArtsList; }
    }

    [SerializeField] private List<Skill> daggerArtsList;
    public List<Skill> DaggerArtsList
    {
        get { return daggerArtsList; }
    }

    [SerializeField] private List<Skill> spearArtsList;
    public List<Skill> SpearArtsList
    {
        get { return spearArtsList; }
    }

    [SerializeField] private List<Skill> axArtsList;
    public List<Skill> AxArtsList
    {
        get { return axArtsList; }
    }

    [SerializeField] private List<Skill> hammerArtsList;
    public List<Skill> HammerArtsList
    {
        get { return hammerArtsList; }
    }

    [SerializeField] private List<Skill> fistArtsList;
    public List<Skill> FistArtsList
    {
        get { return fistArtsList; }
    }

    [SerializeField] private List<Skill> bowArtsList;
    public List<Skill> BowArtsList
    {
        get { return bowArtsList; }
    }

    [SerializeField] private List<Skill> staffArtsList;
    public List<Skill> StaffArtsList
    {
        get { return staffArtsList; }
    }

    [SerializeField] private List<Skill> shieldArtsList;
    public List<Skill> ShieldArtsList
    {
        get { return shieldArtsList; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Skill GetLearningArtsID(Constants.WeaponCategory weaponCategory, int level)
    {
        Skill skill = null;
        switch (weaponCategory)
        {
            case Constants.WeaponCategory.Sword:
                skill = SwordArtsList[level - 1];
                break;
            case Constants.WeaponCategory.Blade:
                skill = BladeArtsList[level - 1];
                break;
            case Constants.WeaponCategory.Dagger:
                skill = DaggerArtsList[level - 1];
                break;
            case Constants.WeaponCategory.Spear:
                skill = SpearArtsList[level - 1];
                break;
            case Constants.WeaponCategory.Ax:
                skill = AxArtsList[level - 1];
                break;
            case Constants.WeaponCategory.Hammer:
                skill = HammerArtsList[level - 1];
                break;
            case Constants.WeaponCategory.Fist:
                skill = FistArtsList[level - 1];
                break;
            case Constants.WeaponCategory.Bow:
                skill = BowArtsList[level - 1];
                break;
            case Constants.WeaponCategory.Staff:
                skill = StaffArtsList[level - 1];
                break;
            case Constants.WeaponCategory.Shield:
                skill = ShieldArtsList[level - 1];
                break;
        }
        return skill;
    }

    /// <summary>
    /// スキルのディープコピーを作成する
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public Skill CopySkill(Skill original)
    {
        // Reflectionを使用して正確なランタイム型を取得し、新しいインスタンスを作成
        Type skillType = original.GetType();
        Skill newInstance = (Skill)ScriptableObject.CreateInstance(skillType);

        // JSONを使用してデータをコピー
        string json = JsonUtility.ToJson(original);
        JsonUtility.FromJsonOverwrite(json, newInstance);

        return newInstance;
    }

    /// <summary>
    /// IDからスキルを取得し、取得したスキルのコピーを返す
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Skill GetSkillByID(string id)
    {
        foreach (Skill skill in AllSkills)
        {
            if (skill.ID == id)
            {
                Skill copy = CopySkill(skill);
                return copy;
            }
        }
        Debug.LogWarning("Skill with ID: " + id + " not found.");
        return null;
    }
}