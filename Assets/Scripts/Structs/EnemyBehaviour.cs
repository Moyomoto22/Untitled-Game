using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class EnemyBehaviour: MonoBehaviour
{
    public Enemy enemy;
    public EnemyComponent component;

    void Awake()
    {
        component = gameObject.GetComponentInParent<EnemyComponent>();
        enemy = component?.status; // componentがnullでない場合にstatusを設定
    }

    public abstract UniTask PerformAction(Character objective);

    public async UniTask Action(Character objective, Skill skill)
    {
        var skillName = skill.SkillName;

        component.ShowSkillName(skillName);              // スキル名称表示
        var manip = component.manipulator;
        if (manip != null)
        {
            await manip.FlashWithHighlightPlus(0.2f, Color.white); // 敵スプライトをフラッシュ
        }
        await enemy.UseSkill(skill, objective);         // スキル効果適用
        await UniTask.Delay(500);
        component.skillNameWindow.SetActive(false);      // スキル名称非表示
    }

    /// <summary>
    /// 行動不能
    /// </summary>
    /// <param name="objective"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public async UniTask Stunned()
    {
        component.ShowSkillName("行動できない");              // スキル名称表示
        var manip = component.manipulator;
        if (manip != null)
        {
            await manip.ShakeHorizon(0.1f, 2.0f); // 敵スプライト左右に動かす
        }
        await UniTask.Delay(500);
        component.skillNameWindow.SetActive(false);      // スキル名称非表示
    }

    public async UniTask Attack(Character objective)
    {
        var skillID = Constants.attackSkillID;
        Skill attack = SkillManager.Instance.GetSkillByID(skillID);
        await Action(objective, attack);
    }

    public async UniTask Guard(Character objective)
    {
        var skillID = Constants.guardSkillID;
        Skill guard = SkillManager.Instance.GetSkillByID(skillID);
        await Action(objective, guard);
    }
}
