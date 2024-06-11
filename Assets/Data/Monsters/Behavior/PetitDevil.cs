using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetitDevil : EnemyBehaviour
{
    public override async UniTask PerformAction(CharacterStatus objective)
    {
        if (Random.value < 0.1f)
        {
            await Guard(status);
        }
        else if (Random.value < 0.3f)
        {
            var skill = SkillManager.Instance.GetSkillByID("0001"); // ファイアボール
            await Action(objective, skill);
        }
        else
        {
            await Attack(objective);
        }
    }
}
