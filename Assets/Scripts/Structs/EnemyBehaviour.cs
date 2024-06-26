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
        enemy = component?.status; // component��null�łȂ��ꍇ��status��ݒ�
    }

    public abstract UniTask PerformAction(Character objective);

    public async UniTask Action(Character objective, Skill skill)
    {
        var skillName = skill.SkillName;

        component.ShowSkillName(skillName);              // �X�L�����̕\��
        var manip = component.manipulator;
        if (manip != null)
        {
            await manip.FlashWithHighlightPlus(0.2f, Color.white); // �G�X�v���C�g���t���b�V��
        }
        await enemy.UseSkill(skill, objective);         // �X�L�����ʓK�p
        await UniTask.Delay(500);
        component.skillNameWindow.SetActive(false);      // �X�L�����̔�\��
    }

    /// <summary>
    /// �s���s�\
    /// </summary>
    /// <param name="objective"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public async UniTask Stunned()
    {
        component.ShowSkillName("�s���ł��Ȃ�");              // �X�L�����̕\��
        var manip = component.manipulator;
        if (manip != null)
        {
            await manip.ShakeHorizon(0.1f, 2.0f); // �G�X�v���C�g���E�ɓ�����
        }
        await UniTask.Delay(500);
        component.skillNameWindow.SetActive(false);      // �X�L�����̔�\��
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
