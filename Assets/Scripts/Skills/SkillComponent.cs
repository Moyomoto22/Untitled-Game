using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillComponent : MonoBehaviour
{
    public Skill skill;
    public Image icon;
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI costCategory;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI point;

    public void Initialize()
    {
        if (skill != null)
        {
            if(skill.icon != null)
            {
                icon.enabled = true;
                icon.sprite = skill.icon;
            }
            else
            {
                icon.enabled = false;
            }
            skillName.text = skill.skillName;
            SetCost();
        }
    }

    private void SetCost()
    {
        if (costCategory != null)
        {
            switch (skill.skillCategory)
            {
                case Constants.SkillCategory.Magic:
                case Constants.SkillCategory.Miracle:
                    costCategory.text = "MP";
                    break;
                case Constants.SkillCategory.Arts:
                    costCategory.text = "TP";
                    break;
                case Constants.SkillCategory.Active:
                case Constants.SkillCategory.Passive:
                    if (skill.isExSkill)
                    {
                        costCategory.text = "";
                    }
                    else
                    {
                        costCategory.text = "SP";
                    }                
                    break;
            }
        }
        if (cost != null)
        {
            switch (skill.skillCategory)
            {
                case Constants.SkillCategory.Magic:
                case Constants.SkillCategory.Miracle:
                    var m = skill as MagicMiracle;
                    cost.text = m.MPCost.ToString();
                    break;
                case Constants.SkillCategory.Arts:
                    var a = skill as Arts;
                    cost.text = a.TPCost.ToString();
                    break;
                case Constants.SkillCategory.Active:
                case Constants.SkillCategory.Passive:
                    if (skill.isExSkill)
                    {
                        cost.text = "-";
                    }
                    else
                    {
                        cost.text = skill.spCost.ToString();
                    }
                    break;
            }
        }
    }
}
