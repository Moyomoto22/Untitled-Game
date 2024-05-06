using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    public List<Skill> AllSkills;

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

    // ID によってスキルを検索するメソッド
    public Skill GetSkillByID(string id)
    {
        foreach (Skill skill in AllSkills)
        {
            if (skill.ID == id)
            {
                return skill;
            }
        }
        Debug.LogWarning("Skill with ID: " + id + " not found.");
        return null;
    }
}