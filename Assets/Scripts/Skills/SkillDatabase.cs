using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Skills/CreateSkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    public List<Skill> skills = new List<Skill>(); // オブジェクトのリスト

    private Dictionary<string, Skill> skillDictionary; // IDとオブジェクトの対応関係を管理するディクショナリ

    private void OnEnable()
    {
        // リストをディクショナリに変換
        skillDictionary = new Dictionary<string, Skill>();
        foreach (var skill in skills)
        {
            skillDictionary.Add(skill.ID, skill);
        }
    }

    // IDでオブジェクトを検索するメソッド
    public Skill GetObjectByID(string id)
    {
        if (skillDictionary.ContainsKey(id))
        {
            return skillDictionary[id];
        }
        else
        {
            Debug.LogWarning("Object with ID " + id + " not found.");
            return null;
        }
    }
}