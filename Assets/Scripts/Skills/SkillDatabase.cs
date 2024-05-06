using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Skills/CreateSkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    public List<Skill> skills = new List<Skill>(); // �I�u�W�F�N�g�̃��X�g

    private Dictionary<string, Skill> skillDictionary; // ID�ƃI�u�W�F�N�g�̑Ή��֌W���Ǘ�����f�B�N�V���i��

    private void OnEnable()
    {
        // ���X�g���f�B�N�V���i���ɕϊ�
        skillDictionary = new Dictionary<string, Skill>();
        foreach (var skill in skills)
        {
            skillDictionary.Add(skill.ID, skill);
        }
    }

    // ID�ŃI�u�W�F�N�g���������郁�\�b�h
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